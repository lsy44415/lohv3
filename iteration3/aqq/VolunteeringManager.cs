
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
	public partial class VolunteeringManager
	{
		static VolunteeringManager defaultInstance = new VolunteeringManager();
		MobileServiceClient client;

#if OFFLINE_SYNC_ENABLED
        IMobileServiceSyncTable<Volunteering> volunteeringTable;
#else
		IMobileServiceTable<Volunteering> volunteeringTable;
#endif

		const string offlineDbPath = @"localstore.db";

		private VolunteeringManager()
		{
			this.client = new MobileServiceClient(Constants.ApplicationURL);

#if OFFLINE_SYNC_ENABLED
            var store = new MobileServiceSQLiteStore(offlineDbPath);
            store.DefineTable<Volunteering>();

            //Initializes the SyncContext using the default IMobileServiceSyncHandler.
            this.client.SyncContext.InitializeAsync(store);

            this.volunteeringTable = client.GetSyncTable<Volunteering>();
#else
			this.volunteeringTable = client.GetTable<Volunteering>();
#endif
		}

		public static VolunteeringManager DefaultManager
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
			get { return volunteeringTable is Microsoft.WindowsAzure.MobileServices.Sync.IMobileServiceSyncTable<Volunteering>; }
		}

		public async Task<ObservableCollection<Volunteering>> GetVolunteeringsAsync(bool syncItems = false)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				IEnumerable<Volunteering> items = await volunteeringTable
					// .Where(couItem => !couItem.Done)
					.ToEnumerableAsync();

				return new ObservableCollection<Volunteering>(items);
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

		public async Task<ObservableCollection<Volunteering>> GetEmailClassAsync(string email)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				IEnumerable<Volunteering> items = await volunteeringTable
					// .Where(couItem => couItem.Email1 == email)
					.ToEnumerableAsync();

				return new ObservableCollection<Volunteering>(items);
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

		public async Task<ObservableCollection<Volunteering>> GetTeacherEmailClassAsync(string email)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				IEnumerable<Volunteering> items = await volunteeringTable
					// .Where(couItem => couItem.Email2 == email)
					.ToEnumerableAsync();

				return new ObservableCollection<Volunteering>(items);
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


		public async Task<List<Volunteering>> GetDetailAsync(string name)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				List<Volunteering> items = await volunteeringTable
					 .Where(couItem => couItem.Name.Trim().ToLower() == name.ToLower().Trim())
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




		public async Task<List<Volunteering>> GetNameAsync(int post)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				List<Volunteering> items = await volunteeringTable
                    .Where(couItem => couItem.Postcode < (post+5) && couItem.Postcode > (post - 5))
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

		public async Task<List<Volunteering>> TestbyIDAsync(string email, string id)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				List<Volunteering> items = await volunteeringTable
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







		public async Task SaveTaskAsync(Volunteering item)
		{
			if (item.Id == null)
			{
				await volunteeringTable.InsertAsync(item);
			}
			else
			{
				await volunteeringTable.UpdateAsync(item);
			}
		}

		public async Task UpdateTaskAsync(Volunteering item)
		{

			await volunteeringTable.UpdateAsync(item);

		}

		public async Task DeleteTaskAsync(Volunteering item)
		{

			await volunteeringTable.DeleteAsync(item);

		}


#if OFFLINE_SYNC_ENABLED
        public async Task SyncAsync()
        {
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                await this.client.SyncContext.PushAsync();

                await this.volunteeringTable.PullAsync(
                    //The first parameter is a query name that is used internally by the client SDK to implement incremental sync.
                    //Use a different query name for each unique query in your program
                    "allVolunteerings",
                    this.volunteeringTable.CreateQuery());
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


	public class Volunteering
	{
		string id;
		
		string address;
		int postcode;
		string name;
		
		string suburb;
		






		[JsonProperty(PropertyName = "id")]
		public string Id
		{
			get { return id; }
			set { id = value; }
		}



		


		[JsonProperty(PropertyName = "address")]
		public string Address
		{
			get { return address; }
			set { address = value; }
		}

		[JsonProperty(PropertyName = "suburb")]
		public string Suburb
		{
			get { return suburb; }
			set { suburb = value; }
		}


		


		[JsonProperty(PropertyName = "postcode")]
		public int Postcode
		{
			get { return postcode; }
			set { postcode = value; }
		}
		

		[JsonProperty(PropertyName = "name")]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		


	}
}



