// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SerialOverWebsocketClient;
using SerialOverWebsocketClient.PseudoTerminal;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("Settings/appsettings.json");

var configuration = builder.Build();

var services = new ServiceCollection();
services.AddSingleton(configuration);
services.AddSingleton<Startup>();
services.AddSingleton<RemoteSerialService>();
services.AddSingleton<AuthorizationService>();

services.AddTransient<TcpPseudoTerminal>();
services.AddTransient<HttpPseudoTerminal>();

services.AddSingleton<PseudoTerminalService>();

services.Configure<AuthorizationOptions>(configuration.GetSection("Authorization"));
services.Configure<ConnectionOptions>(configuration.GetSection("Remote"));

var serviceProvider = services.BuildServiceProvider(true);
var startup = serviceProvider.GetRequiredService<Startup>();
await startup.Run();