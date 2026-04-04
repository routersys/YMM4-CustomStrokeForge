using System.Reflection;

namespace CustomStrokeForge
{
    internal static class ShaderResourceLoader
    {
        private const string ShaderSuffix = "StrokePS.cso";

        private static readonly Lazy<byte[]> _strokePS =
            new(LoadStrokePS, LazyThreadSafetyMode.ExecutionAndPublication);

        public static byte[] GetStrokePS() => _strokePS.Value;

        private static byte[] LoadStrokePS()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var names = assembly.GetManifestResourceNames();

            string? matched = null;
            foreach (var name in names)
            {
                if (name.EndsWith(ShaderSuffix, StringComparison.Ordinal))
                {
                    matched = name;
                    break;
                }
            }

            if (matched is null)
                throw new InvalidOperationException(
                    $"Embedded resource '{ShaderSuffix}' was not found.");

            using var stream = assembly.GetManifestResourceStream(matched)!;
            var buffer = new byte[(int)stream.Length];
            stream.ReadExactly(buffer);
            return buffer;
        }
    }
}
