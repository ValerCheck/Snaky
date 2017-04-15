using System;

namespace Snaky
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(50,28);
            Console.SetBufferSize(50,28);
            var game = new GameManager(Console.BufferWidth - 1,Console.WindowHeight - 1);
            game.StateChangedEvent += state =>
            {
                Console.Clear();
                Console.Write(state);
            };
            game.StartGame();
            Console.ReadLine();

        }
    }
}
