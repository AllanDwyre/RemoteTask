using System.Collections.Generic;
using System.Linq;
using _project.scripts.Characters;
using _project.scripts.Core.CombatSystem.MagazineSystem;
using _project.scripts.Core.CombatSystem.Projectiles;
using _project.scripts.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _project.scripts.Core.CombatSystem.Weapon
{
    /// <summary>
    /// Accuracy = (healthCapability × Weapon Accuracy (at range) × Weather × Smoke × Cover) + Combat in Darkness
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class RangedWeapon : MonoBehaviour
    {

        [SerializeField] private Transform bulletPrefab;
        public CharacterBase Character { get; private set;}
        public int Faction { get; private set; }

        #region TimersVariables
        private Timer _reloadTimer;
        private Timer _burstTimer;
        private Timer _cooldownTimer;
        private Timer _warmupTimer;
        #endregion
        
        private bool HaveTarget => _target != null;
        
        private SpriteRenderer _renderer;
        private Transform _target;
        private CombatComponent _shooter;

        private WeaponSettings _settings;
        private Magazine _currentMagazine;
        private readonly List<Magazine> _magazines = new List<Magazine>();
        
        private bool _canShoot;
        private bool _hasWarmUp;
        private int _currentBurstCount;
        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        #region Initialize
        public void Initialize(WeaponSettings weapon, List<MagazineParams> allMags)
        {
            _settings = weapon;
            GetComponent<SpriteRenderer>().sprite = _settings.sprite;
            InitMagazine(allMags);
            SetTimers();
        }

        private void InitMagazine(List<MagazineParams> allMags)
        {
            _magazines.Clear();
            foreach (var mag in allMags)
            {
                for (int i = 0; i < mag.numberOfMagazine; i++)
                {
                    _magazines.Add(Magazine.Create(mag));
                }
            }
        }

        private void SetTimers()
        {
            _warmupTimer = new Timer(_settings.warmUp, OnWarmUpCompleted);
            _reloadTimer = new Timer(_settings.reloadTime, OnReloadCompleted);
            _burstTimer = new Timer(_settings.burst, OnShooting);
            _cooldownTimer = new Timer(_settings.cooldown, OnCooldownCompleted);
        }



        #endregion
        private void Update()
        {
            
            if (!_canShoot || !HaveTarget) return;
            
            RotateWeapon();
            
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

        public void Shoot(Transform target, CombatComponent combatComponent, bool canShoot = true)
        {
            _canShoot = canShoot;
            var lastTarget = _target;
            _target = target;
            
            if (_target is null || lastTarget == _target) return;
            
            _shooter = combatComponent;
            Character = _shooter.Character;
            Faction = Character.Faction;
            
            StopAllTimer();
            _hasWarmUp = false;
            if (_currentMagazine is null || !_currentMagazine.CanFire())
            {
                _reloadTimer.Reset();
                return;
            }
            _warmupTimer.Reset();
                
        }
        public void StartShooting() => _canShoot = true;
        public void StopShooting() => _canShoot = false;

        public void Reload()
        {
            if (_currentMagazine is null)
            {
                _currentMagazine = _magazines.FirstOrDefault();
                return;
            }
            
            var thisMag = _currentMagazine;
            if (thisMag.Remaining <= 0)
            {
                _magazines.Remove(thisMag);
            }
            
            if (_magazines.Count == 0)
            {
                _shooter.Swap();
            }
            
            // get the same type of ammo in priority or get unknown magazine
            _currentMagazine = _magazines.FirstOrDefault(m => m.FilledAmmo == thisMag.FilledAmmo) ?? _magazines.FirstOrDefault();
            if (_hasWarmUp)
            {
                _reloadTimer.Reset();
                return;
            }
            _warmupTimer.Reset();
        }
        
        
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
            _hasWarmUp = true;
            OnShooting();
        }
        private void OnShooting()
        {
            if (!_canShoot) return;
            
            if (!_currentMagazine.CanFire())
            {
                _reloadTimer.Reset();
                return;
            }
            
            if (_currentBurstCount >= _settings.burstCount)
            {
                _burstTimer.Stop();
                _cooldownTimer.Reset();
                return;
            }
            
            // firing sound effect && vfx
            OnBulletInstantiation();
            
            // handle single bullet burst
            if (_settings.burstCount == 1)
            {
                _cooldownTimer.Reset();
            }
            else
            {
                _currentBurstCount++;
                _burstTimer.Reset();
            }
        }

        private void OnReloadCompleted()
        {
            Reload();
            OnShooting();
            // TODO : Stop the reload sound effect (we need to start is on the reloadTimer start
        }

        private void OnCooldownCompleted()
        {
            _currentBurstCount = 0;
            OnShooting();
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
           
           bullet.GetComponent<Bullet>().Launch(transform.position, bulletRotation, _currentMagazine.Speed,this, _currentMagazine.FilledAmmo);
        }

        #region Helpers

        private Vector3 MuzzlePoint()
        {
            var muzzlePoint = _settings.muzzlePoint;
            muzzlePoint.x *= _renderer.flipX ? -1f : 1f;
            return transform.position + transform.rotation * muzzlePoint; 
        }

        private float GetDerivation()
        {
            // TODO : random need to be friendly with network (deterministic random)
            const float agentSwayModifier = 1f; // TODO will be adjust on health
            float maxDerivation = (_settings.sway * agentSwayModifier);
            float randomSpread = Random.Range(-_settings.spread, _settings.spread);
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