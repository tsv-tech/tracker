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
        public string PaymentString { get; set; }
        public int paymentInt = 0;
        public bool IsFetching { get; set; } = false;
        public string Errors { get; set; }
        public NewProjectPage(Project project)
        {
            InitializeComponent();
            this.LocalProject = project;
            this.BindingContext = LocalProject;
        }

        private async void btnCreateClicked(object sender, EventArgs e)
        {
            Errors = string.Empty;

            if (string.IsNullOrWhiteSpace(LocalProject.Name))
            {
                Errors += "Project Name field is required\n";
            }

            if (string.IsNullOrWhiteSpace(PaymentString))
            {
                //await Application.Current.MainPage.DisplayAlert("Alert", "Payment field is required", "OK");
                //return;
                Errors += "Payment field is required\n";
            }

            if (string.IsNullOrWhiteSpace(LocalProject.CustomId))
            {
                //await Application.Current.MainPage.DisplayAlert("Alert", "Custom ID must be at least 1 character long", "OK");
                //return;
                Errors += "Custom ID field is required\n";
            }

            if (!string.IsNullOrEmpty(Errors))
            {
                await Application.Current.MainPage.DisplayAlert("Alert", Errors, "OK");
                return;
            }


            if (Int32.TryParse(PaymentString, out paymentInt))
            {
                LocalProject.Payment = paymentInt;
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Alert", "Payment must be a number", "OK");
                return;
            }

            if (App.PROJECTS_VM.Projects.Any(p => p.CustomId == LocalProject.CustomId))
            {
                await Application.Current.MainPage.DisplayAlert("Alert", "Custom ID already exists in LOCAL DATABASE", "OK");
                return;
            }

            SetIndicator(true);
            var item = await FetchProjectTask(LocalProject.CustomId);
            SetIndicator(false);

            if (item == null)
            {
                await Navigation.PopAsync();

                MessagingCenter.Send<Project>(LocalProject, "MsgCreateProject");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Alert", "Custom ID already exists at REMOTE SERVER\n", "OK");
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

        private void SetIndicator(bool param)
        {
            IsFetchingIndicator.IsRunning = param;
            IsFetchingIndicator.IsVisible = param;
        }
    }
}