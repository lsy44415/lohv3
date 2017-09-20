using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace aqq
{
    public partial class FormPage : ContentPage
    {

        PersonManager manager2;





        public FormPage()
        {
            InitializeComponent();
			background.Source = ImageSource.FromResource("aqq.Image.bg1.jpg");
			manager2 = PersonManager.DefaultManager;
        }

      //  async void Handle_Clicked1(object sender, System.EventArgs e)
      //  {


      //      try
      //      {
      //          if (!name.Text.Trim().Equals(""))
      //          {
      //              label1.IsVisible = false;
      //              if (post.Text.ValidPost())
      //              {
      //                  label3.IsVisible = false;
      //                  var status = Application.Current.Properties["Status"] as string;
      //                  var email = Application.Current.Properties["Email"] as string;
      //                  var phone = Application.Current.Properties["Phone"] as string;
      //                  List<Learner> p1 = await manager2.GetIDAsync(email);

      //                  var people = new Learner
      //                  {
      //                      Id = p1[0].Id,
      //                      Status = status,
      //                      Email = email,
      //                      Phone = phone,
      //                      Skill = picker.SelectedItem.ToString(),
      //                      Name = name.Text,
      //                      Postcode = post.Text
      //                  };
      //                  Application.Current.Properties["Name"] = name.Text;
      //                  Application.Current.Properties["Skill"] = picker.SelectedItem.ToString();
						//Application.Current.Properties["Postcode"] = post.Text;
      //                  await manager2.UpdateTaskAsync(people);
						//await DisplayAlert("Notice", "submit successful!", "ok");
        //            }
        //            else
        //                label3.IsVisible = true;
        //        }
        //        else
        //            label1.IsVisible = true;
        //    }
        //    catch (NullReferenceException ex)
        //    {
        //        await DisplayAlert("Alert", "you need to finish the form and choose one skill", "ok");
        //    }
        //}

        async void Handle_Clicked2(object sender, System.EventArgs e)
        {
            try
            {   
				
				var email = Application.Current.Properties["Email"] as string;
				List<Learner> p1 = await manager2.GetIDAsync(email);
                if (p1.Count > 0 &&p1[0].Skill.Trim().Length > 0 && p1[0].Name.Trim().Length > 0)
                {
                    Application.Current.Properties["Name"] = p1[0].Name;
                    Application.Current.Properties["Skill"] = p1[0].Skill;
					Application.Current.Properties["Postcode"] = p1[0].Postcode;
                    await Navigation.PushAsync(new NameListPage(p1[0].Skill));
                }
                else
					await DisplayAlert("Alert", "you need to finish the form and choose one skill", "ok");
            }
            catch (NullReferenceException ex)
            {
                await DisplayAlert("Alert", "you need to finish the form and choose one skill", "ok");
            }

        }

       async void Handle_Clicked3(object sender, System.EventArgs e)
        {
            try
            {
                var status = Application.Current.Properties["Status"] as string;
                var email = Application.Current.Properties["Email"] as string;
                List<Learner> p1 = await manager2.GetIDAsync(email);
                if (p1.Count > 0 && p1[0].Skill.Trim().Length>0 && p1[0].Name.Trim().Length>0)
                {
                    Application.Current.Properties["Name"] = p1[0].Name;
                    Application.Current.Properties["Skill"] = p1[0].Skill;
                    Application.Current.Properties["Postcode"] = p1[0].Postcode;

                    await Navigation.PushAsync(new learnerClassList());
                }
                else
                    await DisplayAlert("Alert", "you need to finish the form and choose one skill", "OK");
            }
			catch (NullReferenceException ex)
			{
				await DisplayAlert("Alert", "you need to finish the form and choose one skill", "ok");
			}

        }
    }
}
