using System.Net.Sockets;
using System.Text;

namespace SocketServer
{
	// State object for reading client data asynchronously.
    public class StateObject
    {
		// Size of receive buffer.
		public const int BUFFER_SIZE = 1024;

		// Receive buffer.
		public byte[] Buffer { get; set; } = new byte[BUFFER_SIZE];

		// Received data string.
		public StringBuilder Builder { get; set; } = new StringBuilder();

		// Client socket.
		public Socket WorkSocket { get; set; } = null;
    }
}