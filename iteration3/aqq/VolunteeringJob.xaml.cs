using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace aqq
{
    public partial class VolunteeringJob : ContentPage
    {
		VolunteeringManager vojob;
		Entry post = null;
		StackLayout parent = null;
		ActivityIndicator indicator = null;
        ScrollView scroll = new ScrollView();
		StackLayout inner = new StackLayout();
        List<string> temp = new List<string>();
		public VolunteeringJob()
		{
			vojob = VolunteeringManager.DefaultManager;
			parent = new StackLayout();
			Label header = new Label
			{
				Text = " we will help you find volunteering organizations near your postcode: ",
				HorizontalOptions = LayoutOptions.Center
			};
			post = new Entry
			{
				Placeholder = "postcode"
			};

			Button button = new Button
			{
				Text = " Search ",
				HorizontalOptions = LayoutOptions.Center,
                BackgroundColor=Color.FromHex("#4DAFEA"),
				TextColor = Color.White,
				BorderRadius = 10,
				HeightRequest = 35,
				WidthRequest = 80

			};
			button.Clicked += Handle_ClickedAsync;
			indicator = new ActivityIndicator
			{
				IsVisible = false,
				Color = Color.Black,
				IsEnabled = true,
				IsRunning = true,
				MinimumHeightRequest = 20

			};
			parent.Children.Add(header);
			parent.Children.Add(post);
			parent.Children.Add(button);
            parent.Children.Add(indicator);
            parent.BackgroundColor = Color.FromHex("#f8f6f6");
            scroll.Content = parent;
            parent.Children.Add(inner);
            Content = scroll;

		}

		async void Handle_ClickedAsync(object sender, System.EventArgs e)
		{
			indicator.IsVisible = true;
			List<Volunteering> vol1;
			inner.Children.Clear();
			if (post.Text.ValidPost())
			{
				vol1 = await vojob.GetNameAsync(int.Parse(post.Text));


				if (vol1.Count > 0)
				{

					Label label2 = new Label
					{
						Text = "You can contact the following volunteering organizations that are close to your location:"
					};
					inner.Children.Add(label2);
					for (int i = 0; i < vol1.Count; i++)
					{

						Button button1 = new Button
						{

                            Text = vol1[i].Name+" , "+vol1[i].Postcode,
							HorizontalOptions = LayoutOptions.Start

						};
						button1.Clicked += Handle2_ClickedAsync;
						inner.Children.Add(button1);
					}

				}
				else
				{
					Label label2 = new Label
					{
						Text = "There is no provider near you. We have suggested some organizations below:"
					};
                    inner.Children.Add(label2);
					temp.Add("Volunteering Victoria,3000");
					temp.Add("Volunteer West,3011");
					temp.Add("Hume U3A Inc.,3047");
					temp.Add("Moreland U3A Inc.,3056");
					temp.Add("Shekinah Homeless Services Inc.,3065");
					temp.Add("Manningham Youth and Family Services,3108");
					temp.Add("U3A Box Hill Inc.,3129");
					temp.Add("Eastern Volunteer Resource Centre,3134");
					temp.Add("Monash Volunteer Resource Centre,3150");
					temp.Add("Wimmera Volunteers Inc,3400");
					temp.Add("La Trobe Information and Support Centre,3840");
				
					for (int i = 0; i < temp.Count; i++)
					{

						Button button2 = new Button
						{

							Text = temp[i].ToString(),
							HorizontalOptions = LayoutOptions.Start

						};
						button2.Clicked += Handle2_ClickedAsync;
						inner.Children.Add(button2);
					}




					
				}

			}
			else
			{
				Label label2 = new Label
				{
					Text = "Please enter a valid Victoria postcode(e.g.3000)",
					FontSize = 12,
					TextColor = Color.Red
				};
				inner.Children.Add(label2);
			}
            indicator.IsVisible = false; 
		}
		async void Handle2_ClickedAsync(object sender, System.EventArgs e)
		{
			var s = sender as Button;
			await Navigation.PushAsync(new VolunteeringMap(s.Text));
		}

		

}

}
