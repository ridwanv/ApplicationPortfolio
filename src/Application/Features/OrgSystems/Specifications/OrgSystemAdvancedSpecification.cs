namespace CleanArchitecture.Blazor.Application.Features.OrgSystems.Specifications;

public class OrgSystemAdvancedSpecification : Specification<OrgSystem>
{
    public OrgSystemAdvancedSpecification(OrgSystemAdvancedFilter filter)
    {
        Query.Where(x => x.Name != null)
            .Where(x => x.Name!.Contains(filter.Keyword!)
                        || x.Description!.Contains(filter.Keyword!)
                        || (x.Tags != null && x.Tags.Any(t => t.Contains(filter.Keyword!)))
                        || (x.Dependencies != null && x.Dependencies.Contains(filter.Keyword!)),
                !string.IsNullOrWhiteSpace(filter.Keyword))
            .Where(x => x.Criticality == filter.Criticality!.Value, filter.Criticality.HasValue)
            .Where(x => x.LifecycleStatus == filter.LifecycleStatus!.Value, filter.LifecycleStatus.HasValue)
            .Where(x => x.BusinessOwnerId == filter.OwnerId || x.TechnicalOwnerId == filter.OwnerId,
                filter.OwnerId.HasValue)
            .Where(x => x.ParentSystemId == filter.ParentSystemId, filter.ParentSystemId.HasValue)
            .Where(x => x.ParentSystemId == null, filter.OnlyRootSystems)
            .Where(x => x.SubSystems.Any() == filter.HasSubSystems!.Value, filter.HasSubSystems.HasValue)
            .Where(x => filter.Tags!.All(tag => x.Tags != null && x.Tags.Contains(tag)), 
                filter.Tags != null && filter.Tags.Any());

        // Include related entities if needed
        if (filter.IncludeSubSystems)
        {
            Query.Include(x => x.SubSystems)
                 .Include(x => x.ParentSystem);
        }
    }
}



