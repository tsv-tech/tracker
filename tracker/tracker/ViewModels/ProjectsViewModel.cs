using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
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
            CheckDay();

            RecoverGlobalTimer();
            /* команды для управления коллекцией проектов, которые будут привязаны к кнопкам на главной странице
             * каждая команда имеет свою функцию (которая указана в скобках), которая будет вызвана и исполнена с теми параметрами, которые 
             ей будут переданы, например для создания нового проекта будет совершен переход на новую страницу с 
            пустым проектом*/


            CreateProjectCommand = new MvvmHelpers.Commands.Command(CreateProject);
            ManageProjectCommand = new MvvmHelpers.Commands.Command(ManageProject);
            EditTimeCommand = new MvvmHelpers.Commands.Command(EditTime);
            EditDayTimeCommand = new MvvmHelpers.Commands.Command(EditDayTime);
            ToggleTimerCommand = new MvvmHelpers.Commands.Command(ToggleTimer);

            MessagingCenter.Subscribe<Project>(this, "MsgSaveProject", (project) =>
            {
                ExecuteUpdateProject(project);
                Application.Current.MainPage.DisplayAlert("Info", "Changes Saved!", "OK");
            });

            MessagingCenter.Subscribe<Project>(this, "MsgUpdateLastSyncProject", (project) =>
            {
                ExecuteUpdateLastSync(project);
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
                        {
                            var DaysList = App.DBDays.GetItems().Where(s => s.ProjectId == p.Id).ToList();
                            foreach (var d in DaysList)
                            {
                                App.DBDays.DeleteItem(d.Id.Value);
                            }
                            var SessionssList = App.DBSessions.GetItems().Where(s => s.ProjectId == p.Id).ToList();
                            foreach (var s in SessionssList)
                            {
                                App.DBSessions.DeleteItem(s.Id);
                            }

                            Projects.Remove(p);
                        }
                });
                IsBusy = false;
            });

            MessagingCenter.Subscribe<Project>(this, "EditTimeMessage", (project) =>
            {
                ExecuteUpdateTime(project);
            });

            MessagingCenter.Subscribe<Project>(this, "EditDayTimeMessage", (project) =>
            {
                ExecuteUpdateDayTime(project);
            });

            MessagingCenter.Subscribe<Project, TimeSpan>(this, "RecalcDays", (project, gap) =>
            {
                ExecuteRecalcDays(project, gap);
            });
        }

        public INavigation Navigation;
        public ObservableRangeCollection<Project> Projects { get; set; }
        public ICommand CreateProjectCommand { get; }
        public ICommand ManageProjectCommand { get; }
        public ICommand EditTimeCommand { get; }
        public ICommand EditDayTimeCommand { get; }
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
                    CheckDayAfterStop(ActiveProject[0]);
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
                    CheckDayAfterStop(project);
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
            if (ActiveProject.Count == 0)
                return;

            //quick fix
            MoveProjectToTop(ActiveProject[0]);

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
            foreach (var p in Projects)
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

                if (project.CurrentDayId == 0)
                {
                    project.CurrentDayId = App.DBDays.SaveItem(new Day(project.Id, DateTime.Today, project.DayTime));
                }

                project.SessionStartTime = DateTime.Now;
                project.SessionTime = new TimeSpan(0, 0, 0);

                project.IsRunning = true;

                App.DBProjects.SaveItem(project);
            }
        }

        public void Stop(Project project)
        {
            if (project.IsRunning)
            {
                project.IsRunning = false;

                project.Time += project.SessionTime;
                project.DayTime += project.SessionTime;

                App.DBSessions.SaveItem(new Session(project.Id, project.SessionStartTime,
                    project.SessionStartTime + project.SessionTime, project.SessionTime)

                );

                App.DBProjects.SaveItem(project);

                SaveDay2(project);
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
                    App.DBProjects.SaveItem(p);
                    break;
                }
        }

        public void ExecuteUpdateTime(Project project)
        {
            foreach (var p in Projects)
                if (p.Id == project.Id)
                {
                    p.Time = project.Time;
                    App.DBProjects.SaveItem(p);
                    SaveDay2(p);
                    break;
                }
            
        }

        public void ExecuteUpdateLastSync(Project project)
        {
            foreach (var p in Projects)
                if (p.Id == project.Id)
                {
                    p.LastSyncDate = project.LastSyncDate;
                    p.LastSyncTime = project.LastSyncTime;
                    App.DBProjects.SaveItem(p);
                    break;
                }
        }

        public async void EditTime(object parameter)
        {
            var tempProject = new Project(parameter as Project);
            await Navigation.PushAsync(new EditTimePage(tempProject));
        }


        #region DAYTIME FEATURE
        public void CheckDay()
        {
            foreach (var p in Projects)
            {
                if (p.IsRunning) continue;

                //New day started and need push DayTime to DB, assume Time (total) is ok
                if (p.DayTime.TotalHours > 0 && p.SessionStartTime.Date != DateTime.Today)
                {
                    /*var _day = App.DBDays.FindByDate(p.Id, p.SessionStartTime.Date);

                    if (_day == null)
                    {
                        _day = new Day(p.Id, p.SessionStartTime.Date, new TimeSpan());
                    }

                    _day.Time = p.DayTime;
                    App.DBDays.SaveItem(_day);
                    */
                    //Reset DayTime so this won't trigger again untill project touched
                    p.DayTime = new TimeSpan(0,0,0);
                    App.DBProjects.SaveItem(p);
                }
            }
        }

        public async Task CheckDayAfterStop(Project p)
        {

            //New day started while working, 
            if (p.SessionStartTime.Date != DateTime.Today)
            {
                var _day = App.DBDays.FindByDate(p.Id, p.SessionStartTime.Date);

                if (_day == null)
                {
                    _day = new Day(p.Id, p.SessionStartTime.Date, new TimeSpan());
                }

                if (p.DayTime.TotalHours < 12)
                {
                    TimeSpan endedDayTime = p.DayTime - DateTime.Now.TimeOfDay;
                    _day.Time = endedDayTime;

                    App.DBDays.SaveItem(_day);

                    p.SessionStartTime = DateTime.Now.Date;
                    p.DayTime = DateTime.Now.TimeOfDay;

                    App.DBProjects.SaveItem(p);
                    SaveDay2(p);
                }
                else
                {
                    string result = await App.Current.MainPage.DisplayPromptAsync("Correct time",
                        "Enter desired hours (1-12) for last working Day",
                        initialValue: "6", maxLength: 2, keyboard: Keyboard.Numeric);
                    int hours = Convert.ToInt32( Double.Parse(result));

                    _day.Time = new TimeSpan(hours, 0, 0);
                    App.DBDays.SaveItem(_day);

                    p.Time += (-p.DayTime + _day.Time);

                    p.SessionStartTime = DateTime.Today;
                    p.DayTime = new TimeSpan();
                    App.DBProjects.SaveItem(p);
                    SaveDay2(p);
                }
            }
        }

        public void SaveDay2(Project project)
        {
            for (int i = 0; i < 3; i++)
            {
                var _day = App.DBDays.FindByDate(project.Id, DateTime.Today);

                if (_day == null)
                {
                    break;
                }
                else
                {
                    App.DBDays.DeleteItem(_day.Id.Value);
                }
            }

            try
            {
                var _day = App.DBDays.FindByDate(project.Id, DateTime.Today);

                if (_day == null)
                {
                    _day = new Day(project.Id, DateTime.Today, project.DayTime);
                }
                else
                {
                    _day.Time = project.DayTime;
                }

                App.DBDays.SaveItem(_day);
            }
            catch
            {
                App.Current.MainPage.DisplayAlert("Error", "Could not save Day time of the project", "Close");
            }

        }

        public void ExecuteUpdateDayTime(Project project)
        {
            foreach (var p in Projects)
                if (p.Id == project.Id)
                {
                    p.DayTime = project.DayTime;
                    p.Time = project.Time;

                    App.DBProjects.SaveItem(p);
                    SaveDay2(p);
                    break;
                }
            
            
        }

        public async void EditDayTime(object parameter)
        {
            var tempProject = new Project(parameter as Project);
            await Navigation.PushAsync(new EditDayTime(tempProject));
        }

        private void ExecuteRecalcDays (Project project, TimeSpan gap)
        {
            var DaysList = App.DBDays.GetItems().Where(s => s.ProjectId == project.Id).ToList();

            if (DaysList.Count == 0)
            {
                if (gap.TotalHours > 0)
                    App.DBDays.SaveItem(new Day(project.Id, DateTime.Today, gap));

                return;
            }

            if (!DaysList.Any(d => d.Date == DateTime.Today))
            {
                DaysList.Add(new Day(project.Id, DateTime.Today, new TimeSpan()));
            }

            DaysList.Reverse();

            foreach (var d in DaysList)
            {
                if (d.Time.TotalHours + gap.TotalHours < 0)
                {
                    gap += d.Time;
                    App.DBDays.DeleteItem(d.Id.Value);

                    if (d.Date.Date == DateTime.Today)
                    {
                        project.DayTime = new TimeSpan();
                        ExecuteUpdateDayTime(project);
                        //App.DBProjects.SaveItem(project);
                    }
                }
                else
                {
                    d.Time += gap;
                    App.DBDays.SaveItem(d);

                    if (d.Date == DateTime.Today)
                    {
                        project.DayTime += gap ;
                        ExecuteUpdateDayTime(project);

                        //App.DBProjects.SaveItem(project);
                    }

                    return;
                }
            }
        }
        #endregion
    }
}
