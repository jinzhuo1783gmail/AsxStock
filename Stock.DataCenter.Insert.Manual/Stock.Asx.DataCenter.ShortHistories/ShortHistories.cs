using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stock.Asx.DataCenter.EFCore;
using Stock.Asx.DataCenter.EFCore.Model;
using Stock.Asx.DataCenter.ShortHistories.AsxCompanyGroup;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using File = System.IO.File;

namespace Stock.Asx.DataCenter.ShortHistories
{
    public static class ShortHistories
    {
        public static void DownloadAndInsertShort (ILogger logger)
        {
            try
            {
                string url = "https://www.asx.com.au/data/shortsell.txt";
                string filePath = "shortsell" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";

                DownloadFile(url, filePath);

                using (var context = new CompanyContext())
                {
                    ParseAndInsertData(filePath, context, logger);
                }

                File.Delete(filePath);

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw new Exception(ex.Message);
            }   
        }
        
        public static bool DownloadFile(string url, string filePath)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(url, filePath);
            }
            return true;
        }

        public static void ParseAndInsertData(string filePath, CompanyContext context, ILogger logger)
        {
            using (var reader = new StreamReader(filePath))
            {
                DateTime shortdate = new DateTime();

                int lineCount = 0;
                int addItem = 0;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lineCount++;

                    string pattern = @"reported for (\d{2}-[A-Za-z]{3}-\d{4})";
                    Match match = Regex.Match(line, pattern);

                    bool dataStart = false;
            
                    if (lineCount == 1 && match.Success)
                    {
                        string dateStr = match.Groups[1].Value;
                        logger.LogInformation($"Short date: {dateStr}");
                        shortdate = DateTime.ParseExact(dateStr, "dd-MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    }

                    if (lineCount >= 9)
                    {
                        var parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        var symbol = parts[0];
                        
                
                        var companyName = string.Join(" ", parts.Skip(1).TakeWhile(part => part != "FPO" && part != "CDI"));
                        var productClass = parts.Contains("FPO") ? "FPO" : "CDI";
                        var totalShort = long.Parse(parts[parts.Length - 3].Replace(",", ""));
                        var totalIssued = long.Parse(parts[parts.Length - 2].Replace(",", ""));
                        var percentage = decimal.Parse(parts[parts.Length - 1]);



                        var shortHistory = new ShortHistory
                        {
                            Symbol = symbol,
                            CompanyName = companyName,
                            ProductClass = productClass,
                            TotalIssued = totalIssued,
                            TotalShort = totalShort,
                            Percentage = percentage,
                            ShortDate = shortdate.Date, // Adjust the date as needed
                            UploadDate = DateTime.Now.Date
                        };

                        if (!context.ShortHistories.Any(s => s.ShortDate.Date == shortHistory.ShortDate.Date && s.Symbol == shortHistory.Symbol))
                        {
                            context.ShortHistories.Add(shortHistory);
                            addItem++;
                            logger.LogInformation($"Add short Company : {symbol}");
                        }
                        else
                        {
                            logger.LogWarning($"Duplicated record found in the DB Company : : {symbol} Short Date : {shortdate.Date}");
                        }
                
                    }
                }

                if (addItem > 0 ) 
                { 
                    context.SaveChanges();
                    logger.LogInformation("Data inserted successfully.");
                }
                else
                {
                    logger.LogError($"Nothing been added based on the file date of {shortdate.Date}");
                }
            }
        }

        public static void CheckAndAppendCompany(ILogger logger)
        {
            using (var context = new CompanyContext())
            {
                var companiesFromShortList = context.ShortHistories
                    .Select(s => new Company() { 
                        Symbol = s.Symbol, 
                        CompanyName = s.CompanyName, 
                        TotalShares = s.TotalIssued ?? 0,
                        UploadDate = DateTime.Now 
                    })
                    .GroupBy(company => company.Symbol)
                    .Select(group => group.First())
                    .ToList();
                
                var companyExist = context.Companies.Select(c => c.Symbol).ToList() ?? new List<string>();

                var companiesToInsert = companiesFromShortList.Where(c => !companyExist.Contains(c.Symbol)).ToList();



                if (companiesToInsert.Any())
                {
                    context.AddRange(companiesToInsert);
                    context.SaveChanges (); 
                }
            }
        }

        public static void CheckAndAmendExistingCompanyInformation(ILogger logger)
        {

            var clientAsx = new HttpClient();
            clientAsx.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var context = new CompanyContext())
            {
                var companies = context.Companies.Select(c => c.Symbol).ToList();

                foreach (var symbol in companies)
                {
                    logger.LogInformation($"update company {symbol}");
                    var company = GetCompanyFromAsxApi(symbol, clientAsx, context, logger).GetAwaiter().GetResult();
                    
                    if (company != null)
                    {
                        context.Companies.Update(company);
                        context.SaveChanges();
                        logger.LogInformation($"update company {symbol} completed");
                    }
                }
            }
        }

        public static async Task<Company> GetCompanyFromAsxApi(string symbol, HttpClient client, CompanyContext context, ILogger logger)
        {

            var companyUrl = "https://www.asx.com.au/asx/1/company/" + symbol + "?fields=primary_share,latest_annual_reports,last_dividend,primary_share.indices";
            var existCompany = context.Companies.FirstOrDefault(c => c.Symbol == symbol);

            try
            {
                HttpResponseMessage response = client.GetAsync(companyUrl).GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();

                    AsxCompany company;
                    try
                    {
                        company = JsonConvert.DeserializeObject<AsxCompany>(jsonString);
                    }
                    catch
                    {
                        response = client.GetAsync(companyUrl).GetAwaiter().GetResult();
                        jsonString = await response.Content.ReadAsStringAsync();
                        company = JsonConvert.DeserializeObject<AsxCompany>(jsonString);
                    }

                    if ((company != null) && (
                        existCompany.Catergory != company?.IndustryGroupName ||
                        existCompany.SectorName != company?.SectorName ||
                        existCompany.TotalShares != company?.PrimaryShare?.NumberOfShares ||
                        existCompany.LastPrice != company?.PrimaryShare.LastPrice))
                    {
                        if (string.IsNullOrEmpty(company.IndustryGroupName)) 
                        {
                            existCompany.Catergory = company.IndustryGroupName;
                        }
                        
                        if (string.IsNullOrEmpty(company.SectorName)) 
                        {
                            existCompany.SectorName = company.SectorName;
                        }

                        if (company.PrimaryShare?.NumberOfShares != null && company.PrimaryShare?.NumberOfShares >0)
                        {
                            existCompany.TotalShares = company.PrimaryShare.NumberOfShares;
                        }

                        if (company.PrimaryShare?.LastPrice != null && company.PrimaryShare?.LastPrice > 0)
                        {
                            existCompany.LastPrice = company.PrimaryShare.LastPrice;
                        }
                        return existCompany;

                    }

                    return null;

                }

                else
                {
                    logger.LogError($"unable to get asx company infor for company {symbol}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"unable to get asx company infor for company {symbol} {ex.Message}");
                return null;
            }
        }

        public static void InsertOrUpdateCompanyFromAsxApi(ILogger logger)
        {
            try
            {
                string url = "https://www.asx.com.au/asx/research/ASXListedCompanies.csv";
                string filePath = "ASXListedCompanies" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";

                DownloadFile(url, filePath);
            
                using (var context = new CompanyContext())
                {
                    ParseAndInsertCompanyData(filePath, context, logger);
                }

                File.Delete(filePath);
            }

            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw new Exception(ex.Message);
            }   

        }

        public static void ParseAndInsertCompanyData(string filePath, CompanyContext context, ILogger logger)
        {
            using (var reader = new StreamReader(filePath))
            {

                int lineCount = 0;
                int addItem = 0;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lineCount++;

                    if (lineCount >= 4)
                    {
                        var parts = line.Split(",", StringSplitOptions.RemoveEmptyEntries);
                        var symbol = parts[1].Trim('"');
            
    
                        var companyName = parts[0].Trim('"');
                        var IndustryGroup = parts[2].Trim('"');
            


                        var company = new Company
                        {
                            Symbol = symbol, 
                            CompanyName = companyName, 
                            TotalShares = 0,
                            Catergory = IndustryGroup,
                            UploadDate = DateTime.Now 
                        };

                        if (!context.Companies.Any(c => c.Symbol == symbol))
                        {
                            context.Companies.Add(company);
                            addItem++;
                            logger.LogInformation($"Add Company : {symbol}");
                        }
                        else
                        {
                            logger.LogWarning($"Duplicated record found in the DB Company : : {symbol}");
                        }
    
                    }
                }

                if (addItem > 0 ) 
                { 
                    context.SaveChanges();
                    logger.LogInformation("Data inserted successfully.");
                }
                else
                {
                    logger.LogError($"No Company been added");
                }
            }
        }    
    }
}
