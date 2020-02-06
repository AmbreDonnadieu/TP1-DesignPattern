using System;
using System.Text;

namespace Application
{
    class Program
    {
        static void Main(string[] args)
        {
            ComS2S com = new ComS2S();
            com.OnDataReceived += Print;
            com.SendData(Encoding.UTF8.GetBytes("assas"));
            com.Close();
        }

        static void Print(byte[] data)
        {
            Console.WriteLine(Encoding.UTF8.GetString(data));
        }
    }
}
