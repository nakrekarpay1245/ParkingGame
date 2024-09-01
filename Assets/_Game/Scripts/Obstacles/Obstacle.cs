using _Game._Abstracts;
using _Game._helpers;
using _Game._Interfaces;
using DG.Tweening;
using System;
using UnityEngine;

namespace _Game.Obstacles
{
    /// <summary>
    /// Represents an obstacle that can deal damage to other damageable objects upon collision.
    /// Inherits from AbstractDamageableDamagerBase to handle both damage and health management functionalities.
    /// </summary>
    public class Obstacle : AbstractDamageableDamagerBase
    {
        [Header("Damage Settings")]
        [Tooltip("The amount of damage this obstacle inflicts upon collision.")]
        [SerializeField, Range(0f, 100f)]
        private int _damageAmount = 1;

        [Header("Parking Obstacle Params")]
        [SerializeField]
        private Material HitMaterial;
        [SerializeField]
        private Material NormalMaterial;
        [SerializeField]
        private float _hitMaterialChangeInterval = 0.1f;
        [SerializeField]
        private float _damageableDestroyDelay = 1f;

        private Renderer _objectRenderer;

        public float _disposeTime = 0.5f;
        public float _initTime = 0.5f;

        void Awake()
        {
            _objectRenderer = GetComponentInChildren<Renderer>();

            NormalMaterial = _objectRenderer.material;
        }

        public override void TakeDamage(int damageAmount)
        {
            base.TakeDamage(damageAmount);
            ApplyHitEffect();

        }

        void ApplyHitEffect()
        {
            _objectRenderer.material = HitMaterial;

            int time = Convert.ToInt32(_damageableDestroyDelay / (_hitMaterialChangeInterval * 3));

            Sequence hitSequence = DOTween.Sequence();
            for (int i = 0; i < time; i++)
            {
                hitSequence.AppendInterval(_hitMaterialChangeInterval);
                hitSequence.AppendCallback(() => _objectRenderer.material = NormalMaterial);
                hitSequence.AppendInterval(_hitMaterialChangeInterval);
                hitSequence.AppendCallback(() => _objectRenderer.material = HitMaterial);
            }
            hitSequence.AppendInterval(_hitMaterialChangeInterval);
            hitSequence.AppendCallback(() =>
            {
                Dispose();
            });
            hitSequence.Play();
        }

        public void Dispose()
        {
            transform.DOScale(Vector3.zero, _disposeTime).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }

        public void Init()
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, _initTime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Check if the collided object implements IDamageable
            if (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                // Deal damage to the damageable object
                DealDamage(damageable, _damageAmount);
                TakeDamage(_health);
            }
        }
    }
}