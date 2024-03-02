using System;
using System.Collections;
using F8Framework.Core;
using UnityEngine;

namespace F8Framework.Tests
{
    public class IdSequencer : MonoBehaviour
    {
        [SerializeField, Header("References")] TextLocalizer textLocalizer;
        [SerializeField] TextMesh textMesh;

        [SerializeField, Header("Options")] float interval = 3f;
        [SerializeField] float fadeDuration = 1f;
        [SerializeField] bool playOnStart = true;
        [SerializeField] bool loop = true;

        [SerializeField, Header("Set IDs")] string[] Ids;

        Coroutine current;
        float initialAlpha;

        void Start()
        {
            initialAlpha = textMesh.color.a;
            if (playOnStart) Play();
        }

        public void Play()
        {
            if (current != null) StopCoroutine(current);
            current = StartCoroutine(Talk());
        }

        public IEnumerator Talk()
        {
            var queue = new Queue(Ids);
            var wait = new WaitForSeconds(interval);

            while (queue.Count != 0)
            {
                yield return FadeTo(0f, fadeDuration);
                textLocalizer.ChangeID(queue.Dequeue().ToString());
                yield return FadeTo(initialAlpha, fadeDuration);
                yield return wait;
            }

            current = null;

            if (loop) Play();
        }

        IEnumerator FadeTo(float alpha, float duration)
        {
            var diff = textMesh.color.a - alpha;
            while (Math.Abs(textMesh.color.a - alpha) > 0.01f)
            {
                var diffPerFrame = diff * duration * Time.deltaTime;
                var color = textMesh.color;
                color.a -= diffPerFrame;
                textMesh.color = color;
                yield return null;
            }
        }
    }
}