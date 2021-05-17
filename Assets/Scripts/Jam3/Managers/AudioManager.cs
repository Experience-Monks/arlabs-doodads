using System;
using UnityEngine;
using UnityEngine.Audio;
using Jam3.Util;

namespace Jam3
{
    public enum MixerType
    {
        UI = 0,
        Game = 1
    }

    [Serializable]
    public class AudioItem
    {
        public String Name = "";
        public AudioClip Clip = null;
    }

    public class AudioManager : Singleton<AudioManager>
    {
        public AudioItem[] AudioList = null;

        public AudioMixerGroup GameAudioMixer = null;
        public AudioMixerGroup UIAudioMixer = null;

        private AudioSource cachedAudioSource = null;

        public AudioSource PlayAudioClip(string name, MixerType type = MixerType.UI, bool cacheAudio = false, bool stopCached = false)
        {
            AudioClip clip = GetAudioByName(name);

            if (stopCached && cachedAudioSource != null)
                cachedAudioSource.Stop();

            Camera cam = Camera.main;
            if (cam == null || clip == null) return null;

            GameObject go = new GameObject("Audio-" + clip.name);
            go.transform.parent = cam.transform;

            AudioSource source = go.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = false;
            source.outputAudioMixerGroup = type == MixerType.UI ? UIAudioMixer : GameAudioMixer;

            if (cacheAudio)
                cachedAudioSource = source;

            source.Play();

            Destroy(go, (clip.length + 0.5f));

            return source;
        }

        public AudioSource PlayAudioClipAt(string name, Vector3 worldPos, MixerType type = MixerType.Game)
        {
            AudioClip clip = GetAudioByName(name);
            if (clip == null) return null;

            GameObject go = new GameObject("Audio-" + clip.name);
            AudioSource source = go.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = false;
            source.outputAudioMixerGroup = type == MixerType.UI ? UIAudioMixer : GameAudioMixer;
            source.Play();

            Destroy(go, (clip.length + 0.5f));

            return source;
        }

        private AudioClip GetAudioByName(string name)
        {
            AudioClip clip = null;
            foreach (var item in AudioList)
            {
                if (item.Name == name)
                    clip = item.Clip;
            }

            return clip;
        }
    }
}
