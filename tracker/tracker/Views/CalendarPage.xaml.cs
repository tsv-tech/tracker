﻿using System;
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

                Days.Add(day.Date, new List<TimeSpan> { day.Time });
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

            try
            {
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

            foreach (var d in DaysList)
            {
                string day_string = d.Date.ToString("D");
                if (_daysTotalTime.ContainsKey(day_string)) continue;

                _daysTotalTime.Add(day_string, d.Time);
            }

            return _daysTotalTime;
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