using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeImportApp.ProjectorWebServicesV2;

namespace TimeImportApp
{
    public class APIHelper : IAPIHelper
    {
        private PwsProjectorServicesClient pwsProjectorServices;
        private PwsAuthenticateRs authenticationResponse;

        public APIHelper()
        {
            pwsProjectorServices = new PwsProjectorServicesClient();
        }

        public bool AuthenticateUser(string accountCode, string userName, string password)
        {
            try
            {
                authenticationResponse = pwsProjectorServices.PwsAuthenticate(new PwsAuthenticateRq()
                {
                    AccountCode = accountCode,
                    UserName = userName,
                    Password = password
                });

                //If request fails bounce out;
                return authenticationResponse.Status == RequestStatus.Ok && authenticationResponse.SessionTicket != null;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public string GetSessionTicket(string accountCode, string userName, string password)
        {
            return GetSessionTicket(ref pwsProjectorServices, accountCode, userName, password);
        }
        private string GetSessionTicket(ref PwsProjectorServicesClient pwsProjectorServices, string accountCode, string userName, string password)
        {
            PwsAuthenticateRs rs = pwsProjectorServices.PwsAuthenticate(new PwsAuthenticateRq()
            {
                AccountCode = accountCode,
                UserName = userName,
                Password = password
            });

            string sessionTicket = rs.SessionTicket;
            //if request fails bounce out
            if (rs.Status != RequestStatus.Ok)
            {
                return null;
            }

            //To prevent infinite recursion, only try to reconnect if RedirectUrl is different from current url            
            if (rs.RedirectUrl != null && pwsProjectorServices.Endpoint.Address.Uri.AbsoluteUri.StartsWith(rs.RedirectUrl))
            {
                return null;
            }

            //If a RedirectUrl was returned then your account data is on a different server. Retry with new url.
            if (rs.RedirectUrl != null)
            {
                Uri uri = new Uri(pwsProjectorServices.Endpoint.Address.Uri.ToString());
                string newUriText = rs.RedirectUrl + uri.LocalPath;
                pwsProjectorServices = new PwsProjectorServicesClient("BasicHttpBinding_IPwsProjectorServices", newUriText);
                string session = GetSessionTicket(ref pwsProjectorServices, accountCode, userName, password);
                return session;
            }
            return rs.SessionTicket;
        }

        public PwsSaveTimeCardsRs SaveTimeCard(string sessionKey, PwsTimecardDetail timeCard, PwsUserElement userDetails)
        {
            PwsTimecardDetail[] timeCards = { timeCard };
            PwsSaveTimeCardsRq timeCardsRq = new PwsSaveTimeCardsRq();
            timeCardsRq.SessionTicket = sessionKey;
            timeCardsRq.SaveTimeCards = timeCards;
            timeCardsRq.ResourceIdentity = userDetails.ResourceIdentity;
            timeCardsRq.StartDate = DateTime.Parse("2019-04-24T00:00:00Z").ToUniversalTime();
            timeCardsRq.EndDate = DateTime.Parse("2019-04-25T00:00:00Z").ToUniversalTime();
            return pwsProjectorServices.PwsSaveTimeCards(timeCardsRq);
        }

        public PwsTimecardDetail GenerateNewTimeCard()
        {
            //Create a new time card - WIP
            PwsTimecardDetail pwsTimecardDetail = new PwsTimecardDetail();
            //For a new timecard to be valid the below must be set as a minimum
            pwsTimecardDetail.WorkMinutes = 450;
            pwsTimecardDetail.WorkDate = DateTime.Parse("2019-04-24T00:00:00Z").ToUniversalTime();
            pwsTimecardDetail.CardStatus = "D";

            //Also needs the below, each needing at least one of the ID types
            PwsProjectRef pwsProjectRef = new PwsProjectRef();
            pwsProjectRef.ProjectCode = "";
            PwsProjectRateTypeRef pwsProjectRateTypeRef = new PwsProjectRateTypeRef();
            pwsProjectRateTypeRef.ExternalSystemIdentifier = "";
            PwsProjectTaskRef pwsProjectTaskRef = new PwsProjectTaskRef();
            pwsProjectTaskRef.ExternalSystemIdentifier = "";
            PwsProjectRoleRef pwsProjectRoleRef = new PwsProjectRoleRef();
            pwsProjectRoleRef.ExternalSystemIdentifier = "";

            pwsTimecardDetail.ProjectIdentity = pwsProjectRef;
            pwsTimecardDetail.ProjectRateTypeIdentity = pwsProjectRateTypeRef;
            pwsTimecardDetail.ProjectTaskIdentity = pwsProjectTaskRef;
            pwsTimecardDetail.RoleIdentity = pwsProjectRoleRef;
            return pwsTimecardDetail;
        }

        public int CalculateTimeOffInYearToDate(PwsGetTimeCardsRs timeCard)
        {
            //Iterate through time off projects and total YTD timeoff
            PwsTimeEntryTimeOff[] pwsTimeEntryProject = timeCard.TimeEntryTimeOff;
            int minYtd = 0;

            foreach (PwsTimeEntryTimeOff timeOff in pwsTimeEntryProject)
            {
                minYtd += timeOff.MinutesYearToDate;
            }

            return minYtd;
        }

        public PwsGetTimeCardsRs GetExistingTimeCard(string sessionKey, PwsUserElement userDetails)
        {
            //Create timecard request
            PwsGetTimeCardsRq timeCardsRq = new PwsGetTimeCardsRq();
            timeCardsRq.SessionTicket = sessionKey;
            timeCardsRq.StartDate = DateTime.Parse("2019-04-24T00:00:00Z").ToUniversalTime();
            timeCardsRq.EndDate = DateTime.Parse("2019-04-25T00:00:00Z").ToUniversalTime();
            timeCardsRq.ResourceIdentity = userDetails.ResourceIdentity;

            //Send request and check response status
            return pwsProjectorServices.PwsGetTimeCards(timeCardsRq);
        }

        public string GenerateSessionKey(IPwsProjectorServices pwsProjectorServices, string username, string password, string accountCode)
        {
            //Authenticate user and print session key
            Session session = new Session(ref pwsProjectorServices, accountCode, username, password);
            string sessionKey = session.GetSessionTicket();
            return sessionKey;
            //Using session key create and expense report request, and print out request status
        }

        public PwsGetExpenseReportsRs GetExpenseReport(string sessionKey)
        {
            //Using session key create and expense report request, and print out request status
            PwsGetExpenseReportsRq pwsGetExpenseReportsRq = new PwsGetExpenseReportsRq();
            pwsGetExpenseReportsRq.SessionTicket = sessionKey;
            return pwsProjectorServices.PwsGetExpenseReports(pwsGetExpenseReportsRq);
        }

        public PwsUserElement GetUserDetails(string userName, string sessionKey)
        {
            PwsGetUserRq getUserRq = new PwsGetUserRq();
            getUserRq.SessionTicket = sessionKey;
            PwsUserRef userRef = new PwsUserRef();
            userRef.UserDisplayName = userName;
            PwsUserRef[] userArray = new PwsUserRef[] { userRef };
            getUserRq.UserIdentities = userArray;
            PwsGetUserRs getUserRs = pwsProjectorServices.PwsGetUser(getUserRq);
            return getUserRs.Users[0];
        }
    }
}
