using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace DMC_API
{
    public class Class1
    {
        Socket socket;

        public void Connect()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try { socket.Connect("100.100.100.66", 8010); }
            catch { }
        }

        public void Status()
        {
            List<byte> aryByte = new List<byte>();

            aryByte.AddRange(BitConverter.GetBytes(8));
            aryByte.AddRange(BitConverter.GetBytes(104));

            string result = BitConverter.ToString(aryByte.ToArray());

            socket.Send(aryByte.ToArray());

            byte[] header = new byte[8];

            int nchar = socket.Receive(header, 0, 8, SocketFlags.None);
            //  header[0] 위치에 8 바이트를 수신, 저장.
            if (nchar <= 0) return;

            int size = BitConverter.ToInt32(header, 0) - 8;
            int command = BitConverter.ToInt32(header, 4);

            byte[] data = new byte[size];
            nchar = socket.Receive(data, 0, size, SocketFlags.None);

            int index = 0;

            result = BitConverter.ToString(data);

            #region Status
            uint status = BitConverter.ToUInt32(data, index);
            index += 4;

            int error_code = BitConverter.ToInt32(data, index);
            index += 4;

            string maintask_name = System.Text.Encoding.UTF8.GetString(data, index, 20);
            maintask_name = maintask_name.Trim();
            index += 20;

            short maintask_status = BitConverter.ToInt16(data, index);
            index += 2;

            short maintask_curstep = BitConverter.ToInt16(data, index);
            index += 2;

            short maintask_movestep = BitConverter.ToInt16(data, index);
            index += 2;

            short subtask_status = BitConverter.ToInt16(data, index);
            index += 2;

            short subtask_curstep = BitConverter.ToInt16(data, index);
            index += 2;

            short subtask_dummy = BitConverter.ToInt16(data, index);
            index += 2;
            #endregion
        }

        public void Lock()
        {
            List<byte> aryByte = new List<byte>();

            aryByte.AddRange(BitConverter.GetBytes(8));
            aryByte.AddRange(BitConverter.GetBytes(2005));  // command - lock

            string result = BitConverter.ToString(aryByte.ToArray());

            socket.Send(aryByte.ToArray());

            byte[] header = new byte[8];
            int nchar = socket.Receive(header, 0, 8, SocketFlags.None);

            if (nchar <= 0) return;

            aryByte.Clear();

            #region Lock Status 취득하기.
            aryByte.AddRange(BitConverter.GetBytes(8));
            aryByte.AddRange(BitConverter.GetBytes(2007));

            socket.Send(aryByte.ToArray());
            nchar = socket.Receive(header, 0, 8, SocketFlags.None);

            if (nchar <= 0) return;

            int size = BitConverter.ToInt32(header, 0) - 8;
            int command = BitConverter.ToInt32(header, 4);

            byte[] data = new byte[size];

            nchar = socket.Receive(data, 0, size, SocketFlags.None);

            result = BitConverter.ToString(data);

            bool bLock = BitConverter.ToBoolean(data, 0);
            #endregion
        }

        public void ServoOn()
        {
            List<byte> aryByte = new List<byte>();

            aryByte.AddRange(BitConverter.GetBytes(8 + 20));
            aryByte.AddRange(BitConverter.GetBytes(104));

            uint[] cmdData = new uint[4];
            cmdData[2] = 0x1;

            aryByte.AddRange(BitConverter.GetBytes(4));
            aryByte.AddRange(BitConverter.GetBytes(cmdData[0]));
            aryByte.AddRange(BitConverter.GetBytes(cmdData[1]));
            aryByte.AddRange(BitConverter.GetBytes(cmdData[2]));
            aryByte.AddRange(BitConverter.GetBytes(cmdData[3]));

            string result = BitConverter.ToString(aryByte.ToArray());

            socket.Send(aryByte.ToArray());

            byte[] header = new byte[8];
            int nchar = socket.Receive(header, 0, 8, SocketFlags.None);

            if (nchar <= 0) return;

            int size = BitConverter.ToInt32(header, 0) - 8;
            int command = BitConverter.ToInt32(header, 4);

            byte[] data = new byte[size];

            nchar = socket.Receive(data, 0, size, SocketFlags.None);
        }
    }
}