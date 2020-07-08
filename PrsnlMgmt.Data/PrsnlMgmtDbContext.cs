using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace PrsnlMgmt.Data
{
    public class PrsnlMgmtDbContext : DbContext
    {
        public static string DefaultConnectionStringName = "PrsnlMgmtDb";

        public PrsnlMgmtDbContext() : base(DefaultConnectionStringName)
        {

        }

        public PrsnlMgmtDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {

        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeRole> EmployeeRoles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
