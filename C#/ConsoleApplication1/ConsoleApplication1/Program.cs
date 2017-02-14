using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var request = new RestRequest("api/v1/send/loki/fixture", Method.POST);
            var fixtureRequest = "{\"Test\": 1}";
            request.AddHeader("correlationId", Guid.NewGuid().ToString());
            request.RequestFormat = DataFormat.Json;
            request.AddBody(fixtureRequest);
            

            var client = new RestClient("http://localhost:54862");
            var response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                
            }
        }
    }
}
