using DG.Tweening;
using TMPro;
using UnityEngine;
using Game.Core;
using Game.Projectiles;

namespace Game.Gates
{
    public abstract class BaseGate : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] protected UpgradeTypes upgradeType;
        [SerializeField] protected float value;
        [SerializeField] protected bool isLocked;
        [SerializeField] protected int unlockCount;

        [Space] [Header("Components")]
        [SerializeField] protected GameObject greenGate;
        [SerializeField] protected GameObject redGate;
        [SerializeField] protected GameObject grayGate;
        [SerializeField] protected TextMeshPro gateText;
        [SerializeField] protected TextMeshPro valueText;
        [SerializeField] protected GameObject lockBar;
        [SerializeField] protected GameObject pointer;

        protected float _perUnlockCount;
        protected bool _isKilled;

        #region UNITY EVENTS

        protected virtual void Start()
        {
            InitGate(upgradeType, value);
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (_isKilled) return;

            if (other.TryGetComponent(out Bullet bullet))
            {
                VFXSpawner.Instance.PlayVFX("BulletHit", bullet.transform.position);
                InteractEffect();
                bullet.Kill();

                if (isLocked)
                {
                    UpdateLockBar();
                    return;
                }

                UpdateGate();
            }
            else if (other.TryGetComponent(out PlayerController player))
            {
                if (!isLocked)
                {
                    player.Reward(upgradeType, value);
                    KillGate();
                }
            }
        }

        #endregion

        #region PROTECTED VIRTUAL METHODS

        protected virtual void InitGate(UpgradeTypes upgradeTypes, float value)
        {
            if (isLocked)
            {
                lockBar.SetActive(isLocked);
                _perUnlockCount = Mathf.Abs(pointer.transform.localPosition.x) * 2.0f / unlockCount;
            }

            SetGate(value);

            switch (upgradeTypes)
            {
                case UpgradeTypes.FireRange:
                    gateText.text = "Range";
                    break;
                case UpgradeTypes.FirePower:
                    gateText.text = "Power";
                    break;
                case UpgradeTypes.FireRate:
                    gateText.text = "Rate";
                    break;
            }
        }

        protected virtual void SetGate(float value)
        {
            var isGreenGate = value > 0;
            valueText.text = isGreenGate ? $"+{value}" : $"{value}";

            grayGate.SetActive(isLocked);
            greenGate.SetActive(!isLocked && isGreenGate);
            redGate.SetActive(!isLocked && !isGreenGate);
        }

        protected virtual void UpdateGate()
        {
            value++;
            SetGate(value);
        }

        protected virtual void UpdateLockBar()
        {
            unlockCount--;
            if (unlockCount <= 0)
            {
                isLocked = false;
                lockBar.SetActive(isLocked);
                SetGate(value);
            }

            var t = pointer.transform;
            t.DOComplete();
            t.DOLocalMove(new Vector3(t.localPosition.x + _perUnlockCount, t.localPosition.y, t.localPosition.z), 1f)
                .SetSpeedBased(true).SetEase(Ease.OutCubic);
        }

        protected virtual void InteractEffect()
        {
            transform.DOComplete();
            transform.DOShakeScale(0.15f, new Vector3(0.2f, 0.2f, 0.2f));
        }

        protected virtual void KillGate()
        {
            _isKilled = true;
            transform.DOComplete();
            transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InElastic)
                .OnComplete(() => Destroy(gameObject));
        }

        #endregion
    }
}