using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game.Managers;
using Game.Shop;
using TMPro;
using UnityEngine;
using CameraType = Game.Managers.CameraType;

namespace Game.Core
{
    public class MinigameHandler : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private GameObject playerGuns;
        [SerializeField] private GameObject playerHands;
        [SerializeField] private Transform rightHand;
        [SerializeField] private Transform leftHand;
        [SerializeField] private Transform card;
        [SerializeField] private GameObject barcodeXray;
        [SerializeField] private TextMeshProUGUI cardWalletText;

        private List<ShopItemGun> _gunPartsShoppingList = new();
        private PlayerController _player;

        #region PUBLIC METHODS

        public void Init(PlayerController player) => _player = player;

        public void ProcessMinigameStart()
        {
            // Switch gun model to hands model
            playerGuns.transform.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InExpo)
                .OnComplete(() =>
                {
                    playerGuns.SetActive(false);
                    playerHands.transform.localScale = Vector3.zero;
                    playerHands.SetActive(true);
                    playerHands.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutExpo)
                        .OnComplete(() =>
                        {
                            // Set camera and animations for hands
                            AnimationManager.Instance.SetAnimationBool("Hand-R", "default", true);
                            AnimationManager.Instance.SetAnimationBool("Hand-L", "default", true);
                        });

                    EconomyManager.Instance.ShootOutMinigameMoney();
                });
        }

        public void ProcessMinigameEnd(Transform posTransform, Transform receiptTransform, CheckoutChest chest)
        {
            var isListEmpty = _gunPartsShoppingList.Count <= 0;

            if (isListEmpty)
            {
                ProcessShoppingList(isListEmpty, chest);
            }
            else
            {
                AnimationManager.Instance.SetAnimationBool("Hand-R", "default", false);

                // Store original positions and rotations
                var leftHandOriginalPos = leftHand.localPosition;
                var leftHandOriginalRotate = leftHand.eulerAngles;
                var receiptOriginalPos = receiptTransform.position;
                var endPos = posTransform.position;

                // Align with POS object
                transform.DOMove(new Vector3(endPos.x, transform.position.y, transform.position.z), 0.5f)
                    .OnComplete(() =>
                    {
                        // Adjust card position and rotation
                        AnimationManager.Instance.SetAnimationTrigger("Hand-L", "payment");
                        card.DOLocalRotate(new Vector3(-63f, 188f, 89f), 2f);
                        card.DOLocalMove(new Vector3(-0.00957f, 0.02252f, 0.02589f), 2f);

                        // Get POS object closer
                        posTransform.DOLocalRotate(new Vector3(0, -90f, 30f), 0.6f);
                        posTransform.DOLocalMove(new Vector3(1.7f, 2.7f, 43f), 0.6f)
                            .OnComplete(() =>
                            {
                                // Animations for card interaction with POS object
                                leftHand.DOLocalRotate(new Vector3(-30f, -257, 38f), 0.5f);
                                leftHand.DOLocalMove(new Vector3(-0.35f, -1f, 3f), 0.5f)
                                    .OnComplete(() =>
                                    {
                                        leftHand.DOLocalRotate(new Vector3(-30f, -257, 38f), 0.3f);
                                        leftHand.DOLocalMove(new Vector3(-0.4f, -2.7f, 1f), 0.3f)
                                            .OnComplete(() =>
                                            {
                                                // Give alive effect to receipt object
                                                receiptTransform.GetChild(0).transform.DOShakePosition
                                                        (0.25f, new Vector3(0.5f, 0f, 0f), 10)
                                                    .SetLoops(10, LoopType.Yoyo);
                                                receiptTransform.GetComponent<DynamicBone>().m_BlendWeight = 1f;
                                                receiptTransform.DOLocalMove(new Vector3(0.67f, 1.44f, 0f), 1f);

                                                leftHand.DOLocalRotate(leftHandOriginalRotate, 0.6f);
                                                leftHand.DOLocalMove(leftHandOriginalPos, 1f)
                                                    .OnComplete(() =>
                                                    {
                                                        // Move POS object out of screen
                                                        posTransform.DOLocalMoveX(posTransform.position.x - 8f, 2f)
                                                            .SetEase(Ease.OutBounce).OnComplete(() =>
                                                            {
                                                                receiptTransform.GetComponent<DynamicBone>()
                                                                        .m_BlendWeight =
                                                                    0f;
                                                                receiptTransform.localPosition = receiptOriginalPos;
                                                            });

                                                        ProcessShoppingList(isListEmpty, chest);
                                                    });
                                            });
                                    });
                            });
                    });
            }
        }

        public void ProcessShopping(GameObject item, ShopItemGun gunPart)
        {
            // Adjust close lookup and state for barcode reading
            GameManager.Instance.ChangeState(GameState.MinigameShopping);
            CameraManager.Instance.SetCamera(CameraType.CloseLookUp);
            AnimationManager.Instance.SetAnimationBool("Hand-R", "default", false);
            AnimationManager.Instance.SetAnimationBool("Hand-L", "default", false);

            // Align position with shop item
            transform.DOMove(new Vector3(
                item.transform.position.x,
                transform.position.y,
                item.transform.position.z - 5f), 0.5f).OnComplete(() =>
            {
                var originalPos = rightHand.localPosition;
                var originalRotate = rightHand.eulerAngles;

                // Prepare for barcode read animation
                rightHand.DOLocalRotate(new Vector3(-37f, -205f, 8f), 0.3f);
                rightHand.DOLocalMove(new Vector3(0f, -2.8f, 0.7f), 0.3f)
                    .OnComplete(() =>
                    {
                        StartCoroutine(ProcessBarcodeRead(item, gunPart, originalPos, originalRotate));
                    });
            });
        }

        #endregion

        #region PRIVATE METHODS

        private void ProcessShoppingList(bool isListEmpty, CheckoutChest chest)
        {
            // Scale down hands object and prepare for gun upgrade animation
            playerHands.transform.DOScale(Vector3.zero, 0.5f)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    AnimationManager.Instance.SetAnimationBool("Hand-R", "default", false);
                    AnimationManager.Instance.SetAnimationBool("Hand-L", "default", false);
                    EconomyManager.Instance.ResetMinigameMoney();

                    // Move card to the original position
                    card.transform.localPosition = new Vector3
                        (-0.01552f, 0.02178f, 0.01195f);
                    card.transform.localRotation = Quaternion.Euler
                        (new Vector3(-63.11f, 188.52f, 12.37f));

                    // Switch hands model to gun model
                    playerHands.SetActive(false);
                    playerGuns.transform.localScale = Vector3.zero;
                    playerGuns.SetActive(true);
                    playerGuns.transform.DOScale(Vector3.one, 0.5f)
                        .OnComplete(() =>
                        {
                            // Proceed gun upgrade if there is any gun part listed
                            if (!isListEmpty)
                            {
                                CameraManager.Instance.SetCamera(CameraType.Upgrade);
                                _player.AdjustGunParts(_gunPartsShoppingList, chest);
                            }
                            // Otherwise, proceed to running game
                            else
                            {
                                GameManager.Instance.ChangeState(GameState.Running);
                                CameraManager.Instance.SetCamera(CameraType.Running);
                            }
                        });
                });
        }

        private IEnumerator ProcessBarcodeRead(
            GameObject item,
            ShopItemGun gunPart,
            Vector3 originalPos,
            Vector3 originalRotate)
        {
            // Randomly open and close Xray light
            for (int i = 0; i < 5; i++)
            {
                barcodeXray.SetActive(!barcodeXray.activeSelf);
                float randomInterval = Random.Range(0.05f, 0.1f);
                yield return Helpers.BetterWaitForSeconds(randomInterval);
            }

            // Check if money is enough to buy item
            if (EconomyManager.Instance.SpendMinigameMoney(gunPart.Stat.ItemCost))
            {
                Debug.Log("Item added to shopping list!!");
                ConveyorManager.Instance.SendGunPartsToCheckoutChest(item);
                _gunPartsShoppingList.Add(gunPart);
                
                cardWalletText.DOColor(Color.green, 0.1f).SetLoops(4, LoopType.Yoyo)
                    .OnComplete(() => cardWalletText.color = Color.white);
            }
            else
            {
                Debug.Log("Not enough money to buy item!!");
                
                cardWalletText.DOColor(Color.red, 0.1f).SetLoops(7, LoopType.Yoyo)
                    .OnComplete(() => cardWalletText.color = Color.white);
            }

            // Return hand to original pos
            barcodeXray.SetActive(false);
            rightHand.transform.DOLocalRotate(originalRotate, 0.5f);
            rightHand.transform.DOLocalMove(originalPos, 0.5f);

            // Return to minigame running state
            GameManager.Instance.ChangeState(GameState.MinigameRunning);
            CameraManager.Instance.SetCamera(CameraType.Minigame);
            AnimationManager.Instance.SetAnimationBool("Hand-R", "default", true);
            AnimationManager.Instance.SetAnimationBool("Hand-L", "default", true);
        }

        #endregion
    }
}