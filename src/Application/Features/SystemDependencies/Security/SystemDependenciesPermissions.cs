// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Common.Security;

public static partial class Permissions
{
    [DisplayName("System Dependencies Permissions")]
    [Description("Set permissions for system dependency management operations")]
    public static class SystemDependencies
    {
        public const string View = "Permissions.SystemDependencies.View";
        public const string Create = "Permissions.SystemDependencies.Create";
        public const string Edit = "Permissions.SystemDependencies.Edit";
        public const string Delete = "Permissions.SystemDependencies.Delete";
        public const string ViewDependencyGraph = "Permissions.SystemDependencies.ViewDependencyGraph";
        public const string ImpactAnalysis = "Permissions.SystemDependencies.ImpactAnalysis";
    }
}

public class SystemDependenciesAccessRights
{
    public bool View { get; set; }
    public bool Create { get; set; }
    public bool Edit { get; set; }
    public bool Delete { get; set; }
    public bool ViewDependencyGraph { get; set; }
    public bool ImpactAnalysis { get; set; }
}