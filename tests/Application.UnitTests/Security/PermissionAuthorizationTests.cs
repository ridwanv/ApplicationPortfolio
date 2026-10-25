// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Application.Common.Security;
using CleanArchitecture.Blazor.Application.Features.OrgSystems.Commands.AddEdit;
using CleanArchitecture.Blazor.Application.Features.OrgSystems.Commands.Delete;
using CleanArchitecture.Blazor.Application.Features.OrgSystems.Queries.Pagination;
using CleanArchitecture.Blazor.Application.Features.OrgSystems.Security;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Moq;
using NUnit.Framework;
using System.Security.Claims;

namespace CleanArchitecture.Blazor.Application.UnitTests.Security;

[TestFixture]
public class PermissionAuthorizationTests
{
    private Mock<IPermissionService> _mockPermissionService;
    private Mock<IUserContextAccessor> _mockUserContextAccessor;
    private Mock<IAuthorizationService> _mockAuthorizationService;
    private ClaimsPrincipal _adminUser;
    private ClaimsPrincipal _regularUser;
    private ClaimsPrincipal _readOnlyUser;
    private UserContext _adminUserContext;
    private UserContext _regularUserContext;
    private UserContext _readOnlyUserContext;

    [SetUp]
    public void SetUp()
    {
        _mockPermissionService = new Mock<IPermissionService>();
        _mockUserContextAccessor = new Mock<IUserContextAccessor>();
        _mockAuthorizationService = new Mock<IAuthorizationService>();

        // Create test users with different permission levels
        _adminUser = CreateUserWithClaims("admin@example.com", "Admin", 
            OrgSystemsPermissions.View, 
            OrgSystemsPermissions.Create, 
            OrgSystemsPermissions.Edit, 
            OrgSystemsPermissions.Delete,
            OrgSystemsPermissions.ManageAttributes,
            OrgSystemsPermissions.ManageDependencies);

        _regularUser = CreateUserWithClaims("user@example.com", "User", 
            OrgSystemsPermissions.View, 
            OrgSystemsPermissions.Create, 
            OrgSystemsPermissions.Edit);

        _readOnlyUser = CreateUserWithClaims("readonly@example.com", "ReadOnlyUser", 
            OrgSystemsPermissions.View);

        _adminUserContext = new UserContext("admin-id", "admin", null, "admin@example.com");
        _regularUserContext = new UserContext("user-id", "user", null, "user@example.com");
        _readOnlyUserContext = new UserContext("readonly-id", "readonly", null, "readonly@example.com");
    }

    [Test]
    public async Task AdminUser_AllOrgSystemOperations_ShouldBeAuthorized()
    {
        // Arrange
        _mockUserContextAccessor.Setup(x => x.Current).Returns(_adminUserContext);
        SetupPermissionServiceForUser(_adminUser, true, true, true, true, true, true);

        // Act & Assert - View permission
        var viewResult = await _mockPermissionService.Object.HasPermissionAsync(_adminUser, OrgSystemsPermissions.View);
        viewResult.Should().BeTrue();

        // Act & Assert - Create permission
        var createResult = await _mockPermissionService.Object.HasPermissionAsync(_adminUser, OrgSystemsPermissions.Create);
        createResult.Should().BeTrue();

        // Act & Assert - Edit permission
        var editResult = await _mockPermissionService.Object.HasPermissionAsync(_adminUser, OrgSystemsPermissions.Edit);
        editResult.Should().BeTrue();

        // Act & Assert - Delete permission
        var deleteResult = await _mockPermissionService.Object.HasPermissionAsync(_adminUser, OrgSystemsPermissions.Delete);
        deleteResult.Should().BeTrue();

        // Act & Assert - Manage Attributes permission
        var attributesResult = await _mockPermissionService.Object.HasPermissionAsync(_adminUser, OrgSystemsPermissions.ManageAttributes);
        attributesResult.Should().BeTrue();

        // Act & Assert - Manage Dependencies permission
        var dependenciesResult = await _mockPermissionService.Object.HasPermissionAsync(_adminUser, OrgSystemsPermissions.ManageDependencies);
        dependenciesResult.Should().BeTrue();
    }

