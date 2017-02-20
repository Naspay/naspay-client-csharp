using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Naspay
{
    class Program
    {
        static void Main(string[] args)
        {
            string domain = "naspay.com";
            string apiKey = "apiKey";
            string apiSecret = "apiSecret";

            NaspayClient client = new NaspayClient(domain, apiKey, apiSecret, true);
            client.Authenticate();

            object map = client.DoApiRequest("/transactions", HttpMethod.Get, null);

            Console.ReadKey();
        }
    }
}
