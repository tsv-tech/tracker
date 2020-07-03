using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using tracker.Models;
using Xamarin.Forms;

namespace tracker.Tools
{
    /// <summary>
    /// this approach is lacking customization due to using 'Modal' page;
    /// trying to turn this into a 'Normal' page will cause additional problems with Null objects 
    ///     and (not sure) hanging threads
    /// Project now using MessagingCenter instead
    /// </summary>
    
    /*
    public class CreateDialog
    {
        public static Task<Project> InputBox(INavigation navigation, Project project)
        {
            // wait in this proc, until user did his input 
            var tcs = new TaskCompletionSource<Project>();
            //tcs.SetResult(null);

            var localProject = project; // ?? new Project();

            var lblTitle = new Label { Text = "NEW PROJECT", 
                HorizontalOptions = LayoutOptions.Center, 
                FontAttributes = FontAttributes.Bold, FontSize = 30 };
            var tName = new Entry { Placeholder = "Name" };
            var tAuthor = new Entry { Placeholder = "Author" };
            var tCustomId = new Entry { Placeholder = "Custom ID" };
            var tPayment = new Entry { Placeholder = "Payment", Text="25" };
            var tComment = new Entry { Placeholder = "Comment" };

            var btnOk = new Button
            {
                Text = "Create",
                //WidthRequest = 100,
                BackgroundColor = Color.FromRgb(0.8, 0.8, 0.8),
            };
            btnOk.Clicked += async (s, e) =>
            {
                // close page
                var result = new Project( tName.Text, tAuthor.Text, tCustomId.Text, tPayment.Text, tComment.Text);
                await navigation.PopModalAsync();
                // pass result
                tcs.SetResult(result);
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                //WidthRequest = 100,
                BackgroundColor = Color.FromRgb(0.8, 0.8, 0.8)
            };
            btnCancel.Clicked += async (s, e) =>
            {
                // 
                await navigation.PopModalAsync();
                // pass empty result
                tcs.SetResult(null);
            };

            var slButtons = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Orientation = StackOrientation.Horizontal,
                Children = { btnOk, btnCancel },
            };

            var layout = new StackLayout
            {
                //Padding = new Thickness(0, 40, 0, 0),
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Orientation = StackOrientation.Vertical,
                Children = { lblTitle, tName, tAuthor, tCustomId, tPayment, tComment, slButtons },
            };

            // create and show page
            var page = new ContentPage();
            page.BackgroundColor = Color.Transparent;
            //page.Padding = new Thickness(30);
            page.Content = layout;
            navigation.PushModalAsync(page);
            // open keyboard
            //tName.Focus();
            
            // code is waiting her, until result is passed with tcs.SetResult() in btn-Clicked
            // then proc returns the result
            return tcs.Task;
        }


    }*/
}
