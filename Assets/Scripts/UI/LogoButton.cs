using UnityEngine;
using UnityEngine.EventSystems;

namespace Damath.UI
{
    public class LogoButton : MonoBehaviour
    {
        public float SpinSpeed;
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
                    LeanTween.value(gameObject, rect.rotation.z, 0f, 0.5f)
                    .setOnUpdate((float i) =>
                    {
                        var rot = rect.localRotation;
                        rot.z = i;
                        rect.localRotation = rot;
                    });
                }
            }
        }

        public void Spin()
        {
            LeanTween.cancel(gameObject);
            LeanTween.value(gameObject, rect.rotation.z, 360f, SpinSpeed)
            .setOnUpdate((float i) =>
            {
                var rot = rect.localRotation;
                rot.z = i;
                rect.localRotation = rot;
            })
            .setEaseOutExpo()
            .setOnComplete( () =>
            {
                _startTimer = true;
            });
        }
    }
}