using System;

namespace SocketServer
{
	class Program
	{
		static void Main(string[] args)
		{
			int selectedOption = 0;

			while (true)
			{
				Console.WriteLine("Opciones de servidor Socket:\n");
				Console.WriteLine("[1] Servidor Sincrónico.");
				Console.WriteLine("[2] Servidor Asincrónico.");
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
			}

			if (selectedOption == 1)
			{
				Console.WriteLine("\nIniciando servidor sincrónico...");
				SynchronousSocketListener.StartListening();
			}

			if (selectedOption == 2)
			{
				Console.WriteLine("\nIniciando servidor asincrónico...");
				AsynchronousSocketListener.StartListening();
			}
		}
	}
}
