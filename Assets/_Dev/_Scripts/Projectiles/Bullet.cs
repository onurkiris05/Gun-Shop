using UnityEngine;

namespace Game.Projectiles
{
    public class Bullet : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float bulletSpeed;

        public float Power => _power;

        private TrailRenderer _trailRenderer;
        private Rigidbody _rigidbody;
        private Vector3 _startPos;
        private float _range;
        private float _power;

        #region UNITY EVENTS

        private void Awake() => _trailRenderer = GetComponent<TrailRenderer>();

        private void OnEnable()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _startPos = transform.position;
            _rigidbody.AddForce(transform.forward * bulletSpeed, ForceMode.VelocityChange);
        }

        private void OnDisable() => _trailRenderer.Clear();

        private void Update()
        {
            if (Vector3.Distance(_startPos, transform.position) > _range)
                Kill();
        }

        #endregion

        #region PUBLIC METHODS

        public void Init(float range, float power)
        {
            _range = range;
            _power = power;
        }

        public void Kill()
        {
            ObjectPooler.Instance.ReleasePooledObject("Bullet", gameObject);
        }

        #endregion
    }
}