using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.Messaging;
using Xamarin.Forms;

namespace aqq
{
    public partial class FriendListPage : ContentPage
    {
        FriendManager fri;
        EnrolManager en;
        public FriendListPage()
        {
            InitializeComponent();
            fri = FriendManager.DefaultManager;
            en = EnrolManager.DefaultManager;
        }
		
		

		protected override async void OnAppearing()
		{
			base.OnAppearing();
			var status = Application.Current.Properties["Status"] as string;
            var email = Application.Current.Properties["Email"] as string;
            if (status.Equals("Learner"))
            {
                tip.Text = "This page only list peer who follow you and who you follow. Choose a contact and swift left to sms or email your peer.";
                listview.IsVisible = true;
                peers.IsVisible = true;
                mentors.IsVisible = true;
                listview2.IsVisible = false;
				listview3.IsVisible = false;
                listbyoth.IsVisible = true;
                listview.ItemsSource = await fri.GetyoufollowAsync(email);
                listbyoth.ItemsSource = await fri.GetfollowyouPeersAsync(email);
                await Notifaction(email);
            }
            else
            {
				tip.Text = "Choose a contact and swift left to sms or email users who enrolled your course.";
                listview2.IsVisible = true;
                peers.IsVisible = false;
                mentors.IsVisible = false;
				listview.IsVisible = false;
                listview3.IsVisible = false;
                listbyoth.IsVisible = false;
                hint2.IsVisible = false;
                hint1.IsVisible = false;
                listview2.ItemsSource = await en.GetTeacherEmailClassAsync(email);
              
            }
		}

        async Task Notifaction(string email){
			List<Friend> friend = await fri.GetUnacceptPeersAsync(email);
			if (friend.Count > 0)
			{
				for (int i = 0; i < friend.Count; i++)
				{
					string st = "User " + friend[i].Name1 + " followed you, Do you accept this person? ";
					var result = await DisplayAlert("New Notifaction", st, "OK", "Cancel");
					if (result == true)
					{
						var updatefriend = new Friend
						{

							Id = friend[i].Id,
							Email1 = friend[i].Email1,
							Phone1 = friend[i].Phone1,
							Name1 = friend[i].Name1,
							Email2 = friend[i].Email2,
							Phone2 = friend[i].Phone2,
							Name2 = friend[i].Name2,
							Agree = true

						};
						await fri.UpdateTaskAsync(updatefriend);
                        await DisplayAlert("New Notifaction", "This person could send your message and email from now on.", "OK");
                        listbyoth.ItemsSource = await fri.GetfollowyouPeersAsync(email);
					}

				}
			}

        }

		async void Peer_List(object sender, System.EventArgs e)
		{
			var email = Application.Current.Properties["Email"] as string;
			listview.IsVisible = true;
			listview2.IsVisible = false;
			listview3.IsVisible = false;
            listbyoth.IsVisible = true;
			hint1.IsVisible = true;
			hint2.IsVisible = true;
			listview.ItemsSource = await fri.GetyoufollowAsync(email);
			listbyoth.ItemsSource = await fri.GetfollowyouPeersAsync(email);
			


		}

		async void Mentor_List(object sender, System.EventArgs e)
		{
			var email = Application.Current.Properties["Email"] as string;
            listview.IsVisible = false;
			listview2.IsVisible = false;
			listview3.IsVisible = true;
            listbyoth.IsVisible = false;
            hint1.IsVisible = false;
			hint2.IsVisible = false;
			listview3.ItemsSource = await en.GetMentorEmailClassAsync(email);

		}

		async void Unfollow_Clicked(object sender, System.EventArgs e)
		{
			var email = Application.Current.Properties["Email"] as string;
			
		

				var menuItem = sender as MenuItem;
				var friend = menuItem.CommandParameter as Friend;
				await fri.DeleteTaskAsync(friend);
				await DisplayAlert("Notice", "Unfollow successful!", "ok");
			   listview.ItemsSource = await fri.GetyoufollowAsync(email);
				listview.SelectedItem = null;

		}

