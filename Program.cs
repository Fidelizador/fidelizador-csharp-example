
using System.IO;
using FidelizadorApiClient;

namespace Result
{
    public class ApiTest
    {
        public static void Main()
        {
            string client_id = "CLIENT_ID";
            string client_secret = "CLIENT_SCRET";
            string client_slug = "SLUG";

            var api = new FidelizadorClient(client_id, client_secret, client_slug);
            Console.WriteLine("/=======================================/");
            Console.WriteLine("Obtaining lists related to a specific name filter");
            var lists = api.GetLists("new list");
            lists.Wait();
            Console.Write("Printing first result: ");
            Console.WriteLine(lists.Result.Data[0]);
            Console.WriteLine("/=======================================/");
            Console.WriteLine("Creating a new list called 'new list'");
            var newList = api.CreateList("new list");
            newList.Wait();
            Console.Write("New list id: ");
            Console.WriteLine(newList.Result.Id);
            Console.WriteLine("/=======================================/");
            Console.WriteLine("Starting a new import on the new list");
            string filePath = Path.GetFullPath("./files/example.csv");
            Console.WriteLine(filePath);
            var importResult = api.StartImport(newList.Result.Id, filePath);
            importResult.Wait();
            Console.Write("New import result: ");
            Console.WriteLine(importResult.Result);
            Console.WriteLine("/=======================================/");
            Console.WriteLine("Creating a new campaign");
            var campaign = api.CreateCampaign(
                "My campaign from c#",
                1,
                newList.Result.Id,
                1,
                "Welcome to my Campaign",
                "My company",
                "contact@example.org",
                "contact@example.org");
            campaign.Wait();
            Console.Write("New Campaign: ");
            Console.WriteLine(campaign.Result);
            Console.WriteLine("/=======================================/");
        }
    }
}
