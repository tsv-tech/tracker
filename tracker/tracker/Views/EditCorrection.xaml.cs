using System;
using tracker.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace tracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditCorrection : ContentPage
    {
        public int Days { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public bool IsNegative { get; set; } = false;
        public Project LocalProject { get; set; }

        public EditCorrection(Project project)
        {
            InitializeComponent();

            LocalProject = project;
            Hours = (int)project.Correction.TotalHours;
            Minutes = project.Correction.Minutes;
            IsNegative = Hours < 0;

            BindingContext = this;
        }

        private void btCancelClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void btnApplyClicked(object sender, EventArgs e)
        {
            errMinutes.IsVisible = false;
            if (Minutes >= 60 || Minutes < 0)
            {
                errMinutes.IsVisible = true;
                return;
            }

            if (Math.Abs(Hours) > 24)
            {
                Days = Hours / 24;
                Hours %= 24;
            }

            //TimeSpan newTime = !IsNegative ? new TimeSpan(Days, Hours, Minutes, 0) : new TimeSpan(Days, Hours, Minutes, 0).Negate();
            TimeSpan newTime = new TimeSpan(Days, Hours, Minutes, 0);

            LocalProject.Correction = newTime;
            Navigation.PopAsync();
        }
    }
}