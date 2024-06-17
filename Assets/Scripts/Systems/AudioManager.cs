using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Damath
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Main { get; private set; }
        public Dictionary<string, AudioSource> Sources = new();
        public float defaultVolume = 1f;

        void Awake()
        {
            if (Main != null && Main != this)
            {
                Destroy(this);
            } else
            {
                Main = this;
            }
        }

        void Start()
        {
            foreach (Transform audioSource in transform)
            {
                AddSource(audioSource.name, audioSource.GetComponent<AudioSource>());
            }
        }

        public void AddSource(string name, AudioSource audioSource)
        {
            Sources.Add(name, audioSource);
        }

        public void PlaySound(string name)
        {
            Sources[name].Play();
        }
    }
}