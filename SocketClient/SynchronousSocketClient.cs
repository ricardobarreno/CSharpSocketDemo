using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketClient
{
	public class SynchronousSocketClient
	{
		public static void StartClient(string message)
		{
			// Data buffer for incoming data.
			byte[] bytes = new byte[1024];

			// Connect to a remote device.
			try
			{
				// Establish the remote endpoint for the socket.
				// This example uses port 11000 on the local computer.
				IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
				IPAddress ipAddress = ipHostInfo.AddressList[0];
				IPEndPoint remoteEndpoint = new IPEndPoint(ipAddress, 11000);

				// Create a TCP/IP socket.
				Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

				// Connect the socket to the remote endpoint. Catch any errors.
				try
				{
					sender.Connect(remoteEndpoint);

					Console.WriteLine("\n[{0}] - Socket connected to {1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), sender.RemoteEndPoint.ToString());

					// Encode the data string into a byte array.
					byte[] msg = Encoding.ASCII.GetBytes(message + "<EOF>");

					// Send the data through the socket.
					int bytesSent = sender.Send(msg);

					// Receive the response from the remote device.
					int bytesRec = sender.Receive(bytes);
					Console.WriteLine("[{0}] - Echoed test = {1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), Encoding.ASCII.GetString(bytes, 0, bytesRec));

					// Release the socket.
					sender.Shutdown(SocketShutdown.Both);
					sender.Close();
				}
				catch (ArgumentException ane)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("ArgumentNullException: {0}", ane.ToString());
					Console.ResetColor();
				}
				catch (SocketException se)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("SocketException: {0}", se.ToString());
					Console.ResetColor();
				}
				catch (Exception ex)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Unexpected exception: {0}", ex.ToString());
					Console.ResetColor();
				}
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Unexpected exception: {0}", ex.ToString());
				Console.ResetColor();
			}
		}
	}
}
