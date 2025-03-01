using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

namespace _Game._helpers.Audios
{
    public class AudioPlayer : MonoBehaviour
    {
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        public bool IsPlaying => _audioSource.isPlaying;

        public void PlaySound(AudioClip clip, float volume, float pitch, bool loop, bool isMuted, AudioMixerGroup mixerGroup)
        {
            if (clip == null)
            {
                Debug.LogWarning("AudioPlayer: Clip is null. Cannot play sound.");
                return;
            }

            if (float.IsNaN(volume) || float.IsInfinity(volume))
            {
                Debug.LogWarning("AudioPlayer: Volume is invalid. Resetting to 1.");
                volume = 1f;
            }

            if (float.IsNaN(pitch) || float.IsInfinity(pitch))
            {
                Debug.LogWarning("AudioPlayer: Pitch is invalid. Resetting to 1.");
                pitch = 1f;
            }

            if (mixerGroup == null)
            {
                Debug.LogWarning("AudioPlayer: AudioMixerGroup is null. Audio will not be routed through a mixer.");
            }

            // Extra safety: Check clip length and set position safely
            if (clip.length <= 0)
            {
                Debug.LogWarning("AudioPlayer: Clip length is zero or negative. Skipping playback.");
                return;
            }

            _audioSource.outputAudioMixerGroup = mixerGroup;
            _audioSource.clip = clip;
            _audioSource.volume = Mathf.Clamp01(volume);
            _audioSource.pitch = Mathf.Clamp(pitch, 0.1f, 3f);
            _audioSource.loop = loop;
            _audioSource.mute = isMuted;

            _audioSource.time = Mathf.Clamp(_audioSource.time, 0, clip.length); // Prevent invalid position

            _audioSource.Play();

            if (!loop)
            {
                StartCoroutine(DeactivateAfterPlayback());
            }
        }

        private IEnumerator DeactivateAfterPlayback()
        {
            yield return new WaitForSeconds(_audioSource.clip.length / Mathf.Max(_audioSource.pitch, 0.1f));
            _audioSource.Stop();
            gameObject.SetActive(false);
        }
    }
}