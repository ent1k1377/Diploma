using Resources.Scripts.Command.UI;
using UnityEngine;

namespace Resources.Scripts.Command
{
    public class UIContainerCommand : MonoBehaviour
    {
        [SerializeField] private UICommand _uiCommandPrefab;

        private void Start()
        {
            SpawnUICommand();
        }
   
        public void SpawnUICommand()
        {
            Instantiate(_uiCommandPrefab, transform.position, Quaternion.identity, transform).gameObject.SetActive(true);
        }
    }
}
