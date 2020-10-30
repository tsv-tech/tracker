using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using tracker.Models;
using tracker.Views;
using Xamarin.Forms;

namespace tracker.ViewModels
{
    public class ProjectsViewModel : BaseViewModel
    {
        public INavigation Navigation { get; set; }
        public Project ActiveProject { get; set; }
        public DateTime SessionTimeStarted { get; set; }

        public ProjectsViewModel(INavigation navigation)
        {
            this.Navigation = navigation;
            Title = "Projects";


            /* Коллекция хранит в себе все проекты, которые были созданы. Управление этими проектами происходит тоже
              с помощью этой коллекции*/

            Projects = new ObservableRangeCollection<Project>();

            var range = new List<Project>(App.DBProjects.GetItems());
            
            /*List<Project> newRange = new List<Project>();
            foreach (var p in range)
            {
                newRange.Add(new Project(p));
            }
            */
            Projects.AddRange(range);


            /* команды для управления коллекцией проектов, которые будут привязаны к кнопкам на главной странице
             * каждая команда имеет свою функцию (которая указана в скобках), которая будет вызвана и исполнена с теми параметрами, которые 
             ей будут переданы, например для создания нового проекта будет совершен переход на новую страницу с 
            пустым проектом*/

            CreateProjectCommand = new MvvmHelpers.Commands.Command(CreateProject);
            ManageProjectCommand = new MvvmHelpers.Commands.Command(ManageProject);
            EditTimeCommand = new MvvmHelpers.Commands.Command(EditTime);
            ToggleTimerCommand = new MvvmHelpers.Commands.Command(ToggleTimer);

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

        public ObservableRangeCollection<Project> Projects { get; set; }
        public ICommand CreateProjectCommand { get; }
        public ICommand ManageProjectCommand { get; }
        public ICommand EditTimeCommand { get; }
        public ICommand ToggleTimerCommand { get; }


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

        public async void ToggleTimer(object parameter)
        {
            var project = parameter as Project;

            if (ActiveProject == null)
            {
                ActiveProject = project;
                Start(ActiveProject);
                return;
            }

            if (ActiveProject == project)
            {
                Stop(ActiveProject);
                ActiveProject = null;
                return;
            }
            else
            {
                bool ans = await Application.Current.MainPage.DisplayAlert("Project Change", "You are currently working on other project\nDo you want to switch?", "Yes", "No");
                if (ans)
                {
                    Stop(ActiveProject);
                    ActiveProject = project;
                    Start(ActiveProject);
                }
                else { return; }
            }
        }

        public void Start(Project project)
        {
            if (project.IsRunning)
            {
                return;
            }
            else
            {
                SessionTimeStarted = DateTime.Now;
                project.IsRunning = true;

                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    project.Time += new TimeSpan(0, 0, 1);
                    return project.IsRunning;
                });
            }
        }

        public void Stop(Project project)
        {
            if (project.IsRunning)
            {
                project.IsRunning = false;
                App.DBSessions.SaveItem(new Session
                {
                    ProjectId = project.Id,
                    Date = SessionTimeStarted.ToString(),
                    Duration = (SessionTimeStarted - DateTime.Now).ToString(@"hh\:mm\:ss")
                });

                App.DBProjects.SaveItem(project);
                return;
            }
            else return;
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
