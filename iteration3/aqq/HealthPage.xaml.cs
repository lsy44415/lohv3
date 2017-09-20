﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace aqq
{
    public partial class HealthPage : ContentPage
    {
        public HealthPage()
        {
            InitializeComponent();
            bgHel.Source = ImageSource.FromResource("aqq.Image.bg1.jpg");
        }
		void OnLabel1Tapped(object sender, EventArgs args)
		{
			Device.OpenUri(new Uri("https://www.udemy.com/osha-safety-pro-personal-protective-equipment/"));
		}
		void OnLabel2Tapped(object sender, EventArgs args)
		{
			Device.OpenUri(new Uri("https://www.udemy.com/emergency-life-support/"));
		}
		void OnLabel3Tapped(object sender, EventArgs args)
		{
			Device.OpenUri(new Uri("https://www.udemy.com/sports-nutrition-crash-course-get-started-with-basics/"));
		}
		void OnLabel4Tapped(object sender, EventArgs args)
		{
			Device.OpenUri(new Uri("https://www.udemy.com/low-fodmap-diet/"));
		}
		void OnLabel5Tapped(object sender, EventArgs args)
		{
			Device.OpenUri(new Uri("https://www.edx.org/course/healthy-ageing-6-steps-let-environment-delftx-eit001x-0"));
		}
		void OnLabel6Tapped(object sender, EventArgs args)
		{
			Device.OpenUri(new Uri("https://www.edx.org/course/life-diabetes-curtinx-diab1x"));
		}
		async void Handle_Activated(object sender, System.EventArgs e)
		{
			var options = await DisplayActionSheet("Skills list", "Cancel", null, "Food","English", "ICT","Home");

			if (options.Equals("English"))
				await Navigation.PushAsync(new EnglishPage());
			else if (options.Equals("ICT"))
				await Navigation.PushAsync(new ResourcePage());
			else if (options.Equals("Food"))
				await Navigation.PushAsync(new FoodPage());
			else if (options.Equals("Home"))
				await Navigation.PopToRootAsync();
		}
    }
	
}
