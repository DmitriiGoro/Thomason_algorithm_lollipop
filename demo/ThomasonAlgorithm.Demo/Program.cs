using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using ThomasonAlgorithm.Demo;
using ThomasonAlgorithm.Demo.Models;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbInfrastructure(connectionString);

var app = builder.Build();

var options = new ExperimentOptions(1000, 2, 10);
var experimentService = new CubicGraphExperimentService();
await experimentService.ExperimentCubicGraph(options, app);
