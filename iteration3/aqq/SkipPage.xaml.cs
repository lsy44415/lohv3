﻿﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace aqq
{
    public partial class SkipPage : TabbedPage
    {
        PersonManager manager2;
		VolunteerManager vol;
		
		List<Volunteer> l1;
        public SkipPage()
        {
            InitializeComponent();
			manager2 = PersonManager.DefaultManager;
			l1 = new List<Volunteer>();
			vol = VolunteerManager.DefaultManager;
           
		
			backgroundAboutus.Source = ImageSource.FromResource("aqq.Image.bg1.jpg");
			backgroundHelp.Source = ImageSource.FromResource("aqq.Image.bg1.jpg");
            bgME.Source = ImageSource.FromResource("aqq.Image.bg1.jpg");
            NavigationPage.SetHasBackButton(this, false);

        }


		protected override async void OnAppearing()
		{
			
            await setMe();

		}


        async Task setMe(){
			if (Application.Current.Properties.ContainsKey("Status"))
			{
                loginbtn.IsVisible = false;
                logoutbtn.IsVisible = true;
				if (Application.Current.Properties.ContainsKey("Email"))
				{
					myEmail.Text = Application.Current.Properties["Email"] as string;
					myPhone.Text = Application.Current.Properties["Phone"] as string;
					myStatus.Text = Application.Current.Properties["Status"] as string;
					if (myStatus.Text.Equals("Learner"))
					{
						await CheckLea();
                        editlea.IsVisible=true;
                        editvol.IsVisible = false;
						if (Application.Current.Properties.ContainsKey("Name"))
						{
							myName.Text = Application.Current.Properties["Name"] as string;
							mySkill.Text = Application.Current.Properties["Skill"] as string;
							myPostcode.Text = Application.Current.Properties["Postcode"] as string;
						}



					}
					else
					{
						await CheckVol();
                        editvol.IsVisible = true;
                        editlea.IsVisible = false;
						if (Application.Current.Properties.ContainsKey("Name"))
						{
							myName.Text = Application.Current.Properties["Name"] as string;
							mySkill.Text = Application.Current.Properties["Skill"] as string;
							myPostcode.Text = Application.Current.Properties["Postcode"] as string;
						}
					}
				}
			}
            else
            {
				logoutbtn.IsVisible = false;
				loginbtn.IsVisible = true;
            }

        }





        async void Login_Clicked(object sender, System.EventArgs e)
        {
            Application.Current.Properties.Clear();
            await Navigation.PushAsync(new Login());
        }

		async void Logout_Clicked(object sender, System.EventArgs e)
		{
			var result = await DisplayAlert("Log out", "Log out of LOH? ", "OK", "Cancel");
			if (result == true)
			{
				Application.Current.Properties.Clear();
				await Navigation.PushAsync(new Login());
			}
           
		}




        async void OnImage1Tapped(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new SkillPage());
        }

        async void OnImage2Tapped(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new JobPage());

        }
        async void OnImage3Tapped(object sender, EventArgs args)
        {

            await Navigation.PushAsync(new MapMain());
        }
        async void OnImage4Tapped(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new VolunteeringJob());
            //await Navigation.PushAsync(new MyPage());
        }


        async void OnImage5Tapped(object sender, EventArgs args)
        {

            if (Application.Current.Properties.ContainsKey("Status"))
            {
                var st = Application.Current.Properties["Status"] as string;
                if (st.Equals("Volunteer") || st.Equals("Mentor"))
                {

                    if (Application.Current.Properties.ContainsKey("Name"))
                    {

                        await Navigation.PushAsync(new TeachPage());

                    }
                    else
                    {
                        await DisplayAlert("Alert", "You need to complete your personal profile", "OK");


                     

                    }
                }

                if (st.Equals("Learner"))
                {
                    if (Application.Current.Properties.ContainsKey("Name"))
                    {

                        await Navigation.PushAsync(new FormPage());

                    }
                    else
                    {
                        await DisplayAlert("Alert", "You need to complete your personal profile", "OK");


                      
                    }
                }
            }
            else
            {   
                var result = await DisplayAlert("Login", "You need to login for peer-learning", "OK", "Cancel");
                if (result == true)
                {
                    Application.Current.Properties["Click"] = "peer";
                    await Navigation.PushAsync(new Login());
                }
            }

            //await Navigation.PushAsync(new MyPage());
        }

        async void OnImage6Tapped(object sender, EventArgs args)
        {


            if (Application.Current.Properties.ContainsKey("Status"))
            {
                if (Application.Current.Properties.ContainsKey("Name"))
                {

					await Navigation.PushAsync(new FriendListPage());

                }
                else
                {
                    await DisplayAlert("Alert", "You need to complete your personal profile", "OK");

                   
                }

               
            }
            else
            {
                var result = await DisplayAlert("Login", "You need to login for peer-learning", "OK", "Cancel");
                if (result == true)
                {
                    Application.Current.Properties["Click"] = "contact";
                    await Navigation.PushAsync(new Login());
                }
            }
            //await Navigation.PushAsync(new MyPage());
        }


		async void Handle_Clicked1(object sender, System.EventArgs e)
		{


			try
			{
				if (!name1.Text.Trim().Equals(""))
				{
					label11.IsVisible = false;
					if (post1.Text.ValidPost())
					{
						label13.IsVisible = false;
						var status = Application.Current.Properties["Status"] as string;
						var email = Application.Current.Properties["Email"] as string;
						var phone = Application.Current.Properties["Phone"] as string;
						List<Learner> p1 = await manager2.GetIDAsync(email);



						var people = new Learner
						{
							Id = p1[0].Id,
							Status = status,
							Email = email,
							Phone = phone,
							Skill = picker1.SelectedItem.ToString(),
							Name = name1.Text,
							Postcode = post1.Text
						};
						Application.Current.Properties["Name"] = name1.Text;
						Application.Current.Properties["Skill"] = picker1.SelectedItem.ToString();
						Application.Current.Properties["Postcode"] = post1.Text;
						await manager2.UpdateTaskAsync(people);
						await DisplayAlert("Notice", "Update successful! ", "ok");
                        await setMe();
                        picker1.SelectedItem = "";
                        name1.Text = "";
                        post1.Text = "";

					}
					else
						label13.IsVisible = true;
				}
				else
					label11.IsVisible = true;
			}
			catch (NullReferenceException ex)
			{
				await DisplayAlert("Alert", "you need to finish the form and choose one skill", "ok");
			}
		}


		async void OnLabelTapped(object sender, System.EventArgs e)
		{
			await Navigation.PushAsync(new Disclaimer());
		}




		async void Handle_Clicked(object sender, System.EventArgs e)
		{
			try
			{
				if (!name.Text.Trim().Equals(""))
				{
					label1.IsVisible = false;
					if (post.Text.ValidPost())
					{
						label3.IsVisible = false;
						var status = Application.Current.Properties["Status"] as string;
						var email = Application.Current.Properties["Email"] as string;
						var phone = Application.Current.Properties["Phone"] as string;

						l1 = await vol.GetIDAsync(email);


						var people = new Volunteer
						{
							Id = l1[0].Id,
							Status = status,
							Email = email,
							Phone = phone,
							Skill = picker.SelectedItem.ToString(),
							Profile = editor.Text,
							Name = name.Text,
							Postcode = post.Text
						};
						Application.Current.Properties["Name"] = name.Text;
						Application.Current.Properties["Skill"] = picker.SelectedItem.ToString();
						Application.Current.Properties["Postcode"] = post.Text;
						await vol.UpdateTaskAsync(people);
						await DisplayAlert("Notice", "submit successful!", "ok");
                        await setMe();
						picker1.SelectedItem = "";
						name1.Text = "";
						post1.Text = "";
                        editor.Text = "";
					}
					else
						label3.IsVisible = true;
				}
				else
					label1.IsVisible = true;

			}
			catch (NullReferenceException ex)
			{
				await DisplayAlert("Alert", "you need to finish the form and choose one skill", "ok");
			}
		}

        void Edit_Click_lea(object sender, System.EventArgs e)
        {
			form.IsVisible = true;
			volun.IsVisible = false;
        }

		void Edit_Click_vol(object sender, System.EventArgs e)
		{
            form.IsVisible = false;
			volun.IsVisible = true;
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
				List<Learner> p1 = await manager2.GetIDAsync(email);
            if (p1.Count > 0 && p1[0].Skill.Trim().Length > 0 && p1[0].Name.Trim().Length > 0)
            {
                Application.Current.Properties["Name"] = p1[0].Name;
                Application.Current.Properties["Skill"] = p1[0].Skill;
                Application.Current.Properties["Postcode"] = p1[0].Postcode;

            }
            else
                form.IsVisible = true;
			
		}




    }
           
  }
    

