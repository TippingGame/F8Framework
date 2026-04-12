using UnityEngine;
#if LOCALIZER_TIMELINE
using UnityEngine.Timeline;
#endif

namespace F8Framework.Tests
{
	public class FireController : MonoBehaviour
#if LOCALIZER_TIMELINE
		, ITimeControl
#endif
	{
		[SerializeField] ParticleSystem particle;
		[SerializeField] ScaleRandomizer floorLight;

		void Play()
		{
			particle.Play();
			floorLight.Play();
		}

		void Stop()
		{
			if (particle)
			{
				particle.Stop();
			}

			if (floorLight)
			{
				floorLight.Stop();
			}
		}

		public void SetTime(double time)
		{
		}

		public void OnControlTimeStart() => Play();

		public void OnControlTimeStop() => Stop();
	}
}
