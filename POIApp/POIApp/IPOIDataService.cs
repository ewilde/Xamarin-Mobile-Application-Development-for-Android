using System;
using System.Collections.Generic;
using Android.Graphics;

namespace POIApp
{
	public interface IPOIDataService
	{
		IReadOnlyList<PointOfInterest> POIs { get; }
		string GetImageFilename (int id);
		void RefreshCache();
		PointOfInterest GetPOI (int id);
		void SavePOI(PointOfInterest poi);
		void DeletePOI(PointOfInterest poi);
		Bitmap GetPOIImage (int id);
	}
}

