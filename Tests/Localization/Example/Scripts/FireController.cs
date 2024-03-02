using UnityEngine;
using UnityEngine.Timeline;

namespace F8Framework.Tests
{
	public class FireController : MonoBehaviour, ITimeControl
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
