using Microsoft.SemanticKernel;
using Microsoft.Extensions.Configuration;
using System.CommandLine;
using Spectre.Console;
using PromptyApp.Commands;

var builder = Kernel.CreateBuilder();
var configBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var config = configBuilder.Build();

builder.Services.AddAzureOpenAIChatCompletion(
    config["AzureOpenAI:deployment"] ?? throw new ArgumentNullException("AzureOpenAI:deployment configuration is missing"),
    config["AzureOpenAI:endpoint"] ?? throw new ArgumentNullException("AzureOpenAI:endpoint configuration is missing"),
     config["AzureOpenAI:apiKey"] ?? throw new ArgumentNullException("AzureOpenAI:apiKey configuration is missing"));
var kernel = builder.Build();

var rootCmd = new RootCommand("Prompty AI Chatbot");
rootCmd.Description = "A simple AI chatbot using Prompty and Semantic Kernel.";

rootCmd.AddCommand(new PromptCommand(kernel));
rootCmd.AddCommand(new DebugCommand());


rootCmd.SetHandler(async () =>
{
    await DisplayInfo();
});

async Task DisplayInfo()
{
    // AnsiConsole.MarkupLineInterpolated($"[green]Welcome to Prompty AI Chatbot![/]");
    // AnsiConsole.MarkupLineInterpolated($"[green]Use --help for more information.[/]");
}

await rootCmd.InvokeAsync(args);


