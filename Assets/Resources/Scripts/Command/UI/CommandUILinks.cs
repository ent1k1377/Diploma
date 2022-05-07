using UnityEngine;
using UnityEngine.UI;

namespace Resources.Scripts.Command.UI
{
    public class CommandUILinks : MonoBehaviour
    {
        [SerializeField] private GraphicRaycaster _graphicRaycaster;
        [SerializeField] private Transform _commandFieldContainer;
        [SerializeField] private GameObject _plug;
        
        public static CommandUILinks Instance { get; private set; }
        
        public GraphicRaycaster GraphicRaycaster => _graphicRaycaster;
        public Transform CommandFieldContainer => _commandFieldContainer;

        public void EnablePlug() => _plug.SetActive(true);
        public void DisablePlug() => _plug.SetActive(false);
        
        private void Awake()
        {
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
