using Nanoteer.Core;
using Nanoteer.Core.Gpio;

namespace Nanoteer.Modules.GHI
{
    public class Led7R
    {
        private DigitalOutput[] leds = new DigitalOutput[7];
        public int TotalLeds => 7;

        /// <summary></summary>
        /// <param name="socketNumber">The socket that this module is plugged in to.</param>
        public Led7R(int socketNumber)
        {
            // This finds the Socket instance from the user-specified socket number.  
            // This will generate user-friendly error messages if the socket is invalid.
            // If there is more than one socket on this module, then instead of "null" for the last parameter, 
            // put text that identifies the socket to the user (e.g. "S" if there is a socket type S)
            Socket socket = Socket.AttachToSocket(socketNumber, SocketType.Y);
            for (int i = 0; i < leds.Length; i++)
            {
                leds[i] = new DigitalOutput(socket, socket.SocketPins[(int)SocketPin.Three + i]);
            }
        }

        public void SwitchLed(int indexLed, bool status)
        {
            leds[indexLed].Write(status);
        }
    }
}
