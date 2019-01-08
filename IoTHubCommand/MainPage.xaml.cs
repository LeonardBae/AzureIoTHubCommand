using System;
using Windows.UI.Xaml.Controls;
using Windows.Devices.Gpio;
using Microsoft.Azure.Devices.Client;
using System.Threading.Tasks;
using System.Text;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IoTHubCommand
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        const int LEDPinNumber = 5;
        private static GpioPin pin;
        private static GpioPinValue pinValue;


        public MainPage()
        {
            this.InitializeComponent();
            InitGPIO();
            receiveCommandFromAzure();

        }
        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                pin = null;
                return;
            }

            pin = gpio.OpenPin(LEDPinNumber);
            pinValue = GpioPinValue.High;
            pin.Write(pinValue);
            pin.SetDriveMode(GpioPinDriveMode.Output);
        }
        public async Task receiveCommandFromAzure()
        {
            DeviceClient deviceClient = DeviceClient.CreateFromConnectionString("HostName=mdsrmsolutions.azure-devices.net;DeviceId=rpmethod;SharedAccessKey=vKZ/g7AmG229c2n/xR6OU/xZhAqSW676seVORpIfpZk=", TransportType.Mqtt);

            Message receiveMessage;
            string messageData;
            while (true)
            {
                receiveMessage = await deviceClient.ReceiveAsync();
                if (receiveMessage != null)
                {
                    messageData = Encoding.ASCII.GetString(receiveMessage.GetBytes());
                    if (messageData == "TurnOnLight")
                    {
                        Console.WriteLine("Turn on the light...");
                        pinValue = GpioPinValue.Low;
                        pin.Write(pinValue);
                    }
                    else if (messageData == "TurnOffLight")
                    {
                        Console.WriteLine("Turn off the light...");
                        pinValue = GpioPinValue.High;
                        pin.Write(pinValue);
                    }
                    await deviceClient.CompleteAsync(receiveMessage);
                }
            }
        }
    }
}
