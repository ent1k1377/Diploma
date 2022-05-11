using UnityEngine;
using UnityEngine.UI;

namespace Resources.Scripts.Command.UI
{
    public class UICommandManager : MonoBehaviour
    {
        [SerializeField] private GraphicRaycaster _graphicRaycaster;
        [SerializeField] private Transform _commandFieldContainer;
        [SerializeField] private GameObject _content;
        [SerializeField] private GameObject _plug;

        private Camera _camera;
        
        public static UICommandManager Instance { get; private set; }
        
        public GraphicRaycaster GraphicRaycaster => _graphicRaycaster;
        public Transform CommandFieldContainer => _commandFieldContainer;
        public GameObject Content => _content;
        public Camera Camera => _camera;

        public void EnablePlug() => _plug.SetActive(true);
        public void DisablePlug() => _plug.SetActive(false);
        
        private void Awake()
        {
            _camera = Camera.main;
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                return;
            }
            Destroy(gameObject);
        }
    }
}
