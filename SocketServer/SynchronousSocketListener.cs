using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketServer
{
	public class SynchronousSocketListener
	{
		// Incoming data from the client.
		public static string data = null;

		public static void StartListening()
		{
			// Data buffer for incoming data.
			byte[] bytes = new Byte[1024];

			// Establish the local endpoint for the socket
			// Dns.GetHostName returns the name of the
			// host running the application.
			IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
			IPAddress ipAddress = ipHostInfo.AddressList[0];
			IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

			// Create a TCP/IP socket.
			Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

			// Bind the socket to the local endpoint and
			// listen for incoming connections.
			try
			{
				listener.Bind(localEndPoint);
				listener.Listen(100);

				// Start listening for connections.
				while (true)
				{
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine("Waiting for a connection on port 11000...");
					Console.ResetColor();

					// Program is suspended while waiting for an incoming connection.
					Socket handler = listener.Accept();
					data = null;

					// An incoming connection needs to be processed.
					while (true)
					{
						int bytesRec = handler.Receive(bytes);
						data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
						if (data.IndexOf("<EOF>") > -1) {
							break;
						}
					}

					// Show the data on the console.
					Console.WriteLine("[{0}] - Text received: {1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), data);

					// Echo the data back to the client.
					byte[] msg = Encoding.ASCII.GetBytes(data);

					handler.Send(msg);
					handler.Shutdown(SocketShutdown.Both);
					handler.Close();
				}
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.ToString());
				Console.ResetColor();
			}

			Console.WriteLine("\nPress ENTER to continue...");
			Console.Read();
		}
	}
}