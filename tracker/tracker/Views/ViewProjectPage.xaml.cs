using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using tracker.Models;
using tracker.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace tracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewProjectPage : ContentPage
    {
        public Project LocalProject { get; private set; }
        public bool ItemChanged = false;
        public ViewProjectPage(Project prj)
        {
            InitializeComponent();
            LocalProject = prj;
            this.BindingContext = this.LocalProject;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            /*if (ItemChanged)
                App.DBProjects.SaveItem(LocalProject);
            else
                MessagingCenter.Send<Page>(this, "DiscardChanges");*/
        }

        

        private void barSaveClicked(object sender, EventArgs e)
        {
            MessagingCenter.Send<Project>(LocalProject, "MsgSaveProject");
        }

        private async void barDeleteClicked(object sender, EventArgs e)
        {
            
            bool answer = await DisplayAlert("ALERT", "You are going to delete this project \n Continue?", "Yes", "No");
            if (answer)
            {
                await Navigation.PopAsync();

                //ListView throws exception if ItemSource is modified while being in different page
                //Task.Delay(1000).ContinueWith(t => MessagingCenter.Send<Project>(LocalProject, "MsgDeleteProject"));
                MessagingCenter.Send<Project>(LocalProject, "MsgDeleteProject");
            }
        }

        private async void btnSendClicked(object sender, EventArgs e)
        {
            await PostToServerAsync();
        }

        public async Task PostToServerAsync(bool isNewItem = true)
        {
            fetchLabel.Text = "";
            fetchIndicator.IsRunning = true;
            fetchIndicator.IsVisible = true;
            // httpclient будет устанавливать соединение с внешним сервером
            HttpClient client = new HttpClient();

            // uri это ссылка/адрес сервера с которым будет устанавливаться соединение, генерируется из строки с адресом,
            //которая создана в App в разделе глобальных переменных
            Uri uri = new Uri(string.Format(App.SERVER_URL_POST, string.Empty));

            Dictionary<string, string> contentRaw = new Dictionary<string, string>();
            contentRaw.Add("customId", LocalProject.CustomId);
            contentRaw.Add("time", (LocalProject.Time + LocalProject.Correction).ToString());

            //json - объект который "понимает" внешний сервер, т.е. здесь он создается из начального объекта, который
            //приводится в нужную форму

            string json = JsonConvert.SerializeObject(contentRaw, Formatting.Indented);
            // информация о нашем json, который подскажет серверу, как правильно его прочитать и обработать
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            /* процесс установки соединения с сервером и отправки нашего json после чего сервер пришлет ответ (response)  */
            HttpResponseMessage response = null;
            if (isNewItem)
            {
                response = await client.PostAsync(uri, content);
            }

            fetchIndicator.IsRunning = false;
            fetchIndicator.IsVisible = false;
            /* проверка ответа от сервера: если все сработало - то выполняется показ страницы (данная страница тестовая, поэтому
             * пока не обращаем внимания*/
            
            if (response.IsSuccessStatusCode)
            {
                fetchLabel.Text = "Sent!";
                LocalProject.LastSyncDate = DateTime.Now;
                LocalProject.LastSyncTime = LocalProject.Time + LocalProject.Correction;

                MessagingCenter.Send<Project>(LocalProject, "MsgUpdateLastSyncProject");
            }
            else
            {
                fetchLabel.Text = "Error sending data, plese try again in 1 hour.";
                //await DisplayAlert("Error", "Something went wrong\n" + response.StatusCode.ToString(), "Ok");
            }
            
        }

        public async Task ShareText()
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = "\nEnter this ID in Time App: " + LocalProject.CustomId,
                Title = "Share Project"
            });
        }

        private async void barShareClicked(object sender, EventArgs e)
        {
            await ShareText();
        }

        private async void barCalendarClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CalendarPage(LocalProject));
        }

        private async void btnCorrectionClicked(object sender, EventArgs e)
        {
            string initial = LocalProject.Correction.TotalHours.ToString();

            string result = await App.Current.MainPage.DisplayPromptAsync("Correct time",
                        "Edit time correction what will be added to sending time",
                        initialValue: initial, keyboard: Keyboard.Numeric, cancel: "Cancel", accept: "Accept");

            if (result == null) return;

            int hours = Convert.ToInt32(Double.Parse(result));

            LocalProject.Correction = new TimeSpan(hours, 0, 0);
            
        }
    }
}