using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace SocketPing
{
    public class Options
    {
        [Usage(ApplicationAlias = "sp")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                return new List<Example>() {
                        new Example("connect host kentxxq.com,port 443", new Options { Url = "kentxxq.com:443" })
                };
            }
        }

        [Value(0, MetaName = "url", HelpText = "ip:port", Required = true)]
        public string Url { get; set; } = null!;

        [Option('n', "retryTimes", Required = false, HelpText = "default:0,retry forever")]
        public int RetryTimes { get; set; } = 0;

        [Option('t', "timeout", Required = false, HelpText = "default:2 seconds")]
        public int Timeout { get; set; } = 2;

        [Option('q', "quit", Required = false, HelpText = "Quit after connection succeeded")]
        public bool Quit { get; set; } = false;
    }
}
