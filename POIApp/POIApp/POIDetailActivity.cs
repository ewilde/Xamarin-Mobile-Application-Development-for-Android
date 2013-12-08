using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Locations;
using Android.Content.PM;
using Android.Provider;

namespace POIApp
{
	[Activity (Label = "POIDetailActivity")]			
	public class POIDetailActivity : Activity, ILocationListener
	{
		const int CAPTURE_PHOTO = 0;

		EditText _nameEditText;
		EditText _descrEditText;
		EditText _addrEditText;
		EditText _latEditText;
		EditText _longEditText;
		Button _updateLocation;
		Button _mapButton;
		ImageView _poiImageView;
		LocationManager _locMgr;

		int _poiId;
		PointOfInterest _poi;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.POIDetail);

			_nameEditText = FindViewById<EditText> (Resource.Id.nameEditText);
			_descrEditText = FindViewById<EditText> (Resource.Id.descrEditText);
			_addrEditText = FindViewById<EditText> (Resource.Id.addrEditText);
			_latEditText = FindViewById<EditText> (Resource.Id.latEditText);
			_longEditText = FindViewById<EditText> (Resource.Id.longEditText);
			_poiImageView = FindViewById<ImageView> (Resource.Id.poiImageView);
			_updateLocation = FindViewById<Button> (Resource.Id.updateLocationButton);
			_mapButton = FindViewById<Button> (Resource.Id.mapButton);
			_mapButton.Click += MapClicked;
			_updateLocation.Click += GetLocationClicked;
			_poiImageView.Click += NewPhotoClicked;
			_locMgr = (LocationManager)GetSystemService (Context.LocationService);
			_poiId = Intent.GetIntExtra ("poiId", -1);
			if (_poiId == -1) {
				_poi = new PointOfInterest ();
			} else {
				_poi = POIListActivity.POIDataService.GetPOI (_poiId);
			}


