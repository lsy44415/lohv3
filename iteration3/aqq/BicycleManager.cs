
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
	public partial class BicycleManager
	{
		static BicycleManager defaultInstance = new BicycleManager();
		MobileServiceClient client;
#if OFFLINE_SYNC_ENABLED
        IMobileServiceSyncTable<Bicycle> bicycleTable;
#else
		IMobileServiceTable<Bicycle> bicycleTable;
#endif

		const string offlineDbPath = @"localstore.db";

		private BicycleManager()
		{
			this.client = new MobileServiceClient(Constants.ApplicationURL);

#if OFFLINE_SYNC_ENABLED
            var store = new MobileServiceSQLiteStore(offlineDbPath);
            store.DefineTable<Bicycle>();

            //Initializes the SyncContext using the default IMobileServiceSyncHandler.
            this.client.SyncContext.InitializeAsync(store);

            this.bicycleTable = client.GetSyncTable<Bicycle>();
#else
			this.bicycleTable = client.GetTable<Bicycle>();
#endif
		}

		public static BicycleManager DefaultManager
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
			get { return bicycleTable is Microsoft.WindowsAzure.MobileServices.Sync.IMobileServiceSyncTable<Bicycle>; }
		}

		public async Task<ObservableCollection<Bicycle>> GetBicyclesAsync(bool syncItems = false)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				IEnumerable<Bicycle> items = await bicycleTable
					// .Where(couItem => !couItem.Done)
					.ToEnumerableAsync();

				return new ObservableCollection<Bicycle>(items);
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


		public async Task<ObservableCollection<Bicycle>> GetAll()
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif

				IEnumerable<Bicycle> items = await bicycleTable
					// .Where(couItem => !couItem.Done)
					.ToEnumerableAsync();

				return new ObservableCollection<Bicycle>(items);
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




		public async Task<List<Bicycle>> GetAllAsync()
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif

				List<Bicycle> items = new List<Bicycle>();
				
				items = await bicycleTable
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




		public async Task<List<Bicycle>> GetIDAsync(string e1, string date, string time)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				List<Bicycle> items = await bicycleTable
					 //   .Where(couItem => couItem.Email == e1 && couItem.Date == date && couItem.Time == time)
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


		//      public async Task<List<Bicycle>> GetbyNameAsync(string name)
		//      {
		//          try
		//          {
		//#if OFFLINE_SYNC_ENABLED
		//                if (syncItems)
		//                {
		//                    await this.SyncAsync();
		//                }
		//#endif
		//      List<Bicycle> items = await bicycleTable
		//           .Where(couItem => couItem.Name == name)
		//           .ToListAsync();


		//      return items;
		//  }
		//  catch (MobileServiceInvalidOperationException msioe)
		//  {
		//      Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
		//  }
		//  catch (Exception e)
		//  {
		//      Debug.WriteLine(@"Sync error: {0}", e.Message);
		//  }
		//  return null;
		//}


//		public async Task<string> GetbyBicycleAsync(string address)
//		{
//			try
//			{
//#if OFFLINE_SYNC_ENABLED
//                if (syncItems)
//                {
//                    await this.SyncAsync();
//                }
//#endif

		//		List<Bicycle> items = await bicycleTable
		//		//	.Where(couItem => address.Contains(couItem))
		//			 .ToListAsync();
		//		if (items.Count > 0)
		//		{
		//			return items;
		//		}
		//	}
		//	catch (MobileServiceInvalidOperationException msioe)
		//	{
		//		Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
		//	}
		//	catch (Exception e)
		//	{
		//		Debug.WriteLine(@"Sync error: {0}", e.Message);
		//	}
		//	return null;
		//}




		//      public async Task<string> GetPhoneAsync(string name)
		//      {
		//          try
		//          {
		//#if OFFLINE_SYNC_ENABLED
		//                if (syncItems)
		//                {
		//                    await this.SyncAsync();
		//                }
		//#endif
		//      List<Bicycle> items = await bicycleTable
		//           .Where(couItem => couItem.Name == name)
		//           .ToListAsync();


		//      return items[0].Phone;
		//  }
		//  catch (MobileServiceInvalidOperationException msioe)
		//  {
		//      Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
		//  }
		//  catch (Exception e)
		//  {
		//      Debug.WriteLine(@"Sync error: {0}", e.Message);
		//  }
		//  return null;
		//}





		//public async Task SaveTaskAsync(Bicycle item)
		//{
		//	if (item.Id == null)
		//	{
		//		await bicycleTable.InsertAsync(item);
		//	}
		//	else
		//	{
		//		await bicycleTable.UpdateAsync(item);
		//	}
		//}

		public async Task UpdateTaskAsync(Bicycle item)
		{

			await bicycleTable.UpdateAsync(item);

		}

		public async Task DeleteTaskAsync(Bicycle item)
		{

			await bicycleTable.DeleteAsync(item);

		}


#if OFFLINE_SYNC_ENABLED
        public async Task SyncAsync()
        {
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                await this.client.SyncContext.PushAsync();

                await this.bicycleTable.PullAsync(
                    //The first parameter is a query name that is used internally by the client SDK to implement incremental sync.
                    //Use a different query name for each unique query in your program
                    "allBicycles",
                    this.bicycleTable.CreateQuery());
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


	public class Bicycle
	{
        string id;
		string name;
        string lat;
        string address;
		string lon;



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


		[JsonProperty(PropertyName = "name")]
		public string Name
		{
			get { return name; }
			set { name = value; }
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



		


	}
}


