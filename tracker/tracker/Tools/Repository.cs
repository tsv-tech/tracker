using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using tracker.Models;
using MvvmHelpers;

namespace tracker.Tools
{
    public class Repository
    {
        SQLiteConnection database;
        public Repository(string databasePath)
        {
            database = new SQLiteConnection(databasePath);
            database.CreateTable<Project>();
        }
        public List<Project> GetItems()
        {
            return database.Table<Project>().ToList();
        }
        public Project GetItem(int id)
        {
            return database.Get<Project>(id);
        }
        public int DeleteItem(int id)
        {
            return database.Delete<Project>(id);
        }
        public int SaveItem(Project item)
        {
            if (item.Id != 0)
            {
                database.Update(item);
                return item.Id;
            }
            else
            {
                return database.Insert(item);
            }
        }

        public void ClearTable()
        {
            database.DeleteAll<Project>();
        }
    }
}
