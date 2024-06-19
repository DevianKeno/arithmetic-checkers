using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Damath
{
    public class AudioManager : MonoBehaviour
    {
        [Serializable]
        public struct AudioId
        {
            public string Id;
            public AudioClip AudioClip;
        }

        public static AudioManager Main { get; private set; }

        [SerializeField] List<AudioClip> audioClips = new();
        Dictionary<string, AudioClip> _audioClipsDict = new();

        [SerializeField] AudioSource musicSource;
        [SerializeField] AudioSource soundSource;

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

        internal void Initialize()
        {
            var resources = Resources.LoadAll<AudioClip>("Audio");
            foreach (var audioClip in resources)
            {
                audioClips.Add(audioClip);
                _audioClipsDict[audioClip.name] = audioClip;
            }
        }

        public void PlaySound(string name)
        {
            if (_audioClipsDict.ContainsKey(name))
            {
                soundSource.clip = _audioClipsDict[name];
                soundSource.Play();
            }
        }
    }
}