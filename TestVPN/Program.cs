﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TestVPN
{
    internal static class Program
    {
        private static string Host { get; set; }
        private static string Login { get; set; }
        private static string Password { get; set; }
        private static string Path { get; set; }
        private static Status CurrentStatus { get; set; } = Status.Disconnected;
        private static Commands Command { get; set; } = Commands.About;
        private static void Main()
        {
            Console.WriteLine("VPN service for demonstration");
            Console.WriteLine(new string('-', 30));
            Console.WriteLine("Info:\r\n1. connect\r\n2. disconnect\r\n3. about\r\n4. show\r\n5. exit");
            Console.WriteLine(new string('-', 30));

            while (Command != Commands.Exit)
            {
                Console.Write("Enter the command number: ");
                Command = (Commands)(Convert.ToInt32(Console.ReadLine()) - 1);
                switch (Command)
                {
                    case Commands.Connect:
                        if (CurrentStatus != Status.Connected)
                        {
                            Connect();
                            Console.WriteLine("Connection has been established\r\n");
                            CurrentStatus = Status.Connected;
                        }
                        else
                        {
                            Console.WriteLine("You need to disconnect\r\n");
                        }
                        break;
                    case Commands.Disconnect:
                        if (CurrentStatus != Status.Disconnected)
                        {
                            Disconnect();
                            Console.WriteLine("You've been disconnected\r\n");
                            CurrentStatus = Status.Disconnected;
                        }
                        else
                        {
                            Console.WriteLine("You don't have any vpn connections\r\n");
                        }

                        break;
                    case Commands.About:
                        //HERE WILL BE KURSOVAYA RABOTA LOGO SPLASH XPDDLSADASFWEG :)
                        break;
                    case Commands.ShowIp:
                        Console.WriteLine($"Your current ip address is: {GetLocalIpAddress()}\r\n");
                        break;
                    case Commands.Exit:
                        break;
                    default:
                        Console.WriteLine("1. connect\r\n2. disconnect\r\n3. about\r\n4. show\r\n5. exit\r\n");
                        break;
                }
            }
        }

        private static void Connect()
        {
            Console.Write("host: ");
            Host = Console.ReadLine();
            Console.Write("login: ");
            Login = Console.ReadLine();
            Console.Write("password: ");
            Password = Console.ReadLine();
            Path = Directory.GetCurrentDirectory();

            var sb = new StringBuilder();
            sb.AppendLine("[VPN]");
            sb.AppendLine("MEDIA=rastapi");
            sb.AppendLine("Port=VPN2-0");
            sb.AppendLine("Device=WAN Miniport (IKEv2)");
            sb.AppendLine("DEVICE=vpn");
            sb.AppendLine("PhoneNumber=" + Host);
            File.WriteAllText(Path + "\\VpnConnection.pbk", sb.ToString());

            sb = new StringBuilder();
            sb.AppendLine($"rasdial VPN {Login} {Password} /phonebook:{Path}\\VpnConnection.pbk");
            File.WriteAllText(Path + "\\VpnConnection.bat", sb.ToString());

            var connectProcess = new Process
            {
                StartInfo =
                {
                    FileName = Path + "\\VpnConnection.bat",
                    WindowStyle = ProcessWindowStyle.Normal
                }
            };

            connectProcess.Start();
            connectProcess.WaitForExit();
        }

        private static void Disconnect()
        {
            File.WriteAllText(Path + "\\VpnDisconnect.bat", "rasdial /d");

            var disconnectProcess = new Process
            {
                StartInfo =
                {
                    FileName = Path + "\\VpnDisconnect.bat",
                    WindowStyle = ProcessWindowStyle.Normal
                }
            };
            disconnectProcess.Start();
            disconnectProcess.WaitForExit();
        }

        private static string GetLocalIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}