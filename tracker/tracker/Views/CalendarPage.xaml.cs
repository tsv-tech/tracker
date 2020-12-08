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
        public EventCollection Days { get; set; }
        public List<Session> Sessions { get; set; }
        public Project LocalProject { get; set; }
        public int DayTotalTime { get; set; }
        public CalendarPage(Project project)
        {
            InitializeComponent();

            Days = new EventCollection();
            LocalProject = project;
            LoadDays();
            DayTapped(DateTime.Today);

            BindingContext = this;
        }

        public void LoadDays()
        {
            Sessions = App.DBSessions.GetItems().Where(s => s.ProjectId == LocalProject.Id).ToList();

            if (Sessions.Count == 0)
                return;

            List<Session> CurrentDaySessions;

            foreach (var session in Sessions)
            {
                if (Days.ContainsKey(session.StartTime.Date))
                {
                    continue;
                }
                else
                {
                    CurrentDaySessions = Sessions.Where(s => s.StartTime.Date == session.StartTime.Date).ToList();
                    Days.Add(session.StartTime.Date, CurrentDaySessions);
                }

            }

        }

        private void barExportClicked(object sender, EventArgs e)
        {

        }

        private void DayTapped(DateTime date)
        {
            TimeSpan total = new TimeSpan();
            if (Days.ContainsKey(date))
                if (Days[date] != null)
                {

                    foreach (Session s in Days[date])
                    {
                        total += s.Duration;
                    }

                }

            lblDay.Text = date.ToString("M") + ": " + string.Format("{0:D2}:{1:mm}:{1:ss}",
                                 (int)(total).TotalHours,
                                 total);
        }
    }
}