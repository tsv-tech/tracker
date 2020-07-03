using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using tracker.Models;
using tracker.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace tracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewProjectPage : ContentPage
    {
        /*
        public ProjectViewModel LocalViewModel { get; private set; }
        public CreateProjectPage(ProjectViewModel vm)
        {
            InitializeComponent();
            LocalViewModel = vm;
            this.BindingContext = LocalViewModel;
        }
        */
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
            
            bool answer = await DisplayAlert("Question?", "You are going to delete this project \n Continue?", "Yes", "No");
            if (answer)
            {
                await Navigation.PopAsync();

                //ListView throws exception if ItemSource is modified while being in different page
                //Task.Delay(1000).ContinueWith(t => MessagingCenter.Send<Project>(LocalProject, "MsgDeleteProject"));
                MessagingCenter.Send<Project>(LocalProject, "MsgDeleteProject");
            }
        }
    }
}