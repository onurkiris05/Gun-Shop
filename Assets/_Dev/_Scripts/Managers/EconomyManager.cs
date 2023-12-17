using System;
using UnityEngine;

namespace Game.Managers
{
    public class EconomyManager : StaticInstance<EconomyManager>
    {
        [Header("Settings")]
        [SerializeField] private int currentWallet;
        [SerializeField] private int minigameWallet;

        #region ENCAPSULATIONS

        public int CurrentWallet
        {
            get => currentWallet;
            private set
            {
                currentWallet = Mathf.Max(value, 0);
                UIManager.Instance.SetWalletUI(currentWallet, true);
            }
        }

        public int MinigameWallet
        {
            get => minigameWallet;
            private set
            {
                minigameWallet = value;
                // UIManager.Instance.SetCardUI(minigameWallet);
            }
        }

        #endregion

        #region UNITY EVENTS

        private void OnEnable()
        {
            GameManager.OnLevelCompleted += SaveCurrentMoney;
        }

        private void OnDisable()
        {
            GameManager.OnLevelCompleted -= SaveCurrentMoney;
        }

        protected override void Awake()
        {
            base.Awake();

            CurrentWallet = PlayerPrefs.GetInt("Wallet", currentWallet);
        }

        #endregion

        #region PUBLIC METHODS

        public void AddMoney(int amount)
        {
            Debug.Log($"Setting wallet: {CurrentWallet} + {amount}");
            CurrentWallet += amount;
        }

        public bool SpendMoney(int amount)
        {
            if (amount > CurrentWallet)
            {
                Debug.LogError("Insufficient funds!");
                return false;
            }

            CurrentWallet -= amount;
            return true;
        }

        public void AddMinigameMoney(int amount)
        {
            Debug.Log($"Setting minigame wallet: {MinigameWallet} + {amount}");
            MinigameWallet += amount;
            GameManager.Instance.InvokeOnCardUpdate(MinigameWallet);
        }

        public bool SpendMinigameMoney(int amount)
        {
            if (amount > MinigameWallet)
            {
                Debug.LogError("Insufficient minigame funds!");
                return false;
            }

            MinigameWallet -= amount;
            GameManager.Instance.InvokeOnCardShopping(MinigameWallet);
            return true;
        }

        public void ResetMinigameMoney()
        {
            Debug.Log($"Minigame money reset!!");
            MinigameWallet = 0;
            GameManager.Instance.InvokeOnCardUpdate(MinigameWallet);
        }

        public void ShootOutMinigameMoney()
        {
            GameManager.Instance.InvokeOnCardUpdate(MinigameWallet);
        }

        public bool CanAfford(int amount)
        {
            if (amount > CurrentWallet)
                return false;

            return true;
        }

        #endregion

        #region PRIVATE METHODS

        private void SaveCurrentMoney()
        {
            PlayerPrefs.SetInt("Wallet", CurrentWallet);
        }

        #endregion
    }
}