using System;
using _project.scripts.Characters;
using _project.scripts.Core.HealthSystem;
using _project.scripts.Utils;
using UnityEngine;

namespace _project.scripts.Core.CombatSystem.Projectiles
{
    public abstract class Projectile: MonoBehaviour
    {
       // private const int SuppressionRadius = 3;

        private float _shotHeight, _shotSpeed, _shotAngle;
        
        /// <summary>
        /// The current time the projectile is in the air 
        /// </summary>
        private float _shotFlightTime;
        private Weapon _launcher;
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
                OnImpact();
            }

            Vector2 currentPosition = transform.position;
            Vector2 direction = transform.up;
            float distance = _shotSpeed * Time.fixedDeltaTime;

            RaycastHit2D hit = Physics2D.Raycast(currentPosition, direction, distance, collisionMask);
            if (hit.collider is not null && !hit.collider.isTrigger)
            {
                Debug.DrawLine(currentPosition,currentPosition+ direction * distance, Color.magenta,.1f);
                OnCollisionCheck(hit.collider);
            }
            transform.position = currentPosition + direction * distance;
        }
        
        /// <summary>
        /// When the projectile hit something, we check if it can be ignored, or we call the impact function
        /// </summary>
        /// <param name="other"></param>
        private void OnCollisionCheck(Collider2D other)
        {
            // if (other.TryGetComponent(out IObstacle obstacle)) return;
            //
            // Height obstacleHeight = obstacle.GetHeight();
            //
            // if(Height > obstacleHeight.Max || Height < obstacleHeight.Min) return;

            OnImpact(other.transform);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.layer.CompareLayer(collisionMask))
            {
                OnImpact(other.transform);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out CharacterBase character))
            {
                ApplySuppression(character);
            }
        }
       
        /// <summary>
        /// Initialize the bullet trajectory and flight, the shotSpeed is in radiant.
        /// </summary>
        public virtual void Launch(Vector3 origin, Quaternion rotation, float shotSpeed, Weapon launcher)
        {
            _shotHeight = origin.z;
            _shotSpeed = shotSpeed;
            _shotAngle = rotation.eulerAngles.x * Mathf.Deg2Rad;
            _launcher = launcher;
            
            transform.rotation = Quaternion.Euler(0,0, rotation.eulerAngles.z - 90);
        }
        
        protected virtual void OnImpact(Transform impactedObject = null)
        {
            
            
            // sound effect; vfx, apply damage, destroy bullet;
            // TODO calculate the health based on armor and armor penetration;  
            if (impactedObject == null)
            {
                // TODO : vfx on ground, to feedback
                Destroy(gameObject);
                return;
            }
            //if is a character
            if (impactedObject.TryGetComponent(out HealthComponent healthComp))
            {
                healthComp.SetDamage(ProjectileSo.damageAmount);
            }
            
            Destroy(gameObject);
        }
        private void ApplySuppression(CharacterBase character)
        {
            if (_launcher == null || character.Faction == _launcher.Faction) return;
            
            var suppressionAmount = ProjectileSo.damageAmount;
            // TODO : An armor can decrease the suppression inflict by a projectile
            // var penetrationAmount = projectileSetting.penetrationAmount;
            // var armorMod = penetrationAmount <= 0 ? 0 : 1 - MathF.Clamp(pawn.GetStatValue(CE_StatDefOf.AverageSharpArmor) * 0.5f / penetrationAmount, 0, 1);
            // suppressionAmount *= armorMod;
            // character.AddSuppression(suppressionAmount);
        }
        
        private float CalculateCurrentHeight()
        {
            return (float)Math.Round(_shotHeight + _shotSpeed * Mathf.Sin(_shotAngle) * _shotFlightTime - (Helper.GravityConst * _shotFlightTime * _shotFlightTime)/2f , 3);
        }
    }
}