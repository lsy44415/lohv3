using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.Geolocator;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace aqq
{
    public partial class learnerClassList : ContentPage
    {
        Geocoder geoCoder;
		CourseManager cou;
        EnrolManager en;
        LibraryManager lib;
        public learnerClassList()
        {
            InitializeComponent();
            cou = CourseManager.DefaultManager;
            en = EnrolManager.DefaultManager;
            lib = LibraryManager.DefaultManager;
        }


		protected override async void OnAppearing()
		{
			base.OnAppearing();
			var email = Application.Current.Properties["Email"] as string;
			var skill = Application.Current.Properties["Skill"] as string;
			var status = Application.Current.Properties["Status"] as string;
			
				listview1.ItemsSource = await cou.GetSkillClassAsync(skill);
		}

		async void Enrol_Clicked(object sender, System.EventArgs e)
		{
			var menuItem = sender as MenuItem;
			var course = menuItem.CommandParameter as Course;
			var co1 = await cou.GetIDAsync(course.Email, course.Date, course.Time);
            var email = Application.Current.Properties["Email"] as string;
            var name = Application.Current.Properties["Name"] as string;
			var phone = Application.Current.Properties["Phone"] as string;
			Application.Current.Properties["classid"] = co1[0].Id;
			var id = Application.Current.Properties["classid"] as string;

			List<Enrol> enrol1 = await en.TestbyIDAsync(email,id);
            if (enrol1.Count > 0)
            {
                await DisplayAlert("Alert", "You have already enrolled this course!", "ok");
            }
            else
            {

                var enrol = new Enrol
                {
                    Teacher = co1[0].Name,
                    Location = co1[0].Location,
                    Date = co1[0].Date,
                    Time = co1[0].Time,
                    Topic = co1[0].Topic,
                    Email1 = email,
                    Email2 = co1[0].Email,
                    Learner = name,
                    Phone = phone,
                    Courseid = id,
                    Phone2=co1[0].Phone
                };
                await en.SaveTaskAsync(enrol);
                await DisplayAlert("Notice", "Enrolled successful!", "ok");
            }
        }


		async Task Map_Clicked(object sender, System.EventArgs e)
		{
			var menuItem = sender as MenuItem;
			var course = menuItem.CommandParameter as Course;
			var co1 = await cou.GetIDAsync(course.Email, course.Date, course.Time);
            var lib1 = await lib.GetbyNameAsync(co1[0].Location);
            Application.Current.Properties["Lat"] = lib1[0].Lat;
            Application.Current.Properties["Lon"] = lib1[0].Lon;
            Application.Current.Properties["place"] = co1[0].Location;
            await DisplayAlert("Notice", "You will go to the direction map", "ok");
            await SetMap(co1[0].Location);

		}

		async Task MapEnrol_Clicked(object sender, System.EventArgs e)
		{
			var menuItem = sender as MenuItem;
			var enrol = menuItem.CommandParameter as Enrol;
			Application.Current.Properties["place"] = enrol.Location;
			await DisplayAlert("Notice", "You will go to the direction map", "ok");
            await SetMap(enrol.Location);

		}



		async Task SetMap(string destination)
		{
			geoCoder = new Geocoder();
			var position = await CrossGeolocator.Current.GetPositionAsync();
			var currentPosition = new Position(position.Latitude, position.Longitude);
			var Addresses = await geoCoder.GetAddressesForPositionAsync(currentPosition);
			string fulladdress = "";
			foreach (var address in Addresses)
			{
				fulladdress += address;
			}

			var Destination = destination;
			if (Device.RuntimePlatform == Device.iOS)
			{
				//https://developer.apple.com/library/ios/featuredarticles/iPhoneURLScheme_Reference/MapLinks/MapLinks.html
				Device.OpenUri(new Uri("http://maps.apple.com/?daddr=" + Destination + "&saddr=" + fulladdress));

			}
			else if (Device.RuntimePlatform == Device.Android)
			{
				// opens the 'task chooser' so the user can pick Maps, Chrome or other mapping app
				Device.OpenUri(new Uri("http://maps.google.com/?daddr=" + Destination + "saddr=" + fulladdress));

			}
		}







        async void Handle_Clicked(object sender, System.EventArgs e)
        {
            listview2.IsVisible = true;
            listview1.IsVisible = false;
			var email = Application.Current.Properties["Email"] as string;
            listview2.ItemsSource = await en.GetEmailClassAsync(email);
        }

        void Handle_Clicked2(object sender, System.EventArgs e)
        {
            listview1.IsVisible = true;
            listview2.IsVisible = false;
        }

		async void Delete_Clicked(object sender, System.EventArgs e)
		{

			var menuItem = sender as MenuItem;
			var enrol = menuItem.CommandParameter as Enrol;
			await en.DeleteTaskAsync(enrol);
			await DisplayAlert("Notice", "Delete successful!", "ok");
			var email = Application.Current.Properties["Email"] as string;
			listview2.ItemsSource = await en.GetEmailClassAsync(email);
			listview2.SelectedItem = null;


		}
    }
}

