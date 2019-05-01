using System;
using System.Collections.Generic;

namespace TimeImportApp
{
    public interface ICSVHelper
    {
        List<Person> ParseReportToObjects(string reportFilePath);
    }
}
