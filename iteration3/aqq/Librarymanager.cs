
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
	public partial class LibraryManager
	{
		static LibraryManager defaultInstance = new LibraryManager();
		MobileServiceClient client;
#if OFFLINE_SYNC_ENABLED
        IMobileServiceSyncTable<Library> libraryTable;
#else
		IMobileServiceTable<Library> libraryTable;
#endif

		const string offlineDbPath = @"localstore.db";

		private LibraryManager()
		{
			this.client = new MobileServiceClient(Constants.ApplicationURL);

#if OFFLINE_SYNC_ENABLED
            var store = new MobileServiceSQLiteStore(offlineDbPath);
            store.DefineTable<Library>();

            //Initializes the SyncContext using the default IMobileServiceSyncHandler.
            this.client.SyncContext.InitializeAsync(store);

            this.libraryTable = client.GetSyncTable<Library>();
#else
			this.libraryTable = client.GetTable<Library>();
#endif
		}

		public static LibraryManager DefaultManager
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
			get { return libraryTable is Microsoft.WindowsAzure.MobileServices.Sync.IMobileServiceSyncTable<Library>; }
		}

		public async Task<ObservableCollection<Library>> GetLibrarysAsync(bool syncItems = false)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				IEnumerable<Library> items = await libraryTable
					// .Where(couItem => !couItem.Done)
					.ToEnumerableAsync();

				return new ObservableCollection<Library>(items);
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


		public async Task<ObservableCollection<Library>> GetAll()
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif 

				IEnumerable<Library> items = await libraryTable
					// .Where(couItem => !couItem.Done)
					.ToEnumerableAsync();

				return new ObservableCollection<Library>(items);
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




		public async Task<List<Library>> GetAllAsync()
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif

                List<Library> items = new List<Library>();
				items.Capacity = 270;
				items = await libraryTable
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




		public async Task<List<Library>> GetIDAsync(string e1, string date, string time)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				List<Library> items = await libraryTable
				//	 .Where(couItem => couItem.Email == e1 && couItem.Date == date && couItem.Time == time)
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


		public async Task <List<Library>> GetbyNameAsync(string name)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				List<Library> items = await libraryTable
					 .Where(couItem => couItem.Name == name )
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


		public async Task<List<Library>> GetbyPostcodeAsync(int code)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif

				List<Library> items = await libraryTable
                    .Where(couItem => couItem.Postcode>(code-10) && couItem.Postcode < (code + 10))
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




		public async Task<string> GetPhoneAsync(string name)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				List<Library> items = await libraryTable
					 .Where(couItem => couItem.Name == name)
					 .ToListAsync();


				return items[0].Phone;
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





		public async Task SaveTaskAsync(Library item)
		{
			if (item.Id == null)
			{
				await libraryTable.InsertAsync(item);
			}
			else
			{
				await libraryTable.UpdateAsync(item);
			}
		}

		public async Task UpdateTaskAsync(Library item)
		{

			await libraryTable.UpdateAsync(item);

		}

		public async Task DeleteTaskAsync(Library item)
		{

			await libraryTable.DeleteAsync(item);

		}


#if OFFLINE_SYNC_ENABLED
        public async Task SyncAsync()
        {
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                await this.client.SyncContext.PushAsync();

                await this.libraryTable.PullAsync(
                    //The first parameter is a query name that is used internally by the client SDK to implement incremental sync.
                    //Use a different query name for each unique query in your program
                    "allLibrarys",
                    this.libraryTable.CreateQuery());
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


	public class Library
	{
		string id;
		string lat;
		string lon;
		string suburb;
		string address;
		string name;
		string phone;
        int postcode;
		


		[JsonProperty(PropertyName = "id")]
		public string Id
		{
			get { return id; }
			set { id = value; }
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

		[JsonProperty(PropertyName = "address")]
		public string Address
		{
			get { return address; }
			set { address = value; }
		}

		[JsonProperty(PropertyName = "phone")]
		public string Phone
		{
			get { return phone; }
			set { phone = value; }
		}


		[JsonProperty(PropertyName = "name")]
		public string Name
		{
			get { return name; }
			set { name = value; }
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
	}
}

