using System;

namespace BattleBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting robot, press any key to quit...");

            var robot = new MyBattleBot();

            Console.ReadKey();
        }
    }
}
