
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

#if OFFLINE_SYNC_ENABLED
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
#endif

namespace aqq
{
	public partial class MentorManager
	{
		static MentorManager defaultInstance = new MentorManager();
		MobileServiceClient client;

#if OFFLINE_SYNC_ENABLED
        IMobileServiceSyncTable<Mentor> mentorTable;
#else
		IMobileServiceTable<Mentor> mentorTable;
#endif

		const string offlineDbPath = @"localstore.db";

		private MentorManager()
		{
			this.client = new MobileServiceClient(Constants.ApplicationURL);

#if OFFLINE_SYNC_ENABLED
            var store = new MobileServiceSQLiteStore(offlineDbPath);
            store.DefineTable<Mentor>();

            //Initializes the SyncContext using the default IMobileServiceSyncHandler.
            this.client.SyncContext.InitializeAsync(store);

            this.mentorTable = client.GetSyncTable<Mentor>();
#else
			this.mentorTable = client.GetTable<Mentor>();
#endif
		}

		public static MentorManager DefaultManager
		{
			get
			{
				return defaultInstance;
			}
			private set
			{
				defaultInstance = value;
			}
		}

		public MobileServiceClient CurrentClient
		{
			get { return client; }
		}

		public bool IsOfflineEnabled
		{
			get { return mentorTable is Microsoft.WindowsAzure.MobileServices.Sync.IMobileServiceSyncTable<Mentor>; }
		}

		public async Task<ObservableCollection<Mentor>> GetMentorsAsync(bool syncItems = false)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				IEnumerable<Mentor> items = await mentorTable
					// .Where(mentorItem => !mentorItem.Done)
					.ToEnumerableAsync();

				return new ObservableCollection<Mentor>(items);
			}
			catch (MobileServiceInvalidOperationException msioe)
			{
				Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
			}
			catch (Exception e)
			{
				Debug.WriteLine(@"Sync error: {0}", e.Message);
			}
			return null;
		}

		public async Task<ObservableCollection<Mentor>> GetPeersAsync(string skill)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				IEnumerable<Mentor> items = await mentorTable
					 .Where(mentorItem => mentorItem.Skill == skill)
					.ToEnumerableAsync();

				return new ObservableCollection<Mentor>(items);
			}
			catch (MobileServiceInvalidOperationException msioe)
			{
				Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
			}
			catch (Exception e)
			{
				Debug.WriteLine(@"Sync error: {0}", e.Message);
			}
			return null;
		}




		public async Task SaveTaskAsync(Mentor item)
		{
			if (item.Id == null)
			{
				await mentorTable.InsertAsync(item);
			}
			else
			{
				await mentorTable.UpdateAsync(item);
			}
		}


		public async Task<List<Mentor>> GetIDAsync(string e1)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				List<Mentor> items = await mentorTable
					 .Where(menItem => menItem.Email == e1)
					 .ToListAsync();

				return items;
			}
			catch (MobileServiceInvalidOperationException msioe)
			{
				Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
			}
			catch (Exception e)
			{
				Debug.WriteLine(@"Sync error: {0}", e.Message);
			}
			return null;
		}
		public async Task UpdateTaskAsync(Mentor item)
		{

			await mentorTable.UpdateAsync(item);

		}




#if OFFLINE_SYNC_ENABLED
        public async Task SyncAsync()
        {
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                await this.client.SyncContext.PushAsync();

                await this.mentorTable.PullAsync(
                    //The first parameter is a query name that is used internally by the client SDK to implement incremental sync.
                    //Use a different query name for each unique query in your program
                    "allMentors",
                    this.mentorTable.CreateQuery());
            }
            catch (MobileServicePushFailedException exc)
            {
                if (exc.PushResult != null)
                {
                    syncErrors = exc.PushResult.Errors;
                }
            }

            // Simple error/conflict handling. A real application would handle the various errors like network conditions,
            // server conflicts and others via the IMobileServiceSyncHandler.
            if (syncErrors != null)
            {
                foreach (var error in syncErrors)
                {
                    if (error.OperationKind == MobileServiceTableOperationKind.Update && error.Result != null)
                    {
                        //Update failed, reverting to server's copy.
                        await error.CancelAndUpdateItemAsync(error.Result);
                    }
                    else
                    {
                        // Discard local change.
                        await error.CancelAndDiscardItemAsync();
                    }

                    Debug.WriteLine(@"Error executing sync operation. Item: {0} ({1}). Operation discarded.", error.TableName, error.Item["id"]);
                }
            }
        }
#endif
	}


	public class Mentor
	{
		string id;
		string name;
		string email;
		string postcode;
		string phone;
		string skill;
		string status;


		[JsonProperty(PropertyName = "id")]
		public string Id
		{
			get { return id; }
			set { id = value; }
		}

		[JsonProperty(PropertyName = "name")]
		public string Name
		{
			get { return name; }
			set { name = value; }
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