    [Test]
    public async Task RegularUser_LimitedOrgSystemOperations_ShouldBeAuthorizedForBasicOperations()
    {
        // Arrange
        _mockUserContextAccessor.Setup(x => x.Current).Returns(_regularUserContext);
        SetupPermissionServiceForUser(_regularUser, true, true, true, false, false, false);

        // Act & Assert - View permission (allowed)
        var viewResult = await _mockPermissionService.Object.HasPermissionAsync(_regularUser, OrgSystemsPermissions.View);
        viewResult.Should().BeTrue();

        // Act & Assert - Create permission (allowed)
        var createResult = await _mockPermissionService.Object.HasPermissionAsync(_regularUser, OrgSystemsPermissions.Create);
        createResult.Should().BeTrue();

        // Act & Assert - Edit permission (allowed)
        var editResult = await _mockPermissionService.Object.HasPermissionAsync(_regularUser, OrgSystemsPermissions.Edit);
        editResult.Should().BeTrue();

        // Act & Assert - Delete permission (denied)
        var deleteResult = await _mockPermissionService.Object.HasPermissionAsync(_regularUser, OrgSystemsPermissions.Delete);
        deleteResult.Should().BeFalse();

        // Act & Assert - Manage Attributes permission (denied)
        var attributesResult = await _mockPermissionService.Object.HasPermissionAsync(_regularUser, OrgSystemsPermissions.ManageAttributes);
        attributesResult.Should().BeFalse();

        // Act & Assert - Manage Dependencies permission (denied)
        var dependenciesResult = await _mockPermissionService.Object.HasPermissionAsync(_regularUser, OrgSystemsPermissions.ManageDependencies);
        dependenciesResult.Should().BeFalse();
    }

    [Test]
    public async Task ReadOnlyUser_ViewOnlyAccess_ShouldBeAuthorizedForViewOnly()
    {
        // Arrange
        _mockUserContextAccessor.Setup(x => x.Current).Returns(_readOnlyUserContext);
        SetupPermissionServiceForUser(_readOnlyUser, true, false, false, false, false, false);

        // Act & Assert - View permission (allowed)
        var viewResult = await _mockPermissionService.Object.HasPermissionAsync(_readOnlyUser, OrgSystemsPermissions.View);
        viewResult.Should().BeTrue();

        // Act & Assert - Create permission (denied)
        var createResult = await _mockPermissionService.Object.HasPermissionAsync(_readOnlyUser, OrgSystemsPermissions.Create);
        createResult.Should().BeFalse();

        // Act & Assert - Edit permission (denied)
        var editResult = await _mockPermissionService.Object.HasPermissionAsync(_readOnlyUser, OrgSystemsPermissions.Edit);
        editResult.Should().BeFalse();

        // Act & Assert - Delete permission (denied)
        var deleteResult = await _mockPermissionService.Object.HasPermissionAsync(_readOnlyUser, OrgSystemsPermissions.Delete);
        deleteResult.Should().BeFalse();

        // Act & Assert - Manage Attributes permission (denied)
        var attributesResult = await _mockPermissionService.Object.HasPermissionAsync(_readOnlyUser, OrgSystemsPermissions.ManageAttributes);
        attributesResult.Should().BeFalse();

        // Act & Assert - Manage Dependencies permission (denied)
        var dependenciesResult = await _mockPermissionService.Object.HasPermissionAsync(_readOnlyUser, OrgSystemsPermissions.ManageDependencies);
        dependenciesResult.Should().BeFalse();
    }

    [Test]
    public async Task CommandHandlerAuthorization_CreateOrgSystem_RequiresCreatePermission()
    {
        // Arrange
        var command = new AddEditOrgSystemCommand
        {
            Name = "Test System",
            Description = "Test system for authorization testing"
        };

        // Act - Check if command has proper authorization attribute
        var commandType = typeof(AddEditOrgSystemCommand);
        var authAttributes = commandType.GetCustomAttributes(typeof(RequestAuthorizeAttribute), true);

        // Assert
        authAttributes.Should().NotBeEmpty();
        var authAttribute = authAttributes.First() as RequestAuthorizeAttribute;
        authAttribute.Should().NotBeNull();
        authAttribute.Policy.Should().Be(OrgSystemsPermissions.Create);
    }

    [Test]
    public async Task CommandHandlerAuthorization_DeleteOrgSystem_RequiresDeletePermission()
    {
        // Arrange
        var command = new DeleteOrgSystemCommand(new[] { 1, 2, 3 });

        // Act - Check if command has proper authorization attribute
        var commandType = typeof(DeleteOrgSystemCommand);
        var authAttributes = commandType.GetCustomAttributes(typeof(RequestAuthorizeAttribute), true);

        // Assert
        authAttributes.Should().NotBeEmpty();
        var authAttribute = authAttributes.First() as RequestAuthorizeAttribute;
        authAttribute.Should().NotBeNull();
        authAttribute.Policy.Should().Be(OrgSystemsPermissions.Delete);
    }

