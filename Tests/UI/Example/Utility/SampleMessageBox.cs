using UnityEngine;
using UnityEngine.UI;

namespace F8Framework.Tests
{
    public class SampleMessageBox : MonoBehaviour
    {
        private enum State
        {
            NONE,
            OPEN,
            SHOW,
            HIDE
        }

        public float openTime = 0.2f;
        public float showTime = 1;
        public float hideTime = 0.8f;

        public CanvasRenderer canvasRenderer;
        public Text message;

        private State state = State.NONE;


        private float current;

        private void Update()
        {
            if (state == State.OPEN)
            {
                if (current <= 0)
                {
                    current = showTime;
                    state = State.SHOW;

                    transform.localScale = Vector3.one;
                }
                else
                {
                    transform.localScale = Vector2.Lerp(Vector2.zero, Vector2.one, openTime - current / openTime);
                }
            }

            if (state == State.SHOW)
            {
                if (current <= 0)
                {
                    current = hideTime;
                    state = State.HIDE;
                }
            }

            if (state == State.HIDE)
            {
                if (current <= 0)
                {
                    current = 0;
                    state = State.NONE;

                    canvasRenderer.SetAlpha(0);
                }
                else
                {
                    canvasRenderer.SetAlpha(current / hideTime);
                }
            }

            if (state == State.NONE)
            {
                gameObject.SetActive(false);
            }

            current -= Time.deltaTime;
        }

        public void Show(string value)
        {
            message.text = value;

            state = State.OPEN;
            current = openTime;

            transform.localScale = Vector3.zero;
            canvasRenderer.SetAlpha(1);

            gameObject.SetActive(true);
        }
    }
}