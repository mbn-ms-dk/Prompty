using Microsoft.SemanticKernel;
using System.CommandLine;
using Spectre.Console;

namespace PromptyApp.Commands
{
    public class PromptCommand : Command
    {
        private readonly Kernel Kernel;

        public PromptCommand(Kernel kernel) : base("prompt", "Prompt the AI with a question")
        {
            this.Kernel = kernel;
            var promptOption = new Option<int>(["--prompt", "-p"], "Select a prompt file. 1 for Yoda, 2 for Shakespeare")
            {
                IsRequired = true,
                ArgumentHelpName = "prompt"
            };
            AddOption(promptOption);

            var questionOption = new Option<string>(["--question", "-q"], "The question to ask the AI")
            {
                IsRequired = true,
                ArgumentHelpName = "question"
            };
            AddOption(questionOption);
            this.SetHandler(async (promptOption, questionOption) =>
            {
                await ExecuteAsync(promptOption, questionOption);
            }
            , promptOption, questionOption);
        }

        private async Task ExecuteAsync(int promptOption, string questionOption)
        {
            //check if prompt is 1 or 2, if not, show help
            if (promptOption != 1 && promptOption != 2)
            {
                AnsiConsole.MarkupLineInterpolated($"[red]Please select 1 for Yoda or 2 for Shakespeare.[/]");
                return;
            }
            // Load the Prompty file based on the user's selection
            string promptFile = promptOption switch
            {
                1 => "yoda.prompty",
                2 => "shakespeare.prompty",
                _ => throw new ArgumentException("Invalid selection")
            };

            // Load the selected Prompty file and create a kernel function
            KernelFunction kernelFunction = Kernel.CreateFunctionFromPromptyFile(promptFile);
            var responseTypeName = promptOption == 1 ? "Yoda" : "Shakespeare";
            AnsiConsole.MarkupLineInterpolated($"[blue]Your question will be sent to a {responseTypeName} AI:");
            //check if question is null or empty, if so, show help
            if (string.IsNullOrEmpty(questionOption))
            {
                AnsiConsole.MarkupLineInterpolated($"[red]Please enter a question.[/]");
                return;
            }
            string? answer = await kernelFunction.InvokeAsync<string>(Kernel, new()
            {
                {
                    "question", questionOption
                }
            });
            AnsiConsole.MarkupLineInterpolated($"[green]Answer: {answer}");

        }
    }
}

