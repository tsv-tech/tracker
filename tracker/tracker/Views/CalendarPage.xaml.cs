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
using CsvHelper;
using System.IO;
using System.Globalization;
using Xamarin.Essentials;
using CsvHelper.Configuration;

namespace tracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CalendarPage : ContentPage
    {
        public ICommand DayTappedCommand => new Command<DateTime>((date) => DayTapped(date));
        public ICommand ExportTappedCommand => new Command(async () => await ExportAndShare());
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
                    CurrentDaySessions.Reverse();
                    Days.Add(session.StartTime.Date, CurrentDaySessions);
                }

            }

        }

        private async Task ExportAndShare()
        {
            if (Days.Count == 0)
            {
                await DisplayAlert("Export CSV", "This project doesn't have data to export yet", "OK");
                return;
            }
            /*string path = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                            , App.CSV_EXPORT_TMP_FILE);*/

            var path = Path.Combine(FileSystem.CacheDirectory, App.CSV_EXPORT_TMP_FILE);

            try { 
                File.Delete(path);
            }
            catch { }

            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Configuration.RegisterClassMap<DayMap>();
                csv.WriteRecords(GetDaysTotalTime());
            }

            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "Send CSV",
                File = new ShareFile(path)
            });
        }

        public sealed class DayMap : ClassMap<KeyValuePair<string, TimeSpan>>
        {
            public DayMap()
            {
                Map(m => m.Key).Name("Date");
                Map(m => m.Value).Name("Total time");
            }
        }

        public Dictionary<string, TimeSpan> GetDaysTotalTime()
        {
            var _daysTotalTime = new Dictionary<string, TimeSpan>();
            foreach (var d in Days)
            {
                string day_string = d.Key.ToString("D");

                if (!_daysTotalTime.ContainsKey(day_string))
                {
                    _daysTotalTime.Add(day_string, new TimeSpan(0, 0, 0));
                }

                foreach (Session s in d.Value)
                {
                    _daysTotalTime[day_string] += s.Duration;
                }
            }
            return _daysTotalTime;
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