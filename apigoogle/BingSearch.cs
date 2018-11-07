using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace apigoogle
{
    /*
     * https://duckduckgo.com/?q=english+due+to&t=h_&ia=web
     * https://yandex.com/search/?text=english%20due%20to&lr=10552
     * 
        https://azure.microsoft.com/en-us/services/cognitive-services/bing-web-search-api/ 
        Key 1: 306bbbbcd2af44e791d903c8769dbb8f
        Key 2: daaae6c01d704a77b8189e0ee47410c8
    */

    public class SearchResult
    {
        public string jsonResult;
        public Dictionary<string, string> relevantHeaders;
    }

    public class apiBingSearch
    {
        // Enter a valid subscription key.
        const string accessKey = "306bbbbcd2af44e791d903c8769dbb8f";
        /*
         * If you encounter unexpected authorization errors, double-check this value
         * against the endpoint for your Bing Web search instance in your Azure
         * dashboard.
         */
        const string uriBase = "https://api.cognitive.microsoft.com/bing/v7.0/search";
        const string searchTerm = "due to";

        public static string f_search()
        {
            SearchResult result = WebSearch(searchTerm);
            Console.WriteLine("\nRelevant HTTP Headers:\n");
            foreach (var header in result.relevantHeaders)
                Console.WriteLine(header.Key + ": " + header.Value);

            string s = JsonPrettyPrint(result.jsonResult);
            return s;
        }

        /// <summary>
        /// Makes a request to the Bing Web Search API and returns data as a SearchResult.
        /// </summary>
        static SearchResult WebSearch(string searchQuery)
        {
            // Construct the search request URI.
            var uriQuery = uriBase + "?q=" + Uri.EscapeDataString(searchQuery);

            // Perform request and get a response.
            WebRequest request = HttpWebRequest.Create(uriQuery);
            request.Headers["Ocp-Apim-Subscription-Key"] = accessKey;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string json = new StreamReader(response.GetResponseStream()).ReadToEnd();

            // Create a result object.
            SearchResult searchResult = new SearchResult()
            {
                jsonResult = json,
                relevantHeaders = new Dictionary<String, String>()
            };

            // Extract Bing HTTP headers.
            foreach (String header in response.Headers)
            {
                if (header.StartsWith("BingAPIs-") || header.StartsWith("X-MSEdge-"))
                    searchResult.relevantHeaders[header] = response.Headers[header];
            }

            return searchResult;
        }

        /// <summary>
        /// Formats the JSON string by adding line breaks and indents.
        /// </summary>
        /// <param name="json">The raw JSON string.</param>
        /// <returns>The formatted JSON string.</returns>
        static string JsonPrettyPrint(string json)
        {
            if (string.IsNullOrEmpty(json))
                return string.Empty;

            json = json.Replace(Environment.NewLine, "").Replace("\t", "");

            StringBuilder sb = new StringBuilder();
            bool quote = false;
            bool ignore = false;
            char last = ' ';
            int offset = 0;
            int indentLength = 2;

            foreach (char ch in json)
            {
                switch (ch)
                {
                    case '"':
                        if (!ignore) quote = !quote;
                        break;
                    case '\\':
                        if (quote && last != '\\') ignore = true;
                        break;
                }

                if (quote)
                {
                    sb.Append(ch);
                    if (last == '\\' && ignore) ignore = false;
                }
                else
                {
                    switch (ch)
                    {
                        case '{':
                        case '[':
                            sb.Append(ch);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', ++offset * indentLength));
                            break;
                        case ']':
                        case '}':
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', --offset * indentLength));
                            sb.Append(ch);
                            break;
                        case ',':
                            sb.Append(ch);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', offset * indentLength));
                            break;
                        case ':':
                            sb.Append(ch);
                            sb.Append(' ');
                            break;
                        default:
                            if (quote || ch != ' ') sb.Append(ch);
                            break;
                    }
                }
                last = ch;
            }

            return sb.ToString().Trim();
        }
    }
}

