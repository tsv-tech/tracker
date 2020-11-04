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

        public const string SERVER_URL = "https://us-central1-xamarin-tracker.cloudfunctions.net/app/api/read/";
        public const string SERVER_URL_POST = "https://us-central1-xamarin-tracker.cloudfunctions.net/app/api/upsert";

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
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new ProjectsListPage());

        }

        protected override void OnStart()
        {
            if (Application.Current.Properties.ContainsKey("watchMode"))
            {
                Application.Current.MainPage.Navigation.PushAsync(new WatchPage());
            }
            //ResumeActive();
        }

        protected override void OnSleep()
        {
            PROJECTS_VM.StopGlobalTimer();
        }

        protected override void OnResume()
        {
            PROJECTS_VM.ResumeActive();
        }
    }
}
