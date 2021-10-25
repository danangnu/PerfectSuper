using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Accrual> tblAccrual { get; set; }
        public DbSet<Employees> tblEmployees { get; set; }
        public DbSet<Payroll> tblPayroll { get; set; }
        public DbSet<DataRecords> tblDataRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Accrual>()
                .Property(p => p.SuperGuarantee)
                .HasPrecision(precision: 18, scale: 2);
        }
    }
}