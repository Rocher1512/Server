using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;


namespace locationserver
{
    class Program
    {
        public static string path = "Log.txt";
        public static string dictpath = "Dictionary.txt";
        public static Dictionary<string, string> myDict = new Dictionary<string, string>();
        static void Main(string[] args)
        {
            try
            {
                string[] lines = File.ReadAllLines(dictpath);
                for(int i = 0; i<lines.Length;i += 2)
                {
                    myDict.Add(lines[i], lines[i + 1]);
                }
            }
            catch
            {

            }
            
            runServer();
        }
        public static void runServer()
        {
            TcpListener listener;
            Socket connection;
            Handler request;
            try
            {
                listener = new TcpListener(IPAddress.Any, 43);
                listener.Start();
                listener.Server.SendTimeout = 1000;
                listener.Server.ReceiveTimeout = 1000;
                Console.WriteLine("Server Started Listening");
                while (true)
                {
                    connection = listener.AcceptSocket();
                    request = new Handler();
                    Thread t = new Thread(() => request.DoRequest(connection));
                    t.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine((e.ToString()));
            }
        }

    }
    class Handler
    {
        public void DoRequest(Socket connection)
        {
            NetworkStream socketStream = new NetworkStream(connection);
            Console.WriteLine("Conection Received");
            try
            {
                
                StreamWriter sw = new StreamWriter(socketStream);
                StreamReader sr = new StreamReader(socketStream);
                socketStream.ReadTimeout = 1000;
                socketStream.WriteTimeout = 1000;
                bool HTTP = false;
                string line = sr.ReadLine().Trim();
                if (line.Contains("-l"))
                {
                    string[] sectionslog = line.Split(new char[] { ' ' });
                    for(int i =0; i < sectionslog.Length; i++)
                    {
                        Console.WriteLine(sectionslog[i]);
                    }
                    Program.path = sectionslog[1];
                    using (StreamWriter tw = new StreamWriter(Program.dictpath))
                    {
                        tw.WriteLine("");
                    }
                        line = sr.ReadLine().Trim();
                }
                if (line.Contains("-f"))
                {
                    string[] sectionslog = line.Split(new char[] { ' ' });
                    Program.dictpath = sectionslog[1];
                    using (StreamWriter tw = new StreamWriter(Program.dictpath))
                    {
                        foreach (KeyValuePair<string, string> entry in Program.myDict)
                        {
                            tw.WriteLine(entry.Key);
                            tw.WriteLine(entry.Value);
                        }
                    }
                    line = sr.ReadLine().Trim();
                }
                string[] sections = line.Split(new char[] { ' ' });
                try
                {
                    sections = line.Split(new char[] { ' ' }, 2);
                    if (sections[0] == "GET" || sections[0] == "PUT" || sections[0] == "POST")
                    {
                        if (sections[1][0] == '/')
                        {
                            HTTP = true;
                        }
                    }
                }
                catch
                {
                    sections = line.Split(new char[] { ' ' });
                }

                Console.WriteLine(HTTP);
                if (HTTP == true)
                {
                    if (sections[0] == "GET")
                    {
                        sections = line.Split(new char[] { ' ' });
                        try
                        {

                            if (sections[2] == "HTTP/1.0")
                            {
                                string newline = "";
                                string output;
                                for (int i = 2; i < sections[1].Length; i++)
                                {
                                    newline += sections[1][i];
                                }
                                if (Program.myDict.TryGetValue(newline, out output))
                                {
                                    sw.WriteLine("HTTP/1.0 200 OK");
                                    sw.WriteLine("Content-Type: text/plain");
                                    sw.WriteLine("");
                                    sw.WriteLine(output);
                                    sw.Flush();
                                    using (StreamWriter tw = File.AppendText(Program.path))
                                    {
                                        DateTime localDate = DateTime.Now;
                                        tw.WriteLine(IPAddress.Loopback + "- -" + localDate + " GET " + newline + " OK");
                                    }
                                }
                                else
                                {
                                    sw.WriteLine("HTTP/1.0 404 Not Found");
                                    sw.WriteLine("Content-Type: text/plain");
                                    sw.WriteLine("");
                                    sw.Flush();
                                    using (StreamWriter tw = File.AppendText(Program.path))
                                    {
                                        DateTime localDate = DateTime.Now;
                                        tw.WriteLine(IPAddress.Loopback + "- -" + localDate + " GET " + newline + " UNKNOWN");
                                    }
                                }
                            }
                            else if (sections[2] == "HTTP/1.1")
                            {
                                string newline = "";
                                string output;

                                for (int i = 7; i < sections[1].Length; i++)
                                {
                                    newline += sections[1][i];
                                }
                                if (Program.myDict.TryGetValue(newline, out output))
                                {
                                    sw.WriteLine("HTTP/1.1 200 OK");
                                    sw.WriteLine("Content-Type: text/plain");
                                    sw.WriteLine("");
                                    sw.WriteLine(output);
                                    sw.Flush();
                                    using (StreamWriter tw = File.AppendText(Program.path))
                                    {
                                        DateTime localDate = DateTime.Now;
                                        tw.WriteLine(IPAddress.Loopback + "- -" + localDate + " GET " + newline + " OK");
                                    }
                                }
                                else
                                {
                                    sw.WriteLine("HTTP/1.1 404 Not Found");
                                    sw.WriteLine("Content-Type: text/plain");
                                    sw.WriteLine("");
                                    sw.Flush();
                                    using (StreamWriter tw = File.AppendText(Program.path))
                                    {
                                        DateTime localDate = DateTime.Now;
                                        tw.WriteLine(IPAddress.Loopback + "- -" + localDate + " GET " + newline + " UNKNOWN");
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            sections = line.Split(new char[] { ' ' }, 2);
                            string output;
                            string newline = "";
                            for (int i = 5; i < line.Length; i++)
                            {
                                newline += line[i];
                            }
                            if (Program.myDict.TryGetValue(newline, out output))
                            {
                                sw.WriteLine("HTTP/0.9 200 OK");
                                sw.WriteLine("Content-Type: text/plain");
                                sw.WriteLine("");
                                sw.WriteLine(output);
                                sw.Flush();
                                using (StreamWriter tw = File.AppendText(Program.path))
                                {
                                    DateTime localDate = DateTime.Now;
                                    tw.WriteLine(IPAddress.Loopback + "- -" + localDate + " GET " + newline + " OK");
                                }
                            }
                            else
                            {
                                sw.WriteLine("HTTP/0.9 404 Not Found");
                                sw.WriteLine("Content-Type: text/plain");
                                sw.WriteLine("");
                                sw.Flush();
                                using (StreamWriter tw = File.AppendText(Program.path))
                                {
                                    DateTime localDate = DateTime.Now;
                                    tw.WriteLine(IPAddress.Loopback + "- -" + localDate + " GET " + newline + " UNKNOWN");
                                }
                            }
                        }
                        sections = line.Split(new char[] { ' ' }, 2);
                    }
                    else if (sections[0] == "PUT")
                    {
                        string output;
                        string newline = "";
                        string temp = sr.ReadLine().Trim();
                        string location = sr.ReadLine().Trim();
                        for (int i = 5; i < line.Length; i++)
                        {
                            newline += line[i];
                        }
                        if (Program.myDict.TryGetValue(newline, out output))
                        {
                            Program.myDict[newline] = location;
                        }
                        else
                        {
                            Program.myDict.Add(newline, location);
                        }
                        sw.WriteLine("HTTP/0.9 200 OK");
                        sw.WriteLine("Content-Type: text/plain");
                        sw.WriteLine("");
                        sw.Flush();
                        using (StreamWriter tw = File.AppendText(Program.path))
                        {
                            DateTime localDate = DateTime.Now;
                            tw.WriteLine(IPAddress.Loopback + "- -" + localDate + " PUT " + newline + " " + location + " OK");
                        }
                    }
                    else if (sections[0] == "POST")
                    {
                        sections = line.Split(new char[] { ' ' });
                        Console.WriteLine(sections[2]);
                        if (sections[2] == "HTTP/1.0")
                        {
                            string newline = "";
                            string output;
                            string temp = sr.ReadLine().Trim();
                            string temp2 = sr.ReadLine().Trim();
                            string location = sr.ReadLine().Trim();
                            for (int i = 6; i < line.Length - 9; i++)
                            {
                                newline += line[i];
                            }
                            Console.WriteLine(newline);
                            if (Program.myDict.TryGetValue(newline, out output))
                            {
                                Program.myDict[newline] = location;
                            }
                            else
                            {
                                Console.WriteLine("unless");
                                Program.myDict.Add(newline, location);
                            }
                            sw.WriteLine("HTTP/1.0 200 OK");
                            sw.WriteLine("Content-Type: text/plain");
                            sw.WriteLine("");
                            sw.Flush();
                            using (StreamWriter tw = File.AppendText(Program.path))
                            {
                                DateTime localDate = DateTime.Now;
                                tw.WriteLine(IPAddress.Loopback + "- -" + localDate + " POST " + newline + " " + location + " UNKNOWN");
                            }
                        }
                        if (sections[2] == "HTTP/1.1")
                        {
                            string newline = "";
                            string output;
                            string temp = sr.ReadLine().Trim();
                            string temp2 = sr.ReadLine().Trim();
                            string temp3 = sr.ReadLine().Trim();
                            string combo = sr.ReadLine().Trim();
                            string location = "";
                            char c = "&"[0];
                            char d = "="[0];
                            for (int i = 5; i < combo.Length; i++)
                            {
                                if (combo[i] != c)
                                {
                                    newline += combo[i];
                                }
                                else
                                {
                                    break;
                                }
                            }
                            bool found = false;
                            for (int i = 0; i < combo.Length; i++)
                            {
                                try
                                {
                                    if (combo[i - 1] == d)
                                    {
                                        if (combo[i - 2] == 'n')
                                        {
                                            if (combo[i - 3] == 'o')
                                            {
                                                found = true;
                                            }
                                        }
                                    }
                                }
                                catch { }
                                if (found == true)
                                {
                                    location += combo[i];
                                }
                            }
                            if (Program.myDict.TryGetValue(newline, out output))
                            {
                                Program.myDict[newline] = location;
                            }
                            else
                            {
                                Program.myDict.Add(newline, location);
                            }
                            sw.WriteLine("HTTP/1.1 200 OK");
                            sw.WriteLine("Content-Type: text/plain");
                            sw.WriteLine("");
                            sw.Flush();
                            using (StreamWriter tw = File.AppendText(Program.path))
                            {
                                DateTime localDate = DateTime.Now;
                                tw.WriteLine(IPAddress.Loopback + "- -" + localDate + " POST " + newline + " " + location + " UNKNOWN");
                            }
                        }
                    }
                }
                else if (sections.Length == 1)
                {
                    Console.WriteLine("finding a User");
                    string output;
                    if (Program.myDict.TryGetValue(sections[0], out output))
                    {
                        Console.WriteLine(output);
                        sw.WriteLine(output);
                        sw.Flush();
                        using (StreamWriter tw = File.AppendText(Program.path))
                        {
                            DateTime localDate = DateTime.Now;
                            tw.WriteLine(IPAddress.Loopback + "- -" + localDate + " " + sections[0] +" OK");
                        }
                    }
                    else
                    {
                        Console.WriteLine("ERROR: no entries found");
                        sw.WriteLine("ERROR: no entries found");
                        sw.Flush();
                        using (StreamWriter tw = File.AppendText(Program.path))
                        {
                            DateTime localDate = DateTime.Now;
                            tw.WriteLine(IPAddress.Loopback + "- -" + localDate + " " + sections[0] + " UNKNOWN");
                        }
                    }
                }
                else
                {
                    sections = line.Split(new char[] { ' ' }, 2);
                    Console.WriteLine("Saving");

                    string output;
                    if (Program.myDict.TryGetValue(sections[0], out output))
                    {
                        Program.myDict[sections[0]] = sections[1];
                    }
                    else
                    {
                        Program.myDict.Add(sections[0], sections[1]);
                    }
                    Console.WriteLine("OK");
                    sw.WriteLine("OK");
                    sw.Flush();
                    using (StreamWriter tw = File.AppendText(Program.path))
                    {
                        DateTime localDate = DateTime.Now;
                        tw.WriteLine(IPAddress.Loopback + "- -" + localDate + " " + sections[0] + " " + sections[1] + " UNKNOWN");
                    }
                }
                
                connection.Close();
                socketStream.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("No Arguments given");
                socketStream.Close();
                connection.Close();
                Console.WriteLine("Server Timed out");
                Console.WriteLine((e.ToString()));
            }
        }
    }
}
