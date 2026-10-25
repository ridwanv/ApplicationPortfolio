using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Blazor.Domain.Entities;

public class Application : AuditableEntity
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string SystemId { get; set; } = string.Empty;
    
    public SystemCriticality Criticality { get; set; } = SystemCriticality.Medium;
    
    public ApplicationType ApplicationType { get; set; } = ApplicationType.Custom;
    
    public HostingType HostingType { get; set; } = HostingType.OnPremise;
    
    public SystemLifecycleStatus LifecycleStatus { get; set; } = SystemLifecycleStatus.Active;
    
    [MaxLength(100)]
    public string? BusinessOwner { get; set; }
    
    [MaxLength(100)]
    public string? BusinessContact { get; set; }
    
    [MaxLength(100)]
    public string? TechnicalOwner { get; set; }
    
    [MaxLength(100)]
    public string? TechnicalContact { get; set; }
    
    [MaxLength(200)]
    public string? TechnologyStack { get; set; }
    
    [MaxLength(100)]
    public string? Version { get; set; }
    
    [MaxLength(100)]
    public string? License { get; set; }
    
    [MaxLength(200)]
    public string? ComplianceRequirements { get; set; }
    
    public decimal? MonthlyCost { get; set; }
    
    public int? DailyUsers { get; set; }
    
    public decimal? DataStorageGB { get; set; }
    
    public decimal? UptimePercentage { get; set; }
    
    public decimal? AverageResponseTimeSeconds { get; set; }
    
    public decimal? UserRating { get; set; }
    
    public DateTime? EndOfSupportDate { get; set; }
    
    public DateTime? NextReviewDate { get; set; }
    
    public DateTime? PlannedRetirementDate { get; set; }
    
    public SecurityRiskLevel SecurityRisk { get; set; } = SecurityRiskLevel.Low;
    
    public TechnicalDebtLevel TechnicalDebt { get; set; } = TechnicalDebtLevel.Low;
    
    public BusinessImpactLevel BusinessImpact { get; set; } = BusinessImpactLevel.Medium;
    
    public VendorRiskLevel VendorRisk { get; set; } = VendorRiskLevel.Low;
    
    // Navigation properties
    public virtual ICollection<ApplicationDependency> Dependencies { get; set; } = new List<ApplicationDependency>();
    public virtual ICollection<ApplicationDependency> DependentSystems { get; set; } = new List<ApplicationDependency>();
    public virtual ICollection<ApplicationAttribute> Attributes { get; set; } = new List<ApplicationAttribute>();
}