using System;
using System.IO.Ports;

namespace LePeso
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            SerialPort serialPort1 = new SerialPort();
            SerialPort serialPort2 = new SerialPort();
            byte[] buffer1 = new byte[2];
            byte[] buffer2 = new byte[1000];
            string str1 = "";
            string str2 = "";
            string[] portNames = SerialPort.GetPortNames();
            int num1 = 1000;
            buffer1[0] = (byte)27;
            buffer1[1] = (byte)229;

            foreach (string str3 in portNames)
            {
                int num2 = 0;
                Console.WriteLine("\r" + str3);
                SerialPort serialPort3 = new SerialPort(str3, 9600, Parity.None, 8, StopBits.One);

                try
                {
                    serialPort3.Open();
                    serialPort3.Write(buffer1, 0, 2);
                    DateTime now2 = DateTime.Now;

                    while (serialPort3.BytesToRead < 17 && DateTime.Compare(now2.AddMilliseconds(num1), DateTime.Now) >= 0)
                        Console.Write("\rAguardando dados da impressora.");

                    Console.Write("\r                                  ");

                    if (serialPort3.BytesToRead > 16)
                    {
                        num2 = serialPort3.BytesToRead;
                        serialPort3.Read(buffer2, 0, serialPort3.BytesToRead);
                    }

                    serialPort3.Close();

                    if (num2 == 0 || buffer2[0] != (byte)58)
                    {
                        serialPort3 = new SerialPort(str3, 9600, Parity.None, 8, StopBits.One);
                        serialPort3.Open();
                        now2 = DateTime.Now;

                        while (serialPort3.BytesToRead < 17 && DateTime.Compare(now2.AddMilliseconds(num1), DateTime.Now) >= 0)
                            Console.Write("\rAguardando dados da balança.       ");

                        Console.Write("\r                                  ");

                        if (serialPort3.BytesToRead > 16)
                            serialPort3.Read(buffer2, 0, serialPort3.BytesToRead);
                        else
                            serialPort3.DiscardInBuffer();

                        serialPort3.Close();
                    }

                    for (int index1 = 0; index1 < buffer2.Length; ++index1)
                    {
                        if (buffer2[index1] == (byte)63)
                        {
                            for (int index2 = index1; index2 < buffer2.Length - 1; ++index2)
                                buffer2[index2] = buffer2[index2 + 1];
                        }
                    }

                    if (buffer2[0] == (byte)58)
                    {
                        if (str1 == "")
                        {
                            str1 = str3;
                            for (int index = 0; index < buffer2.Length; ++index)
                                buffer2[index] = (byte)0;
                        }
                    }
                    else
                    {
                        for (int index = 0; index < buffer2.Length; ++index)
                        {
                            if (buffer2[index] == (byte)2 && buffer2[index + 1] == (byte)51)
                            {
                                str2 = str3;
                                Console.WriteLine("Porta Balança: " + str2);
                                break;
                            }
                        }
                    }

                    if (str2 != "" && str1 != "")
                        break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Porta " + str3 + " não pode ser utilizada.");
                    Console.ReadKey();
                    return;
                }
            }

            if (str1 == "")
            {
                Console.WriteLine("\nImpressora não encontrada.");
                Console.ReadKey();
            }
            else if (str2 == "")
            {
                Console.WriteLine("Balança não encontrada.");
                Console.ReadKey();
            }
            else
            {
                serialPort2.PortName = str2;
            }
        }
    }
}
