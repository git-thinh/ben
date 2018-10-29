using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace System
{

    public delegate void TranslateCallBack(oEN_TRANSLATE_GOOGLE_MESSAGE otran);

    //{"id":"event_855075b-4935-baa8-685e95443949","x":283,"y":263,"text":"above "}
    public class oEN_TRANSLATE_GOOGLE_MESSAGE
    {
        public bool success { set; get; }

        public string id { set; get; }

        public string text { set; get; }
        public string type { set; get; }
        public string mean_vi { set; get; }

        public int x { set; get; }
        public int y { set; get; }
        
        [JsonIgnore]
        public WebRequest webRequest { set; get; }

        [JsonIgnore]
        public TranslateCallBack translateCallBack { set; get; }

        public oEN_TRANSLATE_GOOGLE_MESSAGE()
        {
            this.success = false;
            this.webRequest = null;
            this.translateCallBack = null;

            this.mean_vi = string.Empty;
            this.type = string.Empty;
        }

        public oEN_TRANSLATE_GOOGLE_MESSAGE(WebRequest _webRequest, TranslateCallBack _translateCallBack)
        {
            this.success = false;
            this.webRequest = _webRequest;
            this.translateCallBack = _translateCallBack;

            this.mean_vi = string.Empty;
            this.type = string.Empty;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", text, mean_vi);
        }
    }
}
