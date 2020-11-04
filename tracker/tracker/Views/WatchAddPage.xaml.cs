using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tracker.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace tracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WatchAddPage : ContentPage
    {
        public Project LocalProject { get; private set; }
        public WatchAddPage(Project project)
        {
            InitializeComponent();
            this.LocalProject = project;
            this.BindingContext = LocalProject;
        }

        private void AddClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
            try
            {
                MessagingCenter.Send<Project>(LocalProject, "MsgAddWatchProject");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.InnerException);
            }
        }

        private async void BackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}