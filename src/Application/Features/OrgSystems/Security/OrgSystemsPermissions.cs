// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Common.Security;

public static partial class Permissions
{
    [DisplayName("Systems Permissions")]
    [Description("Set permissions for organisational systems operations")]
    public static class Systems
    {
        public const string View = "Permissions.Systems.View";
        public const string Create = "Permissions.Systems.Create";
        public const string Edit = "Permissions.Systems.Edit";
        public const string Delete = "Permissions.Systems.Delete";
        public const string Export = "Permissions.Systems.Export";
        public const string Import = "Permissions.Systems.Import";
    }
}

public class OrgSystemsAccessRights
{
    public bool View { get; set; }
    public bool Create { get; set; }
    public bool Edit { get; set; }
    public bool Delete { get; set; }
    public bool Export { get; set; }
    public bool Import { get; set; }
}



