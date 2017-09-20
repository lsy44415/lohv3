using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.Geolocator;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace aqq
{
    public partial class JobMap : ContentPage

    {
        JobManager jo;
        string post;
        string provider;
		Geocoder geoCoder;

        public JobMap(string postcode, string name)
        {
            InitializeComponent();
			jo = JobManager.DefaultManager;
            post = postcode;
            provider = name;
            displayAsync();

        }
		

        async Task displayAsync()
        {
            syncIndicator.IsVisible = true;
			List<Job> job = await jo.GetDetailAsync(post, provider);
            label1.Text = "Name: " + job[0].Name;
			label2.Text = "Address: " + job[0].Address;
			label3.Text = "Phone: " + job[0].Phone;
            if (!job[0].Email.Equals("null"))
            {
                label4.Text = "Email: " + job[0].Email;
            }
            if (!job[0].Url.Equals("null"))
            {
                label5.Text = "Website: " + job[0].Url;
            }
			label6.Text = "Suburb: " + job[0].Location;
			label7.Text = "Postcode: " + job[0].Postcode;

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
			var pin1 = new Pin
			{
				Type = PinType.Place,
				Position = new Position(Double.Parse(job[0].Lat), Double.Parse(job[0].Lon)),
				Label = job[0].Name,
				Address = job[0].Address
			};
			pin1.Clicked += async (object sender, EventArgs e) =>
			{
				var p1 = sender as Pin;
				await SetMap(p1.Address);
			};
	       myMap.Pins.Add(myPin);
           myMap.Pins.Add(pin1);
            myMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(Double.Parse(job[0].Lat), Double.Parse(job[0].Lon)), Distance.FromKilometers(1)));
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
