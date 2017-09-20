﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace aqq
{
    public partial class FoodPage : ContentPage
    {
        public FoodPage()
        {
            InitializeComponent();
            bgFod.Source = ImageSource.FromResource("aqq.Image.bg1.jpg");
        }
		void OnLabel1Tapped(object sender, EventArgs args)
		{
			Device.OpenUri(new Uri("https://www.edx.org/course/science-cooking-haute-cuisine-soft-harvardx-spu27-1x-0"));
		}
		void OnLabel2Tapped(object sender, EventArgs args)
		{
			Device.OpenUri(new Uri("https://www.edx.org/course/sustainable-food-security-food-access-wageningenx-fssfax"));
		}
		void OnLabel3Tapped(object sender, EventArgs args)
		{
			Device.OpenUri(new Uri("https://www.udemy.com/up-beet-cooking-fundamentals/"));
		}
		void OnLabel4Tapped(object sender, EventArgs args)
		{
			Device.OpenUri(new Uri("https://www.edx.org/course/nutrition-health-macronutrients-wageningenx-nutr101x"));
		}
		void OnLabel5Tapped(object sender, EventArgs args)
		{
			Device.OpenUri(new Uri("https://www.udemy.com/simple-and-easy-japanese-fish-recipes-cooked-in-a-microwave/"));
		}
		void OnLabel6Tapped(object sender, EventArgs args)
		{
			Device.OpenUri(new Uri("https://www.udemy.com/faster-than-rice-cooker-microwave-rice-recipes/"));
		}
		async void Handle_Activated(object sender, System.EventArgs e)
		{
			var options = await DisplayActionSheet("Skills list", "Cancel", null, "English", "Health", "ICT","Home");

			if (options.Equals("English"))
				await Navigation.PushAsync(new EnglishPage());
			else if (options.Equals("Health"))
				await Navigation.PushAsync(new HealthPage());
			else if (options.Equals("ICT"))
				await Navigation.PushAsync(new ResourcePage());
			else if (options.Equals("Home"))
				await Navigation.PopToRootAsync();
		}
    }
}
