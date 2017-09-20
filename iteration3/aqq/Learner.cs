using System;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace aqq
{
	public class Learner
	{
		string id;
        string name;
		string email;
		string postcode;
        string skill;
        string status;
        string phone;
		

		[JsonProperty(PropertyName = "id")]
		public string Id
		{
			get { return id; }
			set { id = value;}
		}

        [JsonProperty(PropertyName = "name")]
        public string Name
		{
			get { return name; }
			set { name = value;}
		}


		[JsonProperty(PropertyName = "email")]
		public string Email
		{
			get { return email; }
			set { email = value; }
		}

		[JsonProperty(PropertyName = "postcode")]
		public string Postcode
		{
			get { return postcode; }
			set { postcode = value; }
		}

		[JsonProperty(PropertyName = "skill")]
		public string Skill
		{
			get { return skill; }
			set { skill = value; }
		}

		[JsonProperty(PropertyName = "status")]
		public string Status
		{
			get { return status; }
			set { status = value; }
		}

		[JsonProperty(PropertyName = "phone")]
		public string Phone
		{
			get { return phone; }
			set { phone = value; }
		}


	}
}

