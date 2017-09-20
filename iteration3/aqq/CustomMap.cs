using System;
using System.Collections.Generic;
using Xamarin.Forms.Maps;

namespace aqq
{
    public class CustomMap: Map
    {
		public List<CustomPin> CustomPins { get; set; }
		public CustomCircle Circle { get; set; }

    }


    public class CustomPin{
		public Pin Pin { get; set; }
       public string postcode { get; set; }
    }

	public class CustomCircle
	{
		public Position Position { get; set; }
		public double Radius { get; set; }
	}
}
