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
    public partial class EditDayTime : ContentPage
    {
        public int Days { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }

        public Project LocalProject { get; set; }
        public EditDayTime(Project project)
        {
            InitializeComponent();

            LocalProject = project;
            Hours = (int)project.DayTime.TotalHours;
            Minutes = project.DayTime.Minutes;

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
            if (Hours >= 12)
            {
                errHours.IsVisible = true;
                return;
            }

            Navigation.PopAsync();

            if (Hours > 24)
            {
                Days = Hours / 24;
                Hours %= 24;
            }

            var _newDayTime = new TimeSpan(Days, Hours, Minutes, 0);
            LocalProject.Time += _newDayTime - LocalProject.DayTime;
            LocalProject.DayTime = _newDayTime;
            

            MessagingCenter.Send<Project>(LocalProject, "EditDayTimeMessage");
        }
    }
}