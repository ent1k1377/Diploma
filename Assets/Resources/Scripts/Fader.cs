using UnityEngine;
using UnityEngine.Events;

namespace Resources.Scripts
{
    [RequireComponent(typeof(Animator))]
    public class Fader : MonoBehaviour
    {
        [SerializeField] private static GameObject _fadedCanvas;

        private const string FadePath = "Prefabs/Fader";

        private Animator _animator;
        private bool _faded;

        private static Fader _instance;
        private event UnityAction _fadedInCallback;
        private event UnityAction _fadedOutCallback;

        public static Fader Instance
        {
            get
            {
                if (_instance != null) return _instance;
                var prefab = UnityEngine.Resources.Load<Fader>(FadePath);
                _instance = Instantiate(prefab);
                DontDestroyOnLoad(_instance.gameObject);
                return _instance;
            }
        }

        private bool IsFading { get; set; }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void FadeIn(UnityAction fadedInCallback)
        {
            if (IsFading) return;
            IsFading = true;
            _fadedInCallback = fadedInCallback;

            _faded = true;
            _animator.SetTrigger("sceneEnd");
        }

        public void FadeOut(UnityAction fadedCallback)
        {
            Debug.Log(IsFading);
            if (IsFading) return;
            IsFading = true;
            _fadedOutCallback = fadedCallback;

            _faded = false;
            _animator.SetTrigger("sceneStart");
        }

        private void HandleFadeInAnimationOver()
        {
            _fadedInCallback?.Invoke();
            _fadedInCallback = null;
            IsFading = false;
        }

        private void HandleFadeOutAnimationOver()
        {
            _fadedOutCallback?.Invoke();
            _fadedOutCallback = null;
            IsFading = false;
        }
    }
}