using DG.Tweening;
using Game.Core;
using UnityEngine;

namespace Game.Managers
{
    public class MinigameManager : StaticInstance<MinigameManager>
    {
        [Header("Components")]
        [SerializeField] private Transform[] posObjects;
        [SerializeField] private Transform[] receiptObjects;
        [SerializeField] private Transform[] entranceCards;
        [SerializeField] private CheckoutChest[] checkoutChests;

        private int _index;

        #region PUBLIC METHODS

        public void ProcessMinigameEnter(PlayerController player)
        {
            CameraManager.Instance.SetCamera(CameraType.Minigame);
            GameManager.Instance.ChangeState(GameState.MinigameRunning);
            player.ProcessMinigameStart();

            // Make disappear card at the beginning and play VFX
            entranceCards[_index].DOScale(Vector3.zero, 0.4f).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    VFXSpawner.Instance.PlayVFX("CardDisappearEffect", entranceCards[_index].transform.position);
                });
        }

        public void ProcessMinigameExit(PlayerController player)
        {
            GameManager.Instance.ChangeState(GameState.MinigameEnd);
            CameraManager.Instance.SetCamera(CameraType.CloseLookUp);
            ConveyorManager.Instance.MoveToNextCashBox();

            player.ProcessMinigameEnd(posObjects[_index], receiptObjects[_index], checkoutChests[_index]);
            _index++;
        }

        #endregion
    }
}