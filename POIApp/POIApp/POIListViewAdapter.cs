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

namespace POIApp
{
	public class POIListViewAdapter : BaseAdapter<PointOfInterest>
	{
		private readonly Activity _context;

		public POIListViewAdapter(Activity context)
		{
			_context = context;
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

			string addr = FormatAddr (poi);
			if (String.IsNullOrEmpty (addr))
				view.FindViewById<TextView>(Resource.Id.addrTextView).Visibility
				= ViewStates.Gone;
			else
				view.FindViewById<TextView> (Resource.Id.addrTextView).Text = addr;

			return view;
		}

		public override PointOfInterest this [int index] {
			get {
				return POIListActivity.POIDataService.POIs[index];
			}
		}

		protected string FormatAddr(PointOfInterest poi)
		{
			StringBuilder result = new StringBuilder ();

			if (string.IsNullOrEmpty (poi.AddressLine1)) {
				result.Append (poi.AddressLine1);
				result.Append (", ");
			}


			if (string.IsNullOrEmpty (poi.City)) {
				result.Append (poi.City);
				result.Append (", ");
			}


			if (string.IsNullOrEmpty (poi.PostalCode)) {
				result.Append (poi.AddressLine1);
			}

			return result.ToString ().TrimEnd (new[] { ',', ' ' });
		}
	}
}

