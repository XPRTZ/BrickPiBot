using Iot.Device.BrickPi3;
using Iot.Device.BrickPi3.Models;
using Iot.Device.BrickPi3.Movement;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Iot.Device.BrickPi3.Sensors;
using Iot.Device.BrickPi3.Utils;
using System.ComponentModel;

namespace BrickPiTest
{
    class Program
    {
        private static ManualResetEvent _quitEvent = new ManualResetEvent(false);

        private Brick _brick;
        private Motor _smackMotor;
        private Motor _leftMotor;
        private Motor _rightMotor;
        private EV3UltraSonicSensor _sonic;
        private EV3GyroSensor _gyro;
        private EV3TouchSensor _touch;

        public Program()
        {
           
        }

        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting..."); 

            var prog = new Program();     

            await prog.Setup();

            prog.PrintInfo();
            
            prog.Start();

            Console.ReadKey();

            //_quitEvent.WaitOne();

            //await prog.Smack();
            //await prog.UpDown();
            //await prog.Down();
            //await prog.TestTouch();
            //await prog.TestSonic();
            //await prog.TestColorSensor();
            //await prog.TestGiroSensor();
        }

        public void PrintInfo()
        {
            Console.WriteLine($"ID: {_brick.BrickPi3Info.Id}");
            Console.WriteLine($"Voltage: {_brick.BrickPi3Voltage.Voltage3V3}");
            Console.WriteLine($"Voltage: {_brick.BrickPi3Voltage.Voltage5V}");
            Console.WriteLine($"Voltage: {_brick.BrickPi3Voltage.Voltage9V}");
            Console.WriteLine($"Voltage: {_brick.BrickPi3Voltage.VoltageBattery}");
            Console.WriteLine($"Version: {_brick.BrickPi3Info.SoftwareVersion}");
        }

        public async Task Start()
        {
            _gyro.Reset();
            
            _leftMotor.SetSpeed(30);
            _rightMotor.SetSpeed(-30);

            while(Math.Abs(_gyro.Value) < 85)
            {
                await Task.Delay(20);
            }

             _leftMotor.SetSpeed(0);
             _rightMotor.SetSpeed(0);
        }

        public async Task Setup()
        {
            // Set brick
             _brick = new Brick();

            // Set Smacker ;)
            _smackMotor = new Motor(_brick, Iot.Device.BrickPi3.Models.BrickPortMotor.PortB);
            //_smackMotor.PropertyChanged += HandleTachoPropertyChangedEvent;
            await _smackMotor.RunUntilBlock(50);

            // Set ultrasonic
            _sonic = new EV3UltraSonicSensor(_brick, SensorPort.Port1, UltraSonicMode.Centimeter);
            _sonic.PropertyChanged += HandleSonicSmackPropertyChangedEvent;

            // Set movement motors
            _leftMotor = new Motor(_brick, Iot.Device.BrickPi3.Models.BrickPortMotor.PortC);
            _rightMotor = new Motor(_brick, Iot.Device.BrickPi3.Models.BrickPortMotor.PortD);

            // Gyro
            _gyro = new EV3GyroSensor(_brick, SensorPort.Port3);
            _gyro.Mode = GyroMode.Angle;
            //_gyro.PropertyChanged += HandleGyroMovementPropertyChangedEvent;
        }

        private bool _handlingGyroTurnEvent = false;
        private async void HandleGyroMovementPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine($"Gyro: {Math.Abs(((EV3GyroSensor)sender).Value)}");

