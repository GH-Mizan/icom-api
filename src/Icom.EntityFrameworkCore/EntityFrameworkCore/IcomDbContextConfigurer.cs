using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Icom.EntityFrameworkCore;

public static class IcomDbContextConfigurer
{
    public static void Configure(DbContextOptionsBuilder<IcomDbContext> builder, string connectionString)
    {
        builder.UseSqlServer(connectionString);
    }

    public static void Configure(DbContextOptionsBuilder<IcomDbContext> builder, DbConnection connection)
    {
        builder.UseSqlServer(connection);
    }
}
