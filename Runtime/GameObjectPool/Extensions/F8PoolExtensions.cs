using UnityEngine;

namespace F8Framework.Core
{
    public static class F8PoolExtensions
    {
        /// <summary>
        /// Despawns a particle system when it finishes playing.
        /// </summary>
        /// <param name="particleSystem">A particle system to despawn on complete.</param>
        /// <returns>A particle system to despawn on complete.</returns>
        public static ParticleSystem DespawnOnComplete(this ParticleSystem particleSystem)
        {
            GameObjectPool.Instance.Despawn(particleSystem.gameObject, particleSystem.main.duration);
            return particleSystem;
        }
    }
}