using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeImportApp.Domain;
using TimeImportApp.ProjectorWebServicesV2;

namespace TimeImportApp
{
    public class APIHelper : IAPIHelper
    {
        private PwsProjectorServicesClient pwsProjectorServices;
        private PwsAuthenticateRs authenticationResponse;
        private Dictionary<Person, List<PwsSaveTimecardResult>> failedTimecards;

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
                Console.WriteLine(e.StackTrace);
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

        private PwsResourceElement[] GetProjectorResource(List<Person> person, string sessionTicket)
        {
            PwsResourceRef[] userList = new PwsResourceRef[person.Count];
            for (int i = 0; i < person.Count; i++)
            {
                userList[i] = new PwsResourceRef()
                {
                    ResourceDisplayName = person[i].Name
                };
            }
            PwsGetResourceRq getResourceRq = new PwsGetResourceRq()
            {
                SessionTicket = sessionTicket,
                ResourceIdentities = userList
            };
            return pwsProjectorServices.PwsGetResource(getResourceRq).Resources;
        }

        private PwsProjectElement GetPwsProject(string projectCode, string sessionTicket)
        {
            PwsGetProjectRq getProjectRq = new PwsGetProjectRq();
            PwsProjectRef projectRef = new PwsProjectRef();
            projectRef.ProjectCode = projectCode;
            PwsProjectRef[] projectRefs = { projectRef };
            getProjectRq.ProjectIdentities = projectRefs;
            getProjectRq.SessionTicket = sessionTicket;
            PwsGetProjectRs getProjectRs = pwsProjectorServices.PwsGetProject(getProjectRq);
            return getProjectRs.Projects[0];
        }

        private PwsTimecardDetail[] CreateTimeCards(Person person, string sessionTicket)
        {
            List<PwsTimecardDetail> timecardDetails = new List<PwsTimecardDetail>();
            foreach (Job job in person.Jobs)
            {
                if (job.SoldHours > 0)
                {
                    PwsProjectElement pwsProject = GetPwsProject(job.Code, sessionTicket);
                    if (pwsProject != null)
                    {
                        PwsTimecardDetail timeCard = new PwsTimecardDetail();
                        timeCard.CardStatus = "D";
                        timeCard.ProjectIdentity = new PwsProjectRef() { ProjectUid = pwsProject.ProjectDetail.ProjectUid };
                        timeCard.ProjectRateTypeIdentity = new PwsProjectRateTypeRef() { ProjectRateTypeUid = pwsProject.RateTypes[0].ProjectRateTypeDetail.ProjectRateTypeUid };
                        timeCard.ProjectTaskIdentity = new PwsProjectTaskRef() { ProjectTaskUid = pwsProject.Tasks[0].ProjectTaskDetail.ProjectTaskUid };
                        timeCard.WorkMinutes = (int)(job.SoldHours * 60);
                        //Date details need figured out and added here
                        timecardDetails.Add(timeCard);
                    }
                    else
                    {
                        // TODO: Exception handling
                    }
                }
            }
            return timecardDetails.ToArray();
        }

        private PwsTimeOffCardDetail[] CreateTimeOffCards(Person person, string sessionTicket)
        {
            List<PwsTimeOffCardDetail> pwsTimeOffCards = new List<PwsTimeOffCardDetail>();
            foreach (Job job in person.Jobs)
            {
                if (job.AnnualHolidayHours > 0)
                {
                    PwsTimeOffCardDetail pwsTimeOffCardDetail = new PwsTimeOffCardDetail();
                    pwsTimeOffCardDetail.CardStatus = "D";
                    pwsTimeOffCardDetail.WorkDate = DateTime.Now; //TODO: Replace with actual date data
                    pwsTimeOffCardDetail.WorkMinutes = (int)(job.AnnualHolidayHours * 60);
                    pwsTimeOffCardDetail.TimeOffReasonIdentity = new PwsTimeOffReasonRef()
                    {
                        TimeOffReasonName = "Vacation"
                    };
                    pwsTimeOffCards.Add(pwsTimeOffCardDetail);
                }
                else if (job.IllnessHours > 0)
                {
                    PwsTimeOffCardDetail pwsTimeOffCardDetail = new PwsTimeOffCardDetail();
                    pwsTimeOffCardDetail.CardStatus = "D";
                    pwsTimeOffCardDetail.WorkDate = DateTime.Now; //TODO: Replace with actual date data
                    pwsTimeOffCardDetail.WorkMinutes = (int)(job.IllnessHours * 60);
                    pwsTimeOffCardDetail.TimeOffReasonIdentity = new PwsTimeOffReasonRef()
                    {
                        TimeOffReasonName = "Sick"
                    };
                    pwsTimeOffCards.Add(pwsTimeOffCardDetail);
                }
            }
            return pwsTimeOffCards.ToArray();
        }

