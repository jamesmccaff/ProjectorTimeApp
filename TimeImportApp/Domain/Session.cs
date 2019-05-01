using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeImportApp.ProjectorWebServicesV2;

namespace TimeImportApp
{
    public class Session
    {
        private IPwsProjectorServices psc;
        string accountCode, userName, password;

        public Session(ref IPwsProjectorServices psc, string accountCode, string userName, string password)
        {
            this.psc = psc;
            this.accountCode = accountCode;
            this.userName = userName;
            this.password = password;
        }

        public string GetSessionTicket()
        {

            var authenticationRequest = new PwsAuthenticateRq()
            {
                AccountCode = accountCode,
                UserName = userName,
                Password = password
            };

            PwsAuthenticateRs authenticationResponse = psc.PwsAuthenticate(authenticationRequest);

            //If request fails bounce out
            if (authenticationResponse.Status != RequestStatus.Ok)
            {
                return null;
            }

            //TODO: Fix retry

            ////To prevent infinite recursion, only try to reconnect if RedirectUrl is different from current url
            //if (authenticationResponse.RedirectUrl != null && psc.PwsGetDocumentDetails(new PwsGetDocumentDetailsRq {DocumentIdentity=new PwsDocumentRef() { } }).Document.DocumentUri != null)

            //{
            //    return null;
            //}

            //if(authenticationResponse.RedirectUrl != null && psc.document)

            ////If a RedirectUrl was returned then account data is on a different server. Retry with new url.
            //if (rs.RedirectUrl != null)
            //{
            //    Uri uri = new Uri(psc.Url);
            //    psc.Url = rs.RedirectUrl + uri.LocalPath;
            //    return GetSessionTicket();
            //}

            return authenticationResponse.SessionTicket;
        }
    }
}
