using DG.Tweening;
using Game.Collectables;
using UnityEngine;

namespace Game.Managers
{
    public class ConveyorManager : StaticInstance<ConveyorManager>
    {
        [Header("Settings")]
        [SerializeField] private float conveyorSpeed;

        [Space] [Header("Components")]
        [SerializeField] private Transform[] cashBoxes;
        [SerializeField] private CheckoutChest[] checkoutChests;

        private int _index;

        #region PUBLIC METHODS

        public void SendMoneyToCashBox(Money money)
        {
            var t = money.transform;
            var jumpPos = new Vector3(cashBoxes[_index].position.x,
                cashBoxes[_index].position.y, t.position.z);

            // Jump money object to conveyor at sides
            money.transform.DOJump(jumpPos, 1f, 1, 0.5f)
                .OnComplete(() =>
                {
                    // Then move money object to cash box
                    money.transform.DOMove(cashBoxes[_index].position, conveyorSpeed)
                        .SetEase(Ease.Linear).SetSpeedBased(true).OnComplete(() =>
                        {
                            AnimationManager.Instance.SetAnimationTrigger
                                ($"Cashbox_{_index}", "moneyIn");
                            EconomyManager.Instance.AddMinigameMoney(money.Amount);
                            InteractEffect();
                            money.Kill();
                        });
                });
        }

        public void SendGunPartsToCheckoutChest(GameObject item)
        {
            // Define jump position for gun part
            var t = item.transform;
            var jumpPos = new Vector3(checkoutChests[_index].transform.position.x,
                checkoutChests[_index].transform.position.y, t.position.z);

            // Jump gun part object to conveyor
            item.transform.DOJump(jumpPos, 1f, 1, 0.5f)
                .OnComplete(() =>
                {
                    // Then move gun part object to checkout line
                    var targetPos = new Vector3(checkoutChests[_index].transform.position.x,
                        checkoutChests[_index].transform.position.y, checkoutChests[_index].transform.position.z - 10f);

                    item.transform.DOMove(targetPos, conveyorSpeed).SetEase(Ease.Linear).SetSpeedBased(true)
                        .OnComplete(() =>
                        {
                            checkoutChests[_index].ProcessPutItemInsideAnimation();

                            item.transform.DOJump(checkoutChests[_index].transform.position, 1, 1, 0.3f)
                                .OnComplete(() => { item.SetActive(false); });
                        });
                });
        }

        public void MoveToNextCashBox()
        {
            _index++;
            if (_index >= cashBoxes.Length) _index = cashBoxes.Length - 1;
        }

        #endregion

        #region PRIVATE METHODS

        private void InteractEffect()
        {
            cashBoxes[_index].DOComplete();
            cashBoxes[_index].DOShakeScale(0.3f, new Vector3(0.4f, 0.4f, 0.4f));
        }

        #endregion
    }
}