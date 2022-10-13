using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace FIT5032_Week08A.Utils
{
    public class EmailSender
    {
        // Please use your API KEY here.
        private const String API_KEY = "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";
        // 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

        public void Send(String toEmail, String subject, String contents, HttpPostedFileBase postedFile)
        {
            try
            { 
                var client = new SendGridClient(API_KEY);
                var from = new EmailAddress("superman12e4@gmail.com", "No Reply 2");
                var to = new EmailAddress(toEmail, "");
                var plainTextContent = contents;
                var htmlContent = "<p>" + contents + "</p>";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                //msg.Attachments.Add(new Attachment());
                
                byte[] bytes;
                using (var memoryStream = new MemoryStream())
                {
                    postedFile.InputStream.CopyTo(memoryStream);
                    bytes = memoryStream.ToArray();
                }
                string base64 = Convert.ToBase64String(bytes);
                Attachment attachment = new Attachment();
                msg.AddAttachment(postedFile.FileName,base64);
                var response = client.SendEmailAsync(msg);
                response.Wait();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}