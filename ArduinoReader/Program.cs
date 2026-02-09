using System;
using System.IO.Ports;

class Program
{
    static void Main(string[] args)
    {
        string portName = "/dev/cu.usbserial-110";

        using SerialPort port = new SerialPort(portName, 9600);
        port.NewLine = "\n";
        port.Open();

        Console.WriteLine($"Listening on {portName}. Press Ctrl+C to exit.");

        while (true)
        {
            string line = port.ReadLine().Trim(); // "0" or "1"

            bool isAI = line == "0";

            Console.WriteLine($"Raw: '{line}'  ->  isAI = {isAI}");
        }
    }
}
