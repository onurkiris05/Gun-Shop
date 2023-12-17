using Game.Core;
using Game.Managers;
using Game.Projectiles;
using UnityEngine;

namespace Game.Minigame
{
    public class MinigameExit : MonoBehaviour
    {
        private bool _isTriggered;

        #region UNITY EVENTS

        private void OnTriggerExit(Collider other)
        {
            if (_isTriggered) return;

            if (other.TryGetComponent(out Bullet bullet))
            {
                bullet.Kill();
            }
            else if (other.TryGetComponent(out PlayerController player))
            {
                _isTriggered = true;
                MinigameManager.Instance.ProcessMinigameExit(player);
            }
        }

        #endregion
    }
}