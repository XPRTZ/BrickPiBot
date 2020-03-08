
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
    public abstract class BotBase
    {
        private Brick _brick;
        private Motor _smackMotor;
        private Motor _leftMotor;
        private Motor _rightMotor;
        private EV3UltraSonicSensor _sonic;
        private EV3GyroSensor _gyro;
        private EV3TouchSensor _touch;
        private EV3ColorSensor _color;

        public Motor GetSmackMotor => _smackMotor;
        public Motor GetLeftMotor => _leftMotor;
        public Motor GetRightMotor => _rightMotor;

        public EV3UltraSonicSensor GetSonicSensor => _sonic;
        public EV3GyroSensor GetGyroSensor => _gyro;
        public EV3TouchSensor GetTouchSensor => _touch;
        public EV3ColorSensor GetColorSensor => _color;

        public BotBase()
        {
            Setup();

            PrintInfo();
        }

        private void PrintInfo()
        {
            Console.WriteLine($"ID: {_brick.BrickPi3Info.Id}");
            Console.WriteLine($"Voltage: {_brick.BrickPi3Voltage.Voltage3V3}");
            Console.WriteLine($"Voltage: {_brick.BrickPi3Voltage.Voltage5V}");
            Console.WriteLine($"Voltage: {_brick.BrickPi3Voltage.Voltage9V}");
            Console.WriteLine($"Voltage: {_brick.BrickPi3Voltage.VoltageBattery}");
            Console.WriteLine($"Version: {_brick.BrickPi3Info.SoftwareVersion}");
        }

        public void Setup()
        {
            // Set brick
             _brick = new Brick();

            // Set Smacker ;)
            _smackMotor = new Motor(_brick, Iot.Device.BrickPi3.Models.BrickPortMotor.PortB);
            _smackMotor.PropertyChanged += HandleSmackTachoPropertyChangedEvent;            

            // Set ultrasonic
            _sonic = new EV3UltraSonicSensor(_brick, SensorPort.Port1, UltraSonicMode.Centimeter);
            _sonic.PropertyChanged += HandleSonicSmackPropertyChangedEvent;

            // Set movement motors
            _leftMotor = new Motor(_brick, Iot.Device.BrickPi3.Models.BrickPortMotor.PortC);
            _leftMotor.PropertyChanged += HandleLeftTachoPropertyChangedEvent;  
            _rightMotor = new Motor(_brick, Iot.Device.BrickPi3.Models.BrickPortMotor.PortD);
            _rightMotor.PropertyChanged += HandleRightTachoPropertyChangedEvent;  

            // Gyro
            _gyro = new EV3GyroSensor(_brick, SensorPort.Port3);
            _gyro.Mode = GyroMode.Angle;
            _gyro.PropertyChanged += HandleGyroMovementPropertyChangedEvent;

            // Color
            _color = new EV3ColorSensor(_brick, SensorPort.Port2, ColorSensorMode.Color, 20);
            _color.PropertyChanged += HandleMovementColorPropertyChangedEvent;

            // Touch
            _touch = new EV3TouchSensor(_brick, SensorPort.Port4, 20);
            _touch.PropertyChanged += HandleHit;
        }

        /// <summary>
        /// Smack!
        /// </summary> 
        public async Task Smack()
        { 
            await _smackMotor.RunUntilBlock(50);
            await Task.Delay(100);
            _smackMotor.SetSpeed(-50);
            await Task.Delay(200);
            _smackMotor.SetSpeed(0);
            await Task.Delay(100);
            await _smackMotor.RunUntilBlock(50);
        }

        /// <summary>
        /// Stop Robot
        /// </summary>        
        public virtual async Task Stop()
        {
            // Smacker down
            _smackMotor.SetSpeed(-50);
            await Task.Delay(200);
            _smackMotor.SetSpeed(0);

            _leftMotor.SetSpeed(0);
            _rightMotor.SetSpeed(0);
        }

        /// <summary>
        /// Stop Robot
        /// </summary>  
        public virtual async Task Start(int speed)
        {
            // smacker up...
            await _smackMotor.RunUntilBlock(50);

            _leftMotor.SetSpeed(speed);
            _rightMotor.SetSpeed(speed);
        }

        /// <summary>
        /// Handle touch event
        /// </summary>
        /// <param name="sender">source of event</param>
        /// <param name="e">event args</param>
        protected virtual async void HandleHit(object sender, PropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Handle color change event
        /// </summary>
        /// <param name="sender">source of event</param>
        /// <param name="e">event args</param>
        protected virtual async void HandleMovementColorPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Handle gyro change event
        /// </summary>
        /// <param name="sender">source of event</param>
        /// <param name="e">event args</param>
        protected virtual async void HandleGyroMovementPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Handle Sonic range change event 
        /// </summary>
        /// <param name="sender">source of event</param>
        /// <param name="e">event args</param>
        protected virtual async void HandleSonicSmackPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Handle tacho change event smack motor
        /// </summary>
        /// <param name="sender">source of event</param>
        /// <param name="e">event args</param>
        protected virtual async void HandleSmackTachoPropertyChangedEvent (object sender, PropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Handle tacho change event left motor
        /// </summary>
        /// <param name="sender">source of event</param>
        /// <param name="e">event args</param>
        protected virtual async void HandleLeftTachoPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Handle tacho change event right motor
        /// </summary>
        /// <param name="sender">source of event</param>
        /// <param name="e">event args</param>
        protected virtual async void HandleRightTachoPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
        }
    }
}