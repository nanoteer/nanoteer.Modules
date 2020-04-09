using Nanoteer.Core;
using Nanoteer.Core.Serial;
using System;

namespace Nanoteer.Modules.GHI
{
    public class Rfid
    {
        private SerialSocket _serial;
        private readonly object objLock = new object();

        /// <summary>
        /// Represents the delegate that is used to handle the <see cref="CardRead"/> events.
        /// </summary>
        /// <param name="sender">The <see cref="Rfid"/> object that raised the event.</param>
        /// <param name="code">The code read in string format.</param>
        public delegate void CardReadEventHandler(Rfid sender, string code);

        /// <summary>
        /// Raised when a card is scanned by the <see cref="Rfid"/>.
        /// </summary>
        /// <remarks>
        /// Implement this event handler
        /// when you want to provide an action associated with a scanned card activity.
        /// </remarks>
        public event CardReadEventHandler CardRead
        {
            add
            {
                lock (objLock)
                {
                    onCardRead += value;
                }
            }
            remove
            {
                lock (objLock)
                {
                    onCardRead -= value;
                }
            }
        }

        private CardReadEventHandler onCardRead;
        /// <summary>
        /// Raises the <see cref="JoystickButtonStateChanged"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="Joystick"/> that raised the event.</param>
        /// <param name="JoystickState">The state of the Joystick.</param>
        protected virtual void OnCardRead(Rfid sender, string code)
        {
            onCardRead?.Invoke(sender, code);
        }

        public Rfid(int socketNumber)
        {
            Socket socket = Socket.AttachToSocket(socketNumber, SocketType.U);
            _serial = new SerialSocket(socket, (char)3);
            _serial.SerialBytesRead += SerialBytesRead;
        }

        private void SerialBytesRead(SerialSocket serial, byte[] dataRead)
        {
            string code = string.Empty;
            for (int i = 0; i < dataRead.Length; i++)
            {
                code += (char)dataRead[i];
            }
            if (dataRead.Length == 11)
            {
                // checksum already done
                OnCardRead(this, code);
                return;
            }
            else if (dataRead.Length >= 12)
            {
                // Verify checksum
                int cs = 0;
                for (int x = 1; x < 10; x += 2)
                {
                    cs ^= ParseHexValue(dataRead[x], dataRead[x + 1]);
                }
                if (dataRead[0] != 2 || dataRead[11] != cs)
                {
                    throw new NotImplementedException($"Checksum for pased card not verified : {dataRead}");
                }
                OnCardRead(this, code.Substring(1, 10));
                return;
            }
            throw new ArgumentOutOfRangeException("dataRead", $"Card type not managed : {dataRead}");
        }

        /// <summary>
        /// Concat left and right byte value to convert to the hexadecimal version
        /// </summary>
        /// <param name="upper">Left hexadecimal value</param>
        /// <param name="lower">Right hexadecimal value</param>
        /// <returns>The 2 concat bytes in hex form</returns>
        private static int ParseHexValue(byte upper, byte lower)
        {
            // Remember in ASCII 'A' == 65
            // And there are 7 ascii code between '9' and 'A'
            int high, low;

            if (upper > 64)
                high = upper - 48 - 7;
            else
                high = upper - 48;

            if (lower > 64)
                low = lower - 48 - 7;
            else
                low = lower - 48;

            int num = 0;
            num = high << 4;
            num = num | low;
            return num;
        }
    }
}
