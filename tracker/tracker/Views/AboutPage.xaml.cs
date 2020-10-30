using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace tracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : ContentPage
    {
        public string CurrentYear { get; set; }
        public AboutPage()
        {
            InitializeComponent();
            CurrentYear = DateTime.Now.Year.ToString() + " ©";
            this.BindingContext = this;
        }
    }
}