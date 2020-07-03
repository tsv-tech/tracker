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
        public string Date { get; set; }
        public string Duration { get; set; }

        public Session()
        {

        }
    }
}
