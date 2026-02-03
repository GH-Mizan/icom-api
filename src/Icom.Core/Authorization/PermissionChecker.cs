using Abp.Authorization;
using Icom.Authorization.Roles;
using Icom.Authorization.Users;

namespace Icom.Authorization;

public class PermissionChecker : PermissionChecker<Role, User>
{
    public PermissionChecker(UserManager userManager)
        : base(userManager)
    {
    }
}
