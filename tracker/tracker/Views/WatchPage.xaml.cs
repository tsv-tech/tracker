using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace tracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WatchPage : ContentPage
    {
        public string Time { get; set; }
        public string CustomId { get; set; }
        public WatchPage()
        {
            InitializeComponent();

            BindingContext = this;
        }

        private async void FetchProject(object sender, EventArgs e)
        {
            CustomId = entCustomId.Text;

            if (CustomId == null || CustomId == "")
            {
                await DisplayAlert("Alert", "Custom ID must be at least 1 character long", "OK");
                return;
            }

            var item = await FetchProjectTask();
            if (item == null)
            {
                await DisplayAlert("Alert", "ID does not exist", "OK");
                return;
            }
            try
            {
                entTime.Text = item["time"];
            }
            catch { }
            //var localProject = new Project(item["name"], item["time"], item["customId"], "55", "comment");
            //Projects.Add(localProject);

        }

        public async Task<Dictionary<string, string>> FetchProjectTask()
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
                    await DisplayAlert("Task", "JSON error", "OK");
                    return null;
                }
            }
            else { return null; }

        }

        private void ClearClicked(object sender, EventArgs e)
        {
            entTime.Text = "";
            entCustomId.Text = "";
            CustomId = "";
        }
    }
}