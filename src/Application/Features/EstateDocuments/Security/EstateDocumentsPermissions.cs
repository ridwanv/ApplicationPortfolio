// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace CleanArchitecture.Blazor.Application.Common.Security;

public static partial class Permissions
{
    [DisplayName("Estate Document Permissions")]
    [Description("Set permissions for estate document operations")]
    public static class EstateDocuments
    {
        [Description("Allows viewing estate document details")]
        public const string View = "Permissions.EstateDocuments.View";

        [Description("Allows creating new estate document records")]
        public const string Create = "Permissions.EstateDocuments.Create";

        [Description("Allows modifying existing estate document details")]
        public const string Edit = "Permissions.EstateDocuments.Edit";

        [Description("Allows deleting estate document records")]
        public const string Delete = "Permissions.EstateDocuments.Delete";

        [Description("Allows printing estate document details")]
        public const string Print = "Permissions.EstateDocuments.Print";

        [Description("Allows searching for estate document records")]
        public const string Search = "Permissions.EstateDocuments.Search";

        [Description("Allows exporting estate document records")]
        public const string Export = "Permissions.EstateDocuments.Export";

        [Description("Allows importing estate document records")]
        public const string Import = "Permissions.EstateDocuments.Import";
    }
}

public class EstateDocumentsAccessRights
{
    public bool View { get; set; }
    public bool Create { get; set; }
    public bool Edit { get; set; }
    public bool Delete { get; set; }
    public bool Print { get; set; }
    public bool Search { get; set; }
    public bool Export { get; set; }
    public bool Import { get; set; }
}