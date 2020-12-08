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
        public List<Project> ActiveProject { get; set; }
        public bool IsActive { get; set; }
        public Dictionary<int, DateTime> SessionTimeStarted { get; set; }
        public ProjectsViewModel()
        {
            Title = "Projects";

            /* Коллекция хранит в себе все проекты, которые были созданы. Управление этими проектами происходит тоже
              с помощью этой коллекции*/

            Projects = new ObservableRangeCollection<Project>();
            ActiveProject = new List<Project>();

            var range = new List<Project>(App.DBProjects.GetItems());

            /*List<Project> newRange = new List<Project>();
            foreach (var p in range)
            {
                newRange.Add(new Project(p));
            }
            */
            Projects.AddRange(range);

            RecoverGlobalTimer();
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
                Application.Current.MainPage.DisplayAlert("Info", "Changes Saved!", "OK");
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

        public INavigation Navigation;
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

        private void MoveProjectToTop(Project prj)
        {
            int oldIndex = Projects.IndexOf(prj);
            Projects.Move(oldIndex, 0);

            MessagingCenter.Send<Project>(prj, "MsgScrollToProject");
        }

        public async void ToggleTimer(object parameter)
        {
            var project = parameter as Project;

            if (ActiveProject.Count > 0 && !project.IsRunning)
            {
                bool ans = await Application.Current.MainPage.DisplayAlert("Project Change", "You are currently working on other project\nDo you want to switch?", "Yes", "No");
                if (ans)
                {
                    Stop(ActiveProject[0]);
                    ActiveProject.Clear();
                }
                else { return; }
            }

            if (!project.IsRunning)
            {
                ActiveProject.Add(project);
                Start(project);
                MoveProjectToTop(project);
            }
            else
            {
                try
                {
                    Stop(project);
                    ActiveProject.Remove(project);
                }
                catch
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to stop, please try again", "Ok");
                }
            }

            if (IsActive && ActiveProject.Count == 0)
            {
                StopGlobalTimer();
                return;
            }

            if (!IsActive && ActiveProject.Count > 0)
            {
                StartGlobalTimer();
            }

        }

        public void StartGlobalTimer()
        {
            if (IsActive)
                return;

            IsActive = true;
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                foreach (var p in ActiveProject)
                {
                    p.SessionTime = DateTime.Now - p.SessionStartTime;
                }
                return IsActive;
            });
        }

        public void StopGlobalTimer()
        {
            IsActive = false;
        }

        public void RecoverGlobalTimer()
        {
            ActiveProject.Clear();
            foreach(var p in Projects)
            {
                if (p.IsRunning)
                {
                    p.SessionTime = DateTime.Now - p.SessionStartTime;
                    ActiveProject.Add(p);
                }
            }
            StartGlobalTimer();
            //MoveProjectToTop(ActiveProject[0]);
        }

        public void Start(Project project)
        {
            if (project.IsRunning)
            {
                return;
            }
            else
            {

                project.SessionStartTime = DateTime.Now;
                project.SessionTime = new TimeSpan(0, 0, 0);

                project.IsRunning = true;

                App.DBProjects.SaveItem(project);
            }
        }

        public void Stop(Project project, bool sleep = false)
        {
            if (project.IsRunning)
            {
                project.IsRunning = false;

                if (!sleep)
                {
                    project.Time += project.SessionTime;
                    //App.DBSessions.SaveItem(new Session
                    //{
                    //    ProjectId = project.Id,
                    //    Date = project.SessionStartTime.ToString(),
                    //    Duration = (DateTime.Now - project.SessionStartTime).ToString(@"hh\:mm\:ss")
                    //});
                }

                App.DBProjects.SaveItem(project);
                return;
            }
            else return;
        }

        public void ResumeActive()
        {
            if (ActiveProject.Count == 0)
                return;

            foreach (var p in ActiveProject) 
                p.SessionTime = DateTime.Now - p.SessionStartTime;

            StartGlobalTimer();
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
