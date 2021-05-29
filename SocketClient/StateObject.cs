using System.Net.Sockets;
using System.Text;

namespace SocketClient
{
	// State object for receiving data from remote device.
	public class StateObject
	{
		// Client socket.
		public Socket WorkSocket { get; set; } = null;

		// Size of receive buffer.
		public const int BUFFER_SIZE = 256;

		// Receive buffer.
		public byte[] Buffer { get; set; } = new byte[BUFFER_SIZE];

		// Received data string.
		public StringBuilder Builder { get; set; } = new StringBuilder();
	}
}
