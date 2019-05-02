using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeImportApp.Interfaces
{
    public interface ISeedUtility
    {
        void SeedDatabase(ProjectorIntegrationContext projectorIntegrationContext);
    }
}
