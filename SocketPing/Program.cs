using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using CommandLine;

namespace SocketPing
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                          .WithParsed<Options>(o =>
                          {
                              IPEndPoint endpoint = null!;
                              bool result;
                              try
                              {
                                  var host = o.Url.Split(":")[0];
                                  var ip = Dns.GetHostAddresses(host)[0];
                                  var port = o.Url.Split(":")[1];
                                  endpoint = new IPEndPoint(ip, int.Parse(port));
                              }
                              catch (Exception)
                              {
                                  Console.WriteLine("地址解析失败");
                                  Environment.Exit(1);
                              }


                              if (o.RetryTimes == 0)
                              {
                                  while (true)
                                  {
                                      result = TestConnect(endpoint, o.Timeout);
                                      if (result && o.Quit)
                                      {
                                          Environment.Exit(0);
                                      }
                                  }
                              }
                              else
                              {
                                  for (int i = 0; i < o.RetryTimes; i++)
                                  {
                                      result = TestConnect(endpoint, o.Timeout);
                                      if (result && o.Quit)
                                      {
                                          Environment.Exit(0);
                                      }
                                  }
                                  Environment.Exit(1);
                              }

                          });
        }

        static bool TestConnect(IPEndPoint iPEndPoint, int timeout)
        {
            using (var tcp = new TcpClient())
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var result = tcp.ConnectAsync(iPEndPoint.Address, iPEndPoint.Port).Wait(timeout * 1000);
                stopwatch.Stop();
                if (result)
                {
                    Console.WriteLine($"{iPEndPoint}:连接成功,耗时{stopwatch.ElapsedMilliseconds}毫秒");
                    return true;
                }
                else
                {
                    Console.WriteLine($"{iPEndPoint}:连接失败,耗时{stopwatch.ElapsedMilliseconds}毫秒");
                    return false;
                }
            }
        }
    }
}
