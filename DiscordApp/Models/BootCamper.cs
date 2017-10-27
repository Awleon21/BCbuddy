namespace DiscordApp.Models
{
    using System;
    using System.Collections.Generic;

    public class BootCamper
    {
        public Int32 BootcamperId { get; set; }

        public string DiscordUserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public List<TimeSheet> TimeLogs { get; set; }
    }
}
