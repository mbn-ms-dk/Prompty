using Microsoft.SemanticKernel;
using Microsoft.Extensions.Configuration;

var builder = Kernel.CreateBuilder();
var configBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var config = configBuilder.Build();
builder.Services.AddAzureOpenAIChatCompletion(
    config["AzureOpenAI:deployment"],
    config["AzureOpenAI:endpoint"],
     config["AzureOpenAI:apiKey"]);
var kernel = builder.Build();

//ask user to select a response type by entering a number
Console.WriteLine("Select a response type:");
Console.WriteLine("1. Yoda");
Console.WriteLine("2. Shakespeare");

int responseType = int.Parse(Console.ReadLine() ?? "1");
// Load the Prompty file based on the user's selection
string promptFile = responseType switch
{
    1 => "yoda.prompty",
    2 => "shakespeare.prompty",
    _ => throw new ArgumentException("Invalid selection")
};

// Load the selected Prompty file and create a kernel function
KernelFunction kernelFunction = kernel.CreateFunctionFromPromptyFile(promptFile);
var responseTypeName = responseType == 1 ? "Yoda" : "Shakespeare";
Console.WriteLine($"Ask your question to a {responseTypeName} AI:");
string question = Console.ReadLine();
string? answer = await kernelFunction.InvokeAsync<string>(kernel, new() 
{
    {
        "question", question
    }
});

Console.WriteLine($"Answer: {answer}");

Console.ReadLine();

