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
        PwsSaveTimeCardsRs SaveTimeCard(string sessionKey, PwsTimecardDetail timeCard, PwsUserElement userDetails);
        PwsTimecardDetail GenerateNewTimeCard();
        int CalculateTimeOffInYearToDate(PwsGetTimeCardsRs timeCard);
        PwsGetTimeCardsRs GetExistingTimeCard(string sessionKey, PwsUserElement userDetails);
        string GenerateSessionKey(IPwsProjectorServices pwsProjectorServices, string username, string password, string accountCode);
        PwsGetExpenseReportsRs GetExpenseReport(string sessionKey);
        PwsUserElement GetUserDetails(string userName, string sessionKey);
        bool AddTimeCards(List<Person> people);
        List<ErrorOccurance> GetErrors();
    }   
}
