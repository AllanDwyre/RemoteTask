using System.Collections.Generic;
using _project.scripts.Characters;
using _project.scripts.Core.CombatSystem.Detections;
using _project.scripts.Core.CombatSystem.MagazineSystem;
using _project.scripts.Core.CombatSystem.Projectiles;
using _project.scripts.Utils;
using Unity.Netcode;
using UnityEngine;

namespace _project.scripts.Core.CombatSystem
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class CombatComponent : NetworkBehaviour
    {
        #region Variables
        public CharacterBase Character { get; private set;}
        [SerializeField] private Optional<FieldOfView> fieldOfView;
        
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private float detectionRange;
        [SerializeField] private bool isAI;
        [SerializeField] private float visibilityChecksInterval = .5f;

        [SerializeField] private Weapon primaryWeapon;
        [SerializeField] private Weapon secondaryWeapon;
        [SerializeField] private List<MagazineParams> primaryParams;
        [SerializeField] private List<MagazineParams> secondaryParams;
        
        [SerializeField] private Optional<WeaponSettings> primaryData;
        [SerializeField] private Optional<WeaponSettings> secondaryData;
        
        [SerializeField] private List<ProjectileSetting> throwable;


        private bool _isDisable;
        private Detection _detection;
        private Timer _detectionTimer;
        
        private readonly List<Magazine> _primaryMagazines = new List<Magazine>();
        private readonly List<Magazine> _secondaryMagazines = new List<Magazine>();
        
        private Weapon CurrentWeapon => HasPrimaryInHand ? primaryWeapon : secondaryWeapon;
        private Optional<WeaponSettings> CurrentWeaponData => HasPrimaryInHand ? primaryData : secondaryData;
        private bool _hasPrimaryInHand;
        private bool HasPrimaryInHand
        {
            get => _hasPrimaryInHand;
            set
            {
                if(primaryData.Enable) primaryWeapon.gameObject.SetActive(value);
                if(secondaryData.Enable) secondaryWeapon.gameObject.SetActive(!value);
                _hasPrimaryInHand = value;
            }
        }

        private Transform _target;
        private float _targetHeight;
        #endregion
        
        #region Initialization

        /// <summary>
        /// If this script is owned by an AI, and it's not owned by a client
        /// </summary>
        private void Awake()
        {
            Character = GetComponent<CharacterBase>();
            
            HasPrimaryInHand = primaryData.Enable;
            if(primaryData.Enable) primaryWeapon.SetSetting(primaryData.Value);
            if(secondaryData.Enable) secondaryWeapon.SetSetting(secondaryData.Value);
            InitMagazines();
            
            _detection = new Detection(GetComponent<CircleCollider2D>(), detectionRange, obstacleLayer, isAI);
            _detectionTimer = new Timer(visibilityChecksInterval);

            if (!isAI)
            {
                fieldOfView.Value.SetMaxDistance(detectionRange);
            }
            
        }

        public override void OnNetworkSpawn()
        {
            if(!IsOwner)
            {
                if(fieldOfView.Enable) Destroy(fieldOfView.Value.gameObject);
            }
        }

        private void InitMagazines()
        {
            foreach (var param in primaryParams)
            {
                _primaryMagazines.Add(Magazine.Create(param));
            }
            
            foreach (var param in secondaryParams)
            {
                _secondaryMagazines.Add(Magazine.Create(param));
            }
        }
        // private void AddMagazines(bool onPrimary, MagazineParams params)
        // {
        //     onPrimary ? 
        // }
        #endregion
        private void Update()
        {
            if (_isDisable) return;
            PeriodicVisibilityCheck();
        }

        #region Public
        public void EnableCombat()
        {
            _isDisable = true;
            // will be nice for resurrection
        }
        public void DisableCombat()
        {
            _isDisable = false;
            CurrentWeapon.ResetTarget();
            if (fieldOfView.Enable)
            {
                fieldOfView.Value.gameObject.SetActive(false);
            }
        }
        public Magazine GetNextMagazine(bool isSecondary)
        {
            if (_isDisable) return null;

            if (!isSecondary && _primaryMagazines.Count > 0)
            {
                var magazine = _primaryMagazines[0];
                _primaryMagazines.RemoveAt(0);
                return magazine;
            }
            
            if (isSecondary && _secondaryMagazines.Count > 0)
            {
                var magazine = _secondaryMagazines[0];
                _secondaryMagazines.RemoveAt(0);
                return magazine;
            }
            return null;
        }
        public void OnWeaponSwap()
        {
            if (_isDisable) return;
            
            CurrentWeapon.ResetTarget();
            var magazines = !HasPrimaryInHand ? _primaryMagazines : _secondaryMagazines;
            if (magazines.Count == 0) return; // TODO : bare hands to combat
            HasPrimaryInHand = !HasPrimaryInHand;
            
            if (CurrentWeaponData.Enable)
            {
                CurrentWeapon.SetTarget(_target, _targetHeight, this);
            }
        }
        public void OnPrimaryWeaponChange(WeaponSettings newWeapon)
        {
            if (_isDisable) return;

            primaryData = new Optional<WeaponSettings>(newWeapon);
            primaryWeapon.SetSetting(newWeapon);
            primaryWeapon.ResetTarget();
        }
        public void OnSecondaryWeaponChange(WeaponSettings newWeapon)
        {
            if (_isDisable) return;

            if (!newWeapon.isSecondaryWeapon) return; // TODO: The command to do this Change will need a validator that check this
            secondaryData = new Optional<WeaponSettings>(newWeapon);
            secondaryWeapon.SetSetting(newWeapon);
            secondaryWeapon.ResetTarget();
        }
        #endregion
       
        #region OnDetection
        private void PeriodicVisibilityCheck()
        {
            _detectionTimer.Tick(Time.deltaTime);
            
            if (_detectionTimer.IsTicking) return;
            _detection.PerformVisibilityChecks(transform.position, OnDetection);
            _detectionTimer.Reset();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            _detection.AddDetectedObject(other);
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            _detection.RemoveDetectedObject(other);
        }
        private void OnDetection(Transform detectedTransform, float centerOfFire)
        {
            SetTarget(detectedTransform, centerOfFire);
            // The enemy is detected the player or IA need to make a decision (determine the target) based on the hashset of the detection
        }
        #endregion

        private void SetTarget(Transform target, float centerOfFire)
        {
            if (!CurrentWeaponData.Enable) return;
            _target = target;
            _targetHeight = centerOfFire;
            CurrentWeapon.SetTarget(target, centerOfFire, this);
        }
    }
}
