using System;
using System.IO;
using tracker.Models;
using tracker.Tools;
using tracker.ViewModels;
using tracker.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace tracker
{
    public partial class App : Application
    {
        /* Глобальные переменные (Variables), которые мы используем на уровне всего приложения
         место храненеие базы данных*/
        public static ProjectsViewModel PROJECTS_VM = new ProjectsViewModel();
        public static WatchVM WATCH_VM = new WatchVM();

        public const string PROJECTS_FILE = "projects.db";
        public const string WATCH_FILE = "watch.db";
        public const string SESSIONS_FILE = "sessions.db";
        public const string DAYS_FILE = "days.db";
        public const string CSV_EXPORT_TMP_FILE = "export.csv";

        public const int MAX_DAY_DURATION = 12;

        public const string SERVER_URL = "https://us-central1-time-app-pi.cloudfunctions.net/app/api/read";
        public const string SERVER_URL_POST = "https://us-central1-time-app-pi.cloudfunctions.net/app/api/upsert";

        public const string FULL_VERSION_LINK = "https://play.google.com/store/apps/details?id=com.pi.engineering.tracker_A";

        public static Repository dbProjects;
        public static Repository DBProjects
        {
            get
            {
                if (dbProjects == null)
                {
                    dbProjects = new Repository(
                        Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                            , PROJECTS_FILE));
                }
                return dbProjects;
            }
        }

        public static Repository dbWatch;
        public static Repository DBWatch
        {
            get
            {
                if (dbWatch == null)
                {
                    dbWatch = new Repository(
                        Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                            , WATCH_FILE));
                }
                return dbWatch;
            }
        }
        public static RepositorySessions dbSessions;
        public static RepositorySessions DBSessions
        {
            get
            {
                if (dbSessions == null)
                {
                    dbSessions = new RepositorySessions(
                        Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), SESSIONS_FILE));
                }
                return dbSessions;
            }
        }

        public static RepositoryDays dbDays;
        public static RepositoryDays DBDays
        {
            get
            {
                if (dbDays == null)
                {
                    dbDays = new RepositoryDays(
                        Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DAYS_FILE));
                }
                return dbDays;
            }
        }

        public App()
        {
            InitializeComponent();

            //MainPage = new NavigationPage(new ProjectsListPage());
            MainPage = new TabPage();

        }

        protected override void OnStart()
        {
            /*if (Application.Current.Properties.ContainsKey("watchMode"))
            {
                Application.Current.MainPage.Navigation.PushAsync(new WatchPage());
            }*/
            //ResumeActive();
        }

        protected override void OnSleep()
        {
            PROJECTS_VM.StopGlobalTimer();
        }

        protected override void OnResume()
        {
            PROJECTS_VM.CheckDay();
            PROJECTS_VM.RecoverGlobalTimer();
        }
    }
}
