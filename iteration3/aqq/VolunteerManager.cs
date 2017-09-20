/*
 * To add Offline Sync Support:
 *  1) Add the NuGet package Microsoft.Azure.Mobile.Client.SQLiteStore (and dependencies) to all client projects
 *  2) Uncomment the #define OFFLINE_SYNC_ENABLED
 *
 * For more information, see: http://go.microsoft.com/fwlink/?LinkId=620342
 */
//#define OFFLINE_SYNC_ENABLED

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#if OFFLINE_SYNC_ENABLED
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
#endif

namespace aqq
{
	public partial class VolunteerManager
	{
		static VolunteerManager defaultInstance = new VolunteerManager();
		MobileServiceClient client;

#if OFFLINE_SYNC_ENABLED
        IMobileServiceSyncTable<Volunteer> volunteerTable;
#else
		IMobileServiceTable<Volunteer> volunteerTable;
#endif

		const string offlineDbPath = @"localstore.db";

		private VolunteerManager()
		{
			this.client = new MobileServiceClient(Constants.ApplicationURL);

#if OFFLINE_SYNC_ENABLED
            var store = new MobileServiceSQLiteStore(offlineDbPath);
            store.DefineTable<Volunteer>();

            //Initializes the SyncContext using the default IMobileServiceSyncHandler.
            this.client.SyncContext.InitializeAsync(store);

            this.volunteerTable = client.GetSyncTable<Volunteer>();
#else
			this.volunteerTable = client.GetTable<Volunteer>();
#endif
		}

		public static VolunteerManager DefaultManager
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
			get { return volunteerTable is Microsoft.WindowsAzure.MobileServices.Sync.IMobileServiceSyncTable<Volunteer>; }
		}

		public async Task<ObservableCollection<Volunteer>> GetVolunteersAsync(bool syncItems = false)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				IEnumerable<Volunteer> items = await volunteerTable
					// .Where(volItem => !volItem.Done)
					.ToEnumerableAsync();

				return new ObservableCollection<Volunteer>(items);
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

		public async Task<ObservableCollection<Volunteer>> GetPeersAsync(string skill)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				IEnumerable<Volunteer> items = await volunteerTable
					 .Where(volItem => volItem.Skill == skill)
					.ToEnumerableAsync();

				return new ObservableCollection<Volunteer>(items);
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


		public async Task<List<Volunteer>> GetIDAsync(string e1)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
               List<Volunteer> items = await volunteerTable
                    .Where(volItem => volItem.Email == e1)
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


		public async Task<List<Volunteer>> GetbyPhoneAsync(string p1, string e1)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				List<Volunteer> items = await volunteerTable
					 .Where(volItem => volItem.Phone == p1 && volItem.Email != e1 )
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






		public async Task SaveTaskAsync(Volunteer item)
		{
			if (item.Id == null)
			{
				await volunteerTable.InsertAsync(item);
			}
			else
			{
				await volunteerTable.UpdateAsync(item);
			}
		}

        public async Task UpdateTaskAsync(Volunteer item)
		{
			
			await volunteerTable.UpdateAsync(item);
			
		}



#if OFFLINE_SYNC_ENABLED
        public async Task SyncAsync()
        {
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                await this.client.SyncContext.PushAsync();

                await this.volunteerTable.PullAsync(
                    //The first parameter is a query name that is used internally by the client SDK to implement incremental sync.
                    //Use a different query name for each unique query in your program
                    "allVolunteers",
                    this.volunteerTable.CreateQuery());
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


	public class Volunteer
	{
		string id;
		string name;
		string email;
		string postcode;
		string skill;
		string status;
		string phone;
        string profile;


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

        [JsonProperty(PropertyName = "profile")]
        public string Profile
		{
			get { return profile; }
			set { profile = value; }
		}

	}
}
