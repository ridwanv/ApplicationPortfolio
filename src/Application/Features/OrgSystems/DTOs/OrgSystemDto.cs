// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.OrgSystems.DTOs;

[Description("Systems")]
public class OrgSystemDto
{
    [Description("Id")] public int Id { get; set; }
    [Description("System Name")] public string? Name { get; set; }
    [Description("Description")] public string? Description { get; set; }
    [Description("Business Owner Id")] public int? BusinessOwnerId { get; set; }
    [Description("Technical Owner Id")] public int? TechnicalOwnerId { get; set; }
    [Description("Criticality")] public SystemCriticality Criticality { get; set; }
    [Description("Lifecycle Status")] public SystemLifecycleStatus LifecycleStatus { get; set; }
    [Description("Tags")] public List<string>? Tags { get; set; }
    [Description("Dependencies")] public string? Dependencies { get; set; }
    [Description("Parent System Id")] public int? ParentSystemId { get; set; }
    [Description("Parent System Name")] public string? ParentSystemName { get; set; }
    [Description("SubSystems Count")] public int SubSystemsCount { get; set; }
    [Description("System Attributes")] public Dictionary<string, string?> DynamicAttributes { get; set; } = new();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<OrgSystem, OrgSystemDto>()
                .ForMember(dest => dest.ParentSystemName, opt => opt.MapFrom(src => src.ParentSystem != null ? src.ParentSystem.Name : null))
                .ForMember(dest => dest.SubSystemsCount, opt => opt.MapFrom(src => src.SubSystems.Count))
                .ForMember(dest => dest.DynamicAttributes, opt => opt.MapFrom(src => 
                    src.Attributes.ToDictionary(attr => attr.AttributeDefinition.Name, attr => attr.Value)))
                .ReverseMap()
                .ForMember(dest => dest.ParentSystem, opt => opt.Ignore())
                .ForMember(dest => dest.SubSystems, opt => opt.Ignore())
                .ForMember(dest => dest.Attributes, opt => opt.Ignore());
        }
    }
}



