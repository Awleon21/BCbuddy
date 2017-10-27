namespace DiscordApp.Models
{
    using System;

    public class TimeSheet
    {
        public int TimeSheetId { get; set; }

        public DateTime LoginTime { get; set; }

        public DateTime LunchInTime { get; set; }

        public DateTime LunchOutTime { get; set; }

        public DateTime LogoutTime { get; set; }
    }
}
