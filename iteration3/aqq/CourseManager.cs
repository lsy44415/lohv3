
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
	public partial class CourseManager
	{
		static CourseManager defaultInstance = new CourseManager();
		MobileServiceClient client;

#if OFFLINE_SYNC_ENABLED
        IMobileServiceSyncTable<Course> courseTable;
#else
		IMobileServiceTable<Course> courseTable;
#endif

		const string offlineDbPath = @"localstore.db";

		private CourseManager()
		{
			this.client = new MobileServiceClient(Constants.ApplicationURL);

#if OFFLINE_SYNC_ENABLED
            var store = new MobileServiceSQLiteStore(offlineDbPath);
            store.DefineTable<Course>();

            //Initializes the SyncContext using the default IMobileServiceSyncHandler.
            this.client.SyncContext.InitializeAsync(store);

            this.courseTable = client.GetSyncTable<Course>();
#else
			this.courseTable = client.GetTable<Course>();
#endif
		}

		public static CourseManager DefaultManager
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
			get { return courseTable is Microsoft.WindowsAzure.MobileServices.Sync.IMobileServiceSyncTable<Course>; }
		}

		public async Task<ObservableCollection<Course>> GetCoursesAsync(bool syncItems = false)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				IEnumerable<Course> items = await courseTable
					// .Where(couItem => !couItem.Done)
					.ToEnumerableAsync();

				return new ObservableCollection<Course>(items);
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

		public async Task<ObservableCollection<Course>> GetEmailClassAsync(string email)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				IEnumerable<Course> items = await courseTable
					 .Where(couItem =>  couItem.Email == email)
					.ToEnumerableAsync();

				return new ObservableCollection<Course>(items);
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




		public async Task<ObservableCollection<Course>> GetSkillClassAsync(string skill)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				IEnumerable<Course> items = await courseTable
					 .Where(couItem => couItem.Skill == skill)
					.ToEnumerableAsync();

				return new ObservableCollection<Course>(items);
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




		public async Task<List<Course>> GetIDAsync(string e1,string date,string time)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				List<Course> items = await courseTable
					 .Where(couItem => couItem.Email == e1 && couItem.Date== date && couItem.Time== time)
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


		public async Task<List<Course>> GetUpdateAsync(string e1, string date, string time,string id)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				List<Course> items = await courseTable
					 .Where(couItem => couItem.Email == e1 && couItem.Date == date && couItem.Time == time && couItem.Id!=id)
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









		public async Task SaveTaskAsync(Course item)
		{
			if (item.Id == null)
			{
				await courseTable.InsertAsync(item);
			}
			else
			{
				await courseTable.UpdateAsync(item);
			}
		}

		public async Task UpdateTaskAsync(Course item)
		{

			await courseTable.UpdateAsync(item);

		}

		public async Task DeleteTaskAsync(Course item)
		{

            await courseTable.DeleteAsync(item);

		}


#if OFFLINE_SYNC_ENABLED
        public async Task SyncAsync()
        {
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                await this.client.SyncContext.PushAsync();

                await this.courseTable.PullAsync(
                    //The first parameter is a query name that is used internally by the client SDK to implement incremental sync.
                    //Use a different query name for each unique query in your program
                    "allCourses",
                    this.courseTable.CreateQuery());
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


	public class Course
	{
		string id;
		string email;
        string location;
		string skill;
        string date;
		string time;
        string topic;
        string name;
        string phone;


		[JsonProperty(PropertyName = "id")]
		public string Id
		{
			get { return id; }
			set { id = value; }
		}

		[JsonProperty(PropertyName = "location")]
		public string Location
		{
			get { return location; }
			set { location = value; }
		}


		[JsonProperty(PropertyName = "email")]
		public string Email
		{
			get { return email; }
			set { email = value; }
		}

		[JsonProperty(PropertyName = "date")]
		public string Date
		{
			get { return date; }
			set { date = value; }
		}

		[JsonProperty(PropertyName = "skill")]
		public string Skill
		{
			get { return skill; }
			set { skill = value; }
		}

		[JsonProperty(PropertyName = "time")]
		public string Time
		{
			get { return time; }
			set { time = value; }
		}


		[JsonProperty(PropertyName = "topic")]
		public string Topic
		{
			get { return topic; }
			set { topic = value; }
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

