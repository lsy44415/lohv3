using System;
using System.Collections.Generic;
using Xamarin.Forms.Maps;
using Xamarin.Forms;
using Plugin.Geolocator;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace aqq
{
    public partial class MapMain : TabbedPage
    {
        LibraryManager lib;
        PersonManager per;
        VolunteerManager vol;
        PostcodeManager po;
        BicycleManager bi;
        Geocoder geoCoder;

        public MapMain()
        {
            InitializeComponent();
			cl.Source = ImageSource.FromResource("aqq.Image.cl.png");
			cp.Source = ImageSource.FromResource("aqq.Image.cp.png");
            per = PersonManager.DefaultManager;
            vol = VolunteerManager.DefaultManager;
            lib = LibraryManager.DefaultManager;
            po = PostcodeManager.DefaultManager;
            bi = BicycleManager.DefaultManager;


        }


		async void cpTapped(object sender, EventArgs args)
		{
			if (Application.Current.Properties.ContainsKey("Postcode"))
			{
				await GetPostcode();
			}
			
		}
		async void clTapped(object sender, EventArgs args)
		{
			await GetCurrent();
		}


        protected override async void OnAppearing()
        {
            indicator.IsVisible = true;
			
            await SetPin();
            await SetBicycle();
            indicator.IsVisible = false;
			

        }

        async Task Handle_Clicked(object sender, System.EventArgs e)
        {
            geoCoder = new Geocoder();
            var position = await CrossGeolocator.Current.GetPositionAsync();
            var currentPosition = new Position(position.Latitude, position.Longitude);
            if (Application.Current.Properties.ContainsKey("Postcode"))
            {
                MapCurrent.IsVisible = true;
                myMap.IsVisible = false;
                MapCurrent.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(-37.814, 144.96332), Distance.FromKilometers(10)));
            }
            else
            {
                MapCurrent.IsVisible = false;
                myMap.IsVisible = true;
                myMap.MoveToRegion(MapSpan.FromCenterAndRadius(currentPosition, Distance.FromKilometers(7)));
            }


        }


        void Handle2_Clicked(object sender, System.EventArgs e)
        {
            bicMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                      new Position(-37.814, 144.96332), Distance.FromMiles(3)));
        }


        async Task SetPin()
        {
            
            if (Application.Current.Properties.ContainsKey("Status"))
            {
                var status = Application.Current.Properties["Status"] as string;
                if (status.Equals("Learner"))
                {
                    var email = Application.Current.Properties["Email"] as string;
                    List<Learner> p1 = await per.GetIDAsync(email);
                    if (p1.Count > 0 && p1[0].Skill.Trim().Length > 0 && p1[0].Name.Trim().Length > 0)
                    {

                        Application.Current.Properties["Postcode"] = p1[0].Postcode;

                    }
                }
                else
                {
                    var email = Application.Current.Properties["Email"] as string;
                    List<Volunteer> l1 = await vol.GetIDAsync(email);
                    if (l1.Count > 0)
                    {
                        if (l1[0].Name.Trim().Length > 0 && l1[0].Skill.Trim().Length > 0)
                        {
                            Application.Current.Properties["Postcode"] = l1[0].Postcode;
                        }
                    }
                }
            }
            if (Application.Current.Properties.ContainsKey("Postcode"))
            {
               
                await GetPostcode();
            }
            else
            {
               
                await GetCurrent();
            }
        }
           

        async Task GetPostcode()
        {
            geoCoder = new Geocoder();
            var position = await CrossGeolocator.Current.GetPositionAsync();
            var currentPosition = new Position(position.Latitude, position.Longitude);
            var Addresses = await geoCoder.GetAddressesForPositionAsync(currentPosition);
            MapCurrent.IsVisible = true;
            myMap.IsVisible = false;
		
			string fulladdress = "";
			foreach (var address in Addresses)
			{
				fulladdress += address;
			}
            MapCurrent.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(-37.814, 144.96332), Distance.FromKilometers(10)));
            var post = Application.Current.Properties["Postcode"] as string;
            List<Library> library = await lib.GetbyPostcodeAsync(int.Parse(post));
			//  test.Text = "We display the top " + library.Count.ToString() + " most close Library of your postcode";
			testLogin.IsEnabled = true;
			testLogin.IsVisible = true;
			testNoneLogin.IsEnabled = false;
			testNoneLogin.IsVisible = false;


            if (library.Count > 0)
            {
                for (int i = 0; i < library.Count; i++)
                {
                    var pin = new CustomPin
                    {
                        Pin = new Pin
                        {
                            Type = PinType.Place,
                            Position = new Position(Double.Parse(library[i].Lat), Double.Parse(library[i].Lon)),
                            Label = library[i].Name,
                            Address = library[i].Address
                        }
                    };

                    MapCurrent.CustomPins = new List<CustomPin> { pin };
                    MapCurrent.Pins.Add(pin.Pin);
                    pin.Pin.Clicked += async (object sender, EventArgs e) =>
                    {
                        var p1 = sender as Pin;
                        await SetMap(p1.Address);
                    };
                }
                var myPin = new Pin
                {
                    Type = PinType.Generic,
                    Position = currentPosition,
                    Label = "Current Location",
                    Address=fulladdress
                };

                MapCurrent.Pins.Add(myPin);

            }
        }
      
        async Task GetCurrent(){

			geoCoder = new Geocoder();
			var position = await CrossGeolocator.Current.GetPositionAsync();
			var currentPosition = new Position(position.Latitude, position.Longitude);
			var Addresses = await geoCoder.GetAddressesForPositionAsync(currentPosition);
			MapCurrent.IsVisible = false;
			myMap.IsVisible = true;
			myMap.MoveToRegion(MapSpan.FromCenterAndRadius(currentPosition, Distance.FromKilometers(7)));
			string fulladdress = "";
			foreach (var address in Addresses)
			{
				fulladdress += address;
			}
			var currentcode = await po.GetbyPostcodeAsync(fulladdress);
			List<Library> library = await lib.GetbyPostcodeAsync(3168);
			//test.Text = "We display the top " + library.Count.ToString() + " most close Library of your current position";

			testLogin.IsEnabled = false;
			testLogin.IsVisible = false;
			testNoneLogin.IsEnabled = true;
			testNoneLogin.IsVisible = true;

			for (int i = 0; i < library.Count; i++)
			{
				var pin = new CustomPin
				{
					Pin = new Pin
					{
						Type = PinType.Place,
						Position = new Position(Double.Parse(library[i].Lat), Double.Parse(library[i].Lon)),
						Label = library[i].Name,
						Address = library[i].Address
					}

				};
				myMap.CustomPins = new List<CustomPin> { pin };
				myMap.Pins.Add(pin.Pin);
				pin.Pin.Clicked += async (object sender, EventArgs e) =>
				{
					var p1 = sender as Pin;
					await SetMap(p1.Address);
				};
			}
			var myPin = new Pin
			{
				Type = PinType.Generic,
				Position = currentPosition,
				Label = "Current Location",
                Address=fulladdress
			};
			myMap.Pins.Add(myPin);

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

        async Task SetBicycle(){
            indicator2.IsVisible = true;
            List<Bicycle> bicycle = await bi.GetAllAsync();
			test2.Text = "Free bicycle stands provided by City of Melbourne are displayed below:";
		
				for (int i = 0; i < bicycle.Count; i++)
				{
					var pin = new CustomPin
					{
						Pin = new Pin
						{
							Type = PinType.Place,
							Position = new Position(Double.Parse(bicycle[i].Lat), Double.Parse(bicycle[i].Lon)),
                          Label=bicycle[i].Name,
							Address = bicycle[i].Address
						}
					};

					bicMap.CustomPins = new List<CustomPin> { pin };
					bicMap.Pins.Add(pin.Pin);
				    pin.Pin.Clicked += async (object sender, EventArgs e) =>
                    {
                        var p = sender as Pin;
                        await SetMap(p.Address);

                    };
				}
			geoCoder = new Geocoder();
			var position = await CrossGeolocator.Current.GetPositionAsync();
			var currentPosition = new Position(position.Latitude, position.Longitude);
			var Addresses = await geoCoder.GetAddressesForPositionAsync(currentPosition);
			
			string fulladdress = "";
			foreach (var address in Addresses)
			{
				fulladdress += address;
			}
			var myPin = new Pin
			{
				Type = PinType.Generic,
				Position = currentPosition,
				Label = "Current Location",
                Address=fulladdress
			};
			

			bicMap.Pins.Add(myPin);
			bicMap.MoveToRegion(MapSpan.FromCenterAndRadius(
					  new Position(-37.814, 144.96332), Distance.FromMiles(3)));
			indicator2.IsVisible = false;
		}

    }
}
