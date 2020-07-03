using System;
using System.IO;
using tracker.Tools;
using tracker.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace tracker
{
    public partial class App : Application
    {
        public const string PROJECTS_FILE = "projects.db";
        public const string SESSIONS_FILE = "sessions.db";
        public static Repository dbProjects;
        public static Repository DBProjects
        {
            get
            {
                if (dbProjects == null)
                {
                    dbProjects = new Repository(
                        Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), PROJECTS_FILE));
                }
                return dbProjects;
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

        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
