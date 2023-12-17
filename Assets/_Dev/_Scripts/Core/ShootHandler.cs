using System.Collections;
using Game.Incrementals;
using Game.Managers;
using Game.Projectiles;
using UnityEngine;

namespace Game.Core
{
    public class ShootHandler : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float fireRate;
        [SerializeField] private float fireRateDivider;
        [SerializeField] private float fireRange;
        [SerializeField] private float firePower;

        [Space] [Header("Components")]
        [SerializeField] private Transform muzzle;

        #region ENCAPSULATIONS

        public float FireRate
        {
            get => fireRate;
            private set { fireRate = Mathf.Max(value, 1f); }
        }

        public float FireRange
        {
            get => fireRange;
            private set { fireRange = Mathf.Max(value, 5f); }
        }

        public float FirePower
        {
            get => firePower;
            private set { firePower = Mathf.Max(value, 1); }
        }

        #endregion

        private BoxCollider _gunCollider;
        private Coroutine _shootCoroutine;
        private bool _isMuzzleSettled;

        #region UNITY EVENTS

        private void Awake() => _gunCollider = GetComponent<BoxCollider>();

        private void OnEnable()
        {
            GameManager.OnAfterStateChanged += ProcessShooting;
            GameManager.OnLevelCompleted += SaveAttributes;
        }

        private void OnDisable()
        {
            GameManager.OnAfterStateChanged -= ProcessShooting;
            GameManager.OnLevelCompleted -= SaveAttributes;
        }

        private void Start()
        {
            FireRate = PlayerPrefs.GetFloat("FireRate", fireRate);
            FireRange = PlayerPrefs.GetFloat("FireRange", fireRange);
            FirePower = PlayerPrefs.GetFloat("FirePower", firePower);
        }

        #endregion

        #region PUBLIC METHODS

        public void SetValue(UpgradeTypes upgradeTypes, float value)
        {
            switch (upgradeTypes)
            {
                case UpgradeTypes.FireRate:
                    Debug.Log($"Setting FireRate: {FireRate} + {value}");
                    FireRate += value;
                    break;
                case UpgradeTypes.FirePower:
                    Debug.Log($"Setting FirePower: {FirePower} + {value}");
                    FirePower += value;
                    break;
                case UpgradeTypes.FireRange:
                    Debug.Log($"Setting FireRange: {FireRange} + {value}");
                    FireRange += value;
                    break;
            }
        }

        public void ProcessIncrementalButton(IncrementalButton button)
        {
            if (EconomyManager.Instance.SpendMoney(button.Cost))
            {
                switch (button.Type)
                {
                    case ButtonType.Rate:
                        Debug.Log($"Setting FireRate: {FireRate} + {button.Amount}");
                        FireRate += button.Amount;
                        break;
                    case ButtonType.Power:
                        Debug.Log($"Setting FirePower: {FirePower} + {button.Amount}");
                        FirePower += button.Amount;
                        break;
                    case ButtonType.Range:
                        Debug.Log($"Setting FireRange: {FireRange} + {button.Amount}");
                        FireRange += button.Amount;
                        break;
                }
            }
            else
                Debug.Log("Not enough money for buttons!!");
        }

        public void SetMuzzlePosition()
        {
            if (_isMuzzleSettled) return;

            // Set to generic muzzle position for all guns
            var pos = new Vector3(muzzle.localPosition.x, muzzle.localPosition.y, muzzle.localPosition.z + 2.8f);
            muzzle.localPosition = pos;
            _gunCollider.center = pos;
            _isMuzzleSettled = true;
        }

        #endregion

        #region PRIVATE METHODS

        private void ProcessShooting(GameState gameState)
        {
            if (gameState == GameState.Running)
            {
                _shootCoroutine = StartCoroutine(ShootCoroutine());
            }
            else
            {
                if (_shootCoroutine != null)
                {
                    StopCoroutine(_shootCoroutine);
                    _shootCoroutine = null;
                }
            }
        }

        private IEnumerator ShootCoroutine()
        {
            while (true)
            {
                Shoot();
                yield return new WaitForSeconds(fireRateDivider / FireRate);
            }
        }

        private void Shoot()
        {
            var bullet = ObjectPooler.Instance.Spawn("Bullet", muzzle.position, transform.rotation);
            bullet.GetComponent<Bullet>().Init(FireRange, FirePower);
            VFXSpawner.Instance.PlayVFX("MuzzleEffect", muzzle.position, muzzle.rotation.eulerAngles);
        }

        private void SaveAttributes()
        {
            PlayerPrefs.SetFloat("FireRate", fireRate);
            PlayerPrefs.SetFloat("FireRange", fireRange);
            PlayerPrefs.SetFloat("FirePower", firePower);
        }

        #endregion
    }
}