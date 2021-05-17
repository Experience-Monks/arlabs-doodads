//-----------------------------------------------------------------------
// <copyright file="AudioManager.cs" company="Jam3 Inc">
//
// Copyright 2021 Jam3 Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

using System;
using UnityEngine;
using UnityEngine.Audio;
using Jam3.Util;

namespace Jam3
{
    /// <summary>
    /// Mixer type.
    /// </summary>
    public enum MixerType
    {
        UI = 0,
        Game = 1
    }

    /// <summary>
    /// Audio item.
    /// </summary>
    [Serializable]
    public class AudioItem
    {
        public String Name = "";
        public AudioClip Clip = null;
    }

    /// <summary>
    /// Audio manager.
    /// </summary>
    /// <seealso cref="Singleton<AudioManager>" />
    public class AudioManager : Singleton<AudioManager>
    {
        public AudioItem[] AudioList = null;

        public AudioMixerGroup GameAudioMixer = null;
        public AudioMixerGroup UIAudioMixer = null;

        private AudioSource cachedAudioSource = null;

        /// <summary>
        /// Plays audio clip.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="cacheAudio">The cache audio.</param>
        /// <param name="stopCached">The stop cached.</param>
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

        /// <summary>
        /// Plays audio clip at.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="worldPos">The world pos.</param>
        /// <param name="type">The type.</param>
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

        /// <summary>
        /// Gets audio by name.
        /// </summary>
        /// <param name="name">The name.</param>
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
