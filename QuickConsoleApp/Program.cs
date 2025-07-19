using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace QuickConsoleApp
{
    class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: ProjectCreator <project-name>");
                return;
            }
            
            string? githubDir = Environment.GetEnvironmentVariable("GITHUB_DIR");
            if (githubDir == null)
                throw new ValidationException("Github directory not found");
            
            string projectName = args[0];
            string currentDirectory = Directory.GetCurrentDirectory();
            string projectPath = Path.Combine(githubDir, projectName);
            
            // Step 1: Run dotnet new console
            var dotnetProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"new console --use-program-main -o {projectPath}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = false
                }
            };

            Console.WriteLine("Creating .NET project...");
            dotnetProcess.Start();
            dotnetProcess.WaitForExit();

            if (dotnetProcess.ExitCode != 0)
            {
                Console.WriteLine("Failed to create project:");
                Console.WriteLine(dotnetProcess.StandardError.ReadToEnd());
                return;
            }

            Console.WriteLine(dotnetProcess.StandardOutput.ReadToEnd());

            // Step 2: Open Rider with the new project
            var riderPath = Environment.GetEnvironmentVariable("RIDER_PATH");
            Console.WriteLine("Rider Path: " + riderPath);
            var riderProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = riderPath,
                    Arguments = $"\"{projectPath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = false
                }
            };

            Console.WriteLine("Opening project in Rider...");
            riderProcess.Start();
        }
    }
}