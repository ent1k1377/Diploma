using System.Collections.Generic;
using UnityEngine;

namespace Resources.Scripts.Command
{
    public class CommandManager : MonoBehaviour
    {
        [SerializeField] private List<Player.Player> _player;
        
        private List<Command> _commands;

        private void RunAlgorithm()
        {
            foreach (var player in _player)
            {
                
            }
        }
    }
}



