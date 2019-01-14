using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ServiceStack;

namespace SacaDev.Api
{
    public abstract class RestApiBase<TRoute>
    {
        protected string ApiLocation { get; }
        public string this[TRoute path] => ApiLocation + ApiPaths[path];
        public string GetPath(TRoute path) => ApiPaths[path];
        public string GetFullPath(TRoute path) => ApiLocation + ApiPaths[path];
        protected abstract Dictionary<TRoute, string> ApiPaths { get; }
        protected abstract Dictionary<string, string> Headers { get; }
        protected abstract IList<RequestInterceptionObject> RequestIntercepts { get; }
        public RestApiBase(string apiRoot)
        {
			ApiLocation = apiRoot;
			//TODO: refactor without servicestack
		}

        private JsonServiceClient ApiClient {
            get {
                var client = new JsonServiceClient();
                client.AddHeaders(Headers);
                return client;
            }
        }
        #region api call methods

        protected async Task<bool> InterceptResponse(HttpWebResponse response)
        {
            if (RequestIntercepts == null)
				return false;
            var sc = response.StatusCode;
            if (!RequestIntercepts.Any(ri => ri.StatusCode == sc))
				return false;
            var interceptObj = RequestIntercepts.First(ri => ri.StatusCode == sc);

            await interceptObj.CodeToRun.Invoke(response: response);
            if (!interceptObj.RetryRequest)
				return false;
            //retry
            return true;
        }

		protected Task<HttpWebResponse> Post(string url) => _post(url, null, null, 0);
		protected Task<HttpWebResponse> Post(string url, object formData) => _post(url, formData, null, 0);
		protected Task<HttpWebResponse> Post(string url, object formData,InterceptDelegate intercept) => _post(url, formData, intercept, 0);
		protected Task<HttpWebResponse> Post(string url, object formData, InterceptDelegate intercept, int retryCount) => _post(url, formData, intercept, retryCount);
		/// <summary>
		/// Base Post Class, call this if adding a new version of the post method.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="formData">The form data.</param>
		/// <returns></returns>
		private async Task<HttpWebResponse> _post(string url, object formData, InterceptDelegate requestIntercept, int retrycount, Dictionary<string,string> headers = null, int startRetryIndex = 0)
        {
            using (var client = ApiClient) {
                using (var resp = await client.PostAsync<HttpWebResponse>(url, formData)) {
                    await requestIntercept?.Invoke(resp);
                    if (await InterceptResponse(response: resp)) {
                        //this is to prevent endless loops, TODO: change the flow to a pipeline structure for more flexebilety? would be easily possible of we change to another rest client.
                        if (startRetryIndex < retrycount)//TODO: think this out further, max limit of retries, should onlt be 1 for auth, but poor connection may change this mb?
                        {
                            return await _post(url, formData, requestIntercept, ++startRetryIndex);
                        }
                    }
                    return resp;
                }
            }
        }

        protected async Task<HttpWebResponse> _get(string url)
        {
            using (var client = ApiClient) {
                var resp = await client.GetAsync<HttpWebResponse>(url);
                await InterceptResponse(response: resp);
                return resp;
            }
        }
        #endregion
    }
}