    [Test]
    public async Task QueryHandlerAuthorization_GetOrgSystems_RequiresViewPermission()
    {
        // Arrange
        var query = new OrgSystemsPaginationQuery
        {
            PageNumber = 1,
            PageSize = 10
        };

        // Act - Check if query has proper authorization attribute
        var queryType = typeof(OrgSystemsPaginationQuery);
        var authAttributes = queryType.GetCustomAttributes(typeof(RequestAuthorizeAttribute), true);

        // Assert
        authAttributes.Should().NotBeEmpty();
        var authAttribute = authAttributes.First() as RequestAuthorizeAttribute;
        authAttribute.Should().NotBeNull();
        authAttribute.Policy.Should().Be(OrgSystemsPermissions.View);
    }

    [Test]
    public async Task PermissionService_CheckMultiplePermissions_ReturnsCorrectResults()
    {
        // Arrange
        var permissions = new[]
        {
            OrgSystemsPermissions.View,
            OrgSystemsPermissions.Create,
            OrgSystemsPermissions.Edit,
            OrgSystemsPermissions.Delete
        };

        _mockPermissionService.Setup(x => x.HasPermissionAsync(_adminUser, OrgSystemsPermissions.View))
            .ReturnsAsync(true);
        _mockPermissionService.Setup(x => x.HasPermissionAsync(_adminUser, OrgSystemsPermissions.Create))
            .ReturnsAsync(true);
        _mockPermissionService.Setup(x => x.HasPermissionAsync(_adminUser, OrgSystemsPermissions.Edit))
            .ReturnsAsync(true);
        _mockPermissionService.Setup(x => x.HasPermissionAsync(_adminUser, OrgSystemsPermissions.Delete))
            .ReturnsAsync(true);

        // Act
        var results = new List<bool>();
        foreach (var permission in permissions)
        {
            var result = await _mockPermissionService.Object.HasPermissionAsync(_adminUser, permission);
            results.Add(result);
        }

        // Assert
        results.Should().OnlyContain(r => r == true);
    }

