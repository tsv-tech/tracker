using MvvmHelpers;
using MvvmHelpers.Commands;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using tracker.Models;
using Xamarin.Forms;

namespace tracker.ViewModels
{
    public class ProjectViewModel : BaseViewModel
    {
        public ProjectViewModel(Project prj, INavigation navigation)
        {
            Project = prj;
            Navigation = navigation;

            SaveCommand = new MvvmHelpers.Commands.Command(Save);
            CancelCommand = new MvvmHelpers.Commands.Command(Cancel);
        }

        INavigation Navigation;
        ICommand SaveCommand { get; }
        ICommand CancelCommand { get; }

        public bool ItemChanged { get; set; } = false;
        public ObservableRangeCollection<Session> Sessions { get; set; }

        Project Project;

        public void Save()
        {
            ItemChanged = true;
            Navigation.PopAsync();
        }

        public void Cancel()
        {
            ItemChanged = false;
            Navigation.PopAsync();
        }
    }

    

}
