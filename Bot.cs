using AellaDiscordBot.Bots.Commands;
using AellaDiscordBot.Bots.Commons;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AellaDiscordBot.Bots
{
    public class Bot
    {
        public DiscordClient Client { get; private set; }
        public CommandsNextExtension Commands { get; private set; }
        public InteractivityConfiguration Interactivity { get; private set; }

        public ConfigJson Config { get; private set; }

        public GachaTable Gacha { get; private set; }

        public DBConnector Database { get; private set; }

        public async Task RunAsync()
        {

            Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/data");

            Config = RegisterConfig().Result;

            Database = new DBConnector(Config.NadekoBotDB);
            Gacha = new GachaTable(Config);

            var discordConfig = new DiscordConfiguration
            {
                Token = Config.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true,
            };

            Client = new DiscordClient(discordConfig);

            Client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(2),
                
            });


            RegisterEvents();
            RegisterCommands(Config.Prefix);


            DiscordActivity discordActivity = new DiscordActivity("Lunae TCG - Pulling for UR Penthesilea", ActivityType.Playing);

            await Client.ConnectAsync(discordActivity).ConfigureAwait(false);

            await Task.Delay(-1);

        }

        private async Task<ConfigJson> RegisterConfig()
        {
            var json = string.Empty;
            using (var fs = File.OpenRead("data/config.json"))
            using (StreamReader sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            Console.WriteLine("Loaded Config file.");

            return configJson;
        }
        
        private void RegisterEvents()
        {
            Client.Ready += OnClientReady;
        }

        private void RegisterCommands(string prefix)
        {
            // Command Systems
            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { prefix },
                EnableMentionPrefix = true,
                EnableDms = false,
                IgnoreExtraArguments = true
            };

            Commands = Client.UseCommandsNext(commandsConfig);
            

            Commands.RegisterCommands<CommandGacha>();
            Commands.RegisterCommands<CommandRaids>();
        }

        private Task OnClientReady(ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }

    }
}
