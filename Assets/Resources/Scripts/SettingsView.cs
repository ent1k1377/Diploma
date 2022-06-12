using Resources.Scripts.Storage;
using UnityEngine;
using UnityEngine.UI;

namespace Resources.Scripts
{
    [RequireComponent(typeof(Animator))]
    public class SettingsView : MonoBehaviour
    {
        [SerializeField] private Slider _musicAndSoundSlider;
        [SerializeField] private Text _musicAndSoundValue;

        private Animator _animator;

        private Storage.Storage _storage;
        private SettingsData _settingsData;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _storage = new Storage.Storage();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                CloseSettings();
        }

        public void OpenSettings()
        {
            _animator.SetTrigger("open");
            Load();
        }

        public void CloseSettings()
        {
            _animator.SetTrigger("close");
            Save();
        }

        private void Save()
        {
            _settingsData.VolumeLevelMusic = (int)_musicAndSoundSlider.value;
            _settingsData.VolumeLevelSound = (int)_musicAndSoundSlider.value;
            _storage.Save(_settingsData);
            Debug.Log("Settings saved");
        }
        
        private void Load()
        {
            _settingsData = (SettingsData)_storage.Load(new SettingsData());
            _musicAndSoundSlider.value = _settingsData.VolumeLevelMusic;
            Debug.Log("Loaded settings");
        }

        private void HandleCloseSettingsAnimationOver() => _animator.gameObject.SetActive(false);

        public void ChangeValueMusicAndSound()
        {
            _musicAndSoundValue.text = $"{(int)_musicAndSoundSlider.value}";
            MusicController.Instance.ChangeVolume((int)_musicAndSoundSlider.value);
        }
    }
}
