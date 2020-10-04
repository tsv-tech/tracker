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
    public partial class SussessPage : ContentPage
    {
        public SussessPage()
        {
            InitializeComponent();
        }

        private void goBack(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}