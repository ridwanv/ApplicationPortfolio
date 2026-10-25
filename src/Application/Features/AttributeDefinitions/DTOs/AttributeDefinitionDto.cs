// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.AttributeDefinitions.DTOs;

[Description("Attribute Definitions")]
public class AttributeDefinitionDto
{
    [Description("Id")] public int Id { get; set; }
    [Description("Name")] public string Name { get; set; } = string.Empty;
    [Description("Description")] public string? Description { get; set; }
    [Description("Data Type")] public AttributeDataType DataType { get; set; }
    [Description("Is Required")] public bool IsRequired { get; set; }
    [Description("Enum Options")] public string? EnumOptionsCsv { get; set; }
    [Description("Systems Using Count")] public int SystemsUsingCount { get; set; }
    [Description("Created Date")] public DateTime? Created { get; set; }
    [Description("Created By")] public string? CreatedBy { get; set; }
    [Description("Last Modified")] public DateTime? LastModified { get; set; }
    [Description("Modified By")] public string? LastModifiedBy { get; set; }

    // Helper property for UI
    public List<string> EnumOptions => 
        string.IsNullOrWhiteSpace(EnumOptionsCsv) 
            ? new List<string>() 
            : EnumOptionsCsv.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<AttributeDefinition, AttributeDefinitionDto>()
                .ForMember(dest => dest.SystemsUsingCount, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}