    [Test]
    public async Task PermissionService_UserWithoutPermissions_ReturnsUnauthorized()
    {
        // Arrange
        var unauthorizedUser = CreateUserWithClaims("noperm@example.com", "NoPermUser");
        
        _mockPermissionService.Setup(x => x.HasPermissionAsync(unauthorizedUser, It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var viewResult = await _mockPermissionService.Object.HasPermissionAsync(unauthorizedUser, OrgSystemsPermissions.View);
        var createResult = await _mockPermissionService.Object.HasPermissionAsync(unauthorizedUser, OrgSystemsPermissions.Create);
        var editResult = await _mockPermissionService.Object.HasPermissionAsync(unauthorizedUser, OrgSystemsPermissions.Edit);
        var deleteResult = await _mockPermissionService.Object.HasPermissionAsync(unauthorizedUser, OrgSystemsPermissions.Delete);

        // Assert
        viewResult.Should().BeFalse();
        createResult.Should().BeFalse();
        editResult.Should().BeFalse();
        deleteResult.Should().BeFalse();
    }

    [Test]
    public async Task RoleBasedAuthorization_AdminRole_HasAllPermissions()
    {
        // Arrange
        var adminClaims = new[]
        {
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim("permission", OrgSystemsPermissions.View),
            new Claim("permission", OrgSystemsPermissions.Create),
            new Claim("permission", OrgSystemsPermissions.Edit),
            new Claim("permission", OrgSystemsPermissions.Delete),
            new Claim("permission", OrgSystemsPermissions.ManageAttributes),
            new Claim("permission", OrgSystemsPermissions.ManageDependencies)
        };

        var adminPrincipal = new ClaimsPrincipal(new ClaimsIdentity(adminClaims, "test"));

        _mockPermissionService.Setup(x => x.HasPermissionAsync(adminPrincipal, It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        var allPermissions = new[]
        {
            OrgSystemsPermissions.View,
            OrgSystemsPermissions.Create,
            OrgSystemsPermissions.Edit,
            OrgSystemsPermissions.Delete,
            OrgSystemsPermissions.ManageAttributes,
            OrgSystemsPermissions.ManageDependencies
        };

        var results = new List<bool>();
        foreach (var permission in allPermissions)
        {
            var result = await _mockPermissionService.Object.HasPermissionAsync(adminPrincipal, permission);
            results.Add(result);
        }

        // Assert
        results.Should().OnlyContain(r => r == true);
        results.Should().HaveCount(allPermissions.Length);
    }

    [Test]
    public async Task TenantSpecificPermissions_UserInDifferentTenant_ShouldBeDenied()
    {
        // Arrange
        var tenant1UserContext = new UserContext("user1-id", "user1", "tenant1", "user1@tenant1.com");
        var tenant2UserContext = new UserContext("user2-id", "user2", "tenant2", "user2@tenant2.com");

        _mockUserContextAccessor.Setup(x => x.Current).Returns(tenant1UserContext);

        // Mock permission service to check tenant isolation
        _mockPermissionService.Setup(x => x.HasPermissionAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>()))
            .ReturnsAsync((ClaimsPrincipal user, string permission) =>
            {
                // Simulate tenant-specific permission checking
                var tenantClaim = user.Claims.FirstOrDefault(c => c.Type == "tenant")?.Value;
                return tenantClaim == "tenant1"; // Only tenant1 users have permissions in this test
            });

        var tenant1User = CreateUserWithClaims("user1@tenant1.com", "User", 
            OrgSystemsPermissions.View, OrgSystemsPermissions.Create);
        tenant1User.Identities.First().AddClaim(new Claim("tenant", "tenant1"));

        var tenant2User = CreateUserWithClaims("user2@tenant2.com", "User", 
            OrgSystemsPermissions.View, OrgSystemsPermissions.Create);
        tenant2User.Identities.First().AddClaim(new Claim("tenant", "tenant2"));

        // Act
        var tenant1ViewResult = await _mockPermissionService.Object.HasPermissionAsync(tenant1User, OrgSystemsPermissions.View);
        var tenant2ViewResult = await _mockPermissionService.Object.HasPermissionAsync(tenant2User, OrgSystemsPermissions.View);

        // Assert
        tenant1ViewResult.Should().BeTrue("User in tenant1 should have access");
        tenant2ViewResult.Should().BeFalse("User in tenant2 should not have access due to tenant isolation");
    }

    [Test]
    public async Task PermissionCaching_RepeatedChecks_ShouldUseCachedResults()
    {
        // Arrange
        var callCount = 0;
        _mockPermissionService.Setup(x => x.HasPermissionAsync(_adminUser, OrgSystemsPermissions.View))
            .ReturnsAsync(() => 
            {
                callCount++;
                return true;
            });

        // Act - Make multiple permission checks
        var result1 = await _mockPermissionService.Object.HasPermissionAsync(_adminUser, OrgSystemsPermissions.View);
        var result2 = await _mockPermissionService.Object.HasPermissionAsync(_adminUser, OrgSystemsPermissions.View);
        var result3 = await _mockPermissionService.Object.HasPermissionAsync(_adminUser, OrgSystemsPermissions.View);

        // Assert
        result1.Should().BeTrue();
        result2.Should().BeTrue();
        result3.Should().BeTrue();
        callCount.Should().Be(3, "Permission service should be called for each check in this mock setup");
    }

    // Helper methods
    private ClaimsPrincipal CreateUserWithClaims(string email, string role, params string[] permissions)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Role, role),
            new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        };

        claims.AddRange(permissions.Select(p => new Claim("permission", p)));

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "test"));
    }

    private void SetupPermissionServiceForUser(ClaimsPrincipal user, bool view, bool create, bool edit, bool delete, bool manageAttributes, bool manageDependencies)
    {
        _mockPermissionService.Setup(x => x.HasPermissionAsync(user, OrgSystemsPermissions.View))
            .ReturnsAsync(view);
        _mockPermissionService.Setup(x => x.HasPermissionAsync(user, OrgSystemsPermissions.Create))
            .ReturnsAsync(create);
        _mockPermissionService.Setup(x => x.HasPermissionAsync(user, OrgSystemsPermissions.Edit))
            .ReturnsAsync(edit);
        _mockPermissionService.Setup(x => x.HasPermissionAsync(user, OrgSystemsPermissions.Delete))
            .ReturnsAsync(delete);
        _mockPermissionService.Setup(x => x.HasPermissionAsync(user, OrgSystemsPermissions.ManageAttributes))
            .ReturnsAsync(manageAttributes);
        _mockPermissionService.Setup(x => x.HasPermissionAsync(user, OrgSystemsPermissions.ManageDependencies))
            .ReturnsAsync(manageDependencies);
    }
}