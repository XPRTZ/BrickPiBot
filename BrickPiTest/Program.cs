using Iot.Device.BrickPi3;
using Iot.Device.BrickPi3.Movement;
using System;
using System.Threading.Tasks;

namespace BrickPiTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting...");
            Brick brick = new Brick();
            Motor motor = new Motor(brick, Iot.Device.BrickPi3.Models.BrickPortMotor.PortB);

            motor.SetSpeed(10);
            motor.SetTachoCount(0);
            motor.Start();
            await Task.Delay(500);
            motor.Stop();

            Console.WriteLine("Stop...");
        }
    }
}
