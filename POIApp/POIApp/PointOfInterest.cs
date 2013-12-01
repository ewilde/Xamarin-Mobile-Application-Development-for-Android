using System;

namespace POIApp
{
	public class PointOfInterest
	{
		public int? Id { get; set;}
		public string Name { get; set; }
		public string Description { get; set; }
		public string AddressLine1 { get; set; }
		public string AddressLine2 { get; set; }
		public string City { get; set; }
		public string StateAbbr { get; set; }
		public string PostalCode { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }

	}
}

