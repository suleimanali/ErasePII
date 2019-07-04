using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using ErasePII.SystemUser;
using ErasePII.WebApiApplicationUser;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Xrm.Tools.WebAPI;

namespace ErasePII
{
    class Program
    {
        /// <summary>
        /// Update the organization settings of the config file....
        /// </summary>
        private static CrmServiceClient _client;
        public static string TypeOfErase { get; private set; }

        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Started");
                ///Connect to API...
                var applicationUser = ConfigurationManager.AppSettings["ApplicationUser"].ToLower();
                if (applicationUser == "yes")
                {
                    var resource = ConfigurationManager.AppSettings["Resource"];
                    var applicationId = ConfigurationManager.AppSettings["ApplicationId"];
                    var applicationSecret = ConfigurationManager.AppSettings["ApplicationSecret"];
                    var authority = ConfigurationManager.AppSettings["Authority"];

                    var api = CrmConnection.ConnectToCRMWithApplicationUser(authority, applicationSecret, applicationId,
                        resource);
                    ErasePiiAppStarterApplicationUser(api);
                }
                else
                {
                    //ErasePiiAppStarterSystemUser();
                }
                Console.ReadLine();
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                var message = ex.Message;
                throw;
            }
        }


        private static void ErasePiiAppStarterSystemUser()
        {
            int appCounter = 0;
            Console.WriteLine("------------------------------------------------------------------------");
            Console.WriteLine("This tool helps with purging data in an entity per run/session");
            Console.WriteLine("------------------------------------------------------------------------");

            const string appStarter = "Erase Field or Record?\nFor Field Enter (F)\nFor Record Enter (R)" +
                                      "\nMultiple fields in an entity(MF)";

            Console.WriteLine("Erase PII tool started.....\n");
            Console.WriteLine(appStarter);
            TypeOfErase = Console.ReadLine()?.ToLower();
            if (appCounter <= 3)
            {
                if (string.IsNullOrEmpty(TypeOfErase))
                {
                    Console.WriteLine("Please enter a type of erase session:" +
                                      "\nJust records(R)" +
                                      "\nField(F)" +
                                      "\nMultiple Fields in an Entity(MF)");
                    TypeOfErase = Console.ReadLine();
                    appCounter++;
                    ErasePiiAppStarterSystemUser();
                }
                else if (!string.Equals(TypeOfErase, "r", StringComparison.OrdinalIgnoreCase) && !string.Equals(
                             TypeOfErase, "f", StringComparison.OrdinalIgnoreCase) && !string.Equals(
                             TypeOfErase, "mf", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine(appStarter);
                    Console.WriteLine(
                        "You should type either:\n(R) for Records.\n(F) for Field.\nMultiple Fields in an Entity(MF).");
                    TypeOfErase = Console.ReadLine();
                    appCounter++;
                    ErasePiiAppStarterSystemUser();
                }
                else if (!string.IsNullOrWhiteSpace(TypeOfErase))
                    if (string.Equals(TypeOfErase, "r", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(TypeOfErase, "f", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(TypeOfErase, "mf", StringComparison.OrdinalIgnoreCase))
                        StartApp.SystemUser(TypeOfErase,_client);
                Console.WriteLine("Done....");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("The app is shutting down....");
                Console.ReadLine();
            }
        }

        private static void ErasePiiAppStarterApplicationUser(Tuple<string, string, CRMWebAPI> api)
        {
            int appCounter = 0;
            Console.WriteLine("------------------------------------------------------------------------");
            Console.WriteLine("This tool helps with purging data in an entity per run/session");
            Console.WriteLine("------------------------------------------------------------------------");

            const string appStarter = "Erase Field or Record?\nFor Field Enter (F)\nFor Record Enter (R)" +
                                      "\nMultiple fields in an entity(MF)";

            Console.WriteLine("Erase PII tool started.....\n");
            Console.WriteLine(appStarter);
            TypeOfErase = Console.ReadLine()?.ToLower();
            if (appCounter <= 3)
            {
                if (string.IsNullOrEmpty(TypeOfErase))
                {
                    Console.WriteLine("Please enter a type of erase session:" +
                                      "\nJust records(R)" +
                                      "\nField(F)" +
                                      "\nMultiple Fields in an Entity(MF)");
                    TypeOfErase = Console.ReadLine();
                    appCounter++;
                    ErasePiiAppStarterApplicationUser(api);
                }
                else if (!string.Equals(TypeOfErase, "r", StringComparison.OrdinalIgnoreCase) && !string.Equals(
                             TypeOfErase, "f", StringComparison.OrdinalIgnoreCase) && !string.Equals(
                             TypeOfErase, "mf", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine(appStarter);
                    Console.WriteLine(
                        "You should type either:\n(R) for Records.\n(F) for Field.\nMultiple Fields in an Entity(MF).");
                    TypeOfErase = Console.ReadLine();
                    appCounter++;
                    ErasePiiAppStarterApplicationUser(api);
                }
                else if (!string.IsNullOrWhiteSpace(TypeOfErase) && 
                         (string.Equals(TypeOfErase, "r", StringComparison.OrdinalIgnoreCase)
                          || string.Equals(TypeOfErase, "f", StringComparison.OrdinalIgnoreCase)
                          || string.Equals(TypeOfErase, "mf", StringComparison.OrdinalIgnoreCase)))
                    StartApp.ApplicationUser(TypeOfErase, api);

                Console.WriteLine("Done....");
                Console.ReadLine();
            }
        }
    }
}