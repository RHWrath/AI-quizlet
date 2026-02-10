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
            ws.OnClose = () =>
            {
                socket = ws;
                Console.WriteLine("WebSocket Disconnected");
            };
        });
        // before running check which if windows or mac and which port is being used
        // string portName = "COM3"; 
        string portName = "/dev/cu.usbserial-110";

        try
        {
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
                if (socket == null) {Console.WriteLine("No frontend connected, Message dropped") continue;}

                socket?.Send(message);
            }
        }
        catch (TimeoutException)
        {
            
        }
        catch (Exception ex)
        {
            System.Console.WriteLine(e);
            throw;
        }
        
    }
}
