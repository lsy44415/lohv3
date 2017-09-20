using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace aqq
{
    public partial class EditCourse : ContentPage
    {
		CourseManager cou;
        LocalData data;
        public EditCourse()
        {
            InitializeComponent();
            bgHel.Source = ImageSource.FromResource("aqq.Image.bg1.jpg");
            data = new LocalData();
			var to = Application.Current.Properties["topic"] as string;
			var loc= Application.Current.Properties["location"] as string;
            topic1.Text = to;
            location1.ItemsSource = data.librarylist;
            location1.SelectedItem = loc;
			cou = CourseManager.DefaultManager;
        }


        async void Handle_Clicked(object sender, System.EventArgs e)
        {
            try
            {
                if (!topic1.Text.Equals(""))
                {
                    var email = Application.Current.Properties["Email"] as string;
                    var phone = Application.Current.Properties["Phone"] as string;
                    var name = Application.Current.Properties["Name"] as string;
                    var skill = Application.Current.Properties["Skill"] as string;
                    var id = Application.Current.Properties["classid"] as string;
                    var newdate = date1.Date.Day.ToString().FormatDate() + "/" + date1.Date.Month.ToString().FormatDate() + "/" + date1.Date.Year.ToString();
                    List<Course> c1 = await cou.GetUpdateAsync(email, newdate, time1.Time.ToString(), id);
                    if (c1.Count > 0)
                        await DisplayAlert("Sorry", "This date and time are in use. Please choose another one", "Ok");
                    else
                    {
                        var course = new Course
                        {
                            Id = id,
                            Topic = topic1.Text,
                            Email = email,
                            Location = location1.SelectedItem.ToString(),
                            Skill = skill,
                            Name = name,
                            Date = newdate,
                            Time = time1.Time.ToString(),
                            Phone =phone
                        };
                        await cou.UpdateTaskAsync(course);
                        await DisplayAlert("Thanks", "Update successful!", "ok");
                        await Navigation.PushAsync(new ClassListPage());
                    }
                }
				else
					label1.IsVisible = true;
			}
			catch (NullReferenceException ex)
			{
				await DisplayAlert("Alert", "you need to finish the form and choose the location", "ok");
			}
            }
    }
}
