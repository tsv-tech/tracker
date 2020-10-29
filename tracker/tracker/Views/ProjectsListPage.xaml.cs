using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using tracker.ViewModels;

namespace tracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProjectsListPage : ContentPage
    {
        public ProjectsListPage()
        {
            InitializeComponent();
            BindingContext = new ProjectsViewModel(Navigation)
                ;
            MessagingCenter.Subscribe<Page>(this, "DiscardChanges", (project) =>
            {
                DisplayAlert("Title", "Message", "OK");
                //App.DBProjects.SaveItem(tempProject);
                //MessagingCenter.Unsubscribe<ProjectViewModel>(this, "DiscardChanges");
            });
        }

        private async void WatchOther(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new WatchPage());
        }

        private async void btnSettingsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }
    }
}