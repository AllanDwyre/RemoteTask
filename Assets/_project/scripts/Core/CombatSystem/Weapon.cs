using System.Collections.Generic;
using _project.scripts.Characters;
using _project.scripts.Core.CombatSystem.MagazineSystem;
using _project.scripts.Core.CombatSystem.Projectiles;
using _project.scripts.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _project.scripts.Core.CombatSystem
{
    /// <summary>
    /// Accuracy = (healthCapability × Weapon Accuracy (at range) × Weather × Smoke × Cover) + Combat in Darkness
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class Weapon : MonoBehaviour
    {
        private WeaponSettings Settings { get; set; }
        /// <summary>
        /// A bullet prefab is just a placeholder of the bullet script
        /// </summary>
        [SerializeField] private Transform bulletPrefab;

        #region TimersVariables
        private Timer _reloadTimer;
        private Timer _burstTimer;
        private Timer _cooldownTimer;
        private Timer _warmupTimer;
        #endregion
        
        private SpriteRenderer _renderer;
        private Transform _target;
        private CombatComponent _shooter;
        public CharacterBase Character { get; private set;}
        private bool _isTargeting;
        private int _currentBurstCount;
        private float _heightToAim;

        // Technically I don't need it, because the SetTarget will take as parameter an origin of the weapon position that will be directly set by the movement handler class
        private readonly Dictionary<EPosture, float> _heightBasedOnPosture = new Dictionary<EPosture, float>
        {
            { EPosture.Standing, 1f },
            { EPosture.Crouch, .5f },
            { EPosture.Prone, .15f },
        };
        public int Faction { get; private set; }

        public Magazine CurrentMagazine { get; private set; }

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (_target != null)
            {
                RotateWeapon();
            }
            
            if (!_isTargeting) return;
            
            _reloadTimer.Tick(Time.deltaTime);
            _burstTimer.Tick(Time.deltaTime);
            _cooldownTimer.Tick(Time.deltaTime);
            _warmupTimer.Tick(Time.deltaTime);

            // TODO : display all timer thought events (not the cooldown and burst timer) 
            if (_reloadTimer.IsTicking)
            {
                // event to display _reloadTimer.RemainingTime
            }
        }

        

        #region Publics
        public void SetSetting(WeaponSettings newWeapon)
        {
            Settings = newWeapon;
            GetComponent<SpriteRenderer>().sprite = Settings.sprite;
            
            _warmupTimer = new Timer(Settings.warmUp,OnWarmUpCompleted);
            _reloadTimer = new Timer(Settings.reloadTime, OnReloadCompleted);
            _burstTimer = new Timer(Settings.burst, OnBurstCompleted);
            _cooldownTimer = new Timer(Settings.cooldown, OnCooldownCompleted);
        }
        public void SetTarget(Transform target,float centerOfFire, CombatComponent combatComponent)
        {
            if (target == _target) return;
            
            CurrentMagazine ??= combatComponent.GetNextMagazine(Settings.isSecondaryWeapon);
            Faction = combatComponent.Character.Faction;
            _shooter = combatComponent;
            Character = combatComponent.Character;
            _target = target;
            _isTargeting = true;
            _heightToAim = centerOfFire;
            StopAllTimer();
            _warmupTimer.Start();
        }
        public void ResetTarget()
        {
            _isTargeting = false;
            transform.rotation = Quaternion.identity;
            StopAllTimer();
        }
        #endregion

        #region TimerLogic

        private void StopAllTimer()
        {
            _reloadTimer.Stop();
            _burstTimer.Stop();
            _cooldownTimer.Stop();
            _warmupTimer.Stop();
        }

        private void OnWarmUpCompleted()
        {
            OnBurstCompleted(); // first bullet go
        }
        
        private void OnReloadCompleted()
        {
            OnBurstCompleted();
            // TODO : Stop the reload sound effect (we need to start is on the reloadTimer start
        }
        
        private void OnBurstCompleted()
        {
            if (!CurrentMagazine.CanFire())
            {
                // *click* sound effect
                var newMagazine = _shooter.GetNextMagazine(Settings.isSecondaryWeapon);
                if (newMagazine != null)
                {
                    Debug.Log($"{_shooter.Character.name} Reload !! ");

                    CurrentMagazine = newMagazine;
                    _reloadTimer.Reset();
                }
                else
                {
                    _shooter.OnWeaponSwap();
                }
                return;
            }
            if (_currentBurstCount >= Settings.burstCount)
            {
                _burstTimer.Stop();
                _cooldownTimer.Reset();
                return;
            }
            
            // firing sound effect && vfx
            OnBulletInstantiation();
            if (Settings.burstCount == 1)
            {
                _cooldownTimer.Reset();
            }
            else
            {
                _currentBurstCount++;
                _burstTimer.Reset();
            }
        }

        private void OnCooldownCompleted()
        {
            _currentBurstCount = 0;
            OnBurstCompleted();
        }
        #endregion

        // TODO : VFX, Sound
        private void OnBulletInstantiation()
        {
            float deviationZ = GetDerivation();

            Quaternion derivationRotation = Quaternion.Euler(0, 0, deviationZ);
            Quaternion bulletRotation = derivationRotation  * transform.rotation; //* angleRotation
            
            if (_renderer.flipX)
            {
                bulletRotation *= Quaternion.Euler(0, 0, 180);
            }
            
            // TODO : bullet pool;
           Transform bullet = Instantiate(bulletPrefab, MuzzlePoint(), Quaternion.identity);
           
           Vector3 origin = transform.position;
           origin.z = 0.5f; // TODO : base on MovementPosture
           
           bullet.GetComponent<Bullet>().Launch(origin, bulletRotation, CurrentMagazine.Speed,this, CurrentMagazine.FilledAmmo);
        }

        #region Helpers

        private Vector3 MuzzlePoint()
        {
            var muzzlePoint = Settings.muzzlePoint;
            muzzlePoint.x *= _renderer.flipX ? -1f : 1f;
            return transform.position + transform.rotation * muzzlePoint; 
        }

        private float GetDerivation()
        {
            // TODO : random need to be friendly with network (deterministic random)
            const float agentSwayModifier = 1f; // TODO will be adjust on health
            float maxDerivation = (Settings.sway * agentSwayModifier);
            float randomSpread = Random.Range(-Settings.spread, Settings.spread);
            return Random.Range(-maxDerivation, maxDerivation) + randomSpread;
        }
        #endregion

        #region Visual

        private void RotateWeapon()
        {
            Vector3 direction = (_target.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            _renderer.flipX = direction.x < 0;
            angle = direction.x < 0 ? angle - 180f : angle; 
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        #endregion
        
    }
}