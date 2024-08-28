using _Game._Abstracts;
using _Game._Interfaces;
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
        private float _damageAmount = 1f;

        private void OnCollisionEnter(Collision collision)
        {
            // Check if the collided object implements IDamageable
            if (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                // Deal damage to the damageable object
                DealDamage(damageable, _damageAmount);
            }
        }

        //#if UNITY_EDITOR
        //        /// <summary>
        //        /// Draws a Gizmo in the Scene view to visualize the obstacle's damage radius.
        //        /// </summary>
        //        private void OnDrawGizmos()
        //        {
        //            // Draw a wireframe sphere to represent the damage radius
        //            Gizmos.color = Color.red;
        //            Mesh mesh = GetComponent<MeshFilter>().mesh;
        //            Gizmos.DrawWireMesh(mesh, transform.position, transform.rotation, transform.localScale);
        //        }
        //#endif
    }
}