			this.UpdateUI ();
		}

		protected void UpdateUI()
		{
			_nameEditText.Text = _poi.Name;
			_descrEditText.Text = _poi.Description;
			_addrEditText.Text = _poi.Address;
			_latEditText.Text = _poi.Latitude.ToString ();
			_longEditText.Text = _poi.Longitude.ToString ();
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.POIListDetailMenu, menu);
			return base.OnCreateOptionsMenu(menu);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId)
			{
			case Resource.Id.actionSave:
				SavePOI ();
				return true;

			case Resource.Id.actionDelete: 
				DeletePOI ();
				return true;
			default :
				return base.OnOptionsItemSelected(item);
			}
		}

		public override bool OnPrepareOptionsMenu (IMenu menu)
		{
			base.OnPrepareOptionsMenu (menu);

			// disable delete for a new POI
			if (_poi.Id == -1) {
				IMenuItem item = menu.FindItem (Resource.Id.actionDelete);
				item.SetEnabled (false);
			}

			return true;
		}

		protected void SavePOI()
		{
			_poi.Name = _nameEditText.Text;
			_poi.Description = _descrEditText.Text;
			_poi.Address = _addrEditText.Text;
			_poi.Latitude = Double.Parse(_latEditText.Text);
			_poi.Longitude = Double.Parse(_longEditText.Text);

			bool errors = false;

			if (String.IsNullOrEmpty (_nameEditText.Text)) {
				_nameEditText.Error = "Name cannot be empty";
				errors = true;
			}
			else
				_nameEditText.Error = null;


			if (errors) {
				return;
			}

			POIListActivity.POIDataService.SavePOI (_poi);
			Toast toast = Toast.MakeText (this, String.Format ("{0} saved.", _poi.Name), ToastLength.Short);
			toast.Show();

			Finish ();
		}

		protected void ConfirmDelete(object sender, EventArgs e)
		{
			POIListActivity.POIDataService.DeletePOI (_poi);
			Toast toast = Toast.MakeText (this, String.Format ("{0} deleted.", _poi.Name), ToastLength.Short);
			toast.Show();
			Finish ();
		}

		protected void DeletePOI()
		{
			AlertDialog.Builder alertConfirm =
				new AlertDialog.Builder(this);
			alertConfirm.SetCancelable(false);
			alertConfirm.SetPositiveButton("OK", ConfirmDelete);
			alertConfirm.SetNegativeButton("Cancel", delegate {});
			alertConfirm.SetMessage(String.Format("Are you sure you want to delete {0}?", _poi.Name));
			alertConfirm.Show();
		}


		void GetLocationClicked (object sender, EventArgs e)
		{

			Criteria criteria = new Criteria();
			criteria.Accuracy = Accuracy.NoRequirement;
			criteria.PowerRequirement = Power.NoRequirement;

			_locMgr.RequestSingleUpdate (criteria, this, null);
		}

		public void OnLocationChanged (Location location)
		{
			_latEditText.Text = location.Latitude.ToString();
			_longEditText.Text = location.Longitude.ToString ();

			Geocoder geocdr = new Geocoder(this.ApplicationContext);
			IList<Address> addresses = geocdr.GetFromLocation (location.Latitude, location.Longitude, 5);

			if (addresses.Any()) {
				UpdateAddressFields (addresses [0]);
			}
		}

		protected void UpdateAddressFields(Address addr)
		{
			if (String.IsNullOrEmpty(_nameEditText.Text))
				_nameEditText.Text = addr.FeatureName;

			for (int i = 0; i < addr.MaxAddressLineIndex; i++) {
				if (!String.IsNullOrEmpty(_addrEditText.Text))
					_addrEditText.Text += ", ";
				_addrEditText.Text += addr.GetAddressLine (i);
			}
		}

		public void MapClicked(object sender, EventArgs e)
		{
			Android.Net.Uri geoUri;
			if (String.IsNullOrEmpty (_addrEditText.Text)) {
				geoUri = Android.Net.Uri.Parse (String.Format ("geo:{0},{1}", _poi.Latitude, _poi.Longitude));
			}
			else {
				geoUri = Android.Net.Uri.Parse (String.Format ("geo:0,0?q={0}", _addrEditText.Text));
			}

			Intent mapIntent = new Intent (Intent.ActionView, geoUri);
			StartActivity (mapIntent);

		}

		private Java.IO.File _imageFile;
		public void NewPhotoClicked(object sender, EventArgs e)
		{
			if (_poi.Id == -1) {
				AlertDialog.Builder alertConfirm = new AlertDialog.Builder(this);
				alertConfirm.SetCancelable(false);
				alertConfirm.SetPositiveButton("OK", delegate {});
				alertConfirm.SetMessage(String.Format("You must save the POI prior to attaching a photo.", _poi.Name));
				alertConfirm.Show ();
			}
			else {

				Intent cameraIntent = new Intent (MediaStore.ActionImageCapture);

				PackageManager packageManager = PackageManager;
				IList<ResolveInfo> activities = packageManager.QueryIntentActivities(cameraIntent, 0);
				if (activities.Count == 0) {
					AlertDialog.Builder alertConfirm = new AlertDialog.Builder(this);
					alertConfirm.SetCancelable(false);
					alertConfirm.SetPositiveButton("OK", delegate {});
					alertConfirm.SetMessage(String.Format("No camera app available to capture photos.", _poi.Name));
					alertConfirm.Show ();
				}
				else {
					_imageFile = new Java.IO.File (POIListActivity.POIDataService.GetImageFilename (_poi.Id));
					cameraIntent.PutExtra (MediaStore.ExtraOutput, Android.Net.Uri.FromFile (_imageFile));
					//cameraIntent.PutExtra (MediaStore.ExtraSizeLimit, 1.5 * 1024);
					StartActivityForResult (cameraIntent, CAPTURE_PHOTO);
				}
			}
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			if (requestCode == CAPTURE_PHOTO) {

				if (resultCode == Result.Ok) {
					// load image from temp file
					var _poiImage = BitmapFactory.DecodeFile (_imageFile.Path);

					// display saved image
					_poiImageView.SetImageBitmap (_poiImage);
				}
				else {
					// let the user know the photo was cancelled
					Toast toast = Toast.MakeText (this, "No picture captured.", ToastLength.Short);
					toast.Show();
				}
			}
			else
				base.OnActivityResult (requestCode, resultCode, data);
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
	}
}

