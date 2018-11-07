using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace apigoogle
{
    public class GeoCodeProvider
    {
        public GeoLocation GetGeoCode(string address)
        {
            string url = string.Format("http://maps.googleapis.com/maps/api/geocode/json?address={0}&sensor=false", address);
            
            //   "json?address=1600+Amphitheatre+Parkway,+Mountain+View,+CA&sensor=false";
            var response = RequestHelper.Get(url);
            GeoResponse geoResponse = JsonConvert.DeserializeObject<GeoResponse>(response);

            if (geoResponse.Status == "OK")
            {
                foreach (var geoMetry in geoResponse.Results)
                {
                    return geoMetry.Geometry.Location;
                }
            }

            return null;
        }
    }

    public class GeoLocation
    {
        public decimal Lat { get; set; }
        public decimal Lng { get; set; }
    }

    public class GeoGeometry
    {
        public GeoLocation Location { get; set; }
    }

    public class GeoResult
    {
        public GeoGeometry Geometry { get; set; }
    }

    public class GeoResponse
    {
        public string Status { get; set; }
        public GeoResult[] Results { get; set; }
    }
}