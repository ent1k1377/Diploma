using UnityEngine;
using UnityEngine.Events;

namespace Resources.Scripts.Scene
{
    [RequireComponent(typeof(Animator))]
    public class Fader : MonoBehaviour
    {
        private Animator _animator;

        private static Fader _instance;
        private event UnityAction _fadedInCallback;
        private event UnityAction _fadedOutCallback;

        public static Fader Instance => _instance;

        private void Start()
        {
            if (_instance == null) 
                _instance = this; 
            else if(_instance == this)
                Destroy(gameObject);
            
            DontDestroyOnLoad(gameObject);
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

            _animator.SetTrigger("sceneEnd");
        }

        public void FadeOut(UnityAction fadedCallback)
        {
            if (IsFading) return;
            IsFading = true;
            _fadedOutCallback = fadedCallback;

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