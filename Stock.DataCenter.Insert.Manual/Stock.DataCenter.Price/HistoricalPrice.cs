using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System;
using Newtonsoft.Json;
using Stock.Asx.DataCenter.EFCore.Model;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using Newtonsoft.Json.Converters;
using Stock.DataCenter.Others.Convertor;


namespace Stock.DataCenter.Prices
{
    public class HistoricalPrice
    {
        private readonly Dictionary<string, string> ApiKeyList;

        private const string URL_HISTORICAL_PRICE = "https://yahoo-finance15.p.rapidapi.com/api/yahoo/hi/history/{{company}}.AX/1d?diffandsplits=false";


        public static bool GetHistoricalPrices (ILogger logger)
        {
            Dictionary<string, int> ApiKeyList = new Dictionary<string, int>();
            ApiKeyList.Add("723f4c9fdamsh57710b7681839b0p19b820jsn5def7be0f91a", 0); //"jinzhuo1783@gmail.com"
            ApiKeyList.Add("5a62c931aamsh98b980fbe229663p1f5b2djsneb8e4b7ca9ef", 0); //"jinzhuo1784@gmail.com"
            ApiKeyList.Add("fed9dc9434mshd2c7402fcc8829cp145d7fjsn991160ebdc92", 0); //"jinzhuo1784@gmail.com"
            ApiKeyList.Add("5ccdfec5acmshd2cc817c258c10cp1ddb74jsn09b688f4a0ce", 0); //"jinzhuo1784@hotmail.com"
            
            var client = new HttpClient();
            
            var converter = new UnixDateTimeConverter();

            // Deserialize the JSON string into a MyObject instance
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(converter);

            using (var context = new CompanyContext())
            {
                var symbols = context.Companies.Select(c => c.Symbol).ToList();
                foreach (var symbol in symbols)
                {
                    logger.LogInformation($"Read historical price for company {symbol} ");

                    if (context.Prices.Any(p => p.Symbol == symbol))
                    {
                        logger.LogInformation("skip ........ ");
                        continue;
                    }
                        

                    logger.LogInformation($"Get Historical Price for company {symbol}");

                    var urlstr = URL_HISTORICAL_PRICE.Replace("{{company}}", symbol);

                    var apikey = ApiKeyList.OrderByDescending(k => k.Value).Last().Key;

                    var request = new HttpRequestMessage
                    {
	                    Method = HttpMethod.Get,
	                    RequestUri = new Uri(urlstr),
	                    Headers =
	                    {
		                    { "X-RapidAPI-Key", apikey},
		                    { "X-RapidAPI-Host", "yahoo-finance15.p.rapidapi.com" },
	                    },
                    };

                    ApiKeyList[apikey]++;

                    var response = client.SendAsync(request).GetAwaiter().GetResult();

                    var prices = new List<Price>();

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        var yhprices = JsonConvert.DeserializeObject<YahooPrice>(jsonString);
                        
                        if (yhprices?.Body != null)
                        {
                            prices = yhprices.Body.Select(p =>
                            new Price()
                            {
                                Symbol = symbol,
                                Date = p.Value?.Date ?? DateTime.Now,
                                Open = p.Value?.Open,
                                High = p.Value?.High,
                                Low = p.Value?.Low,
                                Close = p.Value?.Close,
                                Volumn = p.Value?.Volume,
                                CloseAdj = p.Value?.AdjClose,
                                UploadDate = DateTime.Now
                            }).ToList();
                        }
                        else
                        {
                            prices = new List<Price>() { new Price() { Date = DateTime.Now, Symbol = symbol, UploadDate = DateTime.Now} };
                        }
                        
                    }
                    else
                    {
                        logger.LogError($"unable to get price for company {symbol}. Insert blank record");
                        prices = new List<Price>() { new Price() { Date = DateTime.Now, Symbol = symbol, UploadDate = DateTime.Now} };
                    }

                    context.Prices.AddRange(prices);
                    context.SaveChanges();
                }

            }

            return true;
        }

        public class Meta
        {
            [JsonProperty("processedTime")]
            public DateTime ProcessedTime { get; set; }

            [JsonProperty("currency")]
            public string Currency { get; set; }

            [JsonProperty("symbol")]
            public string Symbol { get; set; }

            [JsonProperty("exchangeName")]
            public string ExchangeName { get; set; }

            [JsonProperty("fullExchangeName")]
            public string FullExchangeName { get; set; }

            [JsonProperty("instrumentType")]
            public string InstrumentType { get; set; }

            [JsonProperty("firstTradeDate")]
            public long FirstTradeDate { get; set; }

            [JsonProperty("regularMarketTime")]
            public long RegularMarketTime { get; set; }

            [JsonProperty("hasPrePostMarketData")]
            public bool HasPrePostMarketData { get; set; }

            [JsonProperty("gmtoffset")]
            public int Gmtoffset { get; set; }

            [JsonProperty("timezone")]
            public string Timezone { get; set; }

            [JsonProperty("exchangeTimezoneName")]
            public string ExchangeTimezoneName { get; set; }

            [JsonProperty("regularMarketPrice")]
            public double RegularMarketPrice { get; set; }

            [JsonProperty("fiftyTwoWeekHigh")]
            public double FiftyTwoWeekHigh { get; set; }

            [JsonProperty("fiftyTwoWeekLow")]
            public double FiftyTwoWeekLow { get; set; }

            [JsonProperty("regularMarketDayHigh")]
            public double RegularMarketDayHigh { get; set; }

            [JsonProperty("regularMarketDayLow")]
            public double RegularMarketDayLow { get; set; }

            [JsonProperty("regularMarketVolume")]
            public int RegularMarketVolume { get; set; }

            [JsonProperty("chartPreviousClose")]
            public double ChartPreviousClose { get; set; }

            [JsonProperty("priceHint")]
            public int PriceHint { get; set; }

            [JsonProperty("dataGranularity")]
            public string DataGranularity { get; set; }

            [JsonProperty("range")]
            public string Range { get; set; }

            [JsonProperty("version")]
            public string Version { get; set; }

            [JsonProperty("status")]
            public int Status { get; set; }

            [JsonProperty("copywrite")]
            public string Copywrite { get; set; }
        }

        public class BodyData
        {
            [JsonProperty("date")]
            [JsonConverter(typeof(YahooDateTimeConverter))]
            public DateTime Date { get; set; }

            [JsonProperty("date_utc")]
            public long DateUtc { get; set; }

            [JsonProperty("open")]
            public double? Open { get; set; }

            [JsonProperty("high")]
            public double? High { get; set; }

            [JsonProperty("low")]
            public double? Low { get; set; }

            [JsonProperty("close")]
            public double? Close { get; set; }

            [JsonProperty("volume")]
            public double? Volume { get; set; }

            [JsonProperty("adjclose")]
            public double? AdjClose { get; set; }
        }

        public class YahooPrice
        {

            [JsonProperty("body")]
            public Dictionary<string, BodyData> Body { get; set; }
        }
    }
}
