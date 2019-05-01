using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeImportApp
{
    public class DatabaseHelper : IDatabaseHelper   
    {
        public DatabaseHelper() { }

        public void CreateNewDatabaseIfNoneExists()
        {
            using (var context = new ProjectorIntegrationcContext())
            {
                Database.SetInitializer(new CreateDatabaseIfNotExists<ProjectorIntegrationcContext>());
                context.Database.Initialize(true);
            }
        }
    }

}
