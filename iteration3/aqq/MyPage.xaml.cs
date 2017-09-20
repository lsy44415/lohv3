using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using Newtonsoft.Json;
using SQLite;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace aqq
{

	
    public class Person2
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Skill { get; set; }
        [MaxLength(255)]
        public string Fname { get; set; }
        [MaxLength(255)]
        public string Lname { get; set; }
        public string Postcode { get; set; }
        public string Email { get; set; }
        public string Place { get; set; }
    }

    public class Place
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
    }


  //  public class Suburb
  //  {
  //      public string name { get; set; }
		//public string postcode { get; set; }
  //      [JsonConverter(typeof(CustomJsonConverter<State>))]
		//public List<State> states { get; set; }
		//public string locality { get; set; }
		//public string latitude { get; set; }
		//public string longtitude { get; set; }

  //  }


  //  public class State
  //  {
		//public string name { get; set; }
		//public string abbreviation{ get; set; }
    //}



    public partial class MyPage : ContentPage
    {
		PersonManager manager;
     //   private const string Url = "http://v0.postcodeapi.com.au/suburbs/3168.json";
    //   private HttpClient _client = new HttpClient();
        private SQLiteAsyncConnection _connection;
        //private SQLiteAsyncConnection _connection2;
        private List<string> placelist;
        private List<string> postcode;
        private ObservableCollection<Person2> _people;
    //    private ObservableCollection<Suburb> _suburb;
        //   private ObservableCollection<Place> _place;

        public MyPage()
        {
            InitializeComponent();
            SetPicker();
            setPostcode();
            manager = PersonManager.DefaultManager;
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
			//  _connection2 = DependencyService.Get<ISQLiteDb>().GetConnection();
			//if (manager.IsOfflineEnabled && Device.OS == TargetPlatform.Windows)
			//{
			//	var syncButton = new Button
			//	{
			//		Text = "Sync items",
			//		HeightRequest = 30
			//	};
			//	syncButton.Clicked += OnSyncItems;

			//	buttonsPanel.Children.Add(syncButton);
			//}
        }


        protected override async void OnAppearing()
        {
            //var content = await _client.GetStringAsync(Url);
            //var suburbs = JsonConvert.DeserializeObject<List<Suburb>>(content);

            //_suburb = new ObservableCollection<Suburb>(suburbs);

            //listview2.ItemsSource = _suburb;

            //test.Text = suburbs[0].states.ToString();
               

           // ll1.SetBinding(Label.TextProperty, new Binding("/name"){Source=_suburb});

			await _connection.CreateTableAsync<Person2>();

            var people = await _connection.Table<Person2>().ToListAsync();
            _people = new ObservableCollection<Person2>(people);
          //  listview1.ItemsSource = _people;
           
            //     await _connection2.CreateTableAsync<Place>();
            //    var place = await _connection2.Table<Place>().ToListAsync();
            //  _place = new ObservableCollection<Place>(place);
            // picker2.ItemsSource = _place;
            //picker2.SetBinding(Picker.ItemsSourceProperty, "Place");
            //picker2.ItemDisplayBinding = new Binding("Name");
            //for (int i = 0; i < placelist.Count; i++)
            //{
            //    var p = new Place
            //    {
            //        Name = placelist[i]
            //    };
            //    _place.Add(p);
            //}
            //await _connection2.InsertAllAsync(_place);

            base.OnAppearing();
            await RefreshItems(true, syncItems: true);
        }

		async Task AddItem(Learner item)
		{
			await manager.SaveTaskAsync(item);
			listview1.ItemsSource = await manager.GetPersonsAsync();
		}
		public async void OnSyncItems(object sender, EventArgs e)
		{
			await RefreshItems(true, true);
		}


		//async void OnAdd(object sender, System.EventArgs e)
		//{
		//          var people = new Person
		//          {
		//          Skill = picker.SelectedItem.ToString(),
		//          Fname = fname.Text,
		//          Lname = lname.Text,
		//          Email = email.Text,
		//          Postcode = post.Text,
		//          Place=place.SelectedItem.ToString() };

		//          await _connection.InsertAsync(people);
		//          _people.Add(people);

		//}

		void OnUpdate(object sender, System.EventArgs e)
        {
        }

        async void OnDelete(object sender, System.EventArgs e)
        {
            if (_people.Count > 0)
            {
                var people = _people[0];
                await _connection.DeleteAsync(people);
                _people.Remove(people);
            }

        }

		private async Task RefreshItems(bool showActivityIndicator, bool syncItems)
		{
			using (var scope = new ActivityIndicatorScope(syncIndicator, showActivityIndicator))
			{
               
				listview1.ItemsSource = await manager.GetPersonsAsync(syncItems);
			}
		}


		public async void OnRefresh(object sender, EventArgs e)
		{
			var list = (ListView)sender;
			Exception error = null;
			try
			{
				await RefreshItems(false, true);
			}
			catch (Exception ex)
			{
				error = ex;
			}
			finally
			{
				list.EndRefresh();
			}

			if (error != null)
			{
				await DisplayAlert("Refresh Error", "Couldn't refresh data (" + error.Message + ")", "OK");
			}
		}



		

        void click_display(object sender, System.EventArgs e)
        {
            listview1.IsVisible = true;
        }


        async void Handle_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }

        async void Skip_clicked(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new SkillPage());
        }


        async void Handle_Clicked2(object sender, System.EventArgs e)
        {
            try
            {
                if (!fname.Text.isChar())
                {
                    label1.IsVisible = true;
                    label1.Text = "first name must be letters ";
                }
                else
                {
                    label1.IsVisible = false;
                    if (!lname.Text.isChar())
                    {
                        label2.IsVisible = true;
                        label2.Text = "last name must be letters ";
                    }
                    else
                    {
                        label2.IsVisible = false;
                        if (switch1.IsToggled && !picker2.SelectedItem.ToString().Trim().Equals(""))
                        {
                            if (email.Text.isEmail())
                            {
                                label3.IsVisible = false;
                                if (post.Text.isPost() && ValidPost(post.Text))
                                {

                                    label4.IsVisible = false;

                                    bool findEmail = false;


                                    for (int i = 0; i < _people.Count; i++)
                                    {
                                        if (email.Text.Equals(_people[i].Email))
                                            findEmail = true;

                                    };
                                    if (findEmail == false)
                                    {


                                        var people = new Learner
                                        {
                                            //Skill = picker.SelectedItem.ToString(),
                                            Name = "ddddd",
                                            Skill = "ddddd",
                                            Status = "sfdfd",
                                            Email = "ssss",
                                            Postcode = "3000"
                                        };
                                        await AddItem(people);

                                        //await _connection.InsertAsync(people);

                                       // _people.Add(people);




                                        await Navigation.PushAsync(new SkillPage());

                                    }
                                    else
                                    {
                                      
                                        label3.IsVisible = true;
                                        label3.Text = "This email address already has been used ";
                                    }







                                    //if (picker.SelectedItem.ToString().Equals("ICT"))
                                    //    await Navigation.PushAsync(new ResourcePage());
                                    //else if (picker.SelectedItem.ToString().Equals("English"))
                                    //    await Navigation.PushAsync(new EnglishPage());
                                    //else if (picker.SelectedItem.ToString().Equals("Health"))
                                    //    await Navigation.PushAsync(new HealthPage());
                                    //else if (picker.SelectedItem.ToString().Equals("Food"))
                                    //await Navigation.PushAsync(new FoodPage());
                                }
                                else
                                {
                                    label4.IsVisible = true;
                                    label4.Text = "you need to enter a valid VIC postcode(eg.3000)";
                                }

                            }
                            else
                            {
                                label3.IsVisible = true;
                                label3.Text = "you need to enter a valid email address(e.g.xx@gmail.com)";
                            }


                        }
                        else
                            await DisplayAlert("Alert", "you need to agree our requirements to continue", "ok");
                    }
                }

            }
            catch (NullReferenceException ex)
            {
                await DisplayAlert("Alert", "you need to choose facilites and finish the form", "ok");
            }
        }



		//private class ActivityIndicatorScope : IDisposable
		//{
		//	private bool showIndicator;
		//	private ActivityIndicator indicator;
		//	private Task indicatorDelay;

		//	public ActivityIndicatorScope(ActivityIndicator indicator, bool showIndicator)
		//	{
		//		this.indicator = indicator;
		//		this.showIndicator = showIndicator;

		//		if (showIndicator)
		//		{
		//			indicatorDelay = Task.Delay(2000);
		//			SetIndicatorActivity(true);
		//		}
		//		else
		//		{
		//			indicatorDelay = Task.FromResult(0);
		//		}
		//	}

		//	private void SetIndicatorActivity(bool isActive)
		//	{
		//		this.indicator.IsVisible = isActive;
		//		this.indicator.IsRunning = isActive;
		//	}

		//	public void Dispose()
		//	{
		//		if (showIndicator)
		//		{
		//			indicatorDelay.ContinueWith(t => SetIndicatorActivity(false), TaskScheduler.FromCurrentSynchronizationContext());
		//		}
		//	}
		//}




        void SetPicker()
        {
            placelist = new List<string> {
                "none",
                "AMES Australia",
                "Asylum Seeker Resource Centre",
                "Australian Karen Foundation",
                "Australian Red Cross",
                "Ballarat Community Health",
                "Ballarat Regional Multicultural Council",
                "Baptcare (Sanctuary)",
                "Bendigo Community Health Services",
                "Brigidine Asylum Seekers Project",
                "Brotherhood of St Laurence",
                "Brotherhood of St Laurence, Multicultural Communities Team",
                "Brunswick Connect",
                "Catholic Care",
                "Centre for Multicultural Youth",
                "cohealth",
                "ComputerBank",
                "Diversitat",
                "Familycare",
                "Fitted for Work ",
                "Fitzroy Learning Network",
                "Foundation House",
                "Jesuit Refugee Service",
                "Jesuit Social Services",
                "Life Without Barriers",
                "New Hope Foundation",
                "Noble Park English Language School ",
                "Refugee Legal",
                "RISE",
                "Salvation Army Asylum Seeker Support",
                "Save the Children",
                "Southern Migrant and Refugee Centre",
                "Spectrum Migrant Resource Centre",
                "Springvale Community Aid and Advice Bureau",
                "St Joseph's Flexible Learning Centre",
                "St Vincent de Paul Society Victoria ",
                "The Humanitarian Group",
                "Victorian Arabic Social Services",
                "Victorian Refugee Health Network",
                "Western Region Ethnic Communities Council",
                "Whittlesea Community Connections",
                "Women's Health in the South East",
                "Wyndham Community and Education Centre Inc"

            };
            picker2.ItemsSource = placelist;

        }

        bool ValidPost(string st)
        {
            for (int i = 0; i < postcode.Count;i++)
            {
                if (st.Equals(postcode[i]))
                    return true;
            }
            return false;
        }


        void setPostcode()
        {
            postcode = new List<string>
            {
				"3000","3001","3002","3003","3004","3005",
"3006",
"3008",
"3010",
"3011",
"3012",
"3013",
"3015",
"3016",
"3018",
"3019",
"3020",
"3021",
"3022",
"3023",
"3024",
"3025",
"3026",
"3027",
"3028",
"3029",
"3030",
"3031",
"3032",
"3033",
"3034",
"3036",
"3037",
"3038",
"3039",
"3040",
"3041",
"3042",
"3043",
"3044",
"3045",
"3046",
"3047",
"3048",
"3049",
"3050",
"3051",
"3052",
"3053",
"3054",
"3055",
"3056",
"3057",
"3058",
"3059",
"3060",
"3061",
"3062",
"3063",
"3064",
"3065",
"3066",
"3067",
"3068",
"3070",
"3071",
"3072",
"3073",
"3074",
"3075",
"3076",
"3078",
"3079",
"3081",
"3082",
"3083",
"3084",
"3085",
"3086",
"3087",
"3088",
"3089",
"3090",
"3091",
"3093",
"3094",
"3095",
"3096",
"3097",
"3099",
"3101",
"3102",
"3103",
"3104",
"3105",
"3106",
"3107",
"3108",
"3109",
"3111",
"3113",
"3114",
"3115",
"3116",
"3121",
"3122",
"3123",
"3124",
"3125",
"3126",
"3127",
"3128",
"3129",
"3130",
"3131",
"3132",
"3133",
"3134",
"3135",
"3136",
"3137",
"3138",
"3139",
"3140",
"3141",
"3142",
"3143",
"3144",
"3145",
"3146",
"3147",
"3148",
"3149",
"3150",
"3151",
"3152",
"3153",
"3154",
"3155",
"3156",
"3158",
"3159",
"3160",
"3161",
"3162",
"3163",
"3164",
"3165",
"3166",
"3167",
"3168",
"3169",
"3170",
"3171",
"3172",
"3173",
"3174",
"3175",
"3177",
"3178",
"3179",
"3180",
"3181",
"3182",
"3183",
"3184",
"3185",
"3186",
"3187",
"3188",
"3189",
"3190",
"3191",
"3192",
"3193",
"3194",
"3195",
"3196",
"3197",
"3198",
"3199",
"3200",
"3201",
"3202",
"3204",
"3205",
"3206",
"3207",
"3211",
"3212",
"3214",
"3215",
"3216",
"3217",
"3218",
"3219",
"3220",
"3221",
"3222",
"3223",
"3224",
"3225",
"3226",
"3227",
"3228",
"3230",
"3231",
"3232",
"3233",
"3235",
"3236",
"3237",
"3238",
"3239",
"3240",
"3241",
"3242",
"3243",
"3249",
"3250",
"3251",
"3254",
"3260",
"3264",
"3265",
"3266",
"3267",
"3268",
"3269",
"3270",
"3271",
"3272",
"3273",
"3274",
"3275",
"3276",
"3277",
"3278",
"3279",
"3280",
"3281",
"3282",
"3283",
"3284",
"3285",
"3286",
"3287",
"3289",
"3292",
"3293",
"3294",
"3300",
"3301",
"3302",
"3303",
"3304",
"3305",
"3309",
"3310",
"3311",
"3312",
"3314",
"3315",
"3317",
"3318",
"3319",
"3321",
"3322",
"3323",
"3324",
"3325",
"3328",
"3329",
"3330",
"3331",
"3332",
"3333",
"3334",
"3335",
"3337",
"3338",
"3340",
"3341",
"3342",
"3345",
"3350",
"3351",
"3352",
"3353",
"3354",
"3355",
"3356",
"3357",
"3360",
"3361",
"3363",
"3364",
"3370",
"3371",
"3373",
"3375",
"3377",
"3378",
"3379",
"3380",
"3381",
"3384",
"3385",
"3387",
"3388",
"3390",
"3391",
"3392",
"3393",
"3395",
"3396",
"3400",
"3401",
"3402",
"3407",
"3409",
"3412",
"3413",
"3414",
"3415",
"3418",
"3419",
"3420",
"3423",
"3424",
"3427",
"3428",
"3429",
"3430",
"3431",
"3432",
"3434",
"3435",
"3437",
"3438",
"3440",
"3441",
"3442",
"3444",
"3446",
"3447",
"3448",
"3450",
"3451",
"3453",
"3458",
"3460",
"3461",
"3462",
"3463",
"3464",
"3465",
"3467",
"3468",
"3469",
"3472",
"3475",
"3478",
"3480",
"3482",
"3483",
"3485",
"3487",
"3488",
"3489",
"3490",
"3491",
"3494",
"3496",
"3498",
"3500",
"3501",
"3502",
"3505",
"3506",
"3507",
"3509",
"3512",
"3515",
"3516",
"3517",
"3518",
"3520",
"3521",
"3522",
"3523",
"3525",
"3527",
"3529",
"3530",
"3531",
"3533",
"3537",
"3540",
"3542",
"3544",
"3546",
"3549",
"3550",
"3551",
"3552",
"3555",
"3556",
"3557",
"3558",
"3559",
"3561",
"3562",
"3563",
"3564",
"3565",
"3566",
"3567",
"3568",
"3570",
"3571",
"3572",
"3573",
"3575",
"3576",
"3579",
"3580",
"3581",
"3583",
"3584",
"3585",
"3586",
"3588",
"3589",
"3590",
"3591",
"3594",
"3595",
"3596",
"3597",
"3599",
"3607",
"3608",
"3610",
"3612",
"3614",
"3616",
"3617",
"3618",
"3619",
"3620",
"3621",
"3622",
"3623",
"3624",
"3629",
"3630",
"3631",
"3632",
"3633",
"3634",
"3635",
"3636",
"3637",
"3638",
"3639",
"3640",
"3641",
"3643",
"3644",
"3646",
"3647",
"3649",
"3658",
"3659",
"3660",
"3661",
"3663",
"3664",
"3665",
"3666",
"3669",
"3670",
"3671",
"3672",
"3673",
"3675",
"3676",
"3677",
"3678",
"3682",
"3683",
"3685",
"3687",
"3688",
"3689",
"3690",
"3691",
"3694",
"3695",
"3697",
"3699",
"3700",
"3701",
"3704",
"3705",
"3707",
"3708",
"3709",
"3711",
"3712",
"3713",
"3714",
"3715",
"3717",
"3718",
"3719",
"3720",
"3722",
"3723",
"3724",
"3725",
"3726",
"3727",
"3728",
"3730",
"3732",
"3733",
"3735",
"3736",
"3737",
"3738",
"3739",
"3740",
"3741",
"3744",
"3746",
"3747",
"3749",
"3750",
"3751",
"3752",
"3753",
"3754",
"3755",
"3756",
"3757",
"3758",
"3759",
"3760",
"3763",
"3764",
"3765",
"3766",
"3767",
"3770",
"3775",
"3777",
"3778",
"3779",
"3781",
"3782",
"3783",
"3785",
"3786",
"3787",
"3788",
"3789",
"3791",
"3792",
"3793",
"3795",
"3796",
"3797",
"3799",
"3800",
"3802",
"3803",
"3804",
"3805",
"3806",
"3807",
"3808",
"3809",
"3810",
"3812",
"3813",
"3814",
"3815",
"3816",
"3818",
"3820",
"3821",
"3822",
"3823",
"3824",
"3825",
"3831",
"3832",
"3833",
"3835",
"3840",
"3842",
"3844",
"3847",
"3850",
"3851",
"3852",
"3853",
"3854",
"3856",
"3857",
"3858",
"3859",
"3860",
"3862",
"3864",
"3865",
"3869",
"3870",
"3871",
"3873",
"3874",
"3875",
"3878",
"3880",
"3882",
"3885",
"3886",
"3887",
"3888",
"3889",
"3890",
"3891",
"3892",
"3893",
"3895",
"3896",
"3898",
"3900",
"3902",
"3903",
"3904",
"3909",
"3910",
"3911",
"3912",
"3913",
"3915",
"3916",
"3918",
"3919",
"3921",
"3922",
"3923",
"3925",
"3926",
"3927",
"3928",
"3929",
"3930",
"3931",
"3933",
"3934",
"3936",
"3937",
"3938",
"3939",
"3941",
"3942",
"3943",
"3944",
"3945",
"3946",
"3950",
"3951",
"3953",
"3954",
"3956",
"3957",
"3958",
"3959",
"3960",
"3962",
"3964",
"3965",
"3966",
"3967",
"3971",
"3975",
"3976",
"3977",
"3978",
"3979",
"3980",
"3981",
"3984",
"3987",
"3988",
"3990",
"3991",
"3992",
"3995",
"3996",
"8001",
"8002",
"8004",
"8005",
"8006",
"8009",
"8010",
"8011",
"8045",
"8051",
"8060",
"8061",
"8066",
"8069",
"8070",
"8071",
"8102",
"8103",
"8107",
"8108",
"8111",
"8120",
"8205",
"8383",
"8386",
"8388",
"8390",
"8393",
"8394",
"8396",
"8399",
"8500",
"8507",
"8538",
"8557",
"8576",
"8622",
"8626",
"8627",
                "8785",
"8865",
"8873",
};


        }

    }
}