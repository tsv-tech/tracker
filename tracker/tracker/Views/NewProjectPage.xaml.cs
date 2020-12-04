using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using tracker.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace tracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewProjectPage : ContentPage
    {
        public Project LocalProject { get; private set; }
        public NewProjectPage(Project project)
        {
            InitializeComponent();
            this.LocalProject = project;
            this.BindingContext = LocalProject;
        }

        private async void btnCreateClicked(object sender, EventArgs e)
        {
            if (LocalProject.CustomId == null || LocalProject.CustomId == "")
            {
                await Application.Current.MainPage.DisplayAlert("Alert", "Custom ID must be at least 1 character long", "OK");
                return;
            }
            
            if (App.PROJECTS_VM.Projects.Any(p => p.CustomId == LocalProject.CustomId))
            {
                await Application.Current.MainPage.DisplayAlert("Alert", "Custom ID already exists in LOCAL DATABASE", "OK");
                return;
            }

            var item = await FetchProjectTask(LocalProject.CustomId);
            
            if (item == null)
            {
                await Navigation.PopAsync();

                MessagingCenter.Send<Project>(LocalProject, "MsgCreateProject");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Alert", "Custom ID already exists at REMOTE SERVER", "OK");
                return;
            }

            /*
            try
            {
                //project.Time = TimeSpan.Parse(item["time"]);
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Alert", "Invalid Time format " + e.Message, "OK");
            }
            */


        }

        private void btnCancelClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        public async Task<Dictionary<string, string>> FetchProjectTask(string CustomId)
        {

            HttpClient client = new HttpClient();
            Uri uri = new Uri(string.Format(App.SERVER_URL, string.Empty) + CustomId);

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