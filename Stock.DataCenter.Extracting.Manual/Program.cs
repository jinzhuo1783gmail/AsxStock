// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stock.Asx.DataCenter.EFCore;
using static System.Net.Mime.MediaTypeNames;




var builder = Host.CreateApplicationBuilder(args);

// Configure services
builder.Services.AddScoped<CompanyContext>();
builder.Services.AddTransient<IDummyService, DummyService>();
builder.Services.AddTransient<Application>();

var app = builder.Build();

// Run the application
var application = app.Services.GetRequiredService<Application>();
application.Run();
