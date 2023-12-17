using DG.Tweening;
using UnityEngine;

public class CheckoutChest : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform upperLid;
    [SerializeField] private ParticleSystem shiningVFX;

    public void ProcessPutItemInsideAnimation()
    {
        upperLid.DOComplete();
        upperLid.DOLocalRotate(new Vector3(-165f, 0f, 0f), 0.2f)
            .SetLoops(2, LoopType.Yoyo);
    }
    
    public void ProcessGetItemInsideAnimation()
    {
        upperLid.DOComplete();
        upperLid.DOLocalRotate(new Vector3(-165f, 0f, 0f), 0.2f);
        shiningVFX.Play();
    }
}