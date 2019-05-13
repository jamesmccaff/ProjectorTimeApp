using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeImportApp.Domain;

namespace TimeImportApp
{
    public class ProjectorIntegrationContext : DbContext
    {
        public DbSet<ProjectorProject> ProjectorProjects { get; set; }
        public DbSet<MIPACProject> MIPACProjects { get;set; }
        public DbSet<SharedProject> SharedProjects { get; set; }
        public DbSet<Error> Errors { get; set; }

        public ProjectorIntegrationContext() : base(nameOrConnectionString: "Default")
        {
            this.Configuration.LazyLoadingEnabled = true;
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // PostgreSQL uses the public schema by default - not dbo.
            modelBuilder.HasDefaultSchema("public");
            base.OnModelCreating(modelBuilder);
        }
    }
}
