using Muzubot;

var configuration = Configuration.FromEnvironment();
var bot = new Muzubot.Muzubot(configuration);
await bot.Run();