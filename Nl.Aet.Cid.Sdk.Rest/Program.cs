using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Nl.Aet.Cid.Sdk.Rest.Sdk;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Exceptions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var applicationName = Environment.GetEnvironmentVariable("TaskQueueSettings__ProcessId") ?? "Nl.Aet.Cid.Web.Connectors.Rest.Applicant";
var logEventLevel = (LogEventLevel)Enum.Parse(typeof(LogEventLevel), Environment.GetEnvironmentVariable("LogEventLevel") ?? "Debug");

var loggerConfig = new LoggerConfiguration()
    .MinimumLevel.ControlledBy(new LoggingLevelSwitch(logEventLevel))
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Filter.ByExcluding("EndsWith(RequestPath, '/status') AND StatusCode=200")
    .Enrich.WithProperty("ApplicationName", applicationName)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithExceptionDetails()
    .WriteTo.Console(outputTemplate: "[{Timestamp:dd.MM.yyyy HH:mm:ss} {ApplicationName} {Level:u3}] {Message:lj}{NewLine}{Exception}");

Log.Logger = loggerConfig.CreateLogger();

try
{
    Log.Information("Starting Application({applicationName})", applicationName);

    var builder = WebApplication.CreateBuilder(args);
    
    //Add SDK wrapper as a singleton service
    builder.Services.AddSingleton(typeof(Wrapper), typeof(Wrapper));
    builder.WebHost.UseKestrel(options =>
    {
    });

    // Add services to the container.

    builder.Services.AddControllers();

    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = applicationName, Version = "v1" });
    });
    
    builder.Services.AddHealthChecks();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseRouting();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapHealthChecks("/status");
        endpoints.MapControllers();
    });

    app.Run();
}
catch (Exception exception)
{
    Log.Fatal(exception, "Application({applicationName}) terminated unexpectedly!", applicationName);
}
finally
{
    Log.Information("Shutting down application {applicationName}", applicationName);
    Log.CloseAndFlush();
}