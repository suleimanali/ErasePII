using System;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Xrm.Tools.WebAPI;

namespace ErasePII.WebApiApplicationUser
{
    public class CrmConnection
    {
        private static CRMWebAPI crmWebApi;

        public static Tuple<string, string, CRMWebAPI> ConnectToCRMWithApplicationUser(string authority, string appSecret,
            string clientId, string resource)
        {
            //Console.WriteLine("Authority is: " +authority);
            //Console.WriteLine("App Secret is: " +appSecret);
            //Console.WriteLine("Client Id is: " + clientId);
            //Console.WriteLine("Resource is: " + resource);
            if (authority == null) throw new ArgumentNullException(nameof(authority));
            if (resource == null) throw new ArgumentNullException(nameof(resource));
            if (appSecret == null) throw new ArgumentNullException(nameof(appSecret));
            if (clientId == null) throw new ArgumentNullException(nameof(clientId));

            try
            {
                var clientCredential = new ClientCredential(clientId, appSecret);
               
                var authContext = new AuthenticationContext(authority);
                var authenticationResult = authContext.AcquireToken(resource, clientCredential);
                var crmToken = authenticationResult.AccessToken;
                crmWebApi = new CRMWebAPI(resource + "/api/data/v9.1/", authenticationResult.AccessToken);
                //Console.WriteLine("The CRM Token is: " + crmToken);
                //Console.WriteLine("The client crendential is: " +clientCredential);
                return new Tuple<string, string, CRMWebAPI>(crmToken, resource, crmWebApi);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
