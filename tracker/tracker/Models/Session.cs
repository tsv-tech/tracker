using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace tracker.Models
{
    [Table("Sessions")]
    public class Session
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }
        public TimeSpan Duration { get; set; }

        public Session()
        {

        }
        public Session(int projectId, DateTime startTime, DateTime stopTime, TimeSpan duration)
        {
            ProjectId = projectId;
            StartTime = startTime;
            StopTime = stopTime;
            Duration = duration;
        }

        public string GetStartTime { get => StartTime.ToString("HH:mm:ss"); }
        public string GetStopTime { get => StopTime.ToString("HH:mm:ss"); }
        public string GetDuration { get => string.Format("{0:D2}:{1:mm}:{1:ss}",
                     (int)Duration.TotalHours,
                     Duration);
        }

    }
}
