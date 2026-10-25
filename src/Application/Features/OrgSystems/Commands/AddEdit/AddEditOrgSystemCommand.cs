// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.OrgSystems.Caching;

namespace CleanArchitecture.Blazor.Application.Features.OrgSystems.Commands.AddEdit;

public class AddEditOrgSystemCommand : ICacheInvalidatorRequest<Result<int>>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? BusinessOwnerId { get; set; }
    public int? TechnicalOwnerId { get; set; }
    public SystemCriticality Criticality { get; set; }
    public SystemLifecycleStatus LifecycleStatus { get; set; }
    public List<string>? Tags { get; set; }
    public string? Dependencies { get; set; }
    public int? ParentSystemId { get; set; }
    public string CacheKey => OrgSystemCacheKey.GetAllCacheKey;
    public IEnumerable<string>? TagsInvalidate => OrgSystemCacheKey.Tags;
    IEnumerable<string>? ICacheInvalidatorRequest<Result<int>>.Tags => TagsInvalidate;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<OrgSystemDto, AddEditOrgSystemCommand>(MemberList.None);
            CreateMap<AddEditOrgSystemCommand, OrgSystem>(MemberList.None);
        }
    }
}

public class AddEditOrgSystemCommandHandler : IRequestHandler<AddEditOrgSystemCommand, Result<int>>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IMapper _mapper;
    public AddEditOrgSystemCommandHandler(IApplicationDbContextFactory dbContextFactory, IMapper mapper)
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }

    public async Task<Result<int>> Handle(AddEditOrgSystemCommand request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        
        // Validate parent system exists and is not creating a circular reference
        if (request.ParentSystemId.HasValue)
        {
            var parentExists = await db.OrgSystems.AnyAsync(x => x.Id == request.ParentSystemId.Value, cancellationToken);
            if (!parentExists)
                return await Result<int>.FailureAsync($"Parent system with id: [{request.ParentSystemId}] not found.");
            
            // Prevent circular references - check if the parent would create a cycle
            if (request.Id > 0 && await WouldCreateCircularReference(db, request.Id, request.ParentSystemId.Value, cancellationToken))
                return await Result<int>.FailureAsync("Setting this parent would create a circular reference in the system hierarchy.");
        }
        
        if (request.Id > 0)
        {
            var item = await db.OrgSystems.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (item == null) return await Result<int>.FailureAsync($"OrgSystem with id: [{request.Id}] not found.");
            item = _mapper.Map(request, item);
            await db.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(item.Id);
        }
        else
        {
            var item = _mapper.Map<OrgSystem>(request);
            db.OrgSystems.Add(item);
            await db.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(item.Id);
        }
    }

    private async Task<bool> WouldCreateCircularReference(IApplicationDbContext db, int systemId, int parentId, CancellationToken cancellationToken)
    {
        var visited = new HashSet<int>();
        var current = parentId;
        
        while (current != 0)
        {
            if (current == systemId) return true;
            if (visited.Contains(current)) return true;
            
            visited.Add(current);
            var parent = await db.OrgSystems
                .Where(x => x.Id == current)
                .Select(x => x.ParentSystemId)
                .FirstOrDefaultAsync(cancellationToken);
            
            current = parent ?? 0;
        }
        
        return false;
    }
}


