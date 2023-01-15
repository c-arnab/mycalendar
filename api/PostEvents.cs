using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Arnab.MyCalendar
{
    public static class PostEvents
    {
        [FunctionName("PostEvents")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "events")] HttpRequest req,
            [CosmosDB(
            databaseName: "myCalendar",
            containerName: "eventsCollection",
            Connection = "CosmosDBConnection")]
            IAsyncCollector<CalendarEvent> eventsOut,ILogger log)
        {
            log.LogInformation("Post Event");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            ClientData data = JsonSerializer.Deserialize<ClientData>(requestBody);
            string cuser = data?.cuser;
            ClientPostEvent cevent=data?.cevent;
            log.LogInformation(cuser);
            string eguid =Guid.NewGuid().ToString(); 
            cevent.id=eguid;
            CalendarEvent nevent = new CalendarEvent() { 
                id = cevent.id,
                userName= cuser,
                startsAt=cevent.start,
                endsAt=cevent.end,
                eventTitle=cevent.text,
                barColor=cevent.barColor,
                eventCreateDate=DateTime.Now
                };

            await eventsOut.AddAsync(nevent);
            return new OkObjectResult(cevent);
        }
    }
}