using Flurl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stock.Asx.DataCenter.EFCore;
using Stock.Asx.DataCenter.EFCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using YahooFinanceApi;
using static System.Net.WebRequestMethods;

namespace Stock.DataCenter.Prices
{
    public static class PriceQuote
    {
        public static bool GetQuote(ILogger logger)
        {
            try
            {
                using (var context = new CompanyContext())
                {
                    var allCompaniesSymbols = context.Companies.OrderBy(c => c.Symbol).Select(c => c.Symbol).ToList();
                    var clientAsx = new HttpClient();
                    clientAsx.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    foreach (var symbol in allCompaniesSymbols)
                    {
                        logger.LogInformation($"Processing Quote for company {symbol}");

                        var price = GetQuoteFromYahooApi(symbol, context, logger).GetAwaiter().GetResult();

                        if (price != null && price.Id == 0)
                        {
                            context.Add(price);
                            logger.LogInformation($"Successfully add quote for company {symbol}");
                            
                        }
                        else if (price != null && price.Id > 0)
                        {
                            context.Prices.Update(price);
                            logger.LogInformation($"Successfully update quote for company {symbol}");
                       }

                        
                    }

                    context.SaveChanges();
                    logger.LogInformation($"Successfully add/update quote");
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return false;
            }
        }

        public static async Task<Price> GetQuoteFromYahooApi(string symbol, CompanyContext context, ILogger logger)
        {
            var existPrice = context.Prices.OrderBy(p => p.Date).LastOrDefault(a => a.Symbol == symbol);

            bool update = true;
            if (existPrice == null || existPrice.Date.Date < DateTime.Now.Date) 
            {
                update = false;
            }

            try
            {
                var securities = await Yahoo.Symbols($"{symbol}.AX").Fields(Field.Symbol, Field.RegularMarketOpen, Field.RegularMarketDayHigh,Field.RegularMarketVolume, Field.RegularMarketDayLow, Field.RegularMarketPrice, Field.FiftyTwoWeekHigh).QueryAsync();
                var sec = securities[$"{symbol}.AX"];

                if (update)
                {
                    existPrice.Close = sec[Field.RegularMarketPrice];
                    existPrice.CloseAdj = sec[Field.RegularMarketPrice];
                    existPrice.Open = sec[Field.RegularMarketOpen];
                    existPrice.High = sec[Field.RegularMarketDayHigh];
                    existPrice.Low = sec[Field.RegularMarketDayLow];
                    existPrice.Volumn = sec[Field.RegularMarketVolume];
                    existPrice.UploadDate = DateTime.Now;

                    return existPrice;
                }
                else
                {
                    return new Price()
                    {
                        Date = DateTime.Now.Date,
                        Symbol = symbol,
                        Close = sec[Field.RegularMarketPrice],
                        CloseAdj = sec[Field.RegularMarketPrice],
                        Open = sec[Field.RegularMarketOpen],
                        High = sec[Field.RegularMarketDayHigh],
                        Low = sec[Field.RegularMarketDayLow],
                        Volumn = sec[Field.RegularMarketVolume],
                        UploadDate = DateTime.Now

                    };
                }
                
            }
            catch (Exception ex)
            {
                logger.LogError($"exception happend during price retrieveing for company {symbol} Reason {ex.Message} {ex.InnerException}");
                //return new List<Announcement>();
            }

            return null;

        }


        public static async Task<Price> GetQuoteFromAsxApi(string symbol, HttpClient client, CompanyContext context, ILogger logger)
        {

            var url = "https://www.asx.com.au/asx/1/share/" + symbol;


            var existPrice = context.Prices.OrderBy(p => p.Date).LastOrDefault(a => a.Symbol == symbol);

            bool update = true;
            if (existPrice == null || existPrice.Date.Date < DateTime.Now.Date) 
            {
                update = false;
            }


            try
            {
                HttpResponseMessage response = client.GetAsync(url).GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {

                    var jsonString = await response.Content.ReadAsStringAsync();

                    StockPriceBaseInfo sp;
                    try
                    {
                        sp = JsonConvert.DeserializeObject<StockPriceBaseInfo>(jsonString);
                    }
                    catch
                    {
                        response = client.GetAsync(url).GetAwaiter().GetResult();
                        jsonString = await response.Content.ReadAsStringAsync();
                        sp = JsonConvert.DeserializeObject<StockPriceBaseInfo>(jsonString);
                    }

                    if (update)
                    {
                        existPrice.Close = sp.LastPrice;
                        existPrice.CloseAdj = sp.LastPrice;
                        existPrice.Open = sp.OpenPrice;
                        existPrice.High = sp.DayHighPrice;
                        existPrice.Low = sp.DayLowPrice;
                        existPrice.Volumn = sp.Volume;
                        existPrice.UploadDate = DateTime.Now;

                        return existPrice;
                    }
                    else
                    {
                        return new Price()
                        {
                            Date = DateTime.Now.Date,
                            Symbol = symbol,
                            Close = sp.LastPrice,
                            CloseAdj = sp.LastPrice,
                            Open = sp.OpenPrice,
                            High = sp.DayHighPrice,
                            Low = sp.DayLowPrice,
                            Volumn = sp.Volume,
                            UploadDate = DateTime.Now

                        };
                    }
                }
                else
                {
                    logger.LogError($"unable to get price for company {symbol}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"exception happend during price retrieveing for company {symbol} Reason {ex.Message} {ex.InnerException}");
                //return new List<Announcement>();
            }

            return null;
        }

        public static bool CalculateInvestmentMovement(ILogger logger)
        {
            Dictionary<string, double> invetmentFLow = new Dictionary<string, double>();
            Dictionary<string, double> invetmentWithSubCatFLow = new Dictionary<string, double>();

            var divider = "|||";

            try
            {
                using (var context = new CompanyContext())
                {
                    var companys = context.Companies.AsNoTracking().OrderBy(c => c.Symbol).ToList();

                    foreach (var company in companys)
                    {

                        if (string.IsNullOrEmpty(company.Catergory)) {
                            logger.LogWarning($"company {company.Symbol} belongs no industry");
                            continue;
                        }

                        logger.LogInformation($"process company {company.Symbol} notional  difference");
                        var priceLast2Days = context.Prices.Where(p => p.Symbol == company.Symbol).OrderByDescending(p => p.Date).Take(2).ToList();
                        if (priceLast2Days.Any(p => p.Date.Date == DateTime.Now.Date))
                        {
                            var differenceInPrice = priceLast2Days.First().Close - priceLast2Days.Last().Close;
                            var differenceInNotional = company.TotalShares * differenceInPrice ?? 0;
                            var dictkey = company.Catergory;
                            if (invetmentFLow.ContainsKey(dictkey))
                            {
                                invetmentFLow[dictkey] += differenceInNotional;
                            }
                            else
                            {
                                invetmentFLow[dictkey] = differenceInNotional;
                            }

                            if (!string.IsNullOrEmpty(company.SubCatergory) && company.SubCatergory != "UnSet")
                            {
                                dictkey = company.SubCatergory;

                                if (invetmentWithSubCatFLow.ContainsKey(dictkey))
                                {
                                    invetmentWithSubCatFLow[dictkey] += differenceInNotional;
                                }
                                else
                                {
                                    invetmentWithSubCatFLow[dictkey] = differenceInNotional;
                                }
                            }
                        }
                        else
                        {
                            logger.LogError($"Company {company.Symbol} has to today's price skip calciulation");
                        }



                    }
                
                    foreach (var invFlow in  invetmentFLow)
                    {
                        var singleif = new SectorIndustryInvestment()
                        {
                            SectorName = string.Empty,
                            Catergory = invFlow.Key,
                            SubCatergory = string.Empty,
                            InvestmentInOut = Math.Round(invFlow.Value, 2),
                            UploadDate = DateTime.Now,
                        };

                        context.SectorIndustryInvestmentFlowInOut.Add(singleif);
                    }

                    foreach (var invFlow in  invetmentWithSubCatFLow)
                    {

                        var singleif = new IndustrySubCatInvestmentFlowInOut()
                        {
                            SectorName = string.Empty,
                            Catergory = string.Empty,
                            SubCatergory = invFlow.Key,
                            InvestmentInOut = Math.Round(invFlow.Value, 2),
                            UploadDate = DateTime.Now,
                        };

                        context.IndustrySubCatInvestmentsFlowInOut.Add(singleif);
                    }


                    context.SaveChanges();

                    logger.LogInformation("finish invesitgate investment flow");

                }

                return true;
            }
            catch  (Exception ex)
            {
                logger.LogError(ex.Message);
                return false;
            }
            
        }
    }

    public class StockPriceBaseInfo
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("isin_code")]
        public string IsinCode { get; set; }

        [JsonProperty("desc_full")]
        public string DescFull { get; set; }

        [JsonProperty("last_price")]
        public double LastPrice { get; set; }

        [JsonProperty("open_price")]
        public double OpenPrice { get; set; }

        [JsonProperty("day_high_price")]
        public double DayHighPrice { get; set; }

        [JsonProperty("day_low_price")]
        public double DayLowPrice { get; set; }

        [JsonProperty("change_price")]
        public double ChangePrice { get; set; }

        [JsonProperty("change_in_percent")]
        public string ChangeInPercent { get; set; }

        [JsonProperty("volume")]
        public double Volume { get; set; }

        [JsonProperty("bid_price")]
        public double BidPrice { get; set; }

        [JsonProperty("offer_price")]
        public double OfferPrice { get; set; }

        [JsonProperty("previous_close_price")]
        public double PreviousClosePrice { get; set; }

        [JsonProperty("previous_day_percentage_change")]
        public string PreviousDayPercentageChange { get; set; }

        [JsonProperty("year_high_price")]
        public double YearHighPrice { get; set; }

        [JsonProperty("last_trade_date")]
        public DateTime LastTradeDate { get; set; }

        [JsonProperty("year_high_date")]
        public DateTime YearHighDate { get; set; }

        [JsonProperty("year_low_price")]
        public double YearLowPrice { get; set; }

        [JsonProperty("year_low_date")]
        public DateTime YearLowDate { get; set; }

        [JsonProperty("year_open_price")]
        public double YearOpenPrice { get; set; }

        [JsonProperty("year_open_date")]
        public DateTime YearOpenDate { get; set; }

        [JsonProperty("year_change_price")]
        public double YearChangePrice { get; set; }

        [JsonProperty("year_change_in_percentage")]
        public string YearChangeInPercentage { get; set; }

        [JsonProperty("pe")]
        public double Pe { get; set; }

        [JsonProperty("eps")]
        public double Eps { get; set; }

        [JsonProperty("average_daily_volume")]
        public long AverageDailyVolume { get; set; }

        [JsonProperty("annual_dividend_yield")]
        public double AnnualDividendYield { get; set; }

        [JsonProperty("market_cap")]
        public long MarketCap { get; set; }

        [JsonProperty("number_of_shares")]
        public long NumberOfShares { get; set; }

        [JsonProperty("deprecated_market_cap")]
        public long DeprecatedMarketCap { get; set; }

        [JsonProperty("deprecated_number_of_shares")]
        public long DeprecatedNumberOfShares { get; set; }

        [JsonProperty("suspended")]
        public bool Suspended { get; set; }
    }
}
