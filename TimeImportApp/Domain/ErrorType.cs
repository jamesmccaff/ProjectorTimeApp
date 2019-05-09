using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeImportApp.Domain
{
    public enum ErrorType
    {
        IncorrectFormatFromComment=0,
        JobNotOnProjectorFromComment=1,
        JobInMappingTableButNotInProjector=2,
        JobNotInMappingTable=3,
        NoPermissionsToSaveTimecards=4
    }
}
