// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Common.Security;

public static partial class Permissions
{
    [DisplayName("Attribute Definitions Permissions")]
    [Description("Set permissions for attribute definition management operations")]
    public static class AttributeDefinitions
    {
        public const string View = "Permissions.AttributeDefinitions.View";
        public const string Create = "Permissions.AttributeDefinitions.Create";
        public const string Edit = "Permissions.AttributeDefinitions.Edit";
        public const string Delete = "Permissions.AttributeDefinitions.Delete";
        public const string Export = "Permissions.AttributeDefinitions.Export";
        public const string Import = "Permissions.AttributeDefinitions.Import";
        public const string BulkOperations = "Permissions.AttributeDefinitions.BulkOperations";
    }
}

public class AttributeDefinitionsAccessRights
{
    public bool View { get; set; }
    public bool Create { get; set; }
    public bool Edit { get; set; }
    public bool Delete { get; set; }
    public bool Export { get; set; }
    public bool Import { get; set; }
    public bool BulkOperations { get; set; }
}