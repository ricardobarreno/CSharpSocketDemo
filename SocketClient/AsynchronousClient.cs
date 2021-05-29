using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketClient
{
	public class AsynchronousSocketClient
	{
		// The port number for the remote device.
		private const int PORT = 12000;

		// ManulaResetEvent instances signal completion.
		private static ManualResetEvent connectDone = new ManualResetEvent(false);
		private static ManualResetEvent sendDone = new ManualResetEvent(false);
		private static ManualResetEvent receiveDone = new ManualResetEvent(false);

		// The respons from the remote device.
		private static string response = string.Empty;

		public static void StartClient(string message)
		{
			// Connect to a remote device.
			try
			{
				// Establish the remote endpoint for the socket.
				// The name of the
				// remote device is "host.contoso.com"
				IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
				IPAddress ipAddress = ipHostInfo.AddressList[0];
				IPEndPoint remoteEndpoint = new IPEndPoint(ipAddress, PORT);

				// Create a TCP/IP socket.
				Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

				// Connect to the remote endpoint.
				client.BeginConnect(remoteEndpoint, new AsyncCallback(ConnectCallback), client);
				connectDone.WaitOne();

				// Send test data to the remote device.
				Send(client, $"{message}<EOF>");
				sendDone.WaitOne();

				// Receive the response from the remote device.
				Receive(client);
				receiveDone.WaitOne();

				// Write the response to the console.
				Console.WriteLine("[{0}] - Response received: {1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), response);

				// Release the socket.
				client.Shutdown(SocketShutdown.Both);
				client.Close();
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.ToString());
				Console.ResetColor();
			}
		}

		private static void Receive(Socket client)
		{
			try
			{
				// Create the state object.
				StateObject state = new StateObject();
				state.WorkSocket = client;

				// Begin receiving that data from the remote device.
				client.BeginReceive(state.Buffer, 0, StateObject.BUFFER_SIZE, 0, new AsyncCallback(ReceiveCallback), state);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.ToString());
				Console.ResetColor();
			}
		}

		private static void ReceiveCallback(IAsyncResult ar)
		{
			try
			{
				// Retrieve the state object and the client socket
				// from the asynchronous state object.
				StateObject state = (StateObject)ar.AsyncState;
				Socket client = state.WorkSocket;

				// Read data from the remote device.
				int bytesRead = client.EndReceive(ar);

				if (bytesRead > 0)
				{
					// There might be more data, so store the data received so far.
					state.Builder.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));

					// Get the rest of the data.
					client.BeginReceive(state.Buffer, 0, StateObject.BUFFER_SIZE, 0, new AsyncCallback(ReceiveCallback), state);
				}
				else
				{
					// All the data has arrived; put it in response.
					if (state.Builder.Length > 1)
					{
						response = state.Builder.ToString();
					}

					// Signal that all bytes have been received.
					receiveDone.Set();
				}
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.ToString());
				Console.ResetColor();
			}
		}

		private static void Send(Socket client, string data)
		{
			// Convert the string data to byte data using ASCII encoding.
			byte[] byteData = Encoding.ASCII.GetBytes(data);

			// Begin sending the data to the remote device.
			client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
		}

		private static void SendCallback(IAsyncResult ar)
		{
			try
			{
				// Retrieve the socket from the state object.
				Socket client = (Socket)ar.AsyncState;

				// Complete sending the data to the remote device.
				int bytesSent = client.EndSend(ar);
				Console.WriteLine("[{0}] - Send {1} bytes to server.", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), bytesSent);

				// Signal that all bytes have been sent.
				sendDone.Set();
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.ToString());
				Console.ResetColor();
			}
		}

		private static void ConnectCallback(IAsyncResult ar)
		{
			try
			{
				// Retrieve the socket from the state object.
				Socket client = (Socket)ar.AsyncState;

				// Complete the connection.
				client.EndConnect(ar);

				Console.WriteLine("[{0}] - Socket connected to {1}.", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), client.RemoteEndPoint.ToString());

				// Signal that the connection has been made.
				connectDone.Set();
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
