using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace aqq
{
    public partial class JobPage : ContentPage
    {     
        JobManager jo;
        Job2Manager jo2;
        Entry post = null;
        ActivityIndicator indicator = null;
        StackLayout parent = null;
        StackLayout inner = new StackLayout();
        public JobPage()
        {   
            jo = JobManager.DefaultManager;
			jo2 = Job2Manager.DefaultManager;
            parent = new StackLayout();
			Label header = new Label
			{
                Text = " we will help you find employment services near your postcode: ",
				HorizontalOptions = LayoutOptions.Center
			};
            post = new Entry
            {
                Placeholder="postcode"
            };

            Button button = new Button
            {
                Text = " Search ",
                HorizontalOptions = LayoutOptions.Center,
                BackgroundColor = Color.FromHex("#4DAFEA"),
                TextColor = Color.White,
                BorderRadius=10,
                HeightRequest=35,
                WidthRequest=80
				
				
			};


			button.Clicked += Handle_ClickedAsync;


            indicator = new ActivityIndicator
            {
                IsVisible = false,
                Color=Color.Black,
                IsEnabled=true,
                IsRunning=true,
                 MinimumHeightRequest=20

            };


            parent.Children.Add(header);
            parent.Children.Add(post);
            parent.Children.Add(button);
			parent.Children.Add(indicator);
            Content = parent;
            parent.BackgroundColor = Color.FromHex("#f8f6f6");
			parent.Children.Add(inner);

        }

        async void Handle_ClickedAsync(object sender, System.EventArgs e)
        {
            indicator.IsVisible = true;
            List<Job> job1;
            inner.Children.Clear();
            if (post.Text.ValidPost())
            {
                job1 = await jo.GetNameAsync(post.Text);


                if(job1.Count>0)
                {
                    
                    Label label2 = new Label { 
                        Text="You can contact the following organizations that are close to your location:"
                    };
                    inner.Children.Add(label2);
                    for (int i = 0; i < job1.Count;i++)
                    {

                        Button button1 = new Button
                        {

                            Text = job1[i].Name,
                            HorizontalOptions = LayoutOptions.Start

                        };
                        button1.Clicked += Handle2_ClickedAsync;
                            inner.Children.Add(button1);
                    }
					
                }
                else{
					Label label2 = new Label
					{
                        Text = "There is no provider near you. We have suggested some places near the city center below:"
					};
					

					CustomMap customMap = new CustomMap
					{
						MapType = MapType.Street,
						WidthRequest = 200,
						HeightRequest = 400
					};

                    List<Job2> job2 = await jo2.GetAllAsync();
					
						for (int i = 0; i < job2.Count; i++)
						{
							var pins = new CustomPin
							{
								Pin = new Pin
                                {
                                    Type = PinType.Place,
                                    Position = new Position(Double.Parse(job2[i].Lat), Double.Parse(job2[i].Lon)),
                                    Label = job2[i].Name,
                                    Address = job2[i].Address
                                },
                            postcode=job2[i].Postcode
                           
                               
							};

							customMap.CustomPins = new List<CustomPin> { pins };
							customMap.Pins.Add(pins.Pin);
							pins.Pin.Clicked += async (object sender2, EventArgs e2) =>
							{
								var p1 = sender2 as Pin;
                              
							await Navigation.PushAsync(new JobMap(pins.postcode, p1.Label));
							};
						}
					customMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(-37.814, 144.96332), Distance.FromKilometers(5)));
                    inner.Children.Add(label2);
                    inner.Children.Add(customMap);
				}
				
            }
            else
            {
				Label label2 = new Label
				{
                    Text = "Please enter a valid Victoria postcode(e.g.3000)",
                    FontSize=12,
                    TextColor= Color.Red
				};
				inner.Children.Add(label2);
            }
            indicator.IsVisible = false;
        }
       async  void Handle2_ClickedAsync(object sender, System.EventArgs e)
        {
			var s = sender as Button;
            await Navigation.PushAsync(new JobMap(post.Text,s.Text));
        }

    }
}
