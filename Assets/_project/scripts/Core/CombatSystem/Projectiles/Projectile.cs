using System;
using _project.scripts.Core.CombatSystem.Weapon;
using _project.scripts.Core.HealthSystem;
using _project.scripts.Utils;
using UnityEngine;

namespace _project.scripts.Core.CombatSystem.Projectiles
{
    public abstract class Projectile: MonoBehaviour
    {
        private float _shotHeight, _shotSpeed, _shotAngle;
        
        /// <summary>
        /// The current time the projectile is in the air 
        /// </summary>
        private float _shotFlightTime;
        private RangedWeapon _launcher;
        protected abstract ProjectileSetting ProjectileSo { get; set; }
        [SerializeField] private LayerMask collisionMask;

        protected SpriteRenderer Renderer;
        private float Height => CalculateCurrentHeight();

        private void Awake()
        {
            Renderer = GetComponent<SpriteRenderer>();
        }

        private void FixedUpdate()
        {
            _shotFlightTime += Time.deltaTime;
            if (Height <= 0f)
            {
               // OnImpact();
            }
            Vector2 currentPosition = transform.position;
            Vector2 direction = transform.up;
            float distance = _shotSpeed * Time.fixedDeltaTime;

            RaycastHit2D hit = Physics2D.Raycast(currentPosition, direction, distance, collisionMask);
            if (hit.collider is not null && !hit.collider.isTrigger)
            {
                ValidateHit(hit.collider);
            }
            transform.position = currentPosition + direction * distance;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.isTrigger) return;
            ValidateHit(other);
        }

        private void ValidateHit(Collider2D hit)
        {
            if (hit.gameObject == _launcher.Character.gameObject)
                return;
            OnImpact(hit.transform);
        }
        
        /// <summary>
        /// Initialize the bullet trajectory and flight, the shotSpeed is in radiant.
        /// </summary>
        protected virtual void Launch(Vector3 origin, Quaternion rotation, float shotSpeed, RangedWeapon launcher)
        {
            _shotHeight = 0.5f;
            _shotSpeed = shotSpeed;
            _shotAngle = rotation.eulerAngles.x * Mathf.Deg2Rad;
            _launcher = launcher;
            
            transform.rotation = Quaternion.Euler(0,0, rotation.eulerAngles.z - 90);
        }
        
        protected virtual void OnImpact(Transform impactedObject = null)
        {
            if (impactedObject == null)
            {
                // TODO : vfx on ground, to feedback
                Destroy(gameObject);
                return;
            }
            if (impactedObject.TryGetComponent(out HealthComponent healthComp))
            {
                healthComp.SetDamage(ProjectileSo.damageAmount);
            }
            
            Destroy(gameObject);
        }
       
        private float CalculateCurrentHeight()
        {
            return (float)Math.Round(_shotHeight + _shotSpeed * Mathf.Sin(_shotAngle) * _shotFlightTime - (Helper.GravityConst * _shotFlightTime * _shotFlightTime)/2f , 3);
        }
    }
}