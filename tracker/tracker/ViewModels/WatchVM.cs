using MvvmHelpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using tracker.Models;
using tracker.Views;
using Xamarin.Forms;

namespace tracker.ViewModels
{
    public class WatchVM : BaseViewModel
    {
        public WatchVM()
        {
            WatchProjects = new ObservableRangeCollection<Project>();

            var range = new List<Project>(App.DBWatch.GetItems());
            WatchProjects.AddRange(range);

            MessagingCenter.Subscribe<Project>(this, "MsgAddWatchProject", (project) =>
            {
                App.DBWatch.SaveItem(project);
                WatchProjects.Add(project);

                Fetch(project);
            });

            FetchCommand = new Command(Fetch);

            WatchAddCommand = new Command(async () => { 
                await Application.Current.MainPage.Navigation.PushAsync(new WatchAddPage(new Project())); 
            });

            WatchDeleteCommand = new Command((object parameter) =>
            {
                var project = parameter as Project;
                WatchProjects.Remove(project);
                App.DBWatch.DeleteItem(project.Id);
            });
        }
        public ObservableRangeCollection<Project> WatchProjects { get; set; }

        public ICommand FetchCommand { get; }
        public ICommand WatchAddCommand { get; }
        public ICommand WatchDeleteCommand { get; }

        private async void Fetch(object parameter)
        {
            var project = parameter as Project;

            if (project.CustomId == null || project.CustomId == "")
            {
                await Application.Current.MainPage.DisplayAlert("Alert", "Custom ID must be at least 1 character long", "OK");
                return;
            }

            var item = await FetchProjectTask(project);
            if (item == null)
            {
                await Application.Current.MainPage.DisplayAlert("Alert", "ID does not exist", "OK");
                return;
            }
            try
            {
                project.Time = TimeSpan.Parse(item["time"]);
            }
            catch {
                await Application.Current.MainPage.DisplayAlert("Alert", "Invalid Time format", "OK");
            }

            App.DBWatch.SaveItem(project);

        }
        public async Task<Dictionary<string, string>> FetchProjectTask(Project project)
        {

            HttpClient client = new HttpClient();
            Uri uri = new Uri(string.Format(App.SERVER_URL, string.Empty) + project.CustomId);

            HttpResponseMessage response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var item = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
                    return item;
                }
                catch
                {
                    await Application.Current.MainPage.DisplayAlert("Task", "JSON error", "OK");
                    return null;
                }
            }
            else { return null; }

        }
    }
}
