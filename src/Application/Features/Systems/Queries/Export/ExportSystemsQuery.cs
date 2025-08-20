// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Systems.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Systems.Queries.Export;

public class ExportSystemsQuery : IRequest<Result<byte[]>>
{
    public ExportType ExportType { get; set; }
    public string? Search { get; set; }
}

public class ExportSystemsQueryHandler : IRequestHandler<ExportSystemsQuery, Result<byte[]>>
{
    private readonly IApplicationDbContextFactory _dbFactory;
    private readonly IMapper _mapper;
    private readonly IExcelService _excelService;
    private readonly IPDFService _pdfService;
    private readonly IStringLocalizer<ExportSystemsQueryHandler> _localizer;

    public ExportSystemsQueryHandler(IApplicationDbContextFactory dbFactory, IMapper mapper, IExcelService excelService, IPDFService pdfService, IStringLocalizer<ExportSystemsQueryHandler> localizer)
    {
        _dbFactory = dbFactory;
        _mapper = mapper;
        _excelService = excelService;
        _pdfService = pdfService;
        _localizer = localizer;
    }

    public async Task<Result<byte[]>> Handle(ExportSystemsQuery request, CancellationToken cancellationToken)
    {
        await using var db = await _dbFactory.CreateAsync(cancellationToken);
        var query = db.ApplicationSystems.AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.ToLower();
            query = query.Where(x => (x.Name ?? "").ToLower().Contains(term) || (x.Category ?? "").ToLower().Contains(term));
        }
        var data = await query.AsNoTracking().ProjectTo<SystemDto>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);

        byte[] result;
        var mappers = new Dictionary<string, Func<SystemDto, object?>>
        {
            { _localizer["Name"], x => x.Name },
            { _localizer["Category"], x => x.Category },
            { _localizer["Hosting"], x => x.Hosting },
            { _localizer["Lifecycle"], x => x.LifecycleStage },
            { _localizer["ParentId"], x => x.ParentId }
        };
        result = request.ExportType == ExportType.PDF
            ? await _pdfService.ExportAsync(data, mappers, _localizer["Systems"], true)
            : await _excelService.ExportAsync(data, mappers, _localizer["Systems"]);
        return await Result<byte[]>.SuccessAsync(result);
    }
}