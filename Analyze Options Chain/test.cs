using System;
using System.Collections.Generic;
using System.Text;

namespace Analyze_Options_Chain
{
    class test
    {
        //public async static Task<OptionChain> FetchOptionChain()
        //{
        //    try
        //    {
        //        HttpClient httpClient = new HttpClient();
        //        var optionsdata = await httpClient.GetAsync("https://www.nseindia.com/api/option-chain-indices?symbol=NIFTY");
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        throw;
        //    }


        //    return new OptionChain();
        //}

        //static async Task<string> GetURI(Uri u)
        //{ // var t = Task.Run(() => GetURI(new Uri("https://www.nseindia.com/api/option-chain-indices?symbol=NIFTY")));
        //  //// var t = Task.Run(() => GetURI(new Uri("https://jsonplaceholder.typicode.com/todos/1")));
        //  // t.Wait();

        //    var response = string.Empty;
        //    using (var client = new HttpClient())
        //    {
        //        // client.
        //        //  HttpResponseMessage result = await client.GetAsync(u);
        //        HttpRequestMessage mess = new HttpRequestMessage();
        //        mess.Method = HttpMethod.Get;
        //        mess.RequestUri = u;
        //        client.DefaultRequestHeaders.Accept.Add(
        //            new MediaTypeWithQualityHeaderValue("application/json"));

        //        HttpResponseMessage result = await client.SendAsync(mess);
        //        if (result.IsSuccessStatusCode)
        //        {
        //            response = await result.Content.ReadAsStringAsync();
        //        }
        //    }
        //    return response;
        //}
    }
}
