using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using tracker.Models;
using tracker.Views;
using tracker.Tools;
using Xamarin.Forms;
using System.Threading;
using System.Net.Http;
using Newtonsoft.Json;

namespace tracker.ViewModels
{
    public class ProjectsViewModel : BaseViewModel
    {
        public ProjectsViewModel()
        {
            Title = "Projects";

            /* Коллекция хранит в себе все проекты, которые были созданы. Управление этими проектами происходит тоже
              с помощью этой коллекции*/

            Projects = new ObservableRangeCollection<Project>();

            var range = new List<Project>(App.DBProjects.GetItems());
            List<Project> newRange = new List<Project>();
            foreach (var p in range)
            {
                newRange.Add(new Project(p));
            }
            Projects.AddRange(newRange);


            /* команды для управления коллекцией проектов, которые будут привязаны к кнопкам на главной странице
             * каждая команда имеет свою функцию (которая указана в скобках), которая будет вызвана и исполнена с теми параметрами, которые 
             ей будут переданы, например для создания нового проекта будет совершен переход на новую страницу с 
            пустым проектом*/

            GetProjectsCommand = new MvvmHelpers.Commands.Command(ClearTable);
            CreateProjectCommand = new MvvmHelpers.Commands.Command(CreateProject);
            ManageProjectCommand = new MvvmHelpers.Commands.Command(ManageProject);
            EditTimeCommand = new MvvmHelpers.Commands.Command(EditTime);
            GetRequestCommand = new MvvmHelpers.Commands.Command(PostToServer);

            MessagingCenter.Subscribe<Project>(this, "MsgSaveProject", (project) =>
            {
                ExecuteUpdateProject(project);
            });

            MessagingCenter.Subscribe<Project>(this, "MsgCreateProject", (project) =>
            {
                App.DBProjects.SaveItem(project);
                Projects.Add(project);
            });
            MessagingCenter.Subscribe<Project>(this, "MsgDeleteProject", (project) =>
            {
                App.DBProjects.DeleteItem(project.Id);
                IsBusy = true;
                Task.Delay(1000).ContinueWith(t =>
                {
                    foreach (var p in Projects)
                        if (p.Id == project.Id)
                            Projects.Remove(p);
                });
                IsBusy = false;
            });

            MessagingCenter.Subscribe<Project>(this, "EditTimeMessage", (project) =>
            {
                ExecuteUpdateTime(project);
            });
        }

        public INavigation Navigation { get; set; }
        public ObservableRangeCollection<Project> Projects { get; set; }
        public MvvmHelpers.Commands.Command GetProjectsCommand { get; }
        public MvvmHelpers.Commands.Command CreateProjectCommand { get; }
        public MvvmHelpers.Commands.Command ManageProjectCommand { get; }
        public MvvmHelpers.Commands.Command EditTimeCommand { get; }
        public MvvmHelpers.Commands.Command GetRequestCommand { get; }

        public void GetList()
        {
            var projects = new ObservableRangeCollection<Project> {
                new Project("PRJ 1", "AUTH 1", "ID 1", "25", "COMMENT 1"),
                new Project("PRJ 2", "AUTH 2", "ID 2", "25", "COMMENT 2")
                };

            Projects.AddRange(projects);
        }

        public async void GetFromServer()
        {
            var item = await GetRequest();
            if (item == null)
                return;
            var localProject = new Project(item["name"], item["time"], item["customId"], "55", "comment");
            Projects.Add(localProject);
        }

        public async Task<Dictionary<string, string>> GetRequest()
        {
            HttpClient client = new HttpClient();
            Uri uri = new Uri(string.Format(App.SERVER_URL, string.Empty));

            HttpResponseMessage response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                var item = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
                return item;
            }
            else { return null; }

        }

        public async void PostToServer()
        {
            await PostToServerAsync();
        }

        public async Task PostToServerAsync(bool isNewItem = true)
        {
            // httpclient будет устанавливать соединение с внешним сервером
            HttpClient client = new HttpClient();

            // uri это ссылка/адрес сервера с которым будет устанавливаться соединение, генерируется из строки с адресом,
            //которая создана в App в разделе глобальных переменных
            Uri uri = new Uri(string.Format(App.SERVER_URL_POST, string.Empty));

            // localProject - начальный объект, который хранит в себе нашу информацию
            var localProject = new ProjectLight { name = "sender", time="00:12:12", customId="sen11" };

            //json - объект который "понимает" внешний сервер, т.е. здесь он создается из начального объекта, который
            //приводится в нужную форму
            string json = JsonConvert.SerializeObject(localProject);

            // информация о нашем json, который подскажет серверу, как правильно его прочитать и обработать
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            /* процесс установки соединения с сервером и отправки нашего json после чего сервер пришлет ответ (response)  */
            HttpResponseMessage response = null;
            if (isNewItem)
            {
                response = await client.PostAsync(uri, content);
            }

            /* проверка ответа от сервера: если все сработало - то выполняется показ страницы (данная страница тестовая, поэтому
             * пока не обращаем внимания*/
            if (response.IsSuccessStatusCode)
            {
                await Navigation.PushAsync(new WatchPage());
            }
        }

        public void ClearTable()
        {
            Projects.Clear();
            App.DBProjects.ClearTable();
        }

        public async void CreateProject()
        {
            await Navigation.PushAsync(new NewProjectPage(new Project()));
        }

        //async - для того, чтобы пользователь продолжал работу без прерываний 
        public async void ManageProject(object parameter)
        {
            var tempProject = new Project(parameter as Project);
            await Navigation.PushAsync(new ViewProjectPage(tempProject));
        }

        public void ExecuteUpdateProject(Project project)
        {
            foreach (var p in Projects)
                if (p.Id == project.Id)
                {
                    p.Name = project.Name;
                    p.Author = project.Author;
                    //p.CustomId = project.CustomId;
                    p.Payment = project.Payment;
                    p.Comment = project.Comment;
                    p.Time = project.Time;
                    p.DateCreated = project.DateCreated;
                    p.Currency = project.Currency;
                    break;
                }
            App.DBProjects.SaveItem(project);
        }

        public void ExecuteUpdateTime(Project project)
        {
            foreach (var p in Projects)
                if (p.Id == project.Id)
                {
                    p.Time = project.Time;
                    break;
                }
            App.DBProjects.SaveItem(project);
        }

        public async void EditTime(object parameter)
        {
            var tempProject = new Project(parameter as Project);
            await Navigation.PushAsync(new EditTimePage(tempProject));
        }
    }
}
