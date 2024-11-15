using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System;
using Azure;
using Newtonsoft.Json;
using Stock.Asx.DataCenter.EFCore.Model;
using System.Collections.Generic;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf;
using Microsoft.EntityFrameworkCore;
using Stock.Asx.DataCenter.EFCore;

namespace Stock.DataCenter.Announcements
{
    public static class Announcements
    {

        private const string URL = "https://www.asx.com.au/asx/1/company/{{company}}/announcements?count=20&market_sensitive={{senstive}}";
        public static async Task<bool> DownloadAndInsertAnnoucnements(ILogger logger)
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
                        logger.LogInformation($"Processing Announcement for company {symbol}");

                        var announcementsNew = await GetReleaseAnnouncement(symbol, clientAsx, context, logger);
                        //foreach (var announcement in announcementsNew)
                        //{
                        //    if (!string.IsNullOrEmpty(announcement.FileURL))
                        //    {
                        //        var textAnn = await ExtractTextFromPdfUrlAsync(announcement.FileURL, clientAsx);
                        //        announcement.FileContent = StringCompressor.CompressString(textAnn);
                        //    }
                        //}

                        if (announcementsNew.Any()) 
                        { 
                            context.Announcements.AddRange(announcementsNew);
                            context.SaveChanges();

                            logger.LogInformation($"Successfully add announcement number {announcementsNew.Count}");
                        }
                        
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return false;
            }
            
        }

        public static async Task<string> ExtractTextFromPdfUrlAsync(string url, HttpClient client)
        {
            
            // Download the PDF file
            byte[] pdfBytes = client.GetByteArrayAsync(url).GetAwaiter().GetResult();

            // Load the PDF file from the downloaded bytes
            using (MemoryStream ms = new MemoryStream(pdfBytes))
            using (PdfReader reader = new PdfReader(ms))
            using (PdfDocument pdfDoc = new PdfDocument(reader))
            {
                var strategy = new SimpleTextExtractionStrategy();
                var text = new System.Text.StringBuilder();

                // Extract text from each page
                for (int i = 1; i <= pdfDoc.GetNumberOfPages(); ++i)
                {
                    var page = pdfDoc.GetPage(i);
                    string pageContent = PdfTextExtractor.GetTextFromPage(page, strategy);
                    text.Append(pageContent);
                }

                return text.ToString();
            }
            
        }

        public static async Task<List<Announcement>> GetReleaseAnnouncement(string symbol, HttpClient client, CompanyContext context, ILogger logger)
        {

            var sensitiveTrue = URL.Replace("{{company}}", symbol).Replace("{{senstive}}", "true");
            var sensitiveFalse = URL.Replace("{{company}}", symbol).Replace("{{senstive}}", "false");

            var existDocumentIds = context.Announcements.AsNoTracking().Where(a => a.Symbol == symbol).Select(a => a.AsxDocumentId).ToList();

            var mergedCompanyAnnouncement = new List<AnnoucementViewModel>();

            try
            {
                foreach (var url in new List<string>() { sensitiveTrue, sensitiveFalse })
                {
                    HttpResponseMessage response = client.GetAsync(sensitiveTrue).GetAwaiter().GetResult();
                    RootViewModel rootObject;
                    string jsonString;
                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            jsonString = await response.Content.ReadAsStringAsync();
                            rootObject = JsonConvert.DeserializeObject<RootViewModel>(jsonString);
                        }
                        catch
                        {
                            response = client.GetAsync(url).GetAwaiter().GetResult();
                            jsonString = await response.Content.ReadAsStringAsync();
                            rootObject = JsonConvert.DeserializeObject<RootViewModel>(jsonString);
                        }

                        mergedCompanyAnnouncement.AddRange(rootObject?.Data ?? new List<AnnoucementViewModel>());
                    }
                    else
                    {
                        logger.LogError($"unable to get announcement for company {symbol}");
                        return new List<Announcement>();
                    }
                }
            }
            catch (Exception ex)
            { 
                logger.LogError($"exception happend during annoucnement retrieveing for company {symbol} Reason {ex.Message} {ex.InnerException}");
                //return new List<Announcement>();
            }
            

            if (!mergedCompanyAnnouncement.Any())
            {
                return new List<Announcement>();
            }

            var anns = mergedCompanyAnnouncement.GroupBy(a => a.Id).Select(g => g.First())
                .Select(av => new Announcement() { 
                    AsxDocumentId = av.Id,
                    Symbol = symbol,
                    FileURL = av.Url,
                    FileText = string.Empty,
                    FileSummary1 = string.Empty,
                    FileSummary2 = string.Empty,
                    FileSummary3 = string.Empty,
                    ReleaseDate = av.DocumentReleaseDate,
                    UploadDate = DateTime.Now
                }).ToList();


            anns = anns.Where(a => !existDocumentIds.Contains(a.AsxDocumentId)).ToList();

            // remove duplicated
            return anns;
        }
    }
}
