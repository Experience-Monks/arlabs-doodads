//-----------------------------------------------------------------------
// <copyright file="AudioController.cs" company="Jam3 Inc">
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

namespace Jam3
{
    /// <summary>
    /// Audio material.
    /// </summary>
    public enum AudioMaterial
    {
        None = -1,
        Default = 0,
        Metal = 1,
        Plastic = 2,
        Wood = 3
    }

    /// <summary>
    /// Audio material item.
    /// </summary>
    [Serializable]
    public class AudioMaterialItem
    {
        public AudioMaterial Material = AudioMaterial.Default;
        public AudioClip Clip = null;
    }

    /// <summary>
    /// Audio controller.
    /// </summary>
    /// <seealso cref="MonoBehaviour" />
    [RequireComponent(typeof(AudioSource))]
    public class AudioController : MonoBehaviour
    {
        public bool CanPlayAudio { get { return m_canPlayAudio; } set { m_canPlayAudio = value; } }
        public float AudioAmount { get { return m_audioAmount; } set { m_audioAmount = value; } }

        public AudioMaterialItem[] AudioList = null;

        public AudioSource HitAudioSource = null;

        public float AudioSpeedMax = 0.1f;

        [Range(-3.0f, 1f)]
        public float AudioPitchMin = 0.8f;
        [Range(0.5f, 3f)]
        public float AudioPitchMax = 1.2f;

        [Range(0.0f, 1f)]
        public float AudioVolumeMin = 1f;
        [Range(0.1f, 2f)]
        public float AudioVolumeMax = 1f;

        // Runtime varilables
        private AudioSource m_audioSource = null;
        private AudioMaterial m_currentAudioMaterial = AudioMaterial.None;

        private bool m_canPlayAudio = false;
        private float m_audioAmount = 0.0f;

        /// <summary>
        /// Start.
        /// </summary>
        private void Start()
        {
            m_audioSource = GetComponent<AudioSource>();
            m_audioSource.loop = true;
            m_audioSource.clip = null;
            m_audioSource.playOnAwake = false;
            m_audioSource.volume = 0.0f;
            m_audioSource.pitch = 1.0f;

            if (AudioManager.Instance.GameAudioMixer != null)
                m_audioSource.outputAudioMixerGroup = AudioManager.Instance.GameAudioMixer;

            SetAudioMaterial(AudioMaterial.Default);
        }

        /// <summary>
        /// Sets audio material.
        /// </summary>
        /// <param name="material">The material.</param>
        public void SetAudioMaterial(AudioMaterial material = AudioMaterial.Default)
        {
            if (material != m_currentAudioMaterial)
            {
                AudioClip clip = GetAudioByName(material);
                m_currentAudioMaterial = material;

                if (clip != null && m_audioSource != null)
                    m_audioSource.clip = clip;
            }
        }

        /// <summary>
        /// Update.
        /// </summary>
        private void Update()
        {
            if (m_canPlayAudio && m_audioAmount > 0f)
            {
                AudioPlay();
                float amount = m_audioAmount / AudioSpeedMax;
                amount = Mathf.Clamp01(amount);

                m_audioSource.pitch = Mathf.Lerp(AudioPitchMin, AudioPitchMax, amount);
                m_audioSource.volume = Mathf.Lerp(AudioVolumeMin, AudioVolumeMax, amount);
            }
            else
            {
                AudioPause();
            }
        }

        /// <summary>
        /// Plays hit.
        /// </summary>
        public void PlayHit()
        {
            if (HitAudioSource != null)
                HitAudioSource.Play();
        }

        /// <summary>
        /// Audio controller.
        /// </summary>
        /// <seealso cref="MonoBehaviour" />
        public void AudioPlay()
        {
            if (!m_audioSource.isPlaying)
                m_audioSource.Play();
        }

        /// <summary>
        /// Audio pause.
        /// </summary>
        public void AudioPause()
        {
            if (m_audioSource.isPlaying)
                m_audioSource.Pause();
        }

        /// <summary>
        /// Gets audio by name.
        /// </summary>
        /// <param name="material">The material.</param>
        private AudioClip GetAudioByName(AudioMaterial material)
        {
            AudioClip clip = null;
            foreach (var item in AudioList)
            {
                if (item.Material == material)
                    clip = item.Clip;
            }

            return clip;
        }
    }
}
