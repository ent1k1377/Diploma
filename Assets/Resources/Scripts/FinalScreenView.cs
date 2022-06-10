using UnityEngine;

namespace Resources.Scripts
{
    public class FinalScreenView : MonoBehaviour
    {
        [SerializeField] private GameObject _win;
        [SerializeField] private GameObject _loss;

        public void EnableWinScreen()
        {
            Debug.Log("win");
            _win.gameObject.SetActive(true);
        }
        
        public void EnableLossScreen()
        {
            Debug.Log("loss");
            _loss.gameObject.SetActive(true);
        }
    }
}
