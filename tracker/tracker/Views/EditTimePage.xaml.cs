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
    public partial class EditTimePage : ContentPage
    {
        public int Days { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }

        public Project LocalProject { get; set; }
        public EditTimePage(Project project)
        {
            InitializeComponent();

            LocalProject = project;
            Hours = (int)project.Time.TotalHours;
            Minutes = project.Time.Minutes;

            BindingContext = this;
        }

        private void btCancelClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void btnApplyClicked(object sender, EventArgs e)
        {
            errMinutes.IsVisible = false;
            if (Minutes >= 60)
            {
                errMinutes.IsVisible = true;
                return;
            }

            Navigation.PopAsync();

            if (Hours > 24)
            {
                Days = Hours / 24;
                Hours %= 24;
            }
            LocalProject.Time = new TimeSpan(Days, Hours, Minutes, 0);
            MessagingCenter.Send<Project>(LocalProject, "EditTimeMessage");
        }
    }
}