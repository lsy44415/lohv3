using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.Geolocator;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace aqq
{
    public partial class VolunteeringMap : ContentPage
    {
        string[] text;

        Geocoder geoCoder;
        VolunteeringManager voj;

        public VolunteeringMap(string s)
        {
            InitializeComponent();
            text = s.Split(',');
            voj = VolunteeringManager.DefaultManager;
			displayAsync();

        }
		async Task displayAsync()
		{
            syncIndicator.IsVisible = true;
			List<Volunteering> volun = await voj.GetDetailAsync(text[0]);
           
			label1.Text = volun[0].Name;
			label2.Text = "Address:  " + volun[0].Address;
			label3.Text = "Suburb:   " + volun[0].Suburb;
			label4.Text = "Postcode: " + volun[0].Postcode;

			geoCoder = new Geocoder();
			var position = await CrossGeolocator.Current.GetPositionAsync();
			var currentPosition = new Position(position.Latitude, position.Longitude);
			var Addresses = await geoCoder.GetAddressesForPositionAsync(currentPosition);

			var myPin = new Pin
			{
				Type = PinType.Generic,
				Position = currentPosition,
				Label = "current location"
			};




                var destnationAddress = volun[0].Address;
				var approximateLocation = await geoCoder.GetPositionsForAddressAsync(destnationAddress);
            double lat = 0;
            double lon = 0;
                foreach (var p in approximateLocation)
			    {
                lat = p.Latitude;
                lon = p.Longitude;
			    }


			 var pin1 = new Pin
			{
				Type = PinType.Place,
				Position = new Position(lat,lon),
				Label = volun[0].Name,
				Address = volun[0].Address
			};
			pin1.Clicked += async (object sender, EventArgs e) =>
			{
				var p1 = sender as Pin;
				await SetMap(p1.Address);
			};
			myMap.Pins.Add(myPin);
			myMap.Pins.Add(pin1);
			myMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(lat, lon), Distance.FromKilometers(1)));
            syncIndicator.IsVisible = false;
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
    }
}
