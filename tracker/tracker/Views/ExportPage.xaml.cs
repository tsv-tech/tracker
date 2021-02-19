using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tracker.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CsvHelper;
using System.IO;
using System.Globalization;
using Xamarin.Essentials;
using CsvHelper.Configuration;

namespace tracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExportPage : ContentPage
    {
        public List<Day> DaysList { get; set; }
        public List<Day> selectedDays { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public ExportPage(List<Day> days)
        {
            InitializeComponent();

            DaysList = days;
            FromDate = DateTime.Today.AddDays(-7);
            ToDate = DateTime.Today;

            BindingContext = this;
        }

        private async void btnExportClick(object sender, EventArgs e)
        {
            if (ToDate < FromDate)
            {
                await DisplayAlert("Export CSV", "Please select correct span", "OK");
                return;
            }

            await ExportAndShare();
        }

        private async Task ExportAndShare()
        {
            selectedDays = DaysList.Where(d => d.Date >= FromDate && d.Date <= ToDate).ToList();

            if (selectedDays.Count == 0)
            {
                await DisplayAlert("Export CSV", "No info to export", "OK");
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

            foreach (var d in selectedDays)
            {
                string day_string = d.Date.ToString("D");
                if (_daysTotalTime.ContainsKey(day_string)) continue;

                _daysTotalTime.Add(day_string, d.Time);
            }

            return _daysTotalTime;
        }
    }
}