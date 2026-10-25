// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.SystemDependencies.DTOs;

[Description("System Dependencies")]
public class DependencyEdgeDto
{
    [Description("Id")] public int Id { get; set; }
    [Description("Source System Id")] public int SourceSystemId { get; set; }
    [Description("Source System Name")] public string SourceSystemName { get; set; } = string.Empty;
    [Description("Target System Id")] public int TargetSystemId { get; set; }
    [Description("Target System Name")] public string TargetSystemName { get; set; } = string.Empty;
    [Description("Notes")] public string? Notes { get; set; }
    [Description("Created Date")] public DateTime? Created { get; set; }
    [Description("Created By")] public string? CreatedBy { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<DependencyEdge, DependencyEdgeDto>()
                .ForMember(dest => dest.SourceSystemName, opt => opt.MapFrom(src => src.SourceSystem.Name))
                .ForMember(dest => dest.TargetSystemName, opt => opt.MapFrom(src => src.TargetSystem.Name))
                .ReverseMap()
                .ForMember(dest => dest.SourceSystem, opt => opt.Ignore())
                .ForMember(dest => dest.TargetSystem, opt => opt.Ignore());
        }
    }
}

[Description("System Dependency Graph Node")]
public class SystemDependencyNodeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public SystemCriticality Criticality { get; set; }
    public SystemLifecycleStatus LifecycleStatus { get; set; }
    public List<DependencyConnectionDto> Dependencies { get; set; } = new();
    public List<DependencyConnectionDto> Dependents { get; set; } = new();
    
    // Graph visualization properties
    public int Level { get; set; }
    public bool IsOrphan => !Dependencies.Any() && !Dependents.Any();
}

[Description("Dependency Connection")]
public class DependencyConnectionDto
{
    public int DependencyId { get; set; }
    public int SystemId { get; set; }
    public string SystemName { get; set; } = string.Empty;
    public string? Notes { get; set; }
}