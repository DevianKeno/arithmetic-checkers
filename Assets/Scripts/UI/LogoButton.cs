using UnityEngine;
using UnityEngine.EventSystems;

namespace Damath.UI
{
    public class LogoButton : MonoBehaviour
    {
        public float SpinSpeed;
        public float FixSpeed;
        public float ResetTime;
        [SerializeField] float timer;
        [SerializeField] bool _startTimer;

        [SerializeField] RectTransform rect;

        void Awake()
        {
            rect = GetComponent<RectTransform>();
        }

        void Update()
        {
            if (_startTimer)
            {
                timer += Time.deltaTime;

                if (timer > ResetTime)
                {
                    timer = 0f;
                    _startTimer = false;
                    LeanTween.value(gameObject, rect.rotation.z, 0f, FixSpeed)
                        .setOnUpdate((float i) =>
                        {
                            var rot = rect.rotation;
                            rot.z = i;
                            rect.rotation = rot;
                        });
                }
            }
        }

        public void Spin()
        {
            LeanTween.cancel(gameObject);
            LeanTween.rotate(rect, 360f, SpinSpeed)
                .setEaseOutExpo()
                .setOnComplete( () =>
                {
                    _startTimer = true;
                });
        }
    }
}