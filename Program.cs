using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using CommunicationLibrary;

namespace CommunicationTester
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "--test")
            {
                Server server = new Server();
                int port = 45454;

                log("Starting server on port " + port);
                server.start(port);

                log("Adding ServerReceiveHandler onReceive");
                server.onReceive += new ServerReceiveHandler(
                    (TcpClient client, String type, String message) =>
                    {
                        if (string.IsNullOrEmpty(type)) type = "(empty)";
                        if (string.IsNullOrEmpty(message)) message = "(empty)";
                        log("Server received: \"" + type + "\" | \"" + message + "\"");
                        if (type == "PlayerInfo")
                        {
                            log("Server received PlayerInfo, answering with Join");
                            server.sendToAll("Join", message);
                        }
                    }
                );

                Client c = new Client();

                log("Adding ClientReceiveHandler onReceive");
                c.onReceive += new ClientReceiveHandler(
                    (string type, string message) =>
                    {
                        log("Client received: \"" + type + "\" | \"" + message + "\"");
                    }
                );

                log("Client connects to the server");
                bool connected = c.connect("127.0.0.1", port);
                if (connected) log("Client connected to server");

                log("Client sending PlayerInfo command with TestUser argument");
                c.send("PlayerInfo", "TestUser");
            }
            else
            {
                // Interactive input

                string defaultHost = "127.0.0.1";
                int defaultPort = 45454;

                Console.WriteLine("Starting interactive client...");
                Console.Write("Please enter the hostname to connect to [" + defaultHost + "]: ");
                string host = Console.ReadLine();
                host = host.Trim();
                if (String.IsNullOrEmpty(host)) host = defaultHost;
                host = host.Trim();
                log("Using host \"" + host + "\"");
                Console.Write("Please enter the port you want to connect to [" + defaultPort + "]: ");
                string sPort = Console.ReadLine();
                int port;
                if (String.IsNullOrEmpty(sPort)) port = defaultPort;
                else {
                    try
                    {
                        port = Convert.ToInt16(sPort);
                    }
                    catch (Exception)
                    {
                        port = 45454;
                    }
                }
                log("Using port \"" + port + "\"");
                Client c = new Client();
                Console.WriteLine("Adding ClientReceiveHandler onReceive");
                c.onReceive += new ClientReceiveHandler(
                    (string type, string message) =>
                    {
                        log("R: \"" + type + "\";\"" + message + "\"");
                    }
                );

                log("Connecting...");
                c.connect(host, port);

                while (true)
                {
                    Console.Write("S: ");
                    string line = Console.ReadLine();
                    string[] data = line.Split(new Char[] { ';' }, 2);
                    c.send(data[0], data[1]);
                }
            }
        }

        private static void log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
