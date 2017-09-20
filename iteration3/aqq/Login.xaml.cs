using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using Xamarin.Forms;

namespace aqq
{
    public partial class Login : TabbedPage
    {
        PersonManager per;
       
        VolunteerManager vol;
        List<Volunteer> l1;

        public Login()
        {

            InitializeComponent();
			background.Source = ImageSource.FromResource("aqq.Image.bg1.jpg");
			backgroundAboutus.Source = ImageSource.FromResource("aqq.Image.bg1.jpg");
			backgroundHelp.Source = ImageSource.FromResource("aqq.Image.bg1.jpg");
            NavigationPage.SetHasBackButton(this, false);
            per = PersonManager.DefaultManager;
           // men = MentorManager.DefaultManager;
            vol = VolunteerManager.DefaultManager;

        }

		//async void Job_Clicked(object sender, System.EventArgs e)

		//{
			
		//	await Navigation.PushAsync(new JobPage());
		//	//Navigation.RemovePage(this);
		//}

        async void Skip(object sender, System.EventArgs e)
        {
            Application.Current.Properties["skip"] = "true";
            await Navigation.PushAsync(new SkipPage());

        }
        async void OnLabelTapped(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new Terms());
        }

		async void OnLabel2Tapped(object sender, System.EventArgs e)
		{
			await Navigation.PushAsync(new Disclaimer());
		}


        async Task transform(){
            if(Application.Current.Properties.ContainsKey("Click"))
            {
                string click = Application.Current.Properties["Click"] as string;
                if (click.Equals("peer"))
                {
                    var currentStatus = Application.Current.Properties["Status"] as string;
                    if (currentStatus.Equals("Mentor") || currentStatus.Equals("Volunteer"))
                    {
                        await CheckVol();
                        if (Application.Current.Properties.ContainsKey("Name"))
							await Navigation.PushAsync(new TeachPage());
                        else{
							await DisplayAlert("Notice", " You need go to the me page to update your profile.", "Ok");
                            await Navigation.PushAsync(new SkipPage());
                        }
							
                    }
                    else{
						await CheckLea();
						if (Application.Current.Properties.ContainsKey("Name"))
							await Navigation.PushAsync(new FormPage());
						else
						{
							await DisplayAlert("Notice", " You need go to the me page to update your profile.", "Ok");
							await Navigation.PushAsync(new SkipPage());
						}
                    }
                }
                else
                    await Navigation.PushAsync(new FriendListPage());
            }
            else
                await Navigation.PushAsync(new SkipPage());
        }


