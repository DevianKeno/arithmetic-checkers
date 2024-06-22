using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Damath.UI
{
    public class LoadingIconAnimation : MonoBehaviour
    {
        void Start()
        {
            LeanTween.rotate(transform as RectTransform, 360f, 3f)
                .setLoopClamp();
        }
    }
}