// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace CleanArchitecture.Blazor.Application.Common.Security;

public static partial class Permissions
{
    [DisplayName("System Permissions")]
    [Description("Set permissions for application system operations")]
    public static class Systems
    {
        [Description("Allows viewing systems")]
        public const string View = "Permissions.Systems.View";

        [Description("Allows creating systems")]
        public const string Create = "Permissions.Systems.Create";

        [Description("Allows editing systems")]
        public const string Edit = "Permissions.Systems.Edit";

        [Description("Allows deleting systems")]
        public const string Delete = "Permissions.Systems.Delete";

        [Description("Allows searching systems")]
        public const string Search = "Permissions.Systems.Search";

        [Description("Allows exporting systems")]
        public const string Export = "Permissions.Systems.Export";
        [Description("Allows approving lifecycle workflows")]
        public const string Approve = "Permissions.Systems.Approve";
    }
}

public class SystemsAccessRights
{
    public bool View { get; set; }
    public bool Create { get; set; }
    public bool Edit { get; set; }
    public bool Delete { get; set; }
    public bool Search { get; set; }
    public bool Export { get; set; }
}

