using Resources.Scripts.Storage;
using UnityEngine;

namespace Resources.Scripts
{
    public class MusicController : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip[] _audioClips;

        private int _currentIndexAudioClip;

        private static MusicController _instance;
        
        public static MusicController Instance => _instance;

        private void Start()
        {
            if (_instance == null) 
                _instance = this; 
            else if(_instance == this)
                Destroy(gameObject);
            
            DontDestroyOnLoad(gameObject);
            
            var settingsData = (SettingsData)new Storage.Storage().Load(new SettingsData());
            ChangeVolume(settingsData.VolumeLevelMusic);
            SetNextAudio();
        }

        private void Update()
        {
            if (!_audioSource.isPlaying)
                SetNextAudio();
        }

        private void SetNextAudio()
        {
            if (_audioClips.Length == _currentIndexAudioClip)
                _currentIndexAudioClip = 0;
            
            _audioSource.clip = _audioClips[_currentIndexAudioClip];
            _currentIndexAudioClip++;
            _audioSource.Play();
        }
        
        public void ChangeVolume(int value)
        {
            _audioSource.volume = (float)value / 100;
        }
    }
}
