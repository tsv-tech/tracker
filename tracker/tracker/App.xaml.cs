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
        /* Глобальные переменные (Variables), которые мы используем на уровне всего приложения
         место храненеие базы данных*/

        public const string PROJECTS_FILE = "projects.db";
        public const string SESSIONS_FILE = "sessions.db";
        //public const string SERVER_URL = "https://jsonplaceholder.typicode.com/todos/1";

        /*  */
        public const string SERVER_URL = "https://us-central1-xamarin-tracker.cloudfunctions.net/getItem?customId=id1";
        public const string SERVER_URL_POST = "https://us-central1-xamarin-tracker.cloudfunctions.net/postItem";
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