		async void Follow_Clicked(object sender, System.EventArgs e)
		{
			var email = Application.Current.Properties["Email"] as string;
			var menuItem = sender as MenuItem;
			var friend = menuItem.CommandParameter as Friend;

			var newfriend = new Friend
            {


                Email1 = friend.Email2,
				Phone1 = friend.Phone2,
				Name1 = friend.Name2,
				Email2 = friend.Email1,
				Phone2 = friend.Phone1,
				Name2 = friend.Name1,
				Agree = false

			};

			await fri.SaveTaskAsync(friend);
			await DisplayAlert("Notice", "Waiting Accept......", "ok");
		    listview.ItemsSource = await fri.GetyoufollowAsync(email);
		}







		async void Remove_Clicked(object sender, System.EventArgs e)
		{
			var email = Application.Current.Properties["Email"] as string;

			var menuItem = sender as MenuItem;
			var friend = menuItem.CommandParameter as Friend;
			await fri.DeleteTaskAsync(friend);
			await DisplayAlert("Notice", "Remove successful!", "ok");
			listbyoth.ItemsSource = await fri.GetfollowyouPeersAsync(email);
			listbyoth.SelectedItem = null;




		}






		 async void Email_Clicked(object sender, System.EventArgs e)
		{
			var senderEmail = Application.Current.Properties["Email"] as string;
			
			var menuItem = sender as MenuItem;
			var course = menuItem.CommandParameter as Friend;

			

			string receiver = course.Email2;
			var emailMessenger = CrossMessaging.Current.EmailMessenger;
            if (emailMessenger.CanSendEmail)
            {
                emailMessenger.SendEmail(receiver);

            }
            else
                await DisplayAlert("Alert", "Android phone cannot using email function.", "Ok");
                


		}
		 void Phone_Clicked(object sender, System.EventArgs e)
		{
			var smsMessenger = CrossMessaging.Current.SmsMessenger;

			var senderPhone = Application.Current.Properties["Phone"] as string;
			var senderEmail = Application.Current.Properties["Email"] as string;

		
			var menuItem = sender as MenuItem;
			var course = menuItem.CommandParameter as Friend;

			

			string receiverPhone = course.Phone2;

			if (smsMessenger.CanSendSms)

				smsMessenger.SendSms(receiverPhone);
		}

		async void Email_Clicked2(object sender, System.EventArgs e)
		{
			var senderEmail = Application.Current.Properties["Email"] as string;
		

			var menuItem = sender as MenuItem;
			var course = menuItem.CommandParameter as Enrol;
            string receiver = course.Email1;
			var emailMessenger = CrossMessaging.Current.EmailMessenger;
			if (emailMessenger.CanSendEmail)
			{
				emailMessenger.SendEmail(receiver);

			}
			else
				await DisplayAlert("Alert", "Android phone cannot using email function.", "Ok");

		}

		 void Phone_Clicked2(object sender, System.EventArgs e)
		{
			var smsMessenger = CrossMessaging.Current.SmsMessenger;

			var senderPhone = Application.Current.Properties["Phone"] as string;
			var senderEmail = Application.Current.Properties["Email"] as string;

		

			var menuItem = sender as MenuItem;
			var course = menuItem.CommandParameter as Enrol;


			string receiverPhone = course.Phone;

			if (smsMessenger.CanSendSms)

				smsMessenger.SendSms(receiverPhone);
		}

		async void Email_Clicked3(object sender, System.EventArgs e)
		{
			var senderEmail = Application.Current.Properties["Email"] as string;


			var menuItem = sender as MenuItem;
			var course = menuItem.CommandParameter as Enrol;
			string receiver = course.Email2;
			var emailMessenger = CrossMessaging.Current.EmailMessenger;
			if (emailMessenger.CanSendEmail)
			{
				emailMessenger.SendEmail(receiver);

			}
			else
				await DisplayAlert("Alert", "Android phone cannot using email function.", "Ok");

		}

		void Phone_Clicked3(object sender, System.EventArgs e)
		{
			var smsMessenger = CrossMessaging.Current.SmsMessenger;

			var senderPhone = Application.Current.Properties["Phone"] as string;
			var senderEmail = Application.Current.Properties["Email"] as string;



			var menuItem = sender as MenuItem;
			var course = menuItem.CommandParameter as Enrol;


			string receiverPhone = course.Phone2;

			if (smsMessenger.CanSendSms)

				smsMessenger.SendSms(receiverPhone);
		}

	}
    }
