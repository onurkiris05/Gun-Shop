using Game.Core;
using Game.Managers;
using Game.Projectiles;
using UnityEngine;

namespace Game.Minigame
{
    public class MinigameEnter : MonoBehaviour
    {
        private bool _isTriggered;

        #region UNITY EVENTS

        private void OnTriggerEnter(Collider other)
        {
            if (_isTriggered) return;

            if (other.TryGetComponent(out Bullet bullet))
            {
                bullet.Kill();
            }
            else if (other.TryGetComponent(out PlayerController player))
            {
                _isTriggered = true;
                MinigameManager.Instance.ProcessMinigameEnter(player);
            }
        }

        #endregion
    }
}