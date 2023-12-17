using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game.Managers;
using Game.Shop;
using UnityEngine;
using CameraType = Game.Managers.CameraType;

namespace Game.Core
{
    [System.Serializable]
    public class Gun
    {
        public GameObject[] Parts;
    }

    public class GunHandler : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Transform gunParent;
        [SerializeField] private Gun[] guns;

        private PlayerController _player;

        #region PUBLIC METHODS

        public void Init(PlayerController player) => _player = player;

        public void AdjustGunParts(List<ShopItemGun> gunPartsList, CheckoutChest chest)
        {
            // First rotate gun parent to camera then proceed to upgrade animation
            var originalPos = gunParent.transform.localPosition;
            var originalRotation = new Vector3(0f, gunParent.eulerAngles.y, 0f);

            // Get closer checkout chest
            chest.transform.DOLocalMove(new Vector3(-1f, 3f, 44f), 0.5f);
            chest.transform.DOLocalRotate(new Vector3(-38f, 150f, 9f), 0.5f);

            // Rotate gun parent to see it from side
            gunParent.DOLocalMove(new Vector3(originalPos.x, 2.8f, originalPos.z), 0.5f);
            gunParent.DOLocalRotate(new Vector3(-33f, -175f, 46f), 0.5f)
                .SetEase(Ease.InOutBack).OnComplete(() =>
                    StartCoroutine(ProcessUpgradeAnimation(gunPartsList, originalPos, originalRotation, chest)));
        }

        #endregion

        #region PRIVATE METHODS

        private IEnumerator ProcessUpgradeAnimation(
            List<ShopItemGun> gunPartsList,
            Vector3 originalPos,
            Vector3 originalRotation,
            CheckoutChest chest)
        {
            chest.ProcessGetItemInsideAnimation();

            // Firstly, spawn gun parts somewhere else and then move it original pos
            foreach (var gun in gunPartsList)
            {
                var gunPart = GetPart(gun.Models);
                if (gunPart == null)
                {
                    Debug.LogError("Gun part not found!");
                    continue;
                }

                // Firstly, spawn gun part somewhere else and then move it original pos
                var partOriginalPos = gunPart.localPosition;
                var partOriginalRot = gunPart.localScale;
                var spawnPos = new Vector3(
                    chest.transform.position.x,
                    chest.transform.position.y,
                    chest.transform.position.z);

                gunPart.localScale = Vector3.zero;
                gunPart.position = spawnPos;
                gunPart.gameObject.SetActive(true);
                gunPart.DOScale(partOriginalRot, 0.2f);
                gunPart.DOLocalJump(partOriginalPos, 0.3f, 1, 0.4f)
                    .SetEase(Ease.Linear).OnComplete(() =>
                    {
                        _player.Reward(gun.Stat.UpgradeType, gun.Stat.UpgradeAmount);

                        TextSpawner.Instance.SpawnAndFade("Floating",
                            $"+{gun.Stat.UpgradeAmount} {gun.Stat.UpgradeType}", gunPart.position);

                        gunParent.DOShakeScale(0.3f, 2f);
                        SetPart(gun.Models);
                    });

                yield return Helpers.BetterWaitForSeconds(0.7f);
            }

            // After upgrades, rotate gun parent back to original rotation
            gunParent.DOLocalMove(originalPos, 0.5f);
            gunParent.DOLocalRotate(originalRotation, 0.5f).OnComplete(() =>
            {
                GameManager.Instance.ChangeState(GameState.Running);
                CameraManager.Instance.SetCamera(CameraType.Running);
                gunPartsList.Clear();
            });

            // Wipe off checkout chest
            chest.transform.DOScale(Vector3.zero, 0.3f);
        }

        private void SetPart(Gun[] gunPool)
        {
            for (int i = 0; i < gunPool.Length; i++)
            {
                for (int j = 0; j < gunPool[i].Parts.Length; j++)
                {
                    if (gunPool[i].Parts[j].activeSelf)
                    {
                        ResetPart(j);
                        guns[i].Parts[j].SetActive(true);
                        CheckMuzzlePosition();
                    }
                }
            }
        }

        private Transform GetPart(Gun[] gunPool)
        {
            for (int i = 0; i < gunPool.Length; i++)
            {
                for (int j = 0; j < gunPool[i].Parts.Length; j++)
                {
                    if (gunPool[i].Parts[j].activeSelf)
                        return guns[i].Parts[j].transform;
                }
            }

            return null;
        }

        private void ResetPart(int index)
        {
            for (int i = 0; i < guns.Length; i++)
                guns[i].Parts[index].SetActive(false);
        }

        private void ResetGuns()
        {
            for (int i = 0; i < guns.Length; i++)
            {
                for (int j = 0; j < guns[i].Parts.Length; j++)
                    guns[i].Parts[j].SetActive(false);
            }
        }

        private void SaveGuns()
        {
            for (int i = 0; i < guns.Length; i++)
            {
                for (int j = 0; j < guns[i].Parts.Length; j++)
                {
                    var isActiveKey = $"Gun_{i}_Part_{j}_Active";
                    PlayerPrefs.SetInt(isActiveKey, guns[i].Parts[j].activeSelf ? 1 : 0);
                }
            }
        }

        private void LoadGuns()
        {
            for (int i = 0; i < guns.Length; i++)
            {
                for (int j = 0; j < guns[i].Parts.Length; j++)
                {
                    var isActiveKey = $"Gun_{i}_Part_{j}_Active";
                    var isActive = PlayerPrefs.GetInt(isActiveKey, 0);
                    guns[i].Parts[j].SetActive(isActive == 1);
                }
            }
        }

        private void CheckMuzzlePosition()
        {
            for (int i = 0; i < guns.Length; i++)
            {
                if (guns[i].Parts[2].activeSelf)
                {
                    _player.ProcessMuzzlePosition();
                    break;
                }
            }
        }

        #endregion
    }
}