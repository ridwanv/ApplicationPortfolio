// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Systems.Commands.AddEdit;

public class AddEditSystemCommand : ICacheInvalidatorRequest<Result<int>>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? Hosting { get; set; }
    public string? LifecycleStage { get; set; }
    public int? ParentId { get; set; }

    public string CacheKey => "systems:all";
    public IEnumerable<string>? Tags => new[] { "systems" };

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<AddEditSystemCommand, ApplicationSystem>(MemberList.None);
        }
    }
}

public class AddEditSystemCommandHandler : IRequestHandler<AddEditSystemCommand, Result<int>>
{
    private readonly IApplicationDbContextFactory _dbFactory;
    private readonly IMapper _mapper;

    public AddEditSystemCommandHandler(IApplicationDbContextFactory dbFactory, IMapper mapper)
    {
        _dbFactory = dbFactory;
        _mapper = mapper;
    }

    public async Task<Result<int>> Handle(AddEditSystemCommand request, CancellationToken cancellationToken)
    {
        await using var db = await _dbFactory.CreateAsync(cancellationToken);
        if (request.Id > 0)
        {
            var entity = await db.ApplicationSystems.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (entity == null) return await Result<int>.FailureAsync($"System with id [{request.Id}] not found.");
            entity = _mapper.Map(request, entity);
            await db.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(entity.Id);
        }
        else
        {
            var entity = _mapper.Map<ApplicationSystem>(request);
            db.ApplicationSystems.Add(entity);
            await db.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(entity.Id);
        }
    }
}

