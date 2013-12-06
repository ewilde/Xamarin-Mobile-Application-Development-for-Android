using System;
using System.Collections.Generic;
using Android.Graphics;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace POIApp
{
	public class POIJsonService : IPOIDataService
	{
		private readonly string storagePath;

		private readonly List<PointOfInterest> _pois = new List<PointOfInterest>();

		public POIJsonService (string storagePath)
		{
			this.storagePath = storagePath;
		}


		public void RefreshCache ()
		{
			_pois.Clear ();

			string[] filenames = Directory.GetFiles (storagePath, "*.json");

			foreach (string filename in filenames) {
				string poiString = File.ReadAllText (filename);
				PointOfInterest poi = JsonConvert.DeserializeObject<PointOfInterest> (poiString);
				_pois.Add (poi);
			}

		}

		public PointOfInterest GetPOI (int id)
		{
			return _pois.Find (p => p.Id == id);
		}

		public void SavePOI (PointOfInterest poi)
		{

			Boolean newPOI = false;
			if (poi.Id == -1) {
				poi.Id = GetNextId ();
				newPOI = true;
			}

			// serialize POI
			string poiString = JsonConvert.SerializeObject (poi);
			// write new file or overwrite existing file
			File.WriteAllText (GetFilename (poi.Id), poiString);

			// update cache if file save was successful
			if (newPOI)
				_pois.Add (poi);

		}

		public void DeletePOI (PointOfInterest poi)
		{
			// delete POI JSON file
			if (File.Exists(GetFilename (poi.Id)))
				File.Delete (GetFilename (poi.Id));

			// delete POI image file
			if (File.Exists(GetImageFilename (poi.Id)))
				File.Delete (GetImageFilename(poi.Id));

			// remove POI from cache
			_pois.Remove (poi);

		}

		public System.Collections.Generic.IReadOnlyList<PointOfInterest> POIs {
			get {
				return this._pois;
			}
		}

		private int GetNextId()
		{
			return _pois.Count == 0 ? 1 : _pois.Max (p => p.Id) + 1;
		}

		private string GetFilename(int? id)
		{
			return System.IO.Path.Combine(storagePath,"poi" + id.GetValueOrDefault().ToString() + ".json");
		}

		public string GetImageFilename (int id)
		{
			Java.IO.File picturesDir = new Java.IO.File(
				Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures),
					"POIApp");

			if (!picturesDir.Exists())
			{
				picturesDir.Mkdirs();
			}

			return System.IO.Path.Combine (picturesDir.Path, "poiimage" + id.ToString () + ".jpg");

		}

		public Android.Graphics.Bitmap GetPOIImage (int id)
		{
			if (File.Exists (GetImageFilename (id)))
				return BitmapFactory.DecodeFile (GetImageFilename (id));
			else
				return null;
		}

	}
}

