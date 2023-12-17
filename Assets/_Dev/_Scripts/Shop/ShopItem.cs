using TMPro;
using UnityEngine;

namespace Game.Shop
{
    public abstract class ShopItem : MonoBehaviour
    {
        [SerializeField] protected TextMeshPro itemCostText;

        protected bool _isTriggered;
        
        protected abstract void OnTriggerEnter(Collider other);
    }
}