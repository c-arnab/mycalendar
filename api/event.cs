using System;
namespace Arnab.MyCalendar
{
    public class CalendarEvent
    {
        public string id { get; set; }
        public string userName{ get; set; }
        public string startsAt { get; set; }
        public string endsAt { get; set; }
    #nullable enable
        public string? eventTitle { get; set; }
        public string? barColor { get; set; }
        public DateTime eventCreateDate{ get; set; }
    }

    public class ClientPostEvent
    {
        #nullable enable
        public string? id { get; set; }
        public string? start { get; set; }
        public string? end { get; set; }
        public string? text { get; set; }
        public string? barColor { get; set; }
    }

    public class ClientData
    {
        public string cuser { get; set; }
        public ClientPostEvent cevent { get; set; }
    }
}
