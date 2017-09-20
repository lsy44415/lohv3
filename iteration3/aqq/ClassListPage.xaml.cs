using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace aqq
{
    public partial class ClassListPage : ContentPage
    {

		CourseManager cou;
		
		
        public ClassListPage()
        {
            InitializeComponent();
			var skill = Application.Current.Properties["Skill"] as string;
            title.Text = skill +title.Text;
            cou = CourseManager.DefaultManager;
        }
		protected override async void OnAppearing()
		{
			base.OnAppearing();
			var email = Application.Current.Properties["Email"] as string;
            var skill = Application.Current.Properties["Skill"] as string;
            var status = Application.Current.Properties["Status"] as string;
            if(status.Equals("Volunteer") || status.Equals("Mentor"))
                listview1.ItemsSource = await cou.GetEmailClassAsync( email);
          
		}

        async void Edit_Clicked(object sender, System.EventArgs e)
        {
            
                var menuItem = sender as MenuItem;
                var course = menuItem.CommandParameter as Course;
                var co1 = await cou.GetIDAsync(course.Email, course.Date, course.Time);
                Application.Current.Properties["classid"] = co1[0].Id;
                Application.Current.Properties["topic"] = co1[0].Topic;
                Application.Current.Properties["location"] = co1[0].Location;
                // var id = Application.Current.Properties["classid"] as string;
                await Navigation.PushAsync(new EditCourse());
                listview1.SelectedItem = null;


        }

		async void Delete_Clicked(object sender, System.EventArgs e)
		{
           
                var menuItem = sender as MenuItem;
                var course = menuItem.CommandParameter as Course;
                await cou.DeleteTaskAsync(course);
                await DisplayAlert("Notice", "Delete successful!", "ok");
                var email = Application.Current.Properties["Email"] as string;
                listview1.ItemsSource = await cou.GetEmailClassAsync(email);
				listview1.SelectedItem = null;


		}
    }
}
