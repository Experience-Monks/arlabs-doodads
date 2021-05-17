using System;

using UnityEngine;

namespace Jam3
{
    public enum AudioMaterial
    {
        None = -1,
        Default = 0,
        Metal = 1,
        Plastic = 2,
        Wood = 3
    }

    [Serializable]
    public class AudioMaterialItem
    {
        public AudioMaterial Material = AudioMaterial.Default;
        public AudioClip Clip = null;
    }

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

        private AudioSource m_audioSource = null;
        private AudioMaterial m_currentAudioMaterial = AudioMaterial.None;

        private bool m_canPlayAudio = false;
        private float m_audioAmount = 0.0f;

        void Start()
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

        void Update()
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

        public void PlayHit()
        {
            if (HitAudioSource != null)
                HitAudioSource.Play();
        }

        public void AudioPlay()
        {
            if (!m_audioSource.isPlaying)
                m_audioSource.Play();
        }

        public void AudioPause()
        {
            if (m_audioSource.isPlaying)
                m_audioSource.Pause();
        }

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
