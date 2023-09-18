using UnityEngine;

namespace Damath
{
    public class SplashScene : MonoBehaviour
    {
        [SerializeField] private RectTransform logo;
        float splashDuration = 1f;
        
        void Start()
        {
            // doesn't work
            LeanTween.rotate(logo, 360f, 0.5f)
            .setDelay(0.25f)
            .setEaseOutExpo()
            .setOnComplete(() =>
            {
                logo.localRotation = Quaternion.identity;
            });
            
            Game.Main.LoadScene("Title", playTransition: true, delayInSeconds: 1f);
        }
    }
}