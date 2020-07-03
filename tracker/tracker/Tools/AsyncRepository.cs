using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using tracker.Models;

namespace tracker.Tools
{
    public class AsyncRepository
    {
        SQLiteAsyncConnection database;

        public AsyncRepository(string databasePath)
        {
            database = new SQLiteAsyncConnection(databasePath);
        }

        public async Task CreateTable()
        {
            await database.CreateTableAsync<Project>();
        }
        public async Task<List<Project>> GetItemsAsync()
        {
            return await database.Table<Project>().ToListAsync();

        }

        public async Task<Project> GetItemAsync(int id)
        {
            return await database.GetAsync<Project>(id);
        }
        public async Task<int> DeleteItemAsync(Project item)
        {
            return await database.DeleteAsync(item);
        }
        public async Task<int> SaveItemAsync(Project item)
        {
            if (item.Id != 0)
            {
                await database.UpdateAsync(item);
                return item.Id;
            }
            else
            {
                return await database.InsertAsync(item);
            }
        }
    }
}
