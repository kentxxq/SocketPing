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
                              var endpoint = ParseUri(o.Url);
                              bool result;

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

        static IPEndPoint ParseUri(string url)
        {
            try
            {
                var host = url.Split(":")[0];
                var ip = Dns.GetHostAddresses(host)[0];
                var port = url.Split(":")[1];
                return new IPEndPoint(ip, int.Parse(port));
            }
            catch (Exception)
            {
                throw new ArgumentException("地址解析错误");
            }

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
