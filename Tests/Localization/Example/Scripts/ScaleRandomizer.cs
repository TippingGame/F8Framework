using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace F8Framework.Tests
{
	public class ScaleRandomizer : MonoBehaviour
	{
		[SerializeField] float minScale, maxScale, interval;
		float elapsedTime;
		Vector3 targetScale;
		bool isPlaying;

		void Update()
		{
			if (!isPlaying) return;

			elapsedTime += Time.deltaTime;

			if (elapsedTime > interval)
			{
				elapsedTime = 0f;
				targetScale = Random.Range(minScale, maxScale) * Vector3.one;
			}

			transform.localScale = Vector3.Lerp(transform.localScale, targetScale, 0.5f);
		}

		public void Play()
		{
			isPlaying = true;
		}

		public void Stop()
		{
			isPlaying = false;
			StartCoroutine(FadeStop());
		}

		IEnumerator FadeStop()
		{
			while (Vector3.Distance(transform.localScale, Vector3.zero) > 0.01f)
			{
				transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.5f);
				yield return null;
			}
		}
	}
}
