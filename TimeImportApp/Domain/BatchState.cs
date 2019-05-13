using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeImportApp.Domain
{
    public enum BatchState
    {
        CompletedSuccesfully=0,
        CompletedWithErrors=1,
        Failed=2
    }
}
