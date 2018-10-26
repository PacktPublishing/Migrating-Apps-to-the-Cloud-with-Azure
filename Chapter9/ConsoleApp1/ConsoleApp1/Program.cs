using Microsoft.Azure.ServiceBus;
using System.Text;

namespace ConsoleApp1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string text = "This is some text.";
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            QueueClient client = new QueueClient("[your connectionstring]", "myqueue");
            var msg = new Message(bytes);
            msg.Label = "Some label";
            client.SendAsync(msg).Wait();
            client.CloseAsync().Wait();
        }
    }
}
