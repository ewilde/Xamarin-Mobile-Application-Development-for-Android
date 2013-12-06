using System;
using System.Text;
using Newtonsoft.Json;

namespace POIApp
{
	public class PointOfInterest
	{
		public int Id { get; set;}
		public string Name { get; set; }
		public string Description { get; set; }
		public string AddressLine1 { get; set; }
		public string AddressLine2 { get; set; }
		public string City { get; set; }
		public string StateAbbr { get; set; }
		public string PostalCode { get; set; }
		public double? Latitude { get; set; }
		public double? Longitude { get; set; }

		[JsonIgnore]
		public string Address {
			get
			{
				StringBuilder result = new StringBuilder ();

				if (!string.IsNullOrEmpty (this.AddressLine1)) {
					result.AppendLine (this.AddressLine1);
					result.Append (", ");
				}

				if (!string.IsNullOrEmpty (this.AddressLine2)) {
					result.Append (this.AddressLine2);
					result.Append (", ");
				}

				if (!string.IsNullOrEmpty (this.City)) {
					result.AppendLine (this.City);
					result.Append (", ");
				}

				if (!string.IsNullOrEmpty (this.PostalCode)) {
					result.AppendLine (this.PostalCode);
					result.Append (", ");
				}

				return result.ToString ().TrimEnd(new[] {',', ' '});
			}

			set{
				var lines = value.Split (new[] {','}, StringSplitOptions.RemoveEmptyEntries);

				this.AddressLine1 = lines.Length > 0 ? lines[0].Trim() : string.Empty;
				this.AddressLine2 = lines.Length > 1 ? lines[1].Trim() : string.Empty;
				this.City = lines.Length > 2 ? lines[2].Trim() : string.Empty;
				this.PostalCode = lines.Length > 3 ? lines[3].Trim() : string.Empty;
			}
		}

		public PointOfInterest()
		{
			this.Id = -1;
		}
	}
}

