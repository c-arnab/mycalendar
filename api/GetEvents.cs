using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Arnab.MyCalendar
{
     public static class GetEvents
    {
        [FunctionName("GetEvents")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "events")] HttpRequest req,
            [CosmosDB(
                databaseName: "myCalendar",
                containerName: "eventsCollection",
                Connection = "CosmosDBConnection")] CosmosClient client,
            ILogger log)
        {
            log.LogInformation("Get Events list");
            string name = req.Query["u"];

            List<Dictionary<string, string>> results=new  List<Dictionary<string, string>>();
            Container myContainer = client.GetDatabase("myCalendar").GetContainer("eventsCollection");
            QueryDefinition queryDefinition = new QueryDefinition(
                "SELECT * FROM items i WHERE (i.userName = @searchterm)")
                .WithParameter("@searchterm", name);
            string continuationToken = null;
            do
            {
            FeedIterator<CalendarEvent> feedIterator = 
                myContainer.GetItemQueryIterator<CalendarEvent>(
                        queryDefinition, 
                        continuationToken: continuationToken);

                while (feedIterator.HasMoreResults)
                {
                    FeedResponse<CalendarEvent> feedResponse = await feedIterator.ReadNextAsync();
                    continuationToken = feedResponse.ContinuationToken;
                    foreach (CalendarEvent item in feedResponse)
                    {
                        results.Add(new Dictionary<string, string>(){
                                    {"start", item.startsAt},
                                    {"end", item.endsAt},
                                    {"id", item.id},
                                    {"text", item.eventTitle},
                                    {"barColor", item.barColor}
                        });
                    }
                }
            } while (continuationToken != null);

            return new OkObjectResult(results);

        }
    } 
}