using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace location
{
    class Request
    {
        string[] args;
        public string windowsresponse;
        public Request(string[] vs)
        {
            args = vs;
            requesting();
        }

        public void requesting()
        {
            //whois.net.dcs.hull.ac.uk
            string servername = ("localhost");
            int portnumber = 43;
            int timeout = 1000;
            bool debug = false;
            int sendextra = 0;
            string filelog = "";
            List<string> input = new List<string>();
            input = args.ToList();
            if (input.Contains("-h"))
            {
                int i = input.IndexOf("-h");
                servername = input[i + 1];
                input.Remove(servername);
                input.Remove("-h");
            }
            if (input.Contains("-p"))
            {
                int i = input.IndexOf("-p");
                try
                {
                    portnumber = int.Parse(input[i + 1]);
                }
                catch
                {
                    Console.WriteLine("Error not a number given");
                }
                input.Remove(portnumber.ToString());
                input.Remove("-p");
            }

            if (input.Contains("-t"))
            {
                int i = input.IndexOf("-t");
                try
                {
                    timeout = int.Parse(input[i + 1]);
                }
                catch
                {
                    Console.WriteLine("Error not a number given");
                }
                input.Remove(timeout.ToString());
                input.Remove("-t");
            }
            if (input.Contains("-d"))
            {
                input.Remove("-d");
                debug = true;
            }

            if (input.Contains("-l"))
            {
                int i = input.IndexOf("-l");
                filelog = input[i + 1];
                sendextra = 1;
                input.Remove("-d");
                input.Remove(filelog);

            }
            if (input.Contains("-f"))
            {
                int i = input.IndexOf("-f");
                filelog = input[i + 1];
                sendextra = 2;
                input.Remove("-f");
                input.Remove(filelog);
            }
            TcpClient client = new TcpClient();
            client.Connect(servername, portnumber);
            client.SendTimeout = timeout;
            client.ReceiveTimeout = timeout;
            StreamWriter sw = new StreamWriter(client.GetStream());
            StreamReader sr = new StreamReader(client.GetStream());
            if (sendextra == 1)
            {
                sw.WriteLine(" -l " + filelog);
            }
            else if (sendextra == 2)
            {
                sw.WriteLine(" -f " + filelog);
            }
            if (input.Contains("-h1"))
            {
                input.Remove("-h1");
                args = input.ToArray();

                if (args.Length == 1)
                {
                    sw.WriteLine("GET /?name=" + args[0] + " HTTP/1.1");
                    sw.WriteLine("Host: " + servername);
                    sw.WriteLine("" + "\r\nConnection: Close\r\n");
                    sw.Flush();
                    string response = sr.ReadToEnd();
                    bool website = false;
                    for (int i = 0; i < response.Length; i++)
                    {
                        try
                        {
                            if (response[i] == '<')
                            {
                                if (response[i + 1] == 'h')
                                {
                                    website = true;
                                    break;
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                    if (website == false)
                    {
                        response += sr.ReadLine();
                        string location = "";
                        string temp = "";
                        for (int i = 45; i < response.Length; i++)
                        {
                            temp += response[i];
                        }

                        for (int i = 0; i < temp.Length - 2; i++)
                        {
                            location += temp[i];
                        }
                        Console.WriteLine(args[0] + " is " + location);
                        windowsresponse = (args[0] + " is " + location);
                    }
                    else
                    {
                        string newmessage = args[0] + " is ";
                        bool read = false;
                        for (int i = 0; i < response.Length; i++)
                        {
                            try
                            {
                                if (read == true)
                                {
                                    if (response[i + 1] == 'H')
                                    {
                                        if (response[i + 2] == 'T')
                                        {
                                            read = false;
                                        }
                                    }
                                    newmessage += response[i];
                                }
                                else
                                {
                                    if (response[i + 1] == '<')
                                    {
                                        if (response[i + 2] == 'h')
                                        {
                                            read = true;
                                        }
                                    }
                                }
                            }
                            catch
                            {

                            }

                        }
                        Console.WriteLine(newmessage);
                        windowsresponse = newmessage;
                    }
                }
                else
                {
                    sw.WriteLine("POST / HTTP/1.1");
                    sw.WriteLine("Host: " + servername);
                    string tocount = ("name=" + input[0] + "&location=" + input[1]);
                    sw.WriteLine("Content-Length: " + tocount.Count());
                    sw.WriteLine("");
                    sw.WriteLine(tocount);
                    sw.Flush();
                    try
                    {
                        string response = sr.ReadLine();
                    }
                    catch
                    {
                    }
                    Console.WriteLine(input[0] + " location changed to be " + input[1]);
                    windowsresponse = (input[0] + " location changed to be " + input[1]);
                    sw.Close();
                }
            }
            else if (input.Contains("-h0"))
            {
                input.Remove("-h0");
                args = input.ToArray();
                if (args.Length == 1)
                {
                    sw.WriteLine("GET /?" + args[0] + " HTTP/1.0");
                    sw.WriteLine("");
                    sw.Flush();
                    string response = sr.ReadLine();
                    response += sr.ReadLine();
                    response += sr.ReadLine();
                    response += sr.ReadLine();
                    response += sr.ReadLine();
                    string location = "";
                    for (int i = 39; i < response.Length; i++)
                    {
                        location += response[i];
                    }

                    Console.WriteLine(args[0] + " is " + location);
                    windowsresponse = (args[0] + " is " + location);
                }
                else if (args.Length == 2)
                {
                    sw.WriteLine("POST /" + args[0] + " HTTP/1.0");
                    sw.WriteLine("Content-Length: " + input[1].Count());
                    sw.WriteLine("");
                    sw.WriteLine(input[1]);
                    sw.Flush();
                    string response = sr.ReadLine();
                    Console.WriteLine(args[0] + " location changed to be " + input[1]);
                    windowsresponse = (args[0] + " location changed to be " + input[1]);
                    sw.Close();
                }
            }
            else if (input.Contains("-h9"))
            {
                input.Remove("-h9");
                args = input.ToArray();
                if (args.Length == 1)
                {
                    sw.WriteLine("GET /" + args[0]);
                    sw.Flush();
                    string response = sr.ReadToEnd();
                    string newline = "";
                    for (int i = 45; i < response.Length; i++)
                    {
                        newline += response[i];
                    }
                    Console.WriteLine(args[0] + " is " + newline);
                    windowsresponse = (args[0] + " is " + newline);
                }
                else if (args.Length == 2)
                {
                    sw.WriteLine("PUT /" + args[0]);
                    sw.WriteLine("");
                    sw.WriteLine(args[1]);
                    sw.Flush();
                    string response = sr.ReadLine();
                    Console.WriteLine(args[0] + " location changed to be " + args[1]);
                    windowsresponse = (args[0] + " location changed to be " + args[1]);
                    sw.Close();
                }

            }
            else
            {
                args = input.ToArray();
                try
                {
                    if (args.Length == 1)
                    {
                        sw.WriteLine(args[0]);
                        sw.Flush();

                        string response1 = sr.ReadToEnd();
                        if (response1.Contains("ERROR: no entries found"))
                        {
                            Console.WriteLine(response1);
                            windowsresponse = response1;
                        }
                        else
                        {
                            Console.WriteLine(args[0] + " is " + response1);
                            windowsresponse = (args[0] + " is " + response1);
                        }
                    }
                    else if (args.Length == 2)
                    {
                        try
                        {

                            sw.WriteLine(args[0] + " " + args[1]);
                            sw.Flush();
                            string response = sr.ReadLine();
                            Console.WriteLine(args[0] + " location changed to be " + args[1]);
                            windowsresponse = (args[0] + " location changed to be " + args[1]);
                            Console.WriteLine();
                            sw.Close();
                        }
                        catch
                        {
                            client.Close();
                            sw.Close();
                            sr.Close();
                            Console.WriteLine("Client Timeout");
                            windowsresponse = ("Client Timeout");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Incorrect number of arguments");
                        windowsresponse = ("Incorrect number of arguments");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine("Something Went wrong");
                    windowsresponse = ("Something Went wrong");
                }
            }
            Console.WriteLine();
        }
    }
}
