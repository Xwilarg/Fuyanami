using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordUtils;
using Fuyanami.Module;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fuyanami
{
    public class Program
    {
        public static void Main(string[] args)
               => new Program().MainAsync().GetAwaiter().GetResult();

        public DiscordSocketClient Client { private set; get; }
        private readonly CommandService _commands = new CommandService();
        public static Program P;
        public static string SudoPassword;
        public static string[] AllowedIds;
        public static string[] AllowedPrograms;

        private string logsFolder, reportsFolder;

        Timer timer;

        public DateTime StartTime { private set; get; }

        private Program()
        {
            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
            });
            Client.Log += Utils.Log;
            _commands.Log += Utils.LogErrorAsync;
        }

        private async Task MainAsync()
        {
            var json = JsonConvert.DeserializeObject<JObject>(File.ReadAllText("Keys/Credentials.json"));
            if (json["botToken"] == null || json["sudoPassword"] == null || json["allowedIds"] == null || json["allowedPrograms"] == null
                || json["logsFolder"] == null || json["reportsFolder"] == null)
                throw new NullReferenceException("Invalid Credentials file");

            logsFolder = json["logsFolder"].Value<string>();
            reportsFolder = json["reportsFolder"].Value<string>();

            timer = new Timer(new TimerCallback(UploadReports), null, 0, 60 * 60 * 1000); // Called every hour

            P = this;

            await _commands.AddModuleAsync<Communication>(null);
            await _commands.AddModuleAsync<Info>(null);
            await _commands.AddModuleAsync<Systemctl>(null);

            Client.MessageReceived += HandleCommandAsync;

            SudoPassword = json["sudoPassword"].Value<string>();
            AllowedIds = json["allowedIds"].Value<JArray>().Select(x => (string)x).ToArray();
            AllowedPrograms = json["allowedPrograms"].Value<JArray>().Select(x => (string)x).ToArray();

            StartTime = DateTime.Now;
            await Client.LoginAsync(TokenType.Bot, json["botToken"].Value<string>());
            await Client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            SocketUserMessage msg = arg as SocketUserMessage;
            if (msg == null || arg.Author.IsBot) return;
            int pos = 0;
            if (msg.HasMentionPrefix(Client.CurrentUser, ref pos) || msg.HasStringPrefix("f.", ref pos))
            {
                SocketCommandContext context = new SocketCommandContext(Client, msg);
                await _commands.ExecuteAsync(context, pos, null);
            }
        }

        private void UploadReports(object _)
        {
            foreach (string f in Directory.GetFiles(logsFolder))
            {
                if (f.EndsWith(".log"))
                {
                    var fi = new FileInfo(f);
                    var p = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            FileName = "goaccess",
                            Arguments = f + " -a --log-format CADDY"
                        }
                    };
                    p.Start();
                    string output = p.StandardOutput.ReadToEnd();
                    p.WaitForExit();
                    File.WriteAllText(reportsFolder + "/" + fi.Name + ".html", output);
                }
            }
        }
    }
}
