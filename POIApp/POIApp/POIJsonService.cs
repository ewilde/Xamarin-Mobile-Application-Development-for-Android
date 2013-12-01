using System;
using System.Collections.Generic;
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
			throw new NotImplementedException ();
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
			File.WriteAllText (GetFilename (poi.Id.Value), poiString);

			// update cache if file save was successful
			if (newPOI)
				_pois.Add (poi);

		}

		public void DeletePOI (PointOfInterest poi)
		{
			throw new NotImplementedException ();
		}

		public System.Collections.Generic.IReadOnlyList<PointOfInterest> POIs {
			get {
				throw new NotImplementedException ();
			}
		}

		private int GetNextId()
		{
			return _pois.Max (p => p.Id.GetValueOrDefault()) + 1;
		}

		private string GetFilename(int id)
		{
			return Path.Combine(_storagePath,"poi" + id.ToString() + ".json");
		}

	}
}

