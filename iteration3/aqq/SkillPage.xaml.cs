using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace aqq
{
    public partial class SkillPage : ContentPage
    {
        public SkillPage()
        {
            InitializeComponent();
            bg.Source = ImageSource.FromResource("aqq.Image.bg1.jpg");
        }

   //     async void Handle_ClickedAsync(object sender, System.EventArgs e)
   //     {
   //         try{
                
          
			//if (!picker.SelectedItem.ToString().Trim().Equals(""))
			//{
   //         if (picker.SelectedItem.ToString().Equals("ICT"))
   //             await Navigation.PushAsync(new ResourcePage());
   //         else if (picker.SelectedItem.ToString().Equals("English"))
   //             await Navigation.PushAsync(new EnglishPage());
   //         else if (picker.SelectedItem.ToString().Equals("Health"))
   //             await Navigation.PushAsync(new HealthPage());
   //         else if (picker.SelectedItem.ToString().Equals("Food"))
   //             await Navigation.PushAsync(new FoodPage());
   //          }
			//}
    //        catch(NullReferenceException ex)
    //        {
    //            await DisplayAlert("Alert", "Please choose one skill", "OK");
    //        }
    //}

		async void OnImage1Tapped(object sender, EventArgs args)
		{
			await Navigation.PushAsync(new ResourcePage());
		}
		async void OnImage2Tapped(object sender, EventArgs args)
		{
			await Navigation.PushAsync(new EnglishPage());
			//await Navigation.PushAsync(new MyPage());
		}
		async void OnImage3Tapped(object sender, EventArgs args)
		{
			await Navigation.PushAsync(new HealthPage());
			//await Navigation.PushAsync(new MyPage());
		}
		async void OnImage4Tapped(object sender, EventArgs args)
		{
			await Navigation.PushAsync(new FoodPage());
			//await Navigation.PushAsync(new MyPage());


		}

    }
}
