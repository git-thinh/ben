using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web; 

namespace apigoogle
{
    public interface IDistanceCalculator
    {
        /// <summary>
        /// Calculates the distance in meters.
        /// </summary>
        /// <param name="origin">street numer city.</param>
        /// <param name="destination">street numer city.</param>
        int CalculateDistance(string origin, string destination);
    }

    /// <summary>
    /// https://developers.google.com/maps/documentation/distancematrix/
    /// </summary>
    public class GoogleMapsDistanceCalculator : IDistanceCalculator
    {
        /// <summary>
        /// Calculates the distance in meters.
        /// </summary>
        /// <param name="origin">street numer city.</param>
        /// <param name="destination">street numer city.</param>
        public int CalculateDistance(string origin, string destination)
        {
            string url = string.Format("https://maps.googleapis.com/maps/api/distancematrix/json?") +
                string.Format("origins={0}", origin.Replace(" ", "+")) +
                "&" +
                string.Format("destinations={0}", destination.Replace(" ", "+")) +
                "&" +
                "sensor=false";
            
            var response = RequestHelper.Get(url);
            var distanceMatrix = JsonConvert.DeserializeObject<DistanceMatrix>(response);
                
            foreach (var row in distanceMatrix.Rows)
	        {
                foreach (var element in row.Elements)
	            {
                    if (element.Distance != null)
                    {
                        return int.Parse(element.Distance.Value);
                    }
	            }
	        }

            return 0;
        }

        private class DistanceMatrix
        {
            public string Status { get; set; }
            public string[] Origin_addresses { get; set; }
            public string[] Destination_addresses { get; set; }
            public Row[] Rows { get; set; }
        }

        private class Row
        {
            public Element[] Elements { get; set; }
        }

        private class Element
        {
            public string Status { get; set; }
            public TextValue Duration { get; set; }
            public TextValue Distance { get; set; }
        }

        private class TextValue
        {
            public string Text { get; set; }
            public string Value { get; set; }
        }
    }

}