using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace POIApp
{
	[Activity (Label = "POIApp", MainLauncher = true)]
	public class POIListActivity : Activity
	{
		ListView _poiListView;
		POIListViewAdapter _adapter;
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
		}

		protected void POIClicked(object sender, ListView.ItemClickEventArgs e)
		{
			PointOfInterest poi = POIDataService.GetPOI ((int)e.Id);
			Console.WriteLine ("POIClicked: Address is {0}", poi.AddressLine1);
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
				// place holder for creating new poi
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