            if(!_handlingGyroTurnEvent && Math.Abs(((EV3GyroSensor)sender).Value) > 80)
            {
                _handlingGyroTurnEvent = true;

                Console.WriteLine("Stop!");
                _leftMotor.SetSpeed(0);
                _rightMotor.SetSpeed(0);

                _gyro.Reset();
            }
            _handlingGyroTurnEvent = false;
        }

        private bool _handlingSonicSmackEvent = false;
        private async void HandleSonicSmackPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine($"{((EV3UltraSonicSensor)sender).Value}");
            if(!_handlingSonicSmackEvent && ((EV3UltraSonicSensor)sender).Value < 10)
            {
                _handlingSonicSmackEvent = true;
                Console.WriteLine("Smack!!");
                await Smack();
                await _smackMotor.RunUntilBlock(50);
            }
            _handlingSonicSmackEvent = false;
        }

        public async Task Down()
        {
            var motor = new Motor(_brick, Iot.Device.BrickPi3.Models.BrickPortMotor.PortB);
            //await motor.RunUntilBlock(-100);
            motor.SetSpeed(-50);
            await Task.Delay(200);
            motor.SetSpeed(0);
        }

        public async Task UpDown()
        {
            var motor = new Motor(_brick, Iot.Device.BrickPi3.Models.BrickPortMotor.PortB);

            motor.SetSpeed(100);
            await Task.Delay(1500);
            motor.SetSpeed(0);
            await Task.Delay(500);
            motor.SetSpeed(-100);
            await Task.Delay(1500);
            motor.SetSpeed(0);
        }

        public async Task Smack()
        {
            var motor = new Motor(_brick, Iot.Device.BrickPi3.Models.BrickPortMotor.PortB);

            await motor.RunUntilBlock(50);
            await Task.Delay(100);
            motor.SetSpeed(-50);
            await Task.Delay(200);
            motor.SetSpeed(0);
        }

        public  async Task TestTouch()
        {            
            EV3TouchSensor ev3Touch = new EV3TouchSensor(_brick, SensorPort.Port4, 20);
            
            ev3Touch.PropertyChanged += HandlePropertyChangedEvent;

            while(false)
            {
                if(ev3Touch.IsPressed())
                {
                    Console.WriteLine($"Pressed!");
                    _smackMotor.SetSpeed(255);
                    Thread.Sleep(1000);
                    _smackMotor.SetSpeed(-255);
                    Thread.Sleep(1000);
                    _smackMotor.SetSpeed(0);                    
                }
                await Task.Delay(100);
            } 

            _smackMotor.SetSpeed(0);
        }

        private async void HandleTachoPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine($"Tacho count: {((Motor)sender).TachoCount}");            
        }

        private bool _handlingPressedEvent = false;

        private async void HandlePropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if(!_handlingPressedEvent && ((EV3TouchSensor)sender).IsPressed())
            {
                _handlingPressedEvent = true;
                Console.WriteLine($"PressedEvent! {e.PropertyName}");

                /*_motor.SetSpeed(20);
                await Task.Delay(2000);
                _motor.SetSpeed(-20);
                await Task.Delay(2000);
                _motor.SetSpeed(0); */

                _smackMotor.RunForDegrees(360, 50);

                _handlingPressedEvent = false;
            }
            else if(_handlingPressedEvent && ((EV3TouchSensor)sender).IsPressed())
            {
                Console.WriteLine($"PressedEvent! {e.PropertyName} already running");
            }
        }

        public async Task TestSonic()
        {
            EV3UltraSonicSensor sonic = new EV3UltraSonicSensor(_brick, SensorPort.Port1, UltraSonicMode.Inch);
            sonic.PropertyChanged += HandleSonicPropertyChangedEvent;
            while(false)
            {
                Console.WriteLine($"{sonic.ReadAsString()}");

                 await Task.Delay(500);
            }
        }

        private bool _handlingSonicEvent = false;
        private async void HandleSonicPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if(!_handlingSonicEvent)
            {
                _handlingSonicEvent = true;
                Console.WriteLine($"{((EV3UltraSonicSensor)sender).ReadAsString()}");
            }
            _handlingSonicEvent = false;
        }

        public  async Task TestGiroSensor()
        {
            EV3GyroSensor gyro = new EV3GyroSensor(_brick, SensorPort.Port3);
            gyro.Mode = GyroMode.Angle;
            gyro.PropertyChanged += HandleGiroPropertyChangedEvent;

            
            while(false)
            {
                var deg = gyro.ReadRaw();

                Console.WriteLine($"{deg}");
                
                await Task.Delay(500);
            }
        }

        private async void HandleGiroPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine($"Giro Changed! {((EV3GyroSensor)sender).ReadRaw()}");
        }

        public  async Task  TestColorSensor()
        {
            EV3ColorSensor color = new EV3ColorSensor(_brick, SensorPort.Port2, ColorSensorMode.Blue);
            color.PropertyChanged += HandleColorPropertyChangedEvent;

            while(false)
            {
                try
                {
                    if(color.ColorMode == ColorSensorMode.Color)
                    {
                        Console.WriteLine($"{color.ReadColor()}");
                    }
                    else
                    {            
                        Console.WriteLine(color.CalculateRawAverage());
                        /*        
                        var rgb = color.ReadRGBValues();  
                
                        Console.WriteLine($"Red: {rgb.Red}");
                        Console.WriteLine($"Blue: {rgb.Green}");
                        Console.WriteLine($"Green: {rgb.Blue}");
                        Console.WriteLine($"Ambient: {rgb.Blue}"); 

                        var rgb2 = color.ReadRGBColor();

                        Console.WriteLine($"Red2: {rgb.Red}");
                        Console.WriteLine($"Blue2: {rgb.Green}");
                        Console.WriteLine($"Green2: {rgb.Blue}");
                        */
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }

                await Task.Delay(1000);                
            }
        }

        private bool _handlingColorEvent = false;
        private async void HandleColorPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            var color = ((EV3ColorSensor)sender);

            if(!_handlingColorEvent)
            {
                _handlingColorEvent = true;

                Console.WriteLine("ColorChanged!");

                try
                {
                    if(color.ColorMode == ColorSensorMode.Color)
                    {
                        Console.WriteLine($"{color.ReadColor()}");
                    }
                    else
                    {                    
                        var rgb = color.ReadRGBValues();  
                
                        Console.WriteLine($"Red: {rgb.Red}");
                        Console.WriteLine($"Blue: {rgb.Green}");
                        Console.WriteLine($"Green: {rgb.Blue}");
                        Console.WriteLine($"Ambient: {rgb.Blue}"); 

                        var rgb2 = color.ReadRGBColor();

                        Console.WriteLine($"Red2: {rgb.Red}");
                        Console.WriteLine($"Blue2: {rgb.Green}");
                        Console.WriteLine($"Green2: {rgb.Blue}");
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }

                _handlingColorEvent = false;
            }
        }
    }
}
