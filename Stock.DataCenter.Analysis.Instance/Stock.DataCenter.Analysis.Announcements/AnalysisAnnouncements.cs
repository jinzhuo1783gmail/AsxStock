using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stock.Asx.DataCenter.EFCore;
using System.Net.Http.Headers;
using Microsoft.IdentityModel.Tokens;
using Stock.Asx.DataCenter.EFCore.Model;

namespace Stock.DataCenter.Analysis.Announcements
{
    public class AnalysisAnnouncements
    {
        private CompanyContext _contextCompany;
        private AnalysisContext _contextAnalysis;
        private ILogger<AnalysisAnnouncements> _logger;

        private HttpClient _clientAsx;
                    
        public AnalysisAnnouncements(CompanyContext contextCompany, AnalysisContext contextAnalysis, ILogger<AnalysisAnnouncements> logger)
        {
            _contextCompany = contextCompany;
            _contextAnalysis = contextAnalysis;
            _logger = logger;

            _clientAsx = new HttpClient();
            _clientAsx.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task SyncAnnouncements()
        {
            try
            {
                var anaylysisAnnouncementExist = await _contextAnalysis.Announcements.AsNoTracking().Select(a => a.AsxDocumentId).ToListAsync();
                var companyAnnouncement = await _contextCompany.Announcements.AsNoTracking().ToListAsync();
                var anaylysisAnnouncementNotExist = companyAnnouncement.Where(a => !anaylysisAnnouncementExist.Contains(a.AsxDocumentId)).ToList();    
                
                foreach (var announcement in anaylysisAnnouncementNotExist)
                {
                    announcement.Id = 0;
                    await _contextAnalysis.Announcements.AddAsync(announcement);
                    _logger.LogInformation($"Company {announcement.Symbol} with announcement asx id {announcement.AsxDocumentId} synced");
                }
                if (anaylysisAnnouncementNotExist.Any()) {
                    await _contextAnalysis.SaveChangesAsync();
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unable to sync announcements"  +ex.Message);
            }
        }



        public async Task ConvertAnnouncementsIntoText()
        {
            const int batchSize = 100; // Adjust the batch size as needed
            int offset = 0;

            while (true)
            {

                var announcementsBatch = await _contextAnalysis.Announcements
                    .AsNoTracking()
                    .OrderBy(a => a.Id) // Ensure consistent ordering
                    .Skip(offset)
                    .Take(batchSize)
                    .ToListAsync();

                if (announcementsBatch.Count == 0)
                {
                    break; // Exit the loop if no more records are available
                }

                foreach (var announcement in announcementsBatch)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(announcement.FileURL))
                        {
                            _logger.LogInformation($"Process {announcement.Symbol} announcement pressure released on {announcement.ReleaseDate:yyyy-MM-dd} with ASX doc id {announcement.AsxDocumentId}");

                            var textAnn = await ExtractTextFromPdfUrlAsync(announcement.FileURL, _clientAsx);
                            // announcement.FileContent = StringCompressor.CompressString(textAnn);
                            announcement.FileText = textAnn;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Unable to convert text from pdf announcement for symbol {announcement.Symbol} release date {announcement.ReleaseDate:yyyy-MM-dd}: {ex.Message}");
                        continue;
                    }
                }

                _contextAnalysis.Announcements.UpdateRange(announcementsBatch);
                await _contextAnalysis.SaveChangesAsync();

                offset += batchSize; // Move to the next batch
            }
        }


        private static async Task<string> ExtractTextFromPdfUrlAsync(string url, HttpClient client)
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
    }
}
