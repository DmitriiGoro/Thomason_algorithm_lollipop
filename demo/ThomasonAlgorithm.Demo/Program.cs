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
// await experimentService.ExperimentCubicGraph(options, app);
experimentService.CreateLollipopStepsVisualization("1,33,32_0,2,31_1,3,30_2,4,6_3,5,33_4,6,26_5,7,3_6,8,29_7,9,28_8,10,23_9,11,22_10,12,14_11,13,25_12,14,18_13,15,11_14,16,21_15,17,20_16,18,19_17,19,13_18,20,17_19,21,16_20,22,15_21,23,10_22,24,9_23,25,27_24,26,12_25,27,5_26,28,24_29,27,8_28,30,7_29,31,2_30,32,1_31,33,0_32,0,4");