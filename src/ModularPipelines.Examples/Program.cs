using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using ModularPipelines.Attributes;
using ModularPipelines.Examples;
using ModularPipelines.Examples.Modules;
using ModularPipelines.Examples.Modules.FullGraph.Dependencies;
using ModularPipelines.Examples.Modules.ResolveModules;
using ModularPipelines.Examples.Modules.Success;
using ModularPipelines.Examples.Modules.Triggers;
using ModularPipelines.Extensions;
using ModularPipelines.Host;
using ModularPipelines.Modules;
using ModularPipelines.Options;

await PipelineHostBuilder.Create()
    .ConfigureAppConfiguration((context, builder) =>
    {
        builder.AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true)
            .AddUserSecrets<Program>()
            .AddCommandLine(args)
            .AddEnvironmentVariables();
    })
    .ConfigurePipelineOptions((_, options) =>
    {
        options.ExecutionMode = ExecutionMode.StopOnFirstException;
        options.IgnoreCategories = new[] { "Ignore" };
    })
    .ConfigureServices((context, collection) =>
    {
        collection.Configure<MyOptions>(context.Configuration);

        collection.AddModule<SuccessModule>()
            .AddModule<LogSecretModule>()
            .AddModule<DependencyForReliantModule>()
            .AddModule<ReliantModule>()
            .AddModule<DependencyModule>()
            .AddModule<DependentOnSuccessModule>()
            .AddModule<DependentOn2>()
            .AddModule<DependentOn3>()
            .AddModule<DependentOn4>()
            .AddModule<SubmodulesModule>()
            .AddModule<GitVersionModule>()
            .AddModule<GitLastCommitModule>();
        collection.InjectRequiredModules(args);
        collection.AddModule<TriggeredModule2>();
        collection.AddModule<FullGrapthMainModule>();
        collection.AddModule<FullGrapthDependensOnModule1>();
        collection.AddModule<FullGrapthDependensOnModule2>();
        collection.AddModule<FullGrapthReliantModule1>();
        collection.AddModule<FullGrapthReliantModule2>();
        collection.AddModule<FullGraphMainTriggerModule>();
        collection.AddModule<FullGraphTiggeredByModule>();
        collection.AddModule<FullGrapthTriggeringModule>();
    })
    .ExecutePipelineAsync();
