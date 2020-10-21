#nullable enable

namespace Microscope.Tests.Util {
    using System;

    public static class DebugExt {
        public static T Debug<T>(this T x, Action<T> f) {
            f(x);
            return x;
        }
    }
}
