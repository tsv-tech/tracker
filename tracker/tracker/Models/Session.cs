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
    }
}
