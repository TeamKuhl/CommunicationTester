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
                    if(type == "PlayerInfo") {
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

        private static void log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
