using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace client
{
    public class Class1
    {
        public async Task Get()
        {
            
            var a = new GpioApiClient("http://localhost:5000/", new HttpClient());
           var ab = await a.GpiosAsync();
        }
    }
}
