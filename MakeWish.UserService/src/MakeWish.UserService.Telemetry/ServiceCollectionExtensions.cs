﻿using EnsureThat;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace MakeWish.UserService.Telemetry;

public static class ServiceCollectionExtensions
{
    public static void SetupTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        var optionsSection = configuration.GetSection(OtelOptions.SectionName);
        services.Configure<OtelOptions>(optionsSection);
        
        var options = optionsSection.Get<OtelOptions>();
        EnsureArg.IsNotNull(options, nameof(options));

        if (!options.IsEnabled)
        {
            return;
        }
        
        services.AddOpenTelemetry()
            .WithMetrics(opt => opt
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(options.ServiceName))
                .AddMeter(options.MeterName)
                .AddAspNetCoreInstrumentation()
                .AddRuntimeInstrumentation()
                .AddProcessInstrumentation()
                .AddPrometheusExporter()
            );
    }
}