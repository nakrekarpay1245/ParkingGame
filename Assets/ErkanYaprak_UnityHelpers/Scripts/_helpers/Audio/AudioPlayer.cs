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
            // Add an AudioSource to this game object.
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Expose whether this player is currently playing.
        public bool IsPlaying => _audioSource.isPlaying;

        /// <summary>
        /// Configures and plays the given audio clip.
        /// </summary>
        /// <param name="clip">Audio clip to play.</param>
        /// <param name="volume">Final volume (already combined with any master multipliers).</param>
        /// <param name="pitch">Pitch to apply.</param>
        /// <param name="loop">Whether to loop the sound.</param>
        /// <param name="isMuted">Whether the sound should be muted.</param>
        /// <param name="mixerGroup">Audio mixer group to route the audio through.</param>
        public void PlaySound(AudioClip clip, float volume, float pitch, bool loop, bool isMuted, AudioMixerGroup mixerGroup)
        {
            _audioSource.outputAudioMixerGroup = mixerGroup;
            _audioSource.clip = clip;
            _audioSource.volume = volume;
            _audioSource.pitch = pitch;
            _audioSource.loop = loop;
            _audioSource.mute = isMuted;
            _audioSource.Play();

            if (!loop)
            {
                StartCoroutine(DeactivateAfterPlayback());
            }
        }

        private IEnumerator DeactivateAfterPlayback()
        {
            // Wait for the clip's duration adjusted by the pitch.
            yield return new WaitForSeconds(_audioSource.clip.length / _audioSource.pitch);
            _audioSource.Stop();
            gameObject.SetActive(false);
        }
    }
}