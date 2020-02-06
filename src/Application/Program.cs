using System;
using System.Text;

namespace Application
{
    class Program
    {
        static bool _cancelled;

        static void Main(string[] args)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);

            ComS2S com = new ComS2S(new ComS2S.Options {
                StreamOptions = ComS2S.EStreamOptions.Compressed | ComS2S.EStreamOptions.Encrypted
            });
            com.OnDataReceived += PrintReceivedData;
            com.SendData(Encoding.UTF8.GetBytes("assas"));

            while(!_cancelled)
            {
            }

            com.Close();
        }

        static void PrintReceivedData(byte[] data)
        {
            Console.WriteLine("Local: Data received: " + Encoding.UTF8.GetString(data));
        }

        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("Cancelling");
            if (e.SpecialKey == ConsoleSpecialKey.ControlC)
            {
                _cancelled = true;
                e.Cancel = true;
            }
        }
    }
}
