using System;
using System.Collections.Generic;
using System.Linq;
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

        private void btnCreateClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();

            MessagingCenter.Send<Project>(LocalProject, "MsgCreateProject");
        }

        private void btnCancelClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}