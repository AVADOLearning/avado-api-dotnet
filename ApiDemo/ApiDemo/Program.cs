using System;
using System.Configuration;
using System.Linq;
using System.Security.Authentication;
using ApiDemo.FloreamAuthApi;
using ApiDemo.FloreamSaleslogixApi;
using BusinessLogicService.Security;
using ResponseStatus = ApiDemo.FloreamAuthApi.ResponseStatus;

namespace ApiDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Generate auth token to use for SalesLogix service. 
            var lAuthClient = new AuthenticationClient();
            var lAuthResponse = lAuthClient.Authenticate(new AuthenticationEntitiesAuthenticationRequest
            {
                Username = ConfigurationManager.AppSettings["username"],
                Password = ConfigurationManager.AppSettings["password"]
            });
            if (lAuthResponse.Status != ResponseStatus.Success)
            {
                throw new AuthenticationException("Unable to authenticate");
            }
            var lToken = lAuthResponse.Token;


            var lSalesLogixClientService = new SalesLogixClient();
            lSalesLogixClientService.Endpoint.Binding.ReceiveTimeout = new TimeSpan(30, 0, 0);
            lSalesLogixClientService.Endpoint.Binding.SendTimeout = new TimeSpan(30, 0, 0);
            // Inject token into the service for each request.
            lSalesLogixClientService.Endpoint.Behaviors.Add(
                new ClientMessageInspector
                {
                    AuthToken = lToken
                }
            );

            // Note: All fields present below are required to submit.
            var lResponse = lSalesLogixClientService.CreateLead(
                new CreateLeadEntitiesCreateLeadRequest
                {
                    LeadSource = new CreateLeadEntitiesLeadSource
                    {
                        BrandAssociation = CreateLeadEntitiesBrandAssociation.Floream,
                        CampaignCode = "IDMCVF0016" // This is a leadsource and might change.
                    },
                    CourseInterests = new []{ "AAA" }, // Course code that the user enquired about
                    Contacts = new []
                    {
                        new Contact
                        {
                            Type = ContactType.Lead,
                            FirstName = "Joe",
                            LastName = "Doe",
                            Email = "testlead@homelearningcollege.com",
                            HomeTelephone = "02086766255", // Mobile and work number properties exist as well
                            Reference = "Your unique reference" // Reference in your database
                        }
                    },
                    Questions = new CreateLeadEntitiesQuestions
                    {
                        PayForCourse = CreateLeadEntitiesPlanToPay.Employer, // If the user's employer will sponsor them
                        EnrolmentExpectation = DateTime.UtcNow.AddMonths(6), // The user wishes to enrol is six months time
                    }
                }
            );

            if (lResponse.Status == FloreamSaleslogixApi.ResponseStatus.Success)
            {
                Console.WriteLine("Lead created!");
                Console.WriteLine(lResponse.Contacts.First().Identifier); // If one contact is created, contact in response should exist.
            }
            else
            {
                // Always look at message details for extra information.
                Console.WriteLine(lResponse.Message);
                if (lResponse.MessageDetails == null || lResponse.MessageDetails.Length <= 0) return;

                foreach (var lMessage in lResponse.MessageDetails)
                {
                    Console.WriteLine("- {0}", lMessage.Message);
                }
            }

            Console.ReadLine();
        }
    }
}
