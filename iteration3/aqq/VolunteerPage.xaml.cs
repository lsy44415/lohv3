using System;
using System.Collections.Generic;

using Xamarin.Forms;



namespace aqq
{
    public partial class VolunteerPage : ContentPage
    {
        VolunteerManager vol;

        List<Volunteer> l1;
        public VolunteerPage()
        {
            InitializeComponent();
            l1 = new List<Volunteer>();
            vol = VolunteerManager.DefaultManager;
           
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
                        await Navigation.PushAsync(new TeachPage());
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


       async void Skip_Clicked(object sender, System.EventArgs e)
        {
			
			var email = Application.Current.Properties["Email"] as string;
			l1 = await vol.GetIDAsync(email);
            if (l1.Count > 0)
            {
                if (l1[0].Name.Trim().Length>0 &&  l1[0].Skill.Trim().Length > 0 )
                {
                    Application.Current.Properties["Name"] = l1[0].Name;
                    Application.Current.Properties["Skill"] = l1[0].Skill;
                    Application.Current.Properties["Postcode"] = l1[0].Postcode;
                    await Navigation.PushAsync(new TeachPage());
                }
                else
                    await DisplayAlert("Alert", "Please fill the from to continue to the next step", "OK");
            }

			else
				await DisplayAlert("Alert", "Please fill the from to continue to the next step", "OK");
        }
    }
}

