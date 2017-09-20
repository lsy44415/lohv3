
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
	public partial class Job2Manager
	{
		static Job2Manager defaultInstance = new Job2Manager();
		MobileServiceClient client;

#if OFFLINE_SYNC_ENABLED
        IMobileServiceSyncTable<Job2> job2Table;
#else
		IMobileServiceTable<Job2> job2Table;
#endif

		const string offlineDbPath = @"localstore.db";

		private Job2Manager()
		{
			this.client = new MobileServiceClient(Constants.ApplicationURL);

#if OFFLINE_SYNC_ENABLED
            var store = new MobileServiceSQLiteStore(offlineDbPath);
            store.DefineTable<Job2>();

            //Initializes the SyncContext using the default IMobileServiceSyncHandler.
            this.client.SyncContext.InitializeAsync(store);

            this.job2Table = client.GetSyncTable<Job2>();
#else
			this.job2Table = client.GetTable<Job2>();
#endif
		}

		public static Job2Manager DefaultManager
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
			get { return job2Table is Microsoft.WindowsAzure.MobileServices.Sync.IMobileServiceSyncTable<Job2>; }
		}

		public async Task<ObservableCollection<Job2>> GetJob2sAsync(bool syncItems = false)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				IEnumerable<Job2> items = await job2Table
					// .Where(couItem => !couItem.Done)
					.ToEnumerableAsync();

				return new ObservableCollection<Job2>(items);
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

		public async Task<ObservableCollection<Job2>> GetEmailClasGesAsync(string email)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				IEnumerable<Job2> items = await job2Table
					// .Where(couItem => couItem.Email1 == email)
					.ToEnumerableAsync();

				return new ObservableCollection<Job2>(items);
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

		public async Task<List<Job2>> GetAllAsync()
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				List<Job2> items = await job2Table.ToListAsync();



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


		public async Task<List<Job2>> GetDetailAsync(string post, string name)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				List<Job2> items = await job2Table
					 .Where(couItem => couItem.Postcode == post && couItem.Name == name)
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




		public async Task<List<Job2>> GetNameAsync(string post)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				List<Job2> items = await job2Table
					 .Where(couItem => couItem.Postcode == post)
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

		public async Task<List<Job2>> TestbyIDAsync(string email, string id)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				List<Job2> items = await job2Table
					 // .Where(couItem => couItem.Email1 == email && couItem.Courseid == id)
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







		public async Task SaveTaskAsync(Job2 item)
		{
			if (item.Id == null)
			{
				await job2Table.InsertAsync(item);
			}
			else
			{
				await job2Table.UpdateAsync(item);
			}
		}

		public async Task UpdateTaskAsync(Job2 item)
		{

			await job2Table.UpdateAsync(item);

		}

		public async Task DeleteTaskAsync(Job2 item)
		{

			await job2Table.DeleteAsync(item);

		}


#if OFFLINE_SYNC_ENABLED
        public async Task SyncAsync()
        {
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                await this.client.SyncContext.PushAsync();

                await this.job2Table.PullAsync(
                    //The first parameter is a query name that is used internally by the client SDK to implement incremental sync.
                    //Use a different query name for each unique query in your program
                    "allJob2s",
                    this.job2Table.CreateQuery());
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


	public class Job2
	{
		string id;
		string email;
		string address;
		string postcode;
		string name;
		string lat;
		string lon;
		string location;
		string phone;
		string url;






		[JsonProperty(PropertyName = "id")]
		public string Id
		{
			get { return id; }
			set { id = value; }
		}



		[JsonProperty(PropertyName = "email")]
		public string Email
		{
			get { return email; }
			set { email = value; }
		}


		[JsonProperty(PropertyName = "address")]
		public string Address
		{
			get { return address; }
			set { address = value; }
		}

		[JsonProperty(PropertyName = "url")]
		public string Url
		{
			get { return url; }
			set { url = value; }
		}


		[JsonProperty(PropertyName = "location")]
		public string Location
		{
			get { return location; }
			set { location = value; }
		}


		[JsonProperty(PropertyName = "postcode")]
		public string Postcode
		{
			get { return postcode; }
			set { postcode = value; }
		}
		[JsonProperty(PropertyName = "lat")]
		public string Lat
		{
			get { return lat; }
			set { lat = value; }
		}



		[JsonProperty(PropertyName = "lon")]
		public string Lon
		{
			get { return lon; }
			set { lon = value; }
		}

		[JsonProperty(PropertyName = "name")]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		[JsonProperty(PropertyName = "phone")]
		public string Phone
		{
			get { return phone; }
			set { phone = value; }
		}


	}
}



