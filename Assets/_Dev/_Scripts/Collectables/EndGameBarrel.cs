using DG.Tweening;
using Game.Core;
using Game.Managers;
using Game.Projectiles;
using TMPro;
using UnityEngine;

namespace Game.Collectables
{
    public class EndGameBarrel : BaseCollectable
    {
        [Header("Settings")]
        [SerializeField] private int hitCount;
        
        [Space][Header("Components")]
        [SerializeField] private GameObject barrelObject;
        [SerializeField] private Money moneyPrize;
        [SerializeField] private Transform collectablePos;
        [SerializeField] private TextMeshPro hitCountText;

        private bool _isKilled;

        #region UNITY EVENTS

        private void Start()
        {
            hitCountText.text = $"{hitCount}";
            moneyPrize.SetState(false);
        }
        
        protected override void OnTriggerEnter(Collider other)
        {
            if (_isKilled) return;

            if (other.TryGetComponent(out Bullet bullet))
            {
                hitCount--;
                hitCountText.text = $"{hitCount}";
                
                VFXSpawner.Instance.PlayVFX("BulletHit", bullet.transform.position);
                bullet.Kill();

                barrelObject.transform.DOComplete();
                barrelObject.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);

                if (hitCount <= 0)
                {
                    _isKilled = true;
                    ReleasePrize();
                }
            }
            else if (other.TryGetComponent(out PlayerController player))
            {
                GameManager.Instance.ChangeState(GameState.EndGame);
            }
        }

        #endregion

        #region PRIVATE METHODS

        private void ReleasePrize()
        {
            VFXSpawner.Instance.PlayVFX("BarrelExplosion", transform.position);
            hitCountText.enabled = false;
            barrelObject.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack)
                .OnComplete(() => barrelObject.SetActive(false));
            moneyPrize.transform.DOJump(collectablePos.position, 3, 1, 0.5f)
                .OnComplete(() => moneyPrize.SetState(true));
        }

        #endregion
    }
}