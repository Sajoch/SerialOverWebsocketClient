// See https://aka.ms/new-console-template for more information

using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SerialOverWebsocketClient;
using SerialOverWebsocketClient.PseudoTerminal;
using Serilog;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json");

var configuration = builder.Build();

var services = new ServiceCollection();
services.AddSingleton(configuration);
services.AddSingleton<Startup>();
services.AddSingleton<RemoteSerialService>();
services.AddSingleton<AuthorizationService>();

if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
    services.AddTransient<IPseudoTerminal, PosixPseudoTerminal>();
else
    services.AddTransient<IPseudoTerminal, DummyPseudoTerminal>();

services.AddSingleton<PseudoTerminalService>();

services.Configure<AuthorizationOptions>(configuration.GetSection("Authorization"));
services.Configure<ConnectionOptions>(configuration.GetSection("Remote"));

var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

services.AddSingleton(logger);

var serviceProvider = services.BuildServiceProvider(true);
var startup = serviceProvider.GetRequiredService<Startup>();
await startup.Run();