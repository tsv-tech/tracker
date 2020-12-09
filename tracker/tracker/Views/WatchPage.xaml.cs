using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using tracker.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace tracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WatchPage : ContentPage
    {
        public WatchPage()
        {
            InitializeComponent();
            BindingContext = App.WATCH_VM;
            App.WATCH_VM.Navigation = this.Navigation;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //if (!Application.Current.Properties.ContainsKey("watchMode"))
            //    Application.Current.Properties.Add("watchMode", true);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Application.Current.Properties.Remove("watchMode");
        }
    }
}