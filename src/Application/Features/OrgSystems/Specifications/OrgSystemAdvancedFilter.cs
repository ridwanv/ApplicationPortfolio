namespace CleanArchitecture.Blazor.Application.Features.OrgSystems.Specifications;

public class OrgSystemAdvancedFilter : PaginationFilter
{
    public string? Keyword { get; set; }
    public SystemCriticality? Criticality { get; set; }
    public SystemLifecycleStatus? LifecycleStatus { get; set; }
    public int? OwnerId { get; set; }
    public int? ParentSystemId { get; set; }
    public bool? HasSubSystems { get; set; }
    public bool IncludeSubSystems { get; set; } = false;
    public bool OnlyRootSystems { get; set; } = false;
    public List<string>? Tags { get; set; }
    public UserProfile? CurrentUser { get; set; }
}



