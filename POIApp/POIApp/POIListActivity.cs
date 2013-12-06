using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Locations;

namespace POIApp
{
	[Activity (Label = "POIApp", MainLauncher = true)]
	public class POIListActivity : Activity, ILocationListener 
	{
		ListView _poiListView;
		POIListViewAdapter _adapter;
		LocationManager _locMgr;
		private static readonly IPOIDataService dataService = new POIJsonService (System.Environment.GetFolderPath ( System.Environment.SpecialFolder.MyDocuments));

		public static IPOIDataService POIDataService { get { return dataService; } }

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.POIList);

			_poiListView = FindViewById<ListView> (Resource.Id.poiListView);
			_poiListView.ItemClick += POIClicked;
			_adapter = new POIListViewAdapter (this);
			_poiListView.Adapter = _adapter;

			_locMgr = (LocationManager)GetSystemService (Context.LocationService);
		}

		protected override void OnResume ()
		{
			base.OnResume ();
			_adapter.NotifyDataSetChanged ();

			Criteria criteria = new Criteria ();
			criteria.Accuracy = Accuracy.NoRequirement;
			criteria.PowerRequirement = Power.NoRequirement;

			string provider = _locMgr.GetBestProvider (criteria, true);
			_locMgr.RequestLocationUpdates(provider, 20000, 100, this);

		}

		protected override void OnPause ()
		{
			base.OnPause ();
			_locMgr.RemoveUpdates (this);
		}

		protected void POIClicked(object sender, ListView.ItemClickEventArgs e)
		{
			PointOfInterest poi = POIDataService.GetPOI ((int)e.Id);
			Console.WriteLine ("POIClicked: Address is {0}", poi.AddressLine1);

			Intent poiDetailIntent =
				new Intent (this, typeof(POIDetailActivity));
			poiDetailIntent.PutExtra ("poiId", poi.Id);
			StartActivity (poiDetailIntent);
		}

		public void OnLocationChanged (Location location)
		{
			_adapter.CurrentLocation = location;
			_adapter.NotifyDataSetChanged ();

		}

		public void OnProviderDisabled (string provider)
		{
		}

		public void OnProviderEnabled (string provider)
		{
		}

		public void OnStatusChanged (string provider, Availability status, Bundle extras)
		{
		}


		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.POIListViewMenu, menu);
			return base.OnCreateOptionsMenu(menu);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId)
			{
			case Resource.Id.actionNew: 
				StartActivity (typeof(POIDetailActivity));
				return true;

			case Resource.Id.actionRefresh: 
				POIListActivity.POIDataService.RefreshCache ();
				_adapter.NotifyDataSetChanged ();
				return true;

			default :
				return base.OnOptionsItemSelected(item);
			}
		}

	}
}


