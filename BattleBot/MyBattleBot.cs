using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using Iot.Device.BrickPi3;
using Iot.Device.BrickPi3.Sensors;
using Iot.Device.BrickPi3.Utils;
using Iot.Device.BrickPi3.Models;
using Iot.Device.BrickPi3.Movement;

namespace BattleBot
{
    public class MyBattleBot : BotBase
    {
        public MyBattleBot() : base()
        {

        }


        private bool _handlingHit = false;
        private bool _robotStarted = false;

        /// <summary>
        /// Handle touch event
        /// </summary>
        /// <param name="sender">source of event</param>
        /// <param name="e">event args</param>
        protected override async void HandleHit(object sender, PropertyChangedEventArgs e)
        {
            if(!_handlingHit && ((EV3TouchSensor) sender).IsPressed())
            {
                _handlingHit = true;

                _robotStarted = !_robotStarted;

                if(_robotStarted)
                {
                    Console.WriteLine("Start robot!");
                    await Start(30);
                }
                else
                {
                    Console.WriteLine("Stop robot!");
                    await Stop();
                }

                _handlingHit = false;
            }
        }

        private bool _handlingSonicSmackEvent = false;

        /// <summary>
        /// Handle Sonic range change event 
        /// </summary>
        /// <param name="sender">source of event</param>
        /// <param name="e">event args</param>
        protected override async void HandleSonicSmackPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {            
            if(!_handlingSonicSmackEvent && ((EV3UltraSonicSensor)sender).Value < 10)
            {
                await Stop();
                _handlingSonicSmackEvent = true;
                Console.WriteLine("Smack!!");
                await Smack();                
            }
            _handlingSonicSmackEvent = false;
        }

        private bool _handlingMovementColorEvent = false;

        /// <summary>
        /// Handle color change event
        /// </summary>
        /// <param name="sender">source of event</param>
        /// <param name="e">event args</param>
        protected override async void HandleMovementColorPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            var color = ((EV3ColorSensor)sender);

            var gyro = GetGyroSensor;

            var leftMotor = GetLeftMotor;
            var rightMotor = GetRightMotor;

            var deg = 0;
           
            if(!_handlingMovementColorEvent && color.Value != ((int) Color.White))
            {
                _handlingMovementColorEvent = true;
                
                await Stop();

                if(gyro.Value != 0)
                {         
                    Console.WriteLine($"Gyro: {gyro.Value}, Reset...");       
                    gyro.Reset();
                }
                
                await Task.Delay(250);

                Console.WriteLine("Turn!");
                leftMotor.SetSpeed(30);
                rightMotor.SetSpeed(-30);

                while(Math.Abs(gyro.Value) < 175)
                {
                    deg = gyro.Value;
                    await Task.Delay(20);
                }

                leftMotor.SetSpeed(30);
                rightMotor.SetSpeed(30);

                _handlingMovementColorEvent = false;
            }            
        }
    }
}