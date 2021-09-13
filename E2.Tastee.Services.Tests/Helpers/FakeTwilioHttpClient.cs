using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Twilio.Http;

namespace E2.Tastee.Services.Tests.Helpers
{
    public class FakeTwilioHttpClient : Twilio.Http.HttpClient
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string _responseJson;
        private bool _sendCalled;
        public Response _response;
        public Request _request;

        public FakeTwilioHttpClient(HttpStatusCode statusCode, string responseJson)
        {
            _statusCode = statusCode;
            _responseJson = responseJson;
            _sendCalled = false;
        }

        public bool SendCalled => _sendCalled;
        public Response Response => _response;
        public Request Request => _request;

        public override Response MakeRequest(Request request)
        {
            _request = request;
            _sendCalled = true;
            _response = new Response(_statusCode, _responseJson);
            return _response;
        }

        public override Task<Response> MakeRequestAsync(Request request)
        {
            return Task.FromResult(MakeRequest(request));
        }
    }
}