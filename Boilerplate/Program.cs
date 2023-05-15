using ApolloFramework;
using Boilerplate.Extensions;
using Boilerplate.Services.Apollo;
using Boilerplate.Services.Thing;
using Boilerplate.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.AddConfigurationOptions();
builder.AddLogging();
builder.AddDependentServices();

var application = builder.Build();
await application.StartAsync();

// gets instance of a service
var apolloService = application.Services.GetRequiredService<IApolloService>();

await apolloService.TestApollo();



// use this when accessing encrypted columns
//await application.AddEncryptionProvider();

// need this is the console needs to keep running
//await application.RunAsync();