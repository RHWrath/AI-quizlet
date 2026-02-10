using System;
using System.IO.Ports;
using Fleck;

class Program
{
    static void Main()
    {
        FleckLog.Level = LogLevel.Warn;

        var server = new WebSocketServer("ws://127.0.0.1:8181");
        IWebSocketConnection socket = null;

        server.Start(ws =>
        {
            ws.OnOpen = () => {
                socket = ws;
                Console.WriteLine("WebSocket connected");
            };
            ws.OnClose = () => socket = null;
        });

        string portName = "/dev/cu.usbserial-110";
        using SerialPort port = new SerialPort(portName, 9600);
        port.NewLine = "\n";
        port.Open();

        Console.WriteLine("Listening to Arduino...");

        while (true)
        {
            string line = port.ReadLine().Trim(); // "0" or "1"

            // 0 = AI, 1 = REAL
            string message = line == "0" ? "ai" : "real";

            Console.WriteLine($"Arduino → {message}");

            socket?.Send(message);
        }
    }
}
