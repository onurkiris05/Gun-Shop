using Game.Core;
using Game.Managers;
using UnityEngine;

namespace Game.Shop
{
    public class ShopItemGun : ShopItem
    {
        [Header("Settings")]
        [SerializeField] private GunStats gunPartStat;
        [SerializeField] private float rotateSpeed;
        [SerializeField] private Vector3 itemScale;

        [Space] [Header("Components")]
        [SerializeField] private GameObject gunParent;
        [SerializeField] private Gun[] guns;

        public GunStats Stat => gunPartStat;
        public Gun[] Models => guns;
        

        #region UNITY EVENTS

        private void Start() => Init();

        private void Update()
        {
            gunParent.transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f);
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (_isTriggered) return;

            if (other.TryGetComponent(out PlayerController player))
            {
                _isTriggered = true;
                GameManager.Instance.ChangeState(GameState.MinigameShopping);
                player.ProcessShopping(gunParent, this);
            }
        }

        #endregion

        #region PRIVATE METHODS

        private void Init()
        {
            ResetGuns();

            gunParent.transform.localScale = itemScale;
            itemCostText.text = $"${gunPartStat.ItemCost}";

            var activeItem = guns[gunPartStat.GunLevel - 1].Parts[gunPartStat.GunPartLevel - 1];
            activeItem.transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
            activeItem.SetActive(true);
        }

        private void ResetGuns()
        {
            for (int i = 0; i < guns.Length; i++)
            {
                for (int j = 0; j < guns[i].Parts.Length; j++)
                    guns[i].Parts[j].SetActive(false);
            }
        }

        #endregion
    }
}