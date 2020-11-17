using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using tracker.Models;
using MvvmHelpers;
using tracker.ViewModels;

namespace tracker.Tools
{
    public class RepositorySessions
    {
        SQLiteConnection database;
        public RepositorySessions(string databasePath)
        {
            database = new SQLiteConnection(databasePath);
            database.CreateTable<Session>();
        }
        public List<Session> GetItems()
        {
            return database.Table<Session>().ToList();
        }
        public Session GetItem(int id)
        {
            return database.Get<Session>(id);
        }
        public int DeleteItem(int id)
        {
            return database.Delete<Session>(id);
        }
        public int SaveItem(Session item)
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
            database.DeleteAll<Session>();
        }
    }
}
