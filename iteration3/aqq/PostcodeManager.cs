
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
	public partial class PostcodeManager
	{
		static PostcodeManager defaultInstance = new PostcodeManager();
		MobileServiceClient client;
#if OFFLINE_SYNC_ENABLED
        IMobileServiceSyncTable<Postcode> postcodeTable;
#else
		IMobileServiceTable<Postcode> postcodeTable;
#endif

		const string offlineDbPath = @"localstore.db";

		private PostcodeManager()
		{
			this.client = new MobileServiceClient(Constants.ApplicationURL);

#if OFFLINE_SYNC_ENABLED
            var store = new MobileServiceSQLiteStore(offlineDbPath);
            store.DefineTable<Postcode>();

            //Initializes the SyncContext using the default IMobileServiceSyncHandler.
            this.client.SyncContext.InitializeAsync(store);

            this.postcodeTable = client.GetSyncTable<Postcode>();
#else
			this.postcodeTable = client.GetTable<Postcode>();
#endif
		}

		public static PostcodeManager DefaultManager
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
			get { return postcodeTable is Microsoft.WindowsAzure.MobileServices.Sync.IMobileServiceSyncTable<Postcode>; }
		}

		public async Task<ObservableCollection<Postcode>> GetPostcodesAsync(bool syncItems = false)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				IEnumerable<Postcode> items = await postcodeTable
					// .Where(couItem => !couItem.Done)
					.ToEnumerableAsync();

				return new ObservableCollection<Postcode>(items);
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


		public async Task<ObservableCollection<Postcode>> GetAll()
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif

				IEnumerable<Postcode> items = await postcodeTable
					// .Where(couItem => !couItem.Done)
					.ToEnumerableAsync();

				return new ObservableCollection<Postcode>(items);
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




		public async Task<List<Postcode>> GetAllAsync()
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif

				List<Postcode> items = new List<Postcode>();
				items.Capacity = 270;
				items = await postcodeTable
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




		public async Task<List<Postcode>> GetIDAsync(string e1, string date, string time)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				List<Postcode> items = await postcodeTable
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


//		public async Task<List<Postcode>> GetbyNameAsync(string name)
//		{
//			try
//			{
//#if OFFLINE_SYNC_ENABLED
//                if (syncItems)
//                {
//                    await this.SyncAsync();
//                }
//#endif
		//		List<Postcode> items = await postcodeTable
		//			 .Where(couItem => couItem.Name == name)
		//			 .ToListAsync();


		//		return items;
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


		public async Task<string> GetbyPostcodeAsync(string address)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif

				List<Postcode> items = await postcodeTable
					.Where(couItem => address.Contains(couItem.Suburb))
					 .ToListAsync();
                if (items.Count > 0)
                {
                    return items[0].Code;
                }
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




//		public async Task<string> GetPhoneAsync(string name)
//		{
//			try
//			{
//#if OFFLINE_SYNC_ENABLED
//                if (syncItems)
//                {
//                    await this.SyncAsync();
//                }
//#endif
		//		List<Postcode> items = await postcodeTable
		//			 .Where(couItem => couItem.Name == name)
		//			 .ToListAsync();


		//		return items[0].Phone;
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





		public async Task SaveTaskAsync(Postcode item)
		{
			if (item.Id == null)
			{
				await postcodeTable.InsertAsync(item);
			}
			else
			{
				await postcodeTable.UpdateAsync(item);
			}
		}

		public async Task UpdateTaskAsync(Postcode item)
		{

			await postcodeTable.UpdateAsync(item);

		}

		public async Task DeleteTaskAsync(Postcode item)
		{

			await postcodeTable.DeleteAsync(item);

		}


#if OFFLINE_SYNC_ENABLED
        public async Task SyncAsync()
        {
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                await this.client.SyncContext.PushAsync();

                await this.postcodeTable.PullAsync(
                    //The first parameter is a query name that is used internally by the client SDK to implement incremental sync.
                    //Use a different query name for each unique query in your program
                    "allPostcodes",
                    this.postcodeTable.CreateQuery());
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


	public class Postcode
	{
		string id;
		string code;
		string suburb;
		



		[JsonProperty(PropertyName = "id")]
		public string Id
		{
			get { return id; }
			set { id = value; }
		}

		

		[JsonProperty(PropertyName = "code")]
		public string Code
		{
			get { return code; }
			set { code = value; }
		}


		

		[JsonProperty(PropertyName = "suburb")]
		public string Suburb
		{
			get { return suburb; }
			set { suburb = value; }
		}

		
	}
}

