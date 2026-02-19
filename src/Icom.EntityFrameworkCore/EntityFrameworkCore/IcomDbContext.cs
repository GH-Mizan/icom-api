using Abp.Zero.EntityFrameworkCore;
using Castle.Core.Resource;
using Icom.Authorization.Roles;
using Icom.Authorization.Users;
using Icom.Entities;
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

    public DbSet<Brand> Brands { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<Pricelist> Pricelists { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }

    public DbSet<Asset> Assets { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Due> Dues { get; set; }
    public DbSet<RegularExpense> RegularExpenses { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<ServiceExpense> ServiceExpenses { get; set; }
    public DbSet<SetupExpense> SetupExpenses { get; set; }
    public DbSet<SaleDetail> SaleDetails { get; set; }
    public DbSet<DueReceivedHistory> DueReceivedHistories { get; set; }
    public DbSet<ServiceDueReceivedHistory> ServiceDueReceivedHistories { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
}
