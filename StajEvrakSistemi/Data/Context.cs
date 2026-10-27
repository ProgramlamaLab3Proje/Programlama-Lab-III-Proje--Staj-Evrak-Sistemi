using Microsoft.EntityFrameworkCore;
using StajEvrakSistemi.Models;

namespace StajEvrakSistemi.Data
{
    public class Context : DbContext
    {
        public DbSet<Student> Students {  get; set; }
        public DbSet<Admin>Admins { get; set; }
        public DbSet<StudentRecord> StudentRecords { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source = stajevraksistemverileri.db");
        }
    }
}
