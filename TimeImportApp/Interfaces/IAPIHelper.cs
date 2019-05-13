using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeImportApp.Domain;
using TimeImportApp.ProjectorWebServicesV2;

namespace TimeImportApp
{
    public  interface IAPIHelper
    {
        string GetSessionTicket(string accountCode, string userName, string password);  
        bool AuthenticateUser(string accountCode, string userName, string password);
        bool AddTimeCards(List<Person> people, string sessionTicket);
        List<ErrorOccurance> GetErrors();
    }   
}
