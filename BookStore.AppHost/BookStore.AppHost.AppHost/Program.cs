using Projects;
using Aspire.Hosting.Dapr;


var builder = DistributedApplication.CreateBuilder(args);

var statestore = builder.AddDaprStateStore("bookstatestore");

builder.AddProject<BookOrder>("bookorderservice")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "book-order",
        DaprHttpPort = 3501,
    })
    .WithReference(statestore);

builder.AddProject<Projects.Workflow>("workflow");


builder.AddProject<Projects.Frontend>("frontend");


builder.Build().Run();

