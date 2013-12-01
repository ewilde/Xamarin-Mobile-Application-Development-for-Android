using System;
using NUnit.Framework;
using POIApp;

namespace POITestApp
{
	[TestFixture]
	public class POITestFixture
	{
		IPOIDataService _poiService;


		[SetUp]
		public void Setup ()
		{
			string storagePath = Environment.GetFolderPath ( Environment.SpecialFolder.MyDocuments);
			_poiService = new POIJsonService(storagePath);
		}

		[TearDown]
		public void Tear ()
		{
		}

		[Test]
		public void RefreshCache  ()
		{
			this._poiService.RefreshCache();
		}

		[Test]
		public void SavePOI ()
		{
			this._poiService.SavePOI (new PointOfInterest());
		}

		[Test]
		public void GetPOI()
		{
			this._poiService.GetPOI(1);
		}

		[Test]
		public void DeletePOI()
		{
			this._poiService.DeletePOI (new PointOfInterest());
		}
	}
}

