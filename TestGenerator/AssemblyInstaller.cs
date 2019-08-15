namespace TestGenerator
{
    public interface IAssemblyInstaller
    {
        void InstallDependencies();
    }

    public class AssemblyInstaller : IAssemblyInstaller
    {
        public void InstallDependencies()
        {
            // Install Moq
        }
    }
}
