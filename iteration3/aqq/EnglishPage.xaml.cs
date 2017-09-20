using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace aqq
{
    public partial class EnglishPage : ContentPage
    {
        public EnglishPage()
        {
            InitializeComponent();
            bgEng.Source = ImageSource.FromResource("aqq.Image.bg1.jpg");
        }
		void OnLabel1Tapped(object sender, EventArgs args)
		{
			Device.OpenUri(new Uri("https://www.udemy.com/how-to-self-study-english-online/"));
		}
		void OnLabel2Tapped(object sender, EventArgs args)
		{
			Device.OpenUri(new Uri("https://www.udemy.com/power-writing/"));
		}
		void OnLabel3Tapped(object sender, EventArgs args)
		{
			Device.OpenUri(new Uri("https://www.udemy.com/practise-speaking-english-nursery-rhymes/"));
		}
		void OnLabel4Tapped(object sender, EventArgs args)
		{
			Device.OpenUri(new Uri("https://www.udemy.com/everydayenglish/"));
		}
		void OnLabel5Tapped(object sender, EventArgs args)
		{
			Device.OpenUri(new Uri("https://www.udemy.com/master-idioms/"));
		}
		void OnLabel6Tapped(object sender, EventArgs args)
		{
			Device.OpenUri(new Uri("https://www.udemy.com/5-days-to-a-better-accent/"));
		}
		async void Handle_Activated(object sender, System.EventArgs e)
		{
			var options = await DisplayActionSheet("Skills list", "Cancel", null, "ICT", "Health", "Food","Home");

            if (options.Equals("ICT"))
                await Navigation.PushAsync(new ResourcePage());
            else if (options.Equals("Health"))
                await Navigation.PushAsync(new HealthPage());
            else if (options.Equals("Food"))
                await Navigation.PushAsync(new FoodPage());
            else if (options.Equals("Home"))
               await Navigation.PopToRootAsync();
            
		}
    }
}
