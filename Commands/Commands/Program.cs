using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starte Server...");
            new Server();
            Console.WriteLine("Server gestartet!");
            Console.WriteLine("Um Server zu stoppen drücke ESC!");

            while (true) ///Eine Endlos Schleife welche abgebrochen wird wenn ESC gedrückt wird
            {
                ConsoleKey pressedKey = Console.ReadKey().Key;
                if (pressedKey == ConsoleKey.Escape)
                {
                    break;
                }
            }
        }
    }
}
