using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace _Game._helpers.Audios
{
    public class AudioManager : MonoBehaviour
    {
        [Header("Audio Manager Parameters")]
        [Tooltip("List of audio configurations.")]
        [SerializeField]
        private List<Audio> _audioList = new List<Audio>();

        [Tooltip("Maximum number of AudioPlayers to manage.")]
        [SerializeField, Range(1, 20)]
        private int _maximumPlayerCount = 10;

        [Tooltip("Master volume for all audio.")]
        [SerializeField, Range(0f, 1f)]
        private float _masterVolume = 1f;

        [Tooltip("Mute all audio players.")]
        [SerializeField]
        private bool _isAudioMuted = false;

        [Header("Audio Mixer")]
        [Tooltip("Audio mixer group for sound effects.")]
        [SerializeField]
        private AudioMixerGroup _soundMixerGroup;

        private readonly List<AudioPlayer> _audioPlayers = new List<AudioPlayer>();

        private void Awake()
        {
            ServiceLocator.Register(this);
            InitializeAudioPlayers();
        }

        private void InitializeAudioPlayers()
        {
            for (int i = 0; i < _maximumPlayerCount; i++)
            {
                // Create a new GameObject for each AudioPlayer.
                GameObject playerObj = new GameObject("AudioPlayer_" + i);
                playerObj.transform.parent = this.transform;
                // Add the AudioPlayer component.
                AudioPlayer audioPlayer = playerObj.AddComponent<AudioPlayer>();
                // Deactivate until used.
                playerObj.SetActive(false);
                _audioPlayers.Add(audioPlayer);
            }
        }

        private Audio GetAudioByName(string clipName)
        {
            return _audioList.Find(audio => audio.Name == clipName);
        }

        private AudioPlayer GetAvailableAudioPlayer()
        {
            // First, try to find one that is inactive.
            foreach (var player in _audioPlayers)
            {
                if (!player.gameObject.activeInHierarchy)
                {
                    return player;
                }
            }
            // Alternatively, if active but not playing, consider it.
            foreach (var player in _audioPlayers)
            {
                if (!player.IsPlaying)
                {
                    return player;
                }
            }
            return null;
        }

        /// <summary>
        /// Plays a sound by audio configuration name.
        /// </summary>
        public void PlaySound(string clipName, float volume = 1f, bool loop = false)
        {
            Audio audio = GetAudioByName(clipName);
            if (audio == null)
            {
                Debug.LogWarning("Audio with name " + clipName + " not found.");
                return;
            }
            PlaySound(audio, volume, loop);
        }

        public void PlaySound(string clipName)
        {
            PlaySound(clipName, 1f, false);
        }

        /// <summary>
        /// Plays a sound given a raw AudioClip.
        /// </summary>
        public void PlaySound(AudioClip clip, float volume = 1f, bool loop = false)
        {
            AudioPlayer player = GetAvailableAudioPlayer();
            if (player == null)
            {
                Debug.LogWarning("No available AudioPlayer to play sound.");
                return;
            }
            // Activate the player.
            player.gameObject.SetActive(true);
            // Compute the final volume (master volume applied).
            float finalVolume = _masterVolume * volume;
            player.PlaySound(clip, finalVolume, 1f, loop, _isAudioMuted, _soundMixerGroup);
        }

        /// <summary>
        /// Plays a sound using the Audio configuration.
        /// </summary>
        public void PlaySound(Audio audio, float volume = 1f, bool loop = false)
        {
            AudioPlayer player = GetAvailableAudioPlayer();
            if (player == null)
            {
                Debug.LogWarning("No available AudioPlayer to play sound.");
                return;
            }
            player.gameObject.SetActive(true);
            // Get a random clip from the Audio configuration.
            AudioClip clip = audio.Clip;
            // Compute final volume considering master volume and the audio's individual volume.
            float finalVolume = _masterVolume * volume * audio.Volume;
            player.PlaySound(clip, finalVolume, audio.Pitch, loop, _isAudioMuted, _soundMixerGroup);
        }
    }
}