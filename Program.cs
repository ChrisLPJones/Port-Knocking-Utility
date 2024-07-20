using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using BCrypt.Net;
using System.Threading;

namespace Knock
{
    class Program
    {
        private static readonly byte[] packetData = new byte[] {0x00};
            static void Main(string[] args)
            {
                string hash = "$2a$11$bVqrQ78XHuaJ5Vxwsn7S7uW0.sgBIpkxgpQsu9kq9Lyu6UFkqss22"; // = password123

                Console.Write("Enter Username: ");
                string plainTextUserName = Console.ReadLine();

                Console.Write("Enter Password: ");
                string plainTextPassword = ReadPassword();

                if (BCrypt.Net.BCrypt.Verify(plainTextPassword, hash))
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Login Successful!!");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine();
                    Console.Write("Please enter server address: ");
                    string host = Console.ReadLine();
                    Console.Clear();

                    IPAddress address = ResolveHost(host);
                    int[] sshPorts = { 7000, 8000, 9000 };
                    int[] rdpPorts = { 4000, 5000, 6000 };
                    bool alive = true;

                    while (alive)
                    {
                        Console.Clear();
                        Console.WriteLine($"Connecting to {plainTextUserName}@{address}");
                        Console.WriteLine();
                        Console.WriteLine("1) Connect with SSH");
                        Console.WriteLine("2) Connect with SFTP");
                        Console.WriteLine("3) Connect with RDP");
                        Console.WriteLine("4) Exit");
                        string choice = Console.ReadLine();

                        switch (choice)
                        {
                            // Connect with SSH
                            case "1":
                                string protocol = "ssh";
                                Console.Clear();
                                Connection(protocol, plainTextUserName, address, sshPorts);
                                continue;

                            // Connect with SFTP
                            case "2":
                                protocol = "sftp";
                                Console.Clear();
                                Connection(protocol, plainTextUserName, address, sshPorts);
                                continue;

                            case "3":
                                protocol = "rdp";
                                Console.Clear();
                                Connection(protocol, plainTextUserName, address, rdpPorts);
                                continue;

                            // Exit
                            case "4":
                                Console.Clear();
                                alive = false;
                                break;

                            default:
                                Console.WriteLine("Incorrect entry");
                                continue;
                        }
                    };
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Login Failed!");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.ReadKey();
                }
            }
        private static void OpenPort(IPAddress address, int[] ports)
        {
            Console.WriteLine("Opening Port");

            Socket openSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            foreach (int port in ports)
            {
                Thread.Sleep(1000);
                openSocket.SendTo(packetData, new IPEndPoint(address, port));
            }
        }

        private static void ClosePort(IPAddress address, int[] sshPorts)
        {
            Console.WriteLine("Closing Port");

            Socket closeSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            foreach (int kPort in sshPorts.Reverse())
            {
                Thread.Sleep(1000);
                closeSocket.SendTo(packetData, new IPEndPoint(address, kPort));
            }
        }

        private static void MakeConnection(string protocol, string plainTextUserName, IPAddress address)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();

            startInfo.FileName = "C:\\Windows\\System32\\cmd.exe";
            string sftpCommand = $"C:\\Windows\\System32\\OpenSSH\\sftp.exe -oPort=22 {plainTextUserName}@{address}";
            string sshCommand = $"C:\\Windows\\System32\\OpenSSH\\ssh.exe -p 22 {plainTextUserName}@{address}";
            string rdpCommand = $"C:\\Windows\\System32\\mstsc.exe /v:{address}:3389";

            if (protocol == "ssh")
            {
                startInfo.Arguments = $"/C \"{sshCommand}\"";
            }
            else
            if (protocol == "sftp")
            {
                startInfo.Arguments = $"/C \"{sftpCommand}\"";
            }
            else
            if (protocol == "rdp")
            {
                startInfo.Arguments = $"/C \"{rdpCommand}\"";
            }

            Process process = new Process();
            process.StartInfo = startInfo;
            Console.WriteLine($"Connecting to {address} with {protocol}");
            process.Start();
            process.WaitForExit();
        }

        static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo keyInfo;

            do
            {
                keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password = password.Substring(0, password.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else if (keyInfo.Key != ConsoleKey.Enter)
                {
                    password += keyInfo.KeyChar;
                    Console.Write("*");
                }
            } while (keyInfo.Key != ConsoleKey.Enter);

            Console.WriteLine();

            return password;
        }

        static void Connection(string protocol, string plainTextUserName, IPAddress address, int[] ports)
        {
            OpenPort(address, ports);

            Console.WriteLine("Making Connection");

            MakeConnection(protocol, plainTextUserName, address);

            ClosePort(address, ports);
        }

        static IPAddress ResolveHost(string host)
        {
            IPAddress address = null;
            bool attempt = true;

            while (attempt)
            {
                try
                {
                    address = Dns.GetHostAddresses(host)[0];
                    attempt = false;
                }
                catch (Exception)
                {
                    Console.WriteLine($"Failed to resolve host: {host}");
                    Console.Write("Please re-enter hostname: ");
                    host = Console.ReadLine();
                }

            }
            return address;
        }
    }
}