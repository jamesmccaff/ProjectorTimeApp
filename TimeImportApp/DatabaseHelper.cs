using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeImportApp.Domain;
using TimeImportApp.Interfaces;

namespace TimeImportApp
{
    public class DatabaseHelper : IDatabaseHelper   
    {
        private ISeedUtility seedUtility;
        public DatabaseHelper() { }

        public void CreateNewDatabaseIfNoneExists()
        {
            seedUtility = new SeedUtility();
            using (var context = new ProjectorIntegrationContext())
            {
                Database.SetInitializer(new CreateDatabaseIfNotExists<ProjectorIntegrationContext>());
                context.Database.Initialize(true);
                seedUtility.SeedDatabase(context);
            }
        }
    }

}