        async void Submit(object sender, System.EventArgs e)
        {
           
            try
            {
                indicator.IsVisible = true;
                if (email.Text.Trim().isEmail())
                {
                    label1.IsVisible = false;
                    if (phone.Text.IsPhone())
                    {
                        label2.IsVisible = false;

                        if (picker.SelectedItem.ToString().Equals("Mentor"))
                        {
                            List<Volunteer> v1 = await vol.GetIDAsync(email.Text.Trim());
                            if (v1.Count > 0)
                            {
                                if (!v1[0].Phone.Equals(phone.Text.Trim()))
                                {
                                    await DisplayAlert("Sorry", " This email is in use please enter another one.", "Ok");
                                }
                                else
                                {
                                    List<Volunteer> v2 = await vol.GetbyPhoneAsync(phone.Text.Trim(), email.Text.Trim());
                                    if (v2.Count > 0)
                                    {
                                        await DisplayAlert("Sorry", " This phone number is in use please enter another one.", "Ok");
                                    }
                                    else
                                    {
                                        Application.Current.Properties["skip"] = "false";
                                        Application.Current.Properties["Status"] = picker.SelectedItem.ToString();
                                        Application.Current.Properties["Email"] = email.Text.Trim();
                                        Application.Current.Properties["Phone"] = phone.Text.Trim();

                                        await transform();
                                        Navigation.RemovePage(this);
                                    }
                                }

                            }
                            else
                            {
                                List<Volunteer> v2 = await vol.GetbyPhoneAsync(phone.Text.Trim(), email.Text.Trim());
                                if (v2.Count > 0)
                                {
                                    await DisplayAlert("Sorry", " This phone number is in use please enter another one.", "Ok");
                                }
                                else
                                {
                                    var people = new Volunteer
                                    {
                                        Status = "Mentor",
                                        Email = email.Text.Trim(),
                                        Phone = phone.Text.Trim(),
                                        Name = "",
                                        Postcode = "",
                                        Skill = ""
                                    };


                                    await vol.SaveTaskAsync(people);
                                    Application.Current.Properties["skip"] = "false";
                                    Application.Current.Properties["Status"] = picker.SelectedItem.ToString();
                                    Application.Current.Properties["Email"] = email.Text.Trim();
                                    Application.Current.Properties["Phone"] = phone.Text.Trim();
                                    await transform();
                                    Navigation.RemovePage(this);
                                }
                            }
                        }
                        if (picker.SelectedItem.ToString().Equals("Volunteer"))
                        {
                            List<Volunteer> v1 = await vol.GetIDAsync(email.Text.Trim());
                            if (v1.Count > 0)
                            {
                                if (!v1[0].Phone.Equals(phone.Text.Trim()))
                                {
                                    await DisplayAlert("Sorry", " This email is in use please enter another one.", "Ok");
                                }
                                else
                                {
                                    List<Volunteer> v2 = await vol.GetbyPhoneAsync(phone.Text.Trim(), email.Text.Trim());
                                    if (v2.Count > 0)
                                    {
                                        await DisplayAlert("Sorry", " This phone number is in use please enter another one.", "Ok");
                                    }
                                    else
                                    {
                                        Application.Current.Properties["skip"] = "false";
                                        Application.Current.Properties["Status"] = picker.SelectedItem.ToString();
                                        Application.Current.Properties["Email"] = email.Text.Trim();
                                        Application.Current.Properties["Phone"] = phone.Text.Trim();
                                        await transform();
                                        Navigation.RemovePage(this);
                                    }
                                }
                            }
                            else
                            {
                                List<Volunteer> v2 = await vol.GetbyPhoneAsync(phone.Text.Trim(), email.Text.Trim());
                                if (v2.Count > 0)
                                {
                                    await DisplayAlert("Sorry", " This phone number is in use please enter another one.", "Ok");
                                }
                                else
                                {
                                    var people = new Volunteer
                                    {
                                        Status = "Volunteer",
                                        Email = email.Text.Trim(),
                                        Phone = phone.Text.Trim(),
                                        Name = "",
                                        Postcode = "",
                                        Skill = ""
                                    };

                                    await vol.SaveTaskAsync(people);
                                    Application.Current.Properties["skip"] = "false";
                                    Application.Current.Properties["Status"] = picker.SelectedItem.ToString();
                                    Application.Current.Properties["Email"] = email.Text.Trim();
                                    Application.Current.Properties["Phone"] = phone.Text.Trim();
                                    await transform();
                                    Navigation.RemovePage(this);
                                }
                            }
                        }
                        if (picker.SelectedItem.ToString().Equals("Learner"))
                        {
                            List<Learner> l1 = await per.GetIDAsync(email.Text.Trim());
                            if (l1.Count > 0)
                            {
                                if (!l1[0].Phone.Equals(phone.Text.Trim()))
                                {
                                    await DisplayAlert("Sorry", " This email is in use please enter another one.", "Ok");
                                }
                                else
                                {
                                    List<Learner> l2 = await per.GetbyPhoneAsync(phone.Text.Trim(), email.Text.Trim());
                                    if (l2.Count > 0)
                                    {
                                        await DisplayAlert("Sorry", " This phone number is in use please enter another one.", "Ok");
                                    }
                                    else
                                    {
                                        Application.Current.Properties["skip"] = "false";
                                        Application.Current.Properties["Status"] = picker.SelectedItem.ToString();
                                        Application.Current.Properties["Email"] = email.Text.Trim();
                                        Application.Current.Properties["Phone"] = phone.Text.Trim();
                                        await transform();
                                        Navigation.RemovePage(this);
                                    }
                                }
                            }
                            else
                            {

                                List<Learner> l2 = await per.GetbyPhoneAsync(phone.Text.Trim(), email.Text.Trim());
                                if (l2.Count > 0)
                                {
                                    await DisplayAlert("Sorry", " This phone number is in use please enter another one.", "Ok");
                                }
                                else
                                {
                                    var people = new Learner
                                    {
                                        Status = "Learner",
                                        Email = email.Text.Trim(),
                                        Phone = phone.Text.Trim(),
                                        Name = "",
                                        Postcode = "",
                                        Skill = ""
                                    };

                                    await per.SaveTaskAsync(people);
                                    Application.Current.Properties["skip"] = "false";
                                    Application.Current.Properties["Status"] = picker.SelectedItem.ToString();
                                    Application.Current.Properties["Email"] = email.Text.Trim();
                                    Application.Current.Properties["Phone"] = phone.Text.Trim();
                                    await transform();
                                    Navigation.RemovePage(this);
                                }
                            }

                        }
                    }
                    else
                        label2.IsVisible = true;
                }

                else
                {
                    label1.IsVisible = true;
                }
                indicator.IsVisible = false;

            }
            catch(NullReferenceException ex){
                await DisplayAlert("Alert", "you need to choose status and finish the form", "ok");  
            }




			
        }
		async Task CheckVol()
		{
			var email = Application.Current.Properties["Email"] as string;
			l1 = await vol.GetIDAsync(email);
			if (l1.Count > 0)
			{
				if (l1[0].Name.Trim().Length > 0 && l1[0].Skill.Trim().Length > 0)
				{
					Application.Current.Properties["Name"] = l1[0].Name;
					Application.Current.Properties["Skill"] = l1[0].Skill;
					Application.Current.Properties["Postcode"] = l1[0].Postcode;

				}

			}
		}

		async Task CheckLea()
		{
			var email = Application.Current.Properties["Email"] as string;
			List<Learner> p1 = await per.GetIDAsync(email);
			if (p1.Count > 0 && p1[0].Skill.Trim().Length > 0 && p1[0].Name.Trim().Length > 0)
			{
				Application.Current.Properties["Name"] = p1[0].Name;
				Application.Current.Properties["Skill"] = p1[0].Skill;
				Application.Current.Properties["Postcode"] = p1[0].Postcode;

			}


		}
    }
}