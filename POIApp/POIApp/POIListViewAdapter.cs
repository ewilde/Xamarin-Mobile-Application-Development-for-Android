using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Locations;

namespace POIApp
{
	public class POIListViewAdapter : BaseAdapter<PointOfInterest>
	{
		private readonly Activity _context;

		public POIListViewAdapter(Activity context)
		{
			_context = context;
		}

		public Location CurrentLocation {
			get;
			set;
		}

		public override int Count
		{
			get { return  POIListActivity.POIDataService.POIs.Count; }
		}


		public override long GetItemId (int position)
		{
			return this[position].Id;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			View view = convertView;
			if (view == null)
				view = _context.LayoutInflater.Inflate(Resource.Layout.POIListItem, null);

			var poi = this [position];

			view.FindViewById<TextView>(Resource.Id.nameTextView).Text = poi.Name;

			if (String.IsNullOrEmpty (poi.Address))
				view.FindViewById<TextView>(Resource.Id.addrTextView).Visibility
				= ViewStates.Gone;
			else
				view.FindViewById<TextView> (Resource.Id.addrTextView).Text = poi.Address;

			if ((CurrentLocation != null) &&
				(poi.Latitude.HasValue) && (poi.Longitude.HasValue)) {
				Location poiLocation = new Location ("");
				poiLocation.Latitude = poi.Latitude.Value;	
				poiLocation.Longitude = poi.Longitude.Value;
				float distance = CurrentLocation.DistanceTo (poiLocation) * 0.000621371F; 
				view.FindViewById<TextView> (Resource.Id.distanceTextView).Text = 
					String.Format ("{0:0.00} miles", distance);
			}
			else {
				view.FindViewById<TextView> (Resource.Id.distanceTextView).Text = "??";
			}

			return view;
		}

		public override PointOfInterest this [int index] {
			get {
				return POIListActivity.POIDataService.POIs[index];
			}
		}
	}
}

