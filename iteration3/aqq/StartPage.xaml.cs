using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace aqq
{
	public partial class StartPage : ContentPage
	{
		public StartPage()
		{
			InitializeComponent();
			NavigationPage.SetHasNavigationBar(this, false);
			background.Source = ImageSource.FromResource("aqq.Image.welcomepg.jpg");

		}

		async void Handle_Clicked(object sender, System.EventArgs e)
		{

			await Navigation.PushAsync(new SkipPage());



		}
	}
}