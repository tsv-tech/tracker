using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tracker.Models;

namespace tracker.Tools
{
    public class RepositoryDays
    {
        SQLiteConnection database;
        public RepositoryDays(string databasePath)
        {
            database = new SQLiteConnection(databasePath);
            database.CreateTable<Day>();
        }
        public List<Day> GetItems()
        {
            return database.Table<Day>().ToList();
        }
        public Day GetItem(int id)
        {
            return database.Get<Day>(id);
        }

        public Day FindByDate(int projectId, DateTime date)
        {
            List<Day> days = database.Table<Day>().ToList().Where(d => d.ProjectId == projectId).ToList();
            Day day = days.Find(d => d.Date.Date == date.Date);
            return day;
        }

        public int DeleteItem(int id)
        {
            return database.Delete<Day>(id);
        }
        public int SaveItem(Day item)
        {
            if (item.Id != null)
            {
                database.Update(item);
                return item.Id.Value;
            }
            else
            {
                return database.InsertOrReplace(item);
            }
        }

        public void ClearTable()
        {
            database.DeleteAll<Day>();
        }

    }
}
