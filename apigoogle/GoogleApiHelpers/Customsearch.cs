using Google.Apis.Customsearch.v1;
using Google.Apis.Customsearch.v1.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;

namespace apigoogle
{
    /*
        Custom Search JSON API provides 100 search queries per day for free. 
        https://developers.google.com/custom-search/v1/overview?hl=en_US
        https://github.com/dewitt/opensearch/blob/master/opensearch-1-1-draft-6.md
        https://cse.google.com/cse/

        The Query element: Describes a specific search request that can be made by the search client.
        Attributes:
        * role - Contains a string identifying how the search client should interpret the search request defined by this Query element.
            Restrictions: See the role values specification for allowed role values.
            Requirements: This attribute is required.
        * title - Contains a human-readable plain text string describing the search request.
            Restrictions: The value must contain 256 or fewer characters of plain text. The value must not contain HTML or other markup.
            Requirements: This attribute is optional.
        * totalResults - Contains the expected number of results to be found if the search request were made.
            Restrictions: The value is a non-negative integer.
            Requirements: This attribute is optional.
        * searchTerms - Contains the value representing the searchTerms as a OpenSearch 1.1 parameter names.
            Restrictions: See the searchTerms parameter.
            Requirements: This attribute is optional.
        * count - Contains the value representing the count as a OpenSearch 1.1 parameter names.
            Restrictions: See the count parameter.
            Requirements: This attribute is optional.
        * startIndex - Contains the value representing the startIndex as an OpenSearch 1.1 parameter names.
            Restrictions: See the startIndex parameter.
            Requirements: This attribute is optional.
        * startPage - Contains the value representing the startPage as an OpenSearch 1.1 parameter names.
            Restrictions: See the startPage parameter.
            Requirements: This attribute is optional.
        * language - Contains the value representing the language as an OpenSearch 1.1 parameter names.
            Restrictions: See the language parameter.
            Requirements: This attribute is optional.
        * inputEncoding - Contains the value representing the inputEncoding as an OpenSearch 1.1 parameter names.
            Restrictions: See the inputEncoding parameter.
            Requirements: This attribute is optional.
        * outputEncoding - Contains the value representing the outputEncoding as an OpenSearch 1.1 parameter names.
            Restrictions: See the outputEncoding parameter.
            Requirements: This attribute is optional.

    */
    public class apiGoogle_Customsearch
    {
        const string apiKey = "AIzaSyCgIv5iBWlIDaS9HeibkD-fXDMV_F5-PHQ";
        const string cx = "017296066615265880816:fgnnuqdock8";

        public static string f_search1(string keyword)
        {
            string result = string.Empty;

            string apiKey = "YOUR KEY HERE";
            string cx = "YOUR CX HERE";
            string query = "YOUR SEARCH HERE";

            CustomsearchService svc = new CustomsearchService();
            svc.Key = apiKey;

            CseResource.ListRequest listRequest = svc.Cse.List(query);
            listRequest.Cx = cx;
            Search search = listRequest.Fetch();

            foreach (Result rs in search.Items)
            {
                Console.WriteLine("Title: {0}", rs.Title);
                Console.WriteLine("Link: {0}", rs.Link);
            }

            return result;
        }

        public static string f_search(string keyword)
        {
            string result = string.Empty;

            //https://www.googleapis.com/customsearch/v1?key=AIzaSyCgIv5iBWlIDaS9HeibkD-fXDMV_F5-PHQ&cx=017296066615265880816:fgnnuqdock8&q=english&alt=json

            WebClient webClient = new WebClient(); 
            result = webClient.DownloadString(String.Format("https://www.googleapis.com/customsearch/v1?key={0}&cx={1}&q={2}&alt=json", apiKey, cx, keyword));
            
            Dictionary<string, object> collection = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
            //foreach (Dictionary<string, object> item in (IEnumerable)collection["items"])
            //{
            //    Console.WriteLine("Title: {0}", item["title"]);
            //    Console.WriteLine("Link: {0}", item["link"]);
            //    Console.WriteLine();
            //}

            return result;
        }


        //public async Task Say(params string[] args)
        //{
        //    CustomsearchService service = new CustomsearchService(new BaseClientService.Initializer { ApiKey = Program.config.GoogleApiKey });
        //    SafeSearch = GetSafeSearchVal(); //Tries to grab from json settings or uses hardcoded value
        //    SearchNumber = 1; //Default value number of results to return
        //    string output = "";

        //    var search = HandleSearchRequest(service, args);
        //    output = FormatResultOutput(search, output);
        //    if (output.Trim().Length == 0)
        //    {
        //        await Context.Channel.SendMessageAsync("No Search Results Found.");
        //        return;
        //    }

        //    await Context.Channel.SendMessageAsync(output);
        //}

        //private Search HandleSearchRequest(CustomsearchService service, string[] args)
        //{
        //    string query_input = PrepareInput(args);
        //    ListRequest listRequest = service.Cse.List(query_input);
        //    listRequest.Cx = Program.config.GoogleSearchEngineID;
        //    listRequest.Safe = this.SafeSearch;
        //    Google.Apis.Customsearch.v1.Data.Search search = listRequest.Fetch();
        //    return search;
        //}



        public static string tittle(string querry)
        {

            const string apiKey = "YOUR_APP_KEY";
            const string searchEngineId = "YOUR_ENGINE_ID";
            string query = querry;
            string res = "oh,snap!";
            //////CustomsearchService customSearchService = new CustomsearchService(new Google.Apis.Services.BaseClientService.Initializer() { ApiKey = apiKey });
            //////Google.Apis.Customsearch.v1.CseResource.ListRequest listRequest = customSearchService.Cse.List(query);
            //////listRequest.Cx = searchEngineId;
            //////Search search = listRequest.Fetch();
            //////foreach (var item in search.Items)
            //////{
            //////    res = item.Title;
            //////    break;
            //////}
            return res;
        }

        public static string linkk(string querry)
        {

            const string apiKey = "YOUR_APP_KEY";
            const string searchEngineId = "YOUR_ENGINE_ID";
            string query = querry;
            string res = "oh,snap!";
            //////CustomsearchService customSearchService = new CustomsearchService(new Google.Apis.Services.BaseClientService.Initializer() { ApiKey = apiKey });
            //////Google.Apis.Customsearch.v1.CseResource.ListRequest listRequest = customSearchService.Cse.List(query);
            //////listRequest.Cx = searchEngineId;
            //////Search search = listRequest.Fetch();
            //////foreach (var item in search.Items)
            //////{

            //////    res = item.Link;
            //////    break;
            //////}
            return res;
        }

    }
}
