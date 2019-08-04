using Microsoft.VisualStudio.Shell;

namespace TestGenerator
{
    public static class Extensions
    {
        public static T GetService<T>(this IAsyncServiceProvider serviceProvider)
        {
            var instance = serviceProvider.GetServiceAsync(typeof(T)).Result;

            return (T)instance;
        }
    }
}
