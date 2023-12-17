using System.Collections.Generic;
using DG.Tweening;
using Game.Projectiles;
using NaughtyAttributes;
using UnityEngine;

namespace Game.Collectables
{
    public class FractureMoney : BaseCollectable
    {
        [Header("Settings")]
        [SerializeField] private float objectStrength;
        [SerializeField] private Vector3 minJumpPos;
        [SerializeField] private Vector3 maxJumpPos;
        [SerializeField, ReadOnly] private float strengthPerPiece;
        [SerializeField, ReadOnly] private float strengthPerCollectable;

        [Space] [Header("Components")]
        [SerializeField] private GameObject singlePiece;
        [SerializeField] private GameObject[] fracturePieces;
        [SerializeField] private List<Money> moneyCollectables;

        private Queue<Rigidbody> _fracturePieceRigidbodies = new();
        private bool _isFractured;

        #region UNITY EVENTS

        private void Start()
        {
            Init();
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Bullet bullet))
            {
                if (objectStrength <= 0) return;

                VFXSpawner.Instance.PlayVFX("BulletHit", bullet.transform.position);
                bullet.Kill();
                CheckIfFractured();

                // Reduce object strength according to bullet power and process explode fracture pieces
                objectStrength -= bullet.Power;
                var breakPieceCount = Mathf.CeilToInt(bullet.Power / strengthPerPiece);

                BreakBottle(breakPieceCount, bullet.transform.position);
                SetCollectables();
            }
        }

        #endregion

        #region PRIVATE METHODS

        private void Init()
        {
            // Define strength per piece and collectable to process reward
            strengthPerPiece = objectStrength / fracturePieces.Length;
            strengthPerCollectable = objectStrength / moneyCollectables.Count;
            singlePiece.SetActive(true);

            // Init queue with fracture pieces
            foreach (var fracturePiece in fracturePieces)
            {
                _fracturePieceRigidbodies.Enqueue(fracturePiece.GetComponent<Rigidbody>());
                fracturePiece.SetActive(false);
            }

            // Init collectables
            foreach (var money in moneyCollectables)
                money.SetState(false);
        }

        private void BreakBottle(int breakCount, Vector3 impactPos)
        {
            // Explode fracture pieces according to breakCount
            if (breakCount > _fracturePieceRigidbodies.Count)
                breakCount = _fracturePieceRigidbodies.Count;

            for (int i = 0; i < breakCount; i++)
            {
                var fractureObject = _fracturePieceRigidbodies.Dequeue();
                fractureObject.isKinematic = false;
                fractureObject.AddExplosionForce(10f, impactPos, 2f);
            }
        }

        private void SetCollectables()
        {
            // Unlock collectables according to object strength
            var count = moneyCollectables.Count - Mathf.CeilToInt(objectStrength / strengthPerCollectable);

            for (int i = 0; i < count; i++)
            {
                if (moneyCollectables.Count <= 0) break;

                var jumpPos = Helpers.GenerateRandomVector3(minJumpPos, maxJumpPos);
                moneyCollectables[i].transform.DOLocalJump(jumpPos, 1f, 1, 0.3f);
                moneyCollectables[i].SetState(true);
                moneyCollectables.RemoveAt(i);
            }
        }

        private void CheckIfFractured()
        {
            // Activate fracture object on first hit
            if (!_isFractured)
            {
                singlePiece.SetActive(false);

                foreach (var fracturePiece in fracturePieces)
                    fracturePiece.SetActive(true);

                _isFractured = true;
            }
        }

        #endregion
    }
}