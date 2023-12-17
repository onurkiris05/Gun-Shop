using System.Collections.Generic;
using DG.Tweening;
using Game.Shop;
using UnityEngine;

namespace Game.Core
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private ParticleSystem stunVFX;

        public bool IsTrapped => _isTrapped;

        private MovementHandler _movementHandler;
        private ShootHandler _shootHandler;
        private GunHandler _gunHandler;
        private MinigameHandler _minigameHandler;
        private bool _isTrapped;
        private float _speed;

        #region UNITY EVENTS

        private void Awake()
        {
            _shootHandler = GetComponent<ShootHandler>();
            _gunHandler = GetComponent<GunHandler>();
            _minigameHandler = GetComponent<MinigameHandler>();
            _movementHandler = GetComponent<MovementHandler>();

            _minigameHandler.Init(this);
            _movementHandler.Init(this);
            _gunHandler.Init(this);
        }

        #endregion

        #region PUBLIC METHODS

        public void ProcessMinigameStart()
        {
            _minigameHandler.ProcessMinigameStart();
        }

        public void ProcessMinigameEnd(Transform posTransform, Transform receiptTransform, CheckoutChest chest)
        {
            _minigameHandler.ProcessMinigameEnd(posTransform, receiptTransform, chest);
        }

        public void ProcessShopping(GameObject item, ShopItemGun gunPart)
        {
            _minigameHandler.ProcessShopping(item, gunPart);
        }

        public void ProcessMuzzlePosition()
        {
            _shootHandler.SetMuzzlePosition();
        }

        public void AdjustGunParts(List<ShopItemGun> gunPartsList, CheckoutChest chest)
        {
            _gunHandler.AdjustGunParts(gunPartsList, chest);
        }

        public void PushBack(float pushBackDistance, float pushBackDuration)
        {
            _isTrapped = true;
            stunVFX.Play();

            transform.DOMoveZ(transform.position.z - pushBackDistance, pushBackDuration)
                .OnComplete(() =>
                {
                    stunVFX.Stop();
                    _isTrapped = false;
                });
        }

        public void Reward(UpgradeTypes upgradeType, float value)
        {
            _shootHandler.SetValue(upgradeType, value);
        }

        #endregion
    }
}