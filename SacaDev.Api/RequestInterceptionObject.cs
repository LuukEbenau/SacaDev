using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SacaDev.Api
{
    public class RequestInterceptionObject
    {
        public HttpStatusCode StatusCode { get; private set; }
        public InterceptDelegate CodeToRun { get; private set; }
        public bool RetryRequest { get; set; }
        public RequestInterceptionObject(HttpStatusCode code,InterceptDelegate codeToRun,bool retryRequest)
        {
            this.StatusCode = code;
            this.CodeToRun = codeToRun;
            this.RetryRequest = retryRequest;
        }
    }

    public delegate Task InterceptDelegate(HttpWebResponse response);
}
