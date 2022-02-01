using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Analyze_Options_Chain.Models;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Analyze_Options_Chain
{
    class Program
    {
        static double Nifty = 0;
        static Dictionary<string, string> OurStrikes;
        static OptionChain optionChain = readfromfile();
        private static double? fivedayDownSideAvg = 0;
        private static double? fivedayUPsideAvg = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello Anvi! Let's Analyze Options chain.....");

            Console.WriteLine("\n");


            var CurrentExpiry = getCurrentExpiry(optionChain, 0);
            var NiftyValue = getNiftyValue(optionChain);
            Nifty = Convert.ToDouble(NiftyValue);
            Console.WriteLine("\n");
            FetchHistory();

            int i = 0;
            int j = 1;
            if ((Convert.ToDateTime(CurrentExpiry) - DateTime.Now).Days + 1 < 2)
            {
                i = 1;
                // j = 2;
            }

            var maxExpiries = i + 2;

            for (; i <= maxExpiries; i++)
            {
                OurStrikes = new Dictionary<string, string>();
                ExecProcess(getCurrentExpiry(optionChain, i), i + j);
            }


            Console.ReadLine();
        }

        static void ExecProcess(string currentExpiry, int iteration = 1)
        {
            var daysTillCurrentExpiry = (Convert.ToDateTime(currentExpiry) - DateTime.Now).Days + 1;
            Console.WriteLine(
                $"                                                           {currentExpiry} week suggestions --Days till Expiry Includes Weekends {daysTillCurrentExpiry}");
            Console.WriteLine(
                "------------------------------------------------------------------------------------------------------------------------------------------------------------------");

            if (fivedayDownSideAvg > Constants.StrikePriceDiff &&
                ((fivedayDownSideAvg - Constants.StrikePriceDiff) > 30))
            {
                SuggestByPriceDifference(currentExpiry, daysTillCurrentExpiry, Convert.ToDouble(fivedayDownSideAvg), false);
            }

            if (fivedayUPsideAvg > Constants.StrikePriceDiff &&
                ((fivedayUPsideAvg - Constants.StrikePriceDiff) > 30))
            {
                SuggestByPriceDifference(currentExpiry, daysTillCurrentExpiry, Convert.ToDouble(fivedayUPsideAvg), false);
            }


            SuggestByPriceDifference(currentExpiry, daysTillCurrentExpiry, Constants.StrikePriceDiff, false);

            // Console.WriteLine("####                    ####                   ####                 ####");
            Console.WriteLine("\n");

            SuggestByPriceDifference(currentExpiry, daysTillCurrentExpiry, 100, false);
            Console.WriteLine("\n");
            SuggestByPriceDifference(currentExpiry, daysTillCurrentExpiry, 80, false);

            if (daysTillCurrentExpiry > 7)
            {

               // SuggestByPriceDifference(currentExpiry, daysTillCurrentExpiry, Constants.StrikePriceDiff, false);


                //var diffabove10 = daysTillCurrentExpiry - 10;
                //if (diffabove10 > 0)
                //{
                //    var NiftyDiff1 = Nifty - (diffabove10 * 50);
                //    NiftyDiff1 = NiftyDiff1 - (3 * 80);
                //    var NiftyDiff = NiftyDiff1 - (7 * 130);
                //    double n = Math.Floor(NiftyDiff / 50) * 50;
                //    var price = getPriceofStrike(n.ToString(), optionChain, currentExpiry, "PE", false);
                //    Console.WriteLine(
                //        $"                                                        suggestions based on our 50,80 logic:-");
                //    Console.WriteLine("\n");
                //    Console.WriteLine($"Puts- Strike Price {n.ToString()} Value is {price} ");
                //    var Diffvalue = Math.Round(n - Nifty);
                //    Console.WriteLine($"Daily Break-up is:- {Diffvalue / daysTillCurrentExpiry}");
                //}
                //else
                //{
                //    var NiftyDiff2 = Nifty + (diffabove10 * 100);
                //    var NiftyDiff = NiftyDiff2 - (7 * 130);
                //    double n = Math.Floor(NiftyDiff / 50) * 50;
                //    var price = getPriceofStrike(n.ToString(), optionChain, currentExpiry, "PE", false);
                //}
            }




            Console.WriteLine("\n");
            Console.WriteLine(
                $"                                                        suggestions based on our OPTIONS LTP:- {Constants.OptionPrice} ");
            Console.WriteLine("\n");
            OurStrikes = new Dictionary<string, string>();

            var putStrike = getStrikeofprice(optionChain, currentExpiry, "PE", Constants.OptionPrice,
                daysTillCurrentExpiry);
            var callStrike = getStrikeofprice(optionChain, currentExpiry, "CE", Constants.OptionPrice,
                daysTillCurrentExpiry);
            double value = 0;
            foreach (var item in OurStrikes)
            {
                if (Convert.ToDouble(item.Key) > Nifty)
                {
                    value = Math.Round(Convert.ToDouble(item.Key) - Nifty);
                    Console.WriteLine($"Calls- Strike Price {item.Key} Value is {item.Value} ");
                    Console.WriteLine($"Call is at {value} points away");
                    Console.WriteLine($"Daily Break-up is:- {value / daysTillCurrentExpiry}");
                    Console.WriteLine("\n");
                }
                else
                {
                    value = Math.Round(Nifty - Convert.ToDouble(item.Key));
                    Console.WriteLine($"Puts-  Strike Price {item.Key} Value is {item.Value} ");
                    Console.WriteLine($"Put is at {value} points away");
                    Console.WriteLine($"Daily Break-up is:- {value / daysTillCurrentExpiry}");
                    Console.WriteLine("\n");
                }
            }

            //  Console.WriteLine("####                    ####                   ####                 ####");
            //Console.WriteLine("\n");
            SuggestByPriceDifference(currentExpiry, daysTillCurrentExpiry, 500 * iteration, true);

            //  Console.WriteLine("-----------------------------------------------------------------------------------------");
            //  Console.WriteLine("Open Interest");
            var f = CalculateMaxOI(currentExpiry, "PE", optionChain);

        }

        static void SuggestByPriceDifference(string currentExpiry, int daysTillCurrentExpiry, double strikePriceDiff,
            bool noHeading, int price = 1)
        {
            if (!noHeading)
            {
                //Console.WriteLine(
                //    $"                                                           {currentExpiry} week suggestions --Days till Expiry Includes Weekends {daysTillCurrentExpiry}");
                //Console.WriteLine(
                //    "------------------------------------------------------------------------------------------------------------------------------------------------------------------");
            }

            strikePriceDiff = strikePriceDiff * price;

            Console.WriteLine(
                $"                                                        suggestions based on our Strike Price Difference:- {strikePriceDiff} ");
            OurStrikes = new Dictionary<string, string>();
            var putPrice = getPriceofStrike(getPutStrikePrice(daysTillCurrentExpiry, strikePriceDiff).ToString(),
                optionChain, currentExpiry, "PE");
            var callPrice = getPriceofStrike(getCallStrikePrice(daysTillCurrentExpiry, strikePriceDiff).ToString(),
                optionChain, currentExpiry, "CE");

            //if (putPrice > callPrice)
            //{
            //    Console.WriteLine("Suggesting Market in Up Trend");
            //}

            Console.WriteLine("\n");
            foreach (var item in OurStrikes)
            {
                // Console.WriteLine("\n");
                if (Convert.ToDouble(item.Key) > Nifty)
                {
                    Console.WriteLine($"Calls Strike Price {item.Key} Value is {item.Value} ");
                    if (item.Value == "0")
                    {
                        bool isTrue = true;
                        while (isTrue)
                        {
                            strikePriceDiff = strikePriceDiff - 30;
                            var NewcallPrice = getCallStrikePrice(daysTillCurrentExpiry, strikePriceDiff).ToString();
                            callPrice = getPriceofStrike(NewcallPrice,
                                optionChain, currentExpiry, "CE", false);
                            if (callPrice > 0)
                            {
                                Console.WriteLine(
                                    $"Calls Strike Price with {strikePriceDiff} difference at {NewcallPrice} Value is {callPrice} ");
                                isTrue = false;
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Puts  Strike Price {item.Key}  Value is {item.Value} ");
                    if (item.Value == "0")
                    {
                        bool isTrue = true;
                        while (isTrue)
                        {
                            strikePriceDiff = strikePriceDiff - 30;
                            var NewputPrice = getPutStrikePrice(daysTillCurrentExpiry, strikePriceDiff).ToString();
                            callPrice = getPriceofStrike(NewputPrice,
                                optionChain, currentExpiry, "PE", false);
                            if (callPrice > 0)
                            {
                                // Console.WriteLine("\n");
                                Console.WriteLine(
                                    $"Puts Strike Price with {strikePriceDiff} difference at {NewputPrice} Value is {callPrice} ");
                                isTrue = false;
                            }
                        }
                    }
                }
                //  Console.WriteLine("\n");
                //  Console.WriteLine("\n");
            }
        }

        static OptionChain readfromfile()
        {
            OptionChain optionChain =
                JsonConvert.DeserializeObject<OptionChain>(File.ReadAllText(@"c:\Code\movie.json"));
            return optionChain;
        }

        static string getCurrentExpiry(OptionChain optionChain, int expiry)
        {
            var expDate = optionChain.Records.ExpiryDates[expiry];
            Console.WriteLine($"Next Expiry Date is {expDate}");
            DateTime dt = Convert.ToDateTime(expDate);
            return expDate;
        }

        static string getNiftyValue(OptionChain optionChain)
        {
            var NiftyValue = optionChain.Records.underlyingValue;
            Console.WriteLine($"Nifty is at {NiftyValue}");
            return NiftyValue;
        }

        static double getPutStrikePrice(int daysTillCurrentExpiry, double strikePriceDiff)
        {
            double multiplier = 0;
            if (strikePriceDiff < 500)
            {
                multiplier = (daysTillCurrentExpiry * strikePriceDiff);
            }
            else
            {
                multiplier = strikePriceDiff;
            }

            var Putstrike = Nifty - multiplier;
            double n = Math.Floor(Putstrike / 50) * 50;
            return n;
        }

        static double getCallStrikePrice(int daysTillCurrentExpiry, double strikePriceDiff)
        {
            double multiplier = 0;
            if (strikePriceDiff < 500)
            {
                multiplier = (daysTillCurrentExpiry * strikePriceDiff);
            }
            else
            {
                multiplier = strikePriceDiff;
            }

            var Putstrike = Nifty + multiplier;
            double n = Math.Floor(Putstrike / 50) * 50;
            return n + 50;
        }

        static double getPriceofStrike(string strikePrice, OptionChain optionChain, string expiryDate,
            string PEorCE, bool addToDict = true)
        {
            var contract = optionChain.Records.Data
                .Where(x => x.ExpiryDate == expiryDate && x.StrikePrice == strikePrice).SingleOrDefault();

            if (contract != null)
            {
                var price = contract.CE.LastPrice;

                if (PEorCE == "PE")
                {
                    price = contract.PE?.LastPrice;
                }
                else if (PEorCE == "CE")
                {
                    price = contract.CE?.LastPrice;
                }

                if (addToDict)
                {
                    OurStrikes.Add(strikePrice, price);
                }

                return Convert.ToDouble(price);
            }
            else
            {
                return 0;
            }

        }

        static string getStrikeofprice(OptionChain optionChain, string expiryDate, string PEorCE,
            double strikePrice, int daysTillCurrentExpiry)
        {
            string strike = "";
            Data contract = null;

            if (PEorCE == "PE")
            {
                contract = optionChain.Records.Data.First(x =>
                    x.ExpiryDate == expiryDate && Convert.ToDouble(x.PE?.LastPrice) >= strikePrice);
                strike = contract.PE.StrikePrice;
                strikePrice = Convert.ToDouble(contract.PE.LastPrice);
                OurStrikes.Add(strike, strikePrice.ToString());
                if (checkIfPriceDiffIsLessThan100(strike, daysTillCurrentExpiry))
                {
                    foreach (var strikeContract in optionChain.Records.Data
                        .Where(x => x.ExpiryDate == expiryDate && x.PE != null &&
                                    Convert.ToDouble(x.PE?.StrikePrice) > Nifty).OrderBy(x => x.PE?.StrikePrice)
                        .Select(x => x.PE))
                    {
                        if (!checkIfPriceDiffIsLessThan100(strikeContract.StrikePrice, daysTillCurrentExpiry))
                        {
                            OurStrikes.TryAdd(strikeContract.StrikePrice, strikeContract.LastPrice);
                            break;
                        }


                    }
                }
            }
            else if (PEorCE == "CE")
            {
                contract = optionChain.Records.Data
                    .Where(x => x.ExpiryDate == expiryDate && Convert.ToDouble(x.CE?.LastPrice) >= strikePrice)
                    .OrderBy(x => Convert.ToDouble(x.CE?.LastPrice)).First();
                strike = contract.CE.StrikePrice;
                strikePrice = Convert.ToDouble(contract.CE.LastPrice);
                OurStrikes.Add(strike, strikePrice.ToString());
                if (checkIfPriceDiffIsLessThan100(strike, daysTillCurrentExpiry))
                {
                    // OurStrikes.Add(strike, strikePrice.ToString());
                    foreach (var strikeContract in optionChain.Records.Data
                        .Where(x => x.ExpiryDate == expiryDate && x.CE != null &&
                                    Convert.ToDouble(x.CE?.StrikePrice) > Nifty).OrderBy(x => x.CE?.StrikePrice)
                        .Select(x => x.CE))
                    {
                        if (strikeContract != null)
                        {
                            if (!checkIfPriceDiffIsLessThan100(strikeContract.StrikePrice, daysTillCurrentExpiry))
                            {
                                OurStrikes.TryAdd(strikeContract.StrikePrice, strikeContract.LastPrice);
                                break;
                            }
                        }

                    }
                }
            }

            return strike;
        }

        static bool checkIfPriceDiffIsLessThan100(string strikePrice, int daysTillCurrentExpiry)
        {
            double value = 0;
            value = Math.Round(Convert.ToDouble(strikePrice) - Nifty);
            var priceDiff = value / daysTillCurrentExpiry;
            if (priceDiff < 0)
                priceDiff = priceDiff * -1;
            return priceDiff < 100;
        }

        static string CalculateMaxOI(string ExpiryDate, string PEorCE, OptionChain optionChain)
        {
            string maxOI = "";
            string strike = "";
            if (PEorCE == "PE")
            {
                maxOI = optionChain.Records.Data.Where(x => x.PE?.ExpiryDate == ExpiryDate)
                    .Select(x => x.PE.OpenInterest).Max();

                strike = optionChain.Records.Data.Where(x => x.PE?.OpenInterest == maxOI)
                    .Select(x => x.PE.StrikePrice).First();
            }

            return maxOI;
        }

        static void FetchHistory()
        {
            try
            {
                Console.WriteLine("Last 10 days History Of Nifty!!!");
              
               History his = FetchHistoryWithDays(12);

                        var count = his.chart.Result.FirstOrDefault().timestamp.Length;

                        var downSideValues = new List<int>();
                        var upSideValues = new List<int>();
                        
                        for (int i = 1; i < count; i++)
                        {
                            var date = DateTimeOffset
                                .FromUnixTimeSeconds(Convert.ToInt64(his.chart.Result.FirstOrDefault().timestamp[i]))
                                .DateTime;
                            if (his.chart.Result.FirstOrDefault().Indicators.quote.FirstOrDefault().close[i] != null)
                            {
                                var value = (Convert.ToInt64(his.chart.Result.FirstOrDefault().Indicators.quote.FirstOrDefault().close[i].Split('.')[0]) - Convert.ToInt64(his.chart.Result.FirstOrDefault().Indicators.quote.FirstOrDefault().close[i - 1].Split('.')[0])).ToString().Split('.')[0];
                              var  price =Convert.ToInt32(value);
                                if (price > 0)
                                {
                                    upSideValues.Add(price);
                                }
                                else
                                {
                                    downSideValues.Add(price);
                                }

                                Console.WriteLine(
                                    $"{date.ToString("MMMM dd")}: {his.chart.Result.FirstOrDefault().Indicators.quote.FirstOrDefault().close[i].Split('.')[0]} ::: " +
                                    $" {value}                                                                                                          {date.ToString("MMMM dd")}:     Low To High ::: {(Convert.ToInt64(his.chart.Result.FirstOrDefault().Indicators.quote.FirstOrDefault().high[i].Split('.')[0]) - Convert.ToInt64(his.chart.Result.FirstOrDefault().Indicators.quote.FirstOrDefault().low[i].Split('.')[0])).ToString().Split('.')[0]}");
                            }
                        }

                        Console.WriteLine("\n");
                //Average of last 10 days.
                        //Console.WriteLine($"DownSide Average  { downSideValues.Average().ToString()}");
                        //Console.WriteLine($"UpSide Average  { upSideValues.Average().ToString()}");

                        Console.WriteLine("\n");
                        Console.WriteLine("Average of Last 5 days");

                        fivedayDownSideAvg = -(downSideValues.TakeLast(5)?.Average());
                        fivedayUPsideAvg = (upSideValues.TakeLast(5)?.Average());



                        Console.WriteLine($"DownSide Average  {fivedayDownSideAvg}");
                        Console.WriteLine($"UpSide Average  {fivedayUPsideAvg}");
                        Console.WriteLine("\n");

                        History his30 = FetchHistoryWithDays(30);
                        Console.WriteLine($" Last 10 days Low:-  { his.chart.Result.FirstOrDefault()?.Indicators.quote.Min(x => x.low).Min().Split('.')?[0]}    :::    Last 30 days Low:-  { his30.chart.Result.FirstOrDefault()?.Indicators.quote.Min(x => x.low).Min().Split('.')?[0]}");
                        Console.WriteLine($"Last 10 days High:-  {his.chart.Result.FirstOrDefault()?.Indicators.quote.Max(x => x.high).Max().Split('.')?[0]}    :::    Last 30 days High:- {his30.chart.Result.FirstOrDefault()?.Indicators.quote.Max(x => x.high).Max().Split('.')?[0]}");
                        Console.WriteLine("\n");
                        
                     
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        static History FetchHistoryWithDays(int days)
        {
            History his = new History();
               var today = DateTime.UtcNow - new DateTime(1970, 1, 1);
            var secondsSinceEpoch = (int)today.TotalSeconds;

            var daysback = DateTime.UtcNow.AddDays(-days) - new DateTime(1970, 1, 1);
            var secondsSinceEpochfivedaysback = (int)daysback.TotalSeconds;

            var str =
                "https://query2.finance.yahoo.com/v8/finance/chart/%5ENSEI?formatted=true&crumb=INcfekLoIWS&lang=en-US&region=US&includeAdjustedClose=true&interval=1d&" +
                $"period1={secondsSinceEpochfivedaysback}&period2={secondsSinceEpoch}" +
                "&events=capitalGain%7Cdiv%7Csplit&useYfid=true&corsDomain=finance.yahoo.com";
            using var client = new HttpClient();
            var result = client.GetAsync(new Uri(str)).Result;
            if (result.IsSuccessStatusCode)
            {
                var response = result.Content.ReadAsStringAsync().Result;
                his = JsonConvert.DeserializeObject<History>(response);
            }

            return his;
        }

        static async Task<string> PostURI(Uri u, HttpContent c)
        {
            var response = string.Empty;
            using (var client = new HttpClient())
            {
                HttpResponseMessage result = await client.PostAsync(u, c);
                if (result.IsSuccessStatusCode)
                {
                    response = result.StatusCode.ToString();
                }
            }

            return response;
        }
    }
}
