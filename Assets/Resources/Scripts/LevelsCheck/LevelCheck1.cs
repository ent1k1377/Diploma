using Resources.Scripts.InteractiveObjects;
using UnityEngine;

namespace Resources.Scripts.LevelsCheck
{
    public class LevelCheck1 : MonoBehaviour
    {
        [SerializeField] private InformationBlock _informationBlock;
        [SerializeField] private Player.Player _player;
        [SerializeField] private FinalScreenView _finalScreenView;

        private Transform _initialInformationBlock;
        private Transform _initialPlayer;

        private void Start()
        {
            //_initialInformationBlock = _informationBlock.transform;
            //_initialPlayer = _player.transform;
        }

        public void CheckInformationBlock()
        {
            if (_informationBlock.GetPosition() == new Vector3(-2, -5 , 0))
                _finalScreenView.EnableWinScreen();
            else
                _finalScreenView.EnableLossScreen();

        }

        private void SetInitialValues()
        {
            _informationBlock.transform.position = _initialInformationBlock.position;
            _informationBlock.transform.SetParent(_initialInformationBlock.parent);

            _player.transform.position = _initialPlayer.position;
            _player.transform.SetParent(_initialPlayer.parent);
        }
    }
}
