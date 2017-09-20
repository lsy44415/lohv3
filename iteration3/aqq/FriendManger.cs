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
	public partial class FriendManager
	{
		static FriendManager defaultInstance = new FriendManager();
		MobileServiceClient client;

#if OFFLINE_SYNC_ENABLED
        IMobileServiceSyncTable<Friend> friendTable;
#else
        IMobileServiceTable<Friend> friendTable;
#endif

		const string offlineDbPath = @"localstore.db";

		private FriendManager()
		{
			this.client = new MobileServiceClient(Constants.ApplicationURL);

#if OFFLINE_SYNC_ENABLED
            var store = new MobileServiceSQLiteStore(offlineDbPath);
            store.DefineTable<Friend>();

            //Initializes the SyncContext using the default IMobileServiceSyncHandler.
            this.client.SyncContext.InitializeAsync(store);

            this.friendTable = client.GetSyncTable<Friend>();
#else
			this.friendTable = client.GetTable<Friend>();
#endif
		}

		public static FriendManager DefaultManager
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
			get { return friendTable is Microsoft.WindowsAzure.MobileServices.Sync.IMobileServiceSyncTable<Friend>; }
		}

		public async Task<ObservableCollection<Friend>> GetFriendsAsync(bool syncItems = false)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				IEnumerable<Friend> items = await friendTable
					// .Where(volItem => !volItem.Done)
					.ToEnumerableAsync();

				return new ObservableCollection<Friend>(items);
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

        public async Task<ObservableCollection<Friend>> GetyoufollowAsync(string email)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				IEnumerable<Friend> items = await friendTable
                    .Where(volItem => volItem.Email1 == email && volItem.Agree==true)
					.ToEnumerableAsync();

				return new ObservableCollection<Friend>(items);
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


        public async Task<ObservableCollection<Friend>> GetfollowyouPeersAsync(string email)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				IEnumerable<Friend> items = await friendTable
					.Where(volItem => volItem.Email2 == email && volItem.Agree == true)
					.ToEnumerableAsync();

				return new ObservableCollection<Friend>(items);
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




		public async Task<List<Friend>> GetUnacceptPeersAsync(string email)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				List<Friend> items = await friendTable
					 .Where(volItem => volItem.Email2 == email && volItem.Agree == false)
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

		public async Task<List<Friend>> CheckDuplicateAsync(string peremail,string followemail)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				List<Friend> items = await friendTable
                    .Where(volItem => volItem.Email1 == peremail && volItem.Email2 == followemail)
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





		public async Task<List<Friend>> GetIDAsync(string e1)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				List<Friend> items = await friendTable
					 .Where(volItem => volItem.Email1 == e1)
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

		public async Task<List<Friend>> TestByEmailAsync(string e1,string e2)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				List<Friend> items = await friendTable
					 .Where(volItem => volItem.Email1 == e1 && volItem.Email2 == e2)
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







		public async Task SaveTaskAsync(Friend item)
		{
			if (item.Id == null)
			{
				await friendTable.InsertAsync(item);
			}
			else
			{
				await friendTable.UpdateAsync(item);
			}
		}

		public async Task UpdateTaskAsync(Friend item)
		{

			await friendTable.UpdateAsync(item);

		}
		public async Task DeleteTaskAsync(Friend item)
		{

			await friendTable.DeleteAsync(item);

		}



#if OFFLINE_SYNC_ENABLED
        public async Task SyncAsync()
        {
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                await this.client.SyncContext.PushAsync();

                await this.friendTable.PullAsync(
                    //The first parameter is a query name that is used internally by the client SDK to implement incremental sync.
                    //Use a different query name for each unique query in your program
                    "allFriends",
                    this.friendTable.CreateQuery());
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


	public class Friend
	{
		string id;
		string name1;
		string email1;
		string phone1;
		string name2;
		string email2;
		string phone2;
        bool agree;



		[JsonProperty(PropertyName = "id")]
		public string Id
		{
			get { return id; }
			set { id = value; }
		}

		[JsonProperty(PropertyName = "name1")]
		public string Name1
		{
			get { return name1; }
			set { name1 = value; }
		}


		[JsonProperty(PropertyName = "email1")]
		public string Email1
		{
			get { return email1; }
			set { email1 = value; }
		}

		

		[JsonProperty(PropertyName = "phone1")]
		public string Phone1
		{
			get { return phone1; }
			set { phone1 = value; }
		}

		[JsonProperty(PropertyName = "name2")]
		public string Name2
		{
			get { return name2; }
			set { name2 = value; }
		}


		[JsonProperty(PropertyName = "email2")]
		public string Email2
		{
			get { return email2; }
			set { email2 = value; }
		}



		[JsonProperty(PropertyName = "phone2")]
		public string Phone2
		{
			get { return phone2; }
			set { phone2 = value; }
		}

		[JsonProperty(PropertyName = "agree")]
		public bool Agree
		{
			get { return agree; }
			set { agree = value; }
		}
	}
}
