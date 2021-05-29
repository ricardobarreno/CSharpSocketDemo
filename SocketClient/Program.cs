using System;

namespace SocketClient
{
    class Program
    {
		static void Main(string[] args)
        {
			int selectedOption = 0;

			while (true)
			{
				Console.WriteLine("Cliente Socket:");
				Console.WriteLine("[1] Cliente Sincrónico.");
				Console.WriteLine("[2] Cliente Asincrónico.");
				Console.WriteLine("[0] Salir.");

				Console.Write("\nIngrese su opción: ");
				int.TryParse(Console.ReadLine(), out selectedOption);


				if (selectedOption == 0)
				{
					Console.WriteLine("Gracias...");
					Environment.Exit(0);
				}

				if (selectedOption == 1 || selectedOption == 2)
				{
					break;
				}

				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Opción inválida, vuelva a intentarlo...\n");
				Console.ResetColor();
			}

			while (true)
			{
				Console.Write("\nEscribe mensaje (\"exit\" para salir): ");
				string message = Console.ReadLine();

				if (message == "exit")
				{
					break;
				}

				if (selectedOption == 1)
				{
					SynchronousSocketClient.StartClient(message);
				}

				if (selectedOption == 2)
				{
					AsynchronousSocketClient.StartClient(message);
				}
			}
        }
    }
}