        private PwsSaveTimeCardsRs SaveTimeCards(PwsTimecardDetail[] timeCards, PwsTimeOffCardDetail[] timeOffCards, PwsResourceRef user, string sessionTicket)
        {
            PwsSaveTimeCardsRq saveTimeCardsRq = new PwsSaveTimeCardsRq();
            saveTimeCardsRq.SessionTicket = sessionTicket;
            saveTimeCardsRq.StartDate = DateTime.Now; // Time details need sorted
            saveTimeCardsRq.EndDate = DateTime.Now; // As above
            saveTimeCardsRq.ResourceIdentity = user;
            if (timeCards.Length > 0)
            {
                saveTimeCardsRq.SaveTimeCards = timeCards;
            }
            if (timeOffCards.Length > 0)
            {
                saveTimeCardsRq.SaveTimeOffCards = timeOffCards;
            }
            return pwsProjectorServices.PwsSaveTimeCards(saveTimeCardsRq);
        }

        public bool AddTimeCards(List<Person> people, string sessionTicket)
        {
            //Check which people are projector users
            PwsResourceElement[] pwsUsers = GetProjectorResource(people, sessionTicket);
            //Map together relevant users, could be improved
            Dictionary<Person, PwsResourceElement> userMap = new Dictionary<Person, PwsResourceElement>();
            foreach (Person person in people)
            {
                foreach (PwsResourceElement pwsUser in pwsUsers)
                {
                    if (person.Name.Equals(pwsUser.ResourceDetail.ResourceDisplayName))
                    {
                        userMap.Add(person, pwsUser);
                    }
                }
            }
            //Create timecards for the jobs for each user and try to save
            failedTimecards = new Dictionary<Person, List<PwsSaveTimecardResult>>();
            foreach (KeyValuePair<Person, PwsResourceElement> keyValuePair in userMap)
            {
                PwsTimecardDetail[] timeCards = CreateTimeCards(keyValuePair.Key, sessionTicket);
                PwsTimeOffCardDetail[] timeOffCards = CreateTimeOffCards(keyValuePair.Key, sessionTicket);
                PwsSaveTimeCardsRs saveTimeCardsRs = SaveTimeCards(timeCards, timeOffCards, keyValuePair.Value.ResourceDetail, sessionTicket);
                PwsSaveTimecardResult[] results = saveTimeCardsRs.TimecardResults;
                PwsSaveTimeOffCardResult[] resultsTimeOff = saveTimeCardsRs.TimeOffCardResults;
                List<PwsSaveTimecardResult> failedCards = new List<PwsSaveTimecardResult>();
                List<ErrorOccurance> errors = new List<ErrorOccurance>();
                //handle any errors:
                foreach (PwsSaveTimecardResult result in results)
                {
                    if (result.ErrorDetail != null)
                    {
                        switch (result.ErrorDetail.ErrorNumber)
                        {
                            case 1:
                                errors.Add(new ErrorOccurance()
                                {
                                    Error = new Error()
                                    {
                                        Type = ErrorType.IncorrectFormatFromComment,
                                        ErrorID = result.ErrorDetail.ErrorNumber
                                    },
                                    Person = keyValuePair.Key
                                });
                                break;
                            case 54282:
                                errors.Add(new ErrorOccurance()
                                {
                                    Error = new Error()
                                    {
                                        Type = ErrorType.JobInMappingTableButNotInProjector,
                                        ErrorID = result.ErrorDetail.ErrorNumber
                                    },
                                    Person = keyValuePair.Key
                                });
                                break;
                            case 3:
                                errors.Add(new ErrorOccurance()
                                {
                                    Error = new Error()
                                    {
                                        Type = ErrorType.JobNotInMappingTable,
                                        ErrorID = result.ErrorDetail.ErrorNumber
                                    },
                                    Person = keyValuePair.Key
                                });
                                break;
                            case 4:
                                errors.Add(new ErrorOccurance()
                                {
                                    Error = new Error()
                                    {
                                        Type = ErrorType.JobNotOnProjectorFromComment,
                                        ErrorID = result.ErrorDetail.ErrorNumber
                                    },
                                    Person = keyValuePair.Key
                                });
                                break;
                        }
                    }
                }
                failedTimecards.Add(keyValuePair.Key, failedCards);
                List<PwsSaveTimeOffCardResult> failedTimeOffCards = new List<PwsSaveTimeOffCardResult>();
                foreach (PwsSaveTimeOffCardResult result in resultsTimeOff)
                {
                    if (result.ErrorDetail != null)
                    {
                        failedTimeOffCards.Add(result);
                    }
                }
            }
            //return true if there were no failures
            if (failedTimecards.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<Person> GetErrors()
        {
            return failedTimecards.Keys.ToList();
        }
    }
}
