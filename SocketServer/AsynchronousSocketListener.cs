using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketServer
{
	public class AsynchronousSocketListener
	{
		// Thread signal.
		public static ManualResetEvent allDone = new ManualResetEvent(false);

		public AsynchronousSocketListener()
		{
		}

		public static void StartListening()
		{
			// Establish the local endpoint for the socket
			// The DNS name of the computer
			// running the listener is "host.contoso.com"
			IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
			IPAddress iPAddress = ipHostInfo.AddressList[0];
			IPEndPoint localEndpoint = new IPEndPoint(iPAddress, 12000);

			// Create a TCP/IP socket.
			Socket listener = new Socket(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

			// Bind the socket to the local endpoint and listen for incoming connections.
			try
			{
				listener.Bind(localEndpoint);
				listener.Listen(100);

				while (true)
				{
					// Set the event to nonsignaled state.
					allDone.Reset();

					// Start an asynchronous socket to listen for connections.
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine("Waiting for a connection on port 12000...");
					Console.ResetColor();

					listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

					// Wait until a connection is made before continuing.
					allDone.WaitOne();
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

		private static void AcceptCallback(IAsyncResult ar)
		{
			// Signal the main thread to continue.
			allDone.Set();

			// Get the socket that handles the client request.
			Socket listener = (Socket)ar.AsyncState;
			Socket handler = listener.EndAccept(ar);

			// Create the state object.
			StateObject state = new StateObject();
			state.WorkSocket = handler;
			handler.BeginReceive(state.Buffer, 0, StateObject.BUFFER_SIZE, 0, new AsyncCallback(ReadCallback), state);


		}

		private static void ReadCallback(IAsyncResult ar)
		{
			string content = string.Empty;

			// Retrieve the state object and the handler socket.
			// from the asynchronous state object.
			StateObject state = (StateObject)ar.AsyncState;
			Socket handler = state.WorkSocket;

			// Read data from the client socket.
			int bytesRead = handler.EndReceive(ar);

			if (bytesRead > 0)
			{
				// There might be more data, so store the data received so far.
				state.Builder.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));

				// Check for end-of-file tag. If it is not there, read
				// more data.
				content = state.Builder.ToString();
				if (content.IndexOf("<EOF>") > -1)
				{
					// All the data has been read from the
					// client. Display it on the console.

					Console.WriteLine("[{0}] - Read {1} bytes from socket.", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), content.Length);
					Console.WriteLine("[{0}] - Data: {1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), content);

					// Echo the data back to the client.
					Send(handler, content);
				}
				else
				{
					// Not all data received. Get more.
					handler.BeginReceive(state.Buffer, 0, StateObject.BUFFER_SIZE, 0, new AsyncCallback(ReadCallback), state);
				}
			}
		}

		private static void Send(Socket handler, string data)
		{
			// Convert the string data to byte data using ASCII encoding.
			byte[] byteData = Encoding.ASCII.GetBytes(data);

			// Begin sending the data to the remote device.
			handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
		}

		private static void SendCallback(IAsyncResult ar)
		{
			try
			{
				// Retrieve the socket from the state object.
				Socket handler = (Socket)ar.AsyncState;

				// Complete sending the data to the remote device.
				int bytesSent = handler.EndSend(ar);
				Console.WriteLine("[{0}] - Send {1} bytes to client.", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), bytesSent);

				handler.Shutdown(SocketShutdown.Both);
				handler.Close();
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.ToString());
				Console.ResetColor();
			}
		}
	}
}