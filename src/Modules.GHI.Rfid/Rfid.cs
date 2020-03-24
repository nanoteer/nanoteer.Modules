using Nanoteer.Core;
using Nanoteer.Core.Serial;
using System;

namespace Nanoteer.Modules.GHI
{
    public class Rfid
    {
        private SerialSocket _serial;

        public Rfid(int socketNumber)
        {
            Socket socket = Socket.AttachToSocket(socketNumber, SocketType.U);
            _serial = new SerialSocket(socket, (char)3);
            _serial.SerialBytesRead += SerialBytesRead;
        }

        private void SerialBytesRead(SerialSocket serial, byte[] dataRead)
        {
            Console.Write("Bytes: >>");
            for (int i = 0; i < dataRead.Length; i++)
            {
                Console.Write(dataRead[i] + " ");
            }
            Console.WriteLine("<< ");
        }
    }
}
