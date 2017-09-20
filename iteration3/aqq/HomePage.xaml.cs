﻿﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace aqq
{
    public partial class HomePage : TabbedPage
    {
        public HomePage()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this,false);
           
        }

       async void Login_Clicked(object sender, System.EventArgs e)
        {
           await Navigation.PushAsync(new Login());
        }

        async void OnImage1Tapped(object sender, EventArgs args)
        {
			await Navigation.PushAsync(new SkillPage());
		}
		async void OnImage2Tapped(object sender, EventArgs args)
		{
            if (Application.Current.Properties.ContainsKey("skip"))
            {
                var flag = Application.Current.Properties["skip"] as string;
                if (flag.Equals("false"))
                {
                    var st = Application.Current.Properties["Status"] as string;
                    if (st.Equals("Volunteer") || st.Equals("Mentor"))
                        await Navigation.PushAsync(new VolunteerPage());
                    if (st.Equals("Learner"))
                        await Navigation.PushAsync(new FormPage());
                }
                else
                {
					var result = await DisplayAlert("Login", "You need to login for peer-learning", "OK", "Cancel");
                    if (result == true)

                        await Navigation.PopToRootAsync();
				}
            }
			//await Navigation.PushAsync(new MyPage());
		}
		async void OnImage3Tapped(object sender, EventArgs args)
		{
			
            await Navigation.PushAsync(new MapMain());
		}
		async void OnImage4Tapped(object sender, EventArgs args)
		{
            await Navigation.PushAsync(new FriendListPage());
            //await Navigation.PushAsync(new MyPage());


		}
    }
}
