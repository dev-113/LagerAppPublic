using ClosedXML.Excel;
using LagerApp.Models;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;
using LagerApp.Models.Settings;

namespace LagerApp.Services
{
    public class ExcelWriter : IExcelWriter
    {
        private readonly MailSettings _mailSettings;
        private readonly ExcelSettings _excelSettings;

        public ExcelWriter(IOptions<MailSettings> mailSettings, IOptions<ExcelSettings> excelSettings)
        {
            _mailSettings = mailSettings.Value;
            _excelSettings = excelSettings.Value;
        }

        public async Task<bool> ProductsFromDatabaseExcelWriter(List<Product> products)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Products");

                    worksheet.Cell("A1").Value = "Artikel nummer";
                    worksheet.Cell("B1").Value = "Inköpspris";
                    worksheet.Cell("C1").Value = "Pris";
                    worksheet.Cell("D1").Value = "Vikt";
                    worksheet.Cell("E1").Value = "Mått";
                    worksheet.Cell("F1").Value = "Material";
                    worksheet.Cell("G1").Value = "Antal";

                    int row = 2;
                    foreach (var product in products)
                    {
                        worksheet.Cell(row, 1).Value = product.ArticleNumber;
                        worksheet.Cell(row, 2).Value = product.PurchasePrice;
                        worksheet.Cell(row, 3).Value = product.SellingPrice;
                        worksheet.Cell(row, 4).Value = product.Weight;
                        worksheet.Cell(row, 5).Value = product.Dimension;
                        worksheet.Cell(row, 6).Value = product.Material;
                        worksheet.Cell(row, 7).Value = product.Quantity;
                        row++;
                    }

                    // Convert the workbook to a byte array
                    await using (var memoryStream = new MemoryStream())
                    {
                        workbook.SaveAs(memoryStream);
                        byte[] fileContents = memoryStream.ToArray();

                        // Send the byte array as an email attachment
                        SendEmailWithAttachment("Database Excel Export", fileContents, _excelSettings.Database);
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<bool> ProductsFromListExcelWriter(List<List> products)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Products");

                    worksheet.Cell("A1").Value = "Artikel nummer";
                    worksheet.Cell("B1").Value = "Antal";

                    int row = 2;
                    foreach (var product in products)
                    {
                        worksheet.Cell(row, 1).Value = product.ArticleNumber;
                        worksheet.Cell(row, 2).Value = product.Quantity;
                        row++;
                    }

                    // Convert the workbook to a byte array
                    await using (var memoryStream = new MemoryStream())
                    {
                        workbook.SaveAs(memoryStream);
                        byte[] fileContents = memoryStream.ToArray();

                        // Send the byte array as an email attachment
                        SendEmailWithAttachment("List Excel Export", fileContents, _excelSettings.List);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        private void SendEmailWithAttachment(string subject, byte[] content, string fileName)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(_mailSettings.FromAddress);
                    mail.To.Add(_mailSettings.ToAddress);
                    mail.Subject = subject;
                    mail.Body = "Please find the attached excel file";

                    using (MemoryStream memoryStream = new MemoryStream(content))
                    {
                        memoryStream.Position = 0;
                        mail.Attachments.Add(new Attachment(memoryStream, $"{fileName} - {DateTime.Now.ToString("yyyy-MM-dd")}.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));

                        using (SmtpClient smtpClient = new SmtpClient(_mailSettings.SmtpClient))
                        {
                            smtpClient.Port = 587;
                            smtpClient.Credentials = new NetworkCredential(_mailSettings.FromAddress, _mailSettings.Password);
                            smtpClient.EnableSsl = true;
                            smtpClient.Send(mail);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new Exception(e.Message);
            }
        }
    }
}
