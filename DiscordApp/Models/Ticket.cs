namespace DiscordApp.Models
{
    using System;

    public class Ticket
    {
        public Int64 TicketId { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }

        public string SubmittedBy { get; set; }

        public string Status { get; set; }

        public DateTime  Date { get; set; }
    }
}
