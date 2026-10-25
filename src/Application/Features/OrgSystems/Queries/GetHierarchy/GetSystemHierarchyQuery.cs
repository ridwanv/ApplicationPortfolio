// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.OrgSystems.Caching;
using CleanArchitecture.Blazor.Application.Features.OrgSystems.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.OrgSystems.Queries.GetHierarchy;

public class GetSystemHierarchyQuery : ICacheableRequest<Result<List<SystemHierarchyDto>>>
{
    public int? RootSystemId { get; set; }
    public bool IncludeAttributes { get; set; } = false;
    
    public string CacheKey => OrgSystemCacheKey.GetHierarchyCacheKey(RootSystemId, IncludeAttributes);
    public IEnumerable<string>? Tags => OrgSystemCacheKey.Tags;
}

public class SystemHierarchyDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public SystemCriticality Criticality { get; set; }
    public SystemLifecycleStatus LifecycleStatus { get; set; }
    public int? BusinessOwnerId { get; set; }
    public int? TechnicalOwnerId { get; set; }
    public int? ParentSystemId { get; set; }
    public int Level { get; set; }
    public List<SystemHierarchyDto> Children { get; set; } = new();
    public Dictionary<string, string?> DynamicAttributes { get; set; } = new();
    
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<OrgSystem, SystemHierarchyDto>()
                .ForMember(dest => dest.Level, opt => opt.Ignore())
                .ForMember(dest => dest.Children, opt => opt.Ignore())
                .ForMember(dest => dest.DynamicAttributes, opt => opt.MapFrom(src => 
                    src.Attributes.ToDictionary(attr => attr.AttributeDefinition.Name, attr => attr.Value)));
        }
    }
}

public class GetSystemHierarchyQueryHandler : IRequestHandler<GetSystemHierarchyQuery, Result<List<SystemHierarchyDto>>>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IMapper _mapper;

    public GetSystemHierarchyQueryHandler(IApplicationDbContextFactory dbContextFactory, IMapper mapper)
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }

    public async Task<Result<List<SystemHierarchyDto>>> Handle(GetSystemHierarchyQuery request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        
        var query = db.OrgSystems.AsNoTracking();
        
        if (request.IncludeAttributes)
        {
            query = query.Include(x => x.Attributes)
                         .ThenInclude(attr => attr.AttributeDefinition);
        }
        
        var allSystems = await query.ToListAsync(cancellationToken);
        var systemDtos = _mapper.Map<List<SystemHierarchyDto>>(allSystems);
        
        // Build hierarchy
        var rootSystems = new List<SystemHierarchyDto>();
        var systemsById = systemDtos.ToDictionary(s => s.Id);
        
        foreach (var system in systemDtos)
        {
            if (request.RootSystemId.HasValue)
            {
                // If specific root is requested, find that subtree
                if (system.Id == request.RootSystemId.Value)
                {
                    BuildHierarchy(system, systemsById, 0);
                    rootSystems.Add(system);
                }
            }
            else if (!system.ParentSystemId.HasValue)
            {
                // Build complete hierarchy from roots
                BuildHierarchy(system, systemsById, 0);
                rootSystems.Add(system);
            }
        }
        
        return Result<List<SystemHierarchyDto>>.Success(rootSystems);
    }
    
    private void BuildHierarchy(SystemHierarchyDto parent, Dictionary<int, SystemHierarchyDto> systemsById, int level)
    {
        parent.Level = level;
        parent.Children = systemsById.Values
            .Where(s => s.ParentSystemId == parent.Id)
            .OrderBy(s => s.Name)
            .ToList();
        
        foreach (var child in parent.Children)
        {
            BuildHierarchy(child, systemsById, level + 1);
        }
    }
}