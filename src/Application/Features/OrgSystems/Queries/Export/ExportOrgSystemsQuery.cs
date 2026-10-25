// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.OrgSystems.DTOs;
using CleanArchitecture.Blazor.Application.Features.OrgSystems.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.OrgSystems.Queries.Export;

public class ExportOrgSystemsQuery : OrgSystemAdvancedFilter, IRequest<Result<byte[]>>
{
    public ExportType ExportType { get; set; }
    public OrgSystemAdvancedSpecification Specification => new(this);
}

public class ExportOrgSystemsQueryHandler : IRequestHandler<ExportOrgSystemsQuery, Result<byte[]>>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IMapper _mapper;
    private readonly IExcelService _excelService;
    private readonly IPDFService _pdfService;
    private readonly IStringLocalizer<ExportOrgSystemsQueryHandler> _localizer;

    public ExportOrgSystemsQueryHandler(IApplicationDbContextFactory dbContextFactory, IMapper mapper, IExcelService excelService, IPDFService pdfService, IStringLocalizer<ExportOrgSystemsQueryHandler> localizer)
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
        _excelService = excelService;
        _pdfService = pdfService;
        _localizer = localizer;
    }

    public async Task<Result<byte[]>> Handle(ExportOrgSystemsQuery request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        var data = await db.OrgSystems.ApplySpecification(request.Specification)
            .AsNoTracking()
            .ProjectTo<OrgSystemDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        byte[] result;
        switch (request.ExportType)
        {
            case ExportType.PDF:
            {
                var mappers = new Dictionary<string, Func<OrgSystemDto, object?>>
                {
                    { _localizer["System Name"], x => x.Name },
                    { _localizer["Description"], x => x.Description },
                    { _localizer["Criticality"], x => x.Criticality },
                    { _localizer["Lifecycle Status"], x => x.LifecycleStatus },
                    { _localizer["Dependencies"], x => x.Dependencies },
                    { _localizer["Tags"], x => string.Join(",", x.Tags ?? new List<string>()) }
                };
                result = await _pdfService.ExportAsync(data, mappers, _localizer["Systems"], true);
                break;
            }
            default:
            {
                var mappers = new Dictionary<string, Func<OrgSystemDto, object?>>
                {
                    { _localizer["System Name"], x => x.Name },
                    { _localizer["Description"], x => x.Description },
                    { _localizer["Criticality"], x => x.Criticality },
                    { _localizer["Lifecycle Status"], x => x.LifecycleStatus },
                    { _localizer["Dependencies"], x => x.Dependencies },
                    { _localizer["Tags"], x => string.Join(",", x.Tags ?? new List<string>()) }
                };
                result = await _excelService.ExportAsync(data, mappers, _localizer["Systems"]);
                break;
            }
        }

        return await Result<byte[]>.SuccessAsync(result);
    }
}


