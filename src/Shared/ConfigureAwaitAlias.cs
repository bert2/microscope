namespace Microscope.Shared {
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    public static class ConfigureAwaitAlias {
        public static ConfiguredTaskAwaitable Caf(this Task t) => t.ConfigureAwait(false);
        public static ConfiguredTaskAwaitable<T> Caf<T>(this Task<T> t) => t.ConfigureAwait(false);
    }
}
