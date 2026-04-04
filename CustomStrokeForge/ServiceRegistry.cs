using YukkuriMovieMaker.Commons;

namespace CustomStrokeForge
{
    internal sealed class ServiceRegistry : IDisposable
    {
        private static readonly Lazy<ServiceRegistry> _instance =
            new(static () => new ServiceRegistry(), LazyThreadSafetyMode.ExecutionAndPublication);

        public static ServiceRegistry Instance => _instance.Value;

        private readonly GraphicsEffectPool _effectPool = new();
        private int _disposed;

        private ServiceRegistry() { }

        public GraphicsEffectPool EffectPool
        {
            get
            {
                ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) == 1, this);
                return _effectPool;
            }
        }

        public CustomStrokeForgeEffectProcessor CreateProcessor(
            IGraphicsDevicesAndContext devices,
            CustomStrokeForgeEffect effect)
        {
            ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) == 1, this);
            return new CustomStrokeForgeEffectProcessor(devices, effect);
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 1)
                return;

            _effectPool.Dispose();
        }
    }
}
