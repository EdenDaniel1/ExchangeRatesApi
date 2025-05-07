using RateFetcher;
using DotNetEnv;

var builder = Host.CreateApplicationBuilder(args);

Env.Load();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();