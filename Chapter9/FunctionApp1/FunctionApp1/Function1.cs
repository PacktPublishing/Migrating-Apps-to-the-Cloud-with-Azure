using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace FunctionApp1
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task Run([BlobTrigger("samples-workitems/{name}", Connection = "StorageAccount")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
            MailAddress from = new MailAddress("[your account]@outlook.com");
            MailAddress to = new MailAddress("[your account]@outlook.com");
            using (var client = new SmtpClient("smtp-mail.outlook.com", 587)
            {
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("[your account]@outlook.com", "[your password]")
            })
            using (var mail = new MailMessage(from, to)
            {
                Body = $"A new file named '{name}' was uploaded to your storage account.",
                Subject = "File upload",
                IsBodyHtml = true
            })
            {
                mail.Attachments.Add(new Attachment(myBlob, name));
                await client.SendMailAsync(mail);
            }
        }

        [FunctionName("TimerTrigger")]
        public static void DoStuff([TimerTrigger("*/10 * * * * *", RunOnStartup = true)]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"I was triggered at {DateTime.Now.ToLongTimeString()}.");
            log.LogInformation(myTimer.FormatNextOccurrences(1));
        }

        [FunctionName("WriteFile")]
        [return: Blob("queue-messages/{sys.randguid}.txt", FileAccess.Write, Connection = "StorageAccount")]
        public static string WriteFile([ServiceBusTrigger("myqueue", Connection = "ServiceBus")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
            return myQueueItem;
        }

        [FunctionName("WriteFile2")]
        public static void WriteFile2([ServiceBusTrigger("myqueue", Connection = "ServiceBus")]Message myQueueItem, ILogger log,
            [Blob("queue-messages/{Label}.txt", FileAccess.Read, Connection = "StorageAccount")]string inputBlob,
            [Blob("queue-messages/{sys.randguid}.txt", FileAccess.Write, Connection = "StorageAccount")]out string myblob)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
            myblob = inputBlob;
        }
    }
}
