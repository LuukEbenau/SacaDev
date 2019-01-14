using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack;

namespace SacaDev.Api
{
    public static class ApiHelpers
    {
        public static JsonServiceClient AddHeaders(this JsonServiceClient client, Dictionary<string, string> headers)
        {
            foreach (var header in headers) client.AddHeader(header.Key, header.Value);
            return client;
        }
    }
}
