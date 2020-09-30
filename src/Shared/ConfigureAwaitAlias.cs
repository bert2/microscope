#nullable enable

namespace Microscope.Shared {
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    public static class ConfigureAwaitAlias {
        /// <summary>Alias for `ConfigureAwait(false)`.</summary>
        public static ConfiguredTaskAwaitable Caf(this Task t) => t.ConfigureAwait(false);

        /// <summary>Alias for `ConfigureAwait(false)`.</summary>
        public static ConfiguredTaskAwaitable<T> Caf<T>(this Task<T> t) => t.ConfigureAwait(false);
    }
}
