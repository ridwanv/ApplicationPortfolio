// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace CleanArchitecture.Blazor.Application.Common.Security;

public static partial class Permissions
{
    [DisplayName("Attributes Permissions")]
    [Description("Set permissions for dynamic attribute operations")]
    public static class Attributes
    {
        [Description("Allows viewing attribute definitions and values")]
        public const string View = "Permissions.Attributes.View";

        [Description("Allows creating attribute definitions")]
        public const string Create = "Permissions.Attributes.Create";

        [Description("Allows editing attribute definitions and values")]
        public const string Edit = "Permissions.Attributes.Edit";

        [Description("Allows deleting attribute definitions and values")]
        public const string Delete = "Permissions.Attributes.Delete";

        [Description("Allows exporting attributes data")]
        public const string Export = "Permissions.Attributes.Export";
    }
}

public class AttributesAccessRights
{
    public bool View { get; set; }
    public bool Create { get; set; }
    public bool Edit { get; set; }
    public bool Delete { get; set; }
    public bool Export { get; set; }
}

