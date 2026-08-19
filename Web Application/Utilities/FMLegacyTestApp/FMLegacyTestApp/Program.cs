using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMLegacyTestApp
{
	class Program
	{
		static public TestDllClass testDllClass;
		static void Main(string[] args)
		{
			// running as console app
			Start(args);

			Console.WriteLine("Press:\n\tx,q: To shutdown the server\n\n");

			// wait for exit command.
			do
			{
				ConsoleKeyInfo key = Console.ReadKey();

				if (key.KeyChar == 'q' || key.KeyChar == 'x')
				{
					break;
				}
			}
			while (true);

			Stop();

		}

		private static void Start(string[] args)
		{
			testDllClass = new TestDllClass();

			testDllClass.LoadDriverDll();

		}

		private static void Stop()
		{

		}
	}
}
