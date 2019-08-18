using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using TestGenerator.Generation;
using TestGenerator.Parsing;
using IAsyncServiceProvider = Microsoft.VisualStudio.Shell.IAsyncServiceProvider;
using Task = System.Threading.Tasks.Task;

namespace TestGenerator.Commands
{
    /// <summary>
    /// Command handler
    /// </summary>
    public sealed class UnitTestGenerationCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("4b7d1ede-9a1d-4fc1-84df-9e91107236be");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage _package;

        /// <summary>
        /// Loads the syntax tree for the given document
        /// </summary>
        private readonly ISyntaxTreeFactory _syntaxTreeFactory;

        /// <summary>
        /// Loads specific data from the syntax tree
        /// </summary>
        private readonly IClassParser _classParser;

        /// <summary>
        /// Writes a unit test for a class
        /// </summary>
        private readonly ITestWriter _testWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitTestGenerationCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private UnitTestGenerationCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            _syntaxTreeFactory = new SyntaxTreeFactory();
            _classParser = new ClassParser();
            _testWriter = new TestWriter();

            var menuCommandId = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(Execute, menuCommandId);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static UnitTestGenerationCommand Instance { get; private set; }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IAsyncServiceProvider ServiceProvider => _package;

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in UnitTestGenerationCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var commandService = await package.GetServiceAsync<IMenuCommandService>() as OleMenuCommandService;
            Instance = new UnitTestGenerationCommand(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private async void Execute(object sender, EventArgs e)
        {
            var dte = await ServiceProvider.GetServiceAsync<DTE>();

            if (!dte.ActiveDocument.FullName.EndsWith(".cs")) return;

            var tree = _syntaxTreeFactory.LoadSyntaxTreeFromDocument(dte.ActiveDocument);
            var currentProject = GetCurrentProject(dte.ActiveSolutionProjects as Array);
            var testPath = GetTestPath(currentProject.FullName, dte.ActiveDocument.Path);
            var testProject = LoadTestProject(dte.Solution, currentProject);
            if (testProject == null)
            {
                DisplayMessage("No test project was found. Please create a new test project.", "No Test Project");
                return;
            }

            foreach (var classDefinition in _classParser.LoadClass(tree))
            {
                if (classDefinition == null) continue;

                var fileName = _testWriter.ScaffoldTest(classDefinition, testProject.FullName, testPath);

                testProject.ProjectItems.AddFromFile(fileName);

                VsShellUtilities.OpenDocument(_package, fileName);
            }
        }

        private static Project GetCurrentProject(Array projects)
        {
            return projects.Length > 0 ? projects.GetValue(0) as Project : null;
        }

        private static string GetTestPath(string projectFile, string documentPath)
        {
            var projectPath = Directory.GetParent(projectFile);
            if (!documentPath.Contains(projectPath.FullName)) return string.Empty;

            var testPath = documentPath.Replace(projectPath.FullName, "");

            return Regex.Replace(testPath, @"^\\", "");
        }

        private static Project LoadTestProject(Solution solution, Project currentProject)
        {
            var testProjects = solution.Projects.Cast<Project>()
                .SelectMany(GetProjects)
                .Where(project => project.FileName.EndsWith("Tests.csproj") && Path.GetFileName(project.FileName).ToLower().Contains(currentProject.Name.ToLower()))
                .OrderBy(_ => Path.GetFileName(_.FileName)?.Replace(currentProject.Name, string.Empty).Length);

            // TODO if the project file is not found, prompt to select the project
            var projectFile = testProjects.FirstOrDefault();

            return projectFile;
        }

        private static IEnumerable<Project> GetProjects(Project project)
        {
            if (project.Kind == "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}")
            {
                foreach (ProjectItem projectItem in project.ProjectItems)
                {
                    foreach (var innerProject in GetProjects(projectItem))
                    {
                        yield return innerProject;
                    }
                }
            }
            else
            {
                yield return project;
            }
        }

        private static IEnumerable<Project> GetProjects(ProjectItem project)
        {
            if (project.Object is Project)
            {
                foreach (var innerProject in GetProjects(project.Object as Project))
                {
                    yield return innerProject;
                }
            }
        }

        private void DisplayMessage(string message, string title)
        {
            VsShellUtilities.ShowMessageBox(
                _package,
                message,
                title,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
