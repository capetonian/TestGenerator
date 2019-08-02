using Microsoft.VisualStudio.Shell;

namespace TestGenerator
{
    public static class Extensions
    {
        public static T GetService<T>(this AsyncPackage package)
        {
            var instance = package.GetServiceAsync(typeof(T)).Result;

            return (T)instance;
        }
    }
}
