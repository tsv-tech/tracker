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

namespace tracker.ViewModels
{
    public class ProjectsViewModel : BaseViewModel
    {
        public ProjectsViewModel()
        {
            Title = "Projects";
            Projects = new ObservableRangeCollection<Project>();
            var range = new List<Project>(App.DBProjects.GetItems());
            List<Project> newRange = new List<Project>();
            foreach (var p in range)
            {
                newRange.Add(new Project(p));
            }
            Projects.AddRange(newRange);

            //GetProjectsCommand = new MvvmHelpers.Commands.Command(GetList);
            GetProjectsCommand = new MvvmHelpers.Commands.Command(ClearTable);
            CreateProjectCommand = new MvvmHelpers.Commands.Command(CreateProject);
            ManageProjectCommand = new MvvmHelpers.Commands.Command(ManageProject);
            SendProjectCommand = new MvvmHelpers.Commands.Command(SendProject);

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
                //App.DBProjects.DeleteItem(project.Id);
                IsBusy = true;
                Task.Delay(1000).ContinueWith(t =>
                {
                    foreach (var p in Projects)
                        if (p.Id == project.Id)
                            Projects.Remove(p);
                });
                IsBusy = false;
            });
        }

        public INavigation Navigation { get; set; }
        public ObservableRangeCollection<Project> Projects { get; set; }
        public MvvmHelpers.Commands.Command GetProjectsCommand { get; }
        public MvvmHelpers.Commands.Command CreateProjectCommand { get; }
        public MvvmHelpers.Commands.Command ManageProjectCommand { get; }
        public MvvmHelpers.Commands.Command SendProjectCommand { get; }


        public void GetList()
        {
            var projects = new ObservableRangeCollection<Project> {
                new Project("PRJ 1", "AUTH 1", "ID 1", "25", "COMMENT 1"),
                new Project("PRJ 2", "AUTH 2", "ID 2", "25", "COMMENT 2")
                };

            Projects.AddRange(projects);
        }

        public void ClearTable()
        {
            Projects.Clear();
            App.DBProjects.ClearTable();
        }

        public async void CreateProject()
        {
            /*
            Project input = await CreateDialog.InputBox(this.Navigation, null); //pass Project = null if creating new
            if (input != null)
            {
                Projects.Add(input);
                App.DBProjects.SaveItem(input);
            }*/
            await Navigation.PushAsync(new NewProjectPage(new Project()));
        }

        public async void ManageProject(object parameter)
        {
            var tempProject = new Project(parameter as Project);

            await Navigation.PushAsync(new ViewProjectPage(tempProject));
        }

        public void ExecuteUpdateProject(Project project)
        {
            //int index;
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

        public async void SendProject()
        {

        }
    }
}
