
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
	public partial class EnrolManager
	{
		static EnrolManager defaultInstance = new EnrolManager();
		MobileServiceClient client;

#if OFFLINE_SYNC_ENABLED
        IMobileServiceSyncTable<Enrol> enrolTable;
#else
		IMobileServiceTable<Enrol> enrolTable;
#endif

		const string offlineDbPath = @"localstore.db";

		private EnrolManager()
		{
			this.client = new MobileServiceClient(Constants.ApplicationURL);

#if OFFLINE_SYNC_ENABLED
            var store = new MobileServiceSQLiteStore(offlineDbPath);
            store.DefineTable<Enrol>();

            //Initializes the SyncContext using the default IMobileServiceSyncHandler.
            this.client.SyncContext.InitializeAsync(store);

            this.enrolTable = client.GetSyncTable<Enrol>();
#else
			this.enrolTable = client.GetTable<Enrol>();
#endif
		}

		public static EnrolManager DefaultManager
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
			get { return enrolTable is Microsoft.WindowsAzure.MobileServices.Sync.IMobileServiceSyncTable<Enrol>; }
		}

		public async Task<ObservableCollection<Enrol>> GetEnrolsAsync(bool syncItems = false)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				IEnumerable<Enrol> items = await enrolTable
					// .Where(couItem => !couItem.Done)
					.ToEnumerableAsync();

				return new ObservableCollection<Enrol>(items);
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

		public async Task<ObservableCollection<Enrol>> GetEmailClassAsync(string email)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				IEnumerable<Enrol> items = await enrolTable
					 .Where(couItem => couItem.Email1== email)
					.ToEnumerableAsync();

				return new ObservableCollection<Enrol>(items);
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

		public async Task<ObservableCollection<Enrol>> GetTeacherEmailClassAsync(string email)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				IEnumerable<Enrol> items = await enrolTable
					 .Where(couItem => couItem.Email2 == email)
					.ToEnumerableAsync();

				return new ObservableCollection<Enrol>(items);
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

		public async Task<ObservableCollection<Enrol>> GetMentorEmailClassAsync(string email)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				IEnumerable<Enrol> items = await enrolTable
					 .Where(couItem => couItem.Email1 == email)
					.ToEnumerableAsync();

				return new ObservableCollection<Enrol>(items);
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





		public async Task<List<Enrol>> GetIDAsync(string e1, string date, string time)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				List<Enrol> items = await enrolTable
					 .Where(couItem => couItem.Email1 == e1 && couItem.Date == date && couItem.Time == time)
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

		public async Task<List<Enrol>> TestbyIDAsync(string email, string id)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				List<Enrol> items = await enrolTable
					 .Where(couItem => couItem.Email1 == email && couItem.Courseid == id)
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







		public async Task SaveTaskAsync(Enrol item)
		{
			if (item.Id == null)
			{
				await enrolTable.InsertAsync(item);
			}
			else
			{
				await enrolTable.UpdateAsync(item);
			}
		}

		public async Task UpdateTaskAsync(Enrol item)
		{

			await enrolTable.UpdateAsync(item);

		}

		public async Task DeleteTaskAsync(Enrol item)
		{

			await enrolTable.DeleteAsync(item);

		}


#if OFFLINE_SYNC_ENABLED
        public async Task SyncAsync()
        {
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                await this.client.SyncContext.PushAsync();

                await this.enrolTable.PullAsync(
                    //The first parameter is a query name that is used internally by the client SDK to implement incremental sync.
                    //Use a different query name for each unique query in your program
                    "allEnrols",
                    this.enrolTable.CreateQuery());
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


	public class Enrol
	{
		string id;
		string email1;
        string learner;
        string courseid;
        string topic;
        string date;
        string time;
        string location;
        string teacher;
        string email2;
        string phone;
        string phone2;





		[JsonProperty(PropertyName = "id")]
		public string Id
		{
			get { return id; }
			set { id = value; }
		}

		

		[JsonProperty(PropertyName = "email1")]
		public string Email1
		{
			get { return email1; }
			set { email1 = value; }
		}


		[JsonProperty(PropertyName = "coureseid")]
		public string Courseid
		{
			get { return courseid; }
			set { courseid = value; }
		}

		[JsonProperty(PropertyName = "topic")]
		public string Topic
		{
			get { return topic; }
			set { topic= value; }
		}



		[JsonProperty(PropertyName = "location")]
		public string Location
		{
			get { return location; }
			set { location = value; }
		}


		[JsonProperty(PropertyName = "date")]
		public string Date
		{
			get { return date; }
			set { date = value; }
		}
		[JsonProperty(PropertyName = "time")]
		public string Time
		{
			get { return time; }
			set { time = value; }
		}



		[JsonProperty(PropertyName = "teacher")]
		public string Teacher
		{
			get { return teacher; }
			set { teacher = value; }
		}

		[JsonProperty(PropertyName = "email2")]
		public string Email2
		{
			get { return email2; }
			set { email2 = value; }
		}
		[JsonProperty(PropertyName = "learner")]
		public string Learner
		{
			get { return learner; }
			set { learner = value; }
		}

		[JsonProperty(PropertyName = "phone")]
		public string Phone
		{
			get { return phone; }
			set { phone = value; }
		}
		[JsonProperty(PropertyName = "phone2")]
		public string Phone2
		{
			get { return phone2; }
			set { phone2 = value; }
		}


	}
}

