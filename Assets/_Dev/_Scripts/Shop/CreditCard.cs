using DG.Tweening;
using Game.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Shop
{
    [System.Serializable]
    public class CardStat
    {
        public int MoneyToLevelUp;
    }

    public class CreditCard : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private CardStat[] cardStats;

        [Space] [Header("Components")]
        [SerializeField] private GameObject[] cards;
        [SerializeField] private Slider levelBar;
        [SerializeField] private TextMeshProUGUI cardWalletText;
        [SerializeField] private TextMeshProUGUI cardLevelText;

        private GameObject _currentCard;

        private void OnEnable()
        {
            GameManager.OnCardUpdate += ProcessCardUpdate;
            GameManager.OnCardShopping += SetCardWalletText;
        }

        private void OnDisable()
        {
            GameManager.OnCardUpdate -= ProcessCardUpdate;
            GameManager.OnCardShopping -= SetCardWalletText;
        }

        private void ProcessCardUpdate(int walletAmount)
        {
            SetCardWalletText(walletAmount);

            for (int i = 0; i < cardStats.Length; i++)
            {
                if (walletAmount < cardStats[i].MoneyToLevelUp)
                {
                    // Set card
                    int currentIndex = Mathf.Max(i - 1, 0);
                    if (_currentCard != cards[currentIndex])
                    {
                        ResetCards();
                        _currentCard = cards[currentIndex];
                        _currentCard.SetActive(true);
                        SetLevelBar(0f);
                        SetLevelText(i);
                    }

                    // Set card level bar
                    float playerProgress = walletAmount - cardStats[currentIndex].MoneyToLevelUp;
                    float levelProgress = cardStats[i].MoneyToLevelUp - cardStats[currentIndex].MoneyToLevelUp;

                    var currentProgress = playerProgress / levelProgress;
                    SetLevelBar(currentProgress);

                    break;
                }
            }
        }

        private void SetLevelBar(float amount)
        {
            DOTween.Complete(this);
            DOTween.To(x => levelBar.value = x, levelBar.value, amount, 0.2f)
                .SetEase(Ease.Linear).SetId(this);
        }

        private void SetLevelText(int level)
        {
            cardLevelText.text = $"Level {level}";
        }

        private void SetCardWalletText(int walletAmount)
        {
            cardWalletText.text = $"${walletAmount}";
        }

        private void ResetCards()
        {
            foreach (var card in cards)
                card.SetActive(false);
        }
    }
}