using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace tracker.Models
{
    [Table("Sessions")]
    public class Day
    {
        [PrimaryKey]
        public int? Id { get; set; }
        public int ProjectId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }

        public Day()
        {

        }
        public Day(int projectId, DateTime date, TimeSpan time)
        {
            ProjectId = projectId;
            Date = date;
            Time = time;
        }

        public string GetDate { get => Date.ToString("d"); }
        public string GetTime
        {
            get => string.Format("{0:D2}:{1:mm}:{1:ss}",
                (int)Time.TotalHours,
                Time);
        }

    }
}
