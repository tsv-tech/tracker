using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using tracker.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Plugin.Calendar.Models;


namespace tracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CalendarPage : ContentPage
    {
        public ICommand DayTappedCommand => new Command<DateTime>((date) => DayTapped(date));
        public ICommand ExportTappedCommand => new Command(ExportAndShare);
        public EventCollection Days { get; set; }

        public List<Day> DaysList { get; set; }

        public Project LocalProject { get; set; }
        public int DayTotalTime { get; set; }
        public CalendarPage(Project project)
        {
            InitializeComponent();

            Days = new EventCollection();

            LocalProject = project;
            DaysList = App.DBDays.GetItems().Where(s => s.ProjectId == LocalProject.Id).ToList();

            LoadDaysFromSessions();
            DayTapped(DateTime.Today);

            BindingContext = this;
        }


        public void LoadDaysFromSessions()
        {
            if (DaysList.Count == 0)
                return;

            foreach (var day in DaysList)
            {
                if (Days.ContainsKey(day.Date))
                {
                    continue;
                }

                Days.Add(day.Date, new List<string> { FormatDayTime(day.Time) });
            }
        }

        private string FormatDayTime (TimeSpan time)
        {
            return string.Format("{0}h {1:mm}m",
                     (int)time.TotalHours,
                     time);
        }

        private async void ExportAndShare()
        {
            await Navigation.PushAsync(new ExportPage(DaysList));
        }

        private void DayTapped(DateTime date)
        {
            /*
            TimeSpan total = new TimeSpan();

            foreach (var d in DaysList)
            {
                if (d.Date == date.Date)
                {
                    total = d.Time;
                    break;
                }
            }

            lblDay.Text = date.ToString("M") + ": " + string.Format("{0:D2}:{1:mm}:{1:ss}",
                                 (int)(total).TotalHours,
                                 total);*/
        }
    }
}