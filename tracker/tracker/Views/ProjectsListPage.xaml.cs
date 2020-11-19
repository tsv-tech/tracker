using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using tracker.ViewModels;
using tracker.Models;

namespace tracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProjectsListPage : ContentPage
    {
        public ProjectsListPage()
        {
            InitializeComponent();
            BindingContext = App.PROJECTS_VM;
            App.PROJECTS_VM.Navigation = this.Navigation;

            MessagingCenter.Subscribe<Project>(this, "MsgScrollToProject", (project) =>
            {
                ProjectsListView.ScrollTo(project, ScrollToPosition.MakeVisible, true);
            });
        }

        private async void btnSettingsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }

    }
}