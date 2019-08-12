using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;

namespace TestGenerator
{
    public static class Extensions
    {
        public static async Task<T> GetServiceAsync<T>(this IAsyncServiceProvider serviceProvider)
        {
            var instance = await serviceProvider.GetServiceAsync(typeof(T));

            return (T)instance;
        }
    }
}
