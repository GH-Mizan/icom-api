using Abp.Zero.EntityFrameworkCore;
using Icom.Authorization.Roles;
using Icom.Authorization.Users;
using Icom.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace Icom.EntityFrameworkCore;

public class IcomDbContext : AbpZeroDbContext<Tenant, Role, User, IcomDbContext>
{
    /* Define a DbSet for each entity of the application */

    public IcomDbContext(DbContextOptions<IcomDbContext> options)
        : base(options)
    {
    }
}
