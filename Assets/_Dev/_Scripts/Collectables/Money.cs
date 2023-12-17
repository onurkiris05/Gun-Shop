using DG.Tweening;
using Game.Core;
using Game.Managers;
using UnityEngine;

namespace Game.Collectables
{
    public class Money : BaseCollectable
    {
        [Header("Settings")]
        [SerializeField] private int amount;
        [SerializeField] private bool isGoingToConveyour;
        
        public int Amount => amount;

        private BoxCollider _collider;
        private bool _isCollected;

        #region UNITY EVENTS

        private void Awake() => _collider = GetComponent<BoxCollider>();
        
        protected override void OnTriggerEnter(Collider other)
        {
            if (_isCollected) return;

            if (other.TryGetComponent(out PlayerController player))
            {
                _isCollected = true;

                if (isGoingToConveyour)
                    ConveyorManager.Instance.SendMoneyToCashBox(this);
                else
                {
                    transform.DOScale(Vector3.zero, 0.2f).OnComplete(() =>
                    {
                        ImageSpawner.Instance.SpawnAndMove("Money", transform.position,
                            UIManager.Instance.WalletTextUI, 3);

                        EconomyManager.Instance.AddMoney(amount);
                        Kill();
                    });
                }
            }
        }

        #endregion

        #region PUBLIC METHODS

        public void SetState(bool state) => _collider.enabled = state;

        public void Kill() => Destroy(gameObject);

        #endregion
    }
}