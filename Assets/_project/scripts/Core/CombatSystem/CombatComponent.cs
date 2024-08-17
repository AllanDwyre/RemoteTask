using System.Collections.Generic;
using System.Linq;
using _project.scripts.Characters;
using _project.scripts.Core.CombatSystem.Detections;
using _project.scripts.Core.CombatSystem.MagazineSystem;
using _project.scripts.Core.CombatSystem.Projectiles;
using _project.scripts.Core.CombatSystem.Weapon;
using _project.scripts.Core.HealthSystem;
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

        [Space]
        [SerializeField] private RangedWeapon primaryRangedWeapon;
        [SerializeField] private Optional<WeaponSettings> primaryData;
        [SerializeField] private List<MagazineParams> primaryParams;
        
        [Space]
        [SerializeField] private RangedWeapon secondaryRangedWeapon;
        [SerializeField] private Optional<WeaponSettings> secondaryData;
        [SerializeField] private List<MagazineParams> secondaryParams;

        [Space]
        [SerializeField] private List<ProjectileSetting> throwable;

    
        private bool _isEnable;
        // detection
        private Detection _detection;
        private Timer _detectionTimer;

        
        // weapon
        private RangedWeapon CurrentRangedWeapon => HasPrimaryInHand ? primaryRangedWeapon : secondaryRangedWeapon;
        private bool _hasPrimaryInHand;
        private bool HasPrimaryInHand
        {
            get => _hasPrimaryInHand;
            set
            {
                if(primaryData.Enable) primaryRangedWeapon.gameObject.SetActive(value);
                if(secondaryData.Enable) secondaryRangedWeapon.gameObject.SetActive(!value);
                _hasPrimaryInHand = value;
            }
        }
        #endregion
        
        #region Initialization

        /// <summary>
        /// If this script is owned by an AI, and it's not owned by a client
        /// </summary>
        private void Awake()
        {
            Character = GetComponent<CharacterBase>();
            
            _detection = new Detection(GetComponent<CircleCollider2D>(), detectionRange, obstacleLayer, isAI);
            _detectionTimer = new Timer(visibilityChecksInterval);
        }

        private void Start()
        {
            _isEnable = true;
            
            HasPrimaryInHand = primaryData.Enable;
            if(primaryData.Enable) primaryRangedWeapon.Initialize(primaryData.Value, primaryParams);
            if(secondaryData.Enable) secondaryRangedWeapon.Initialize(secondaryData.Value, secondaryParams);
            
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
        #endregion
        private void Update()
        {
            if (!_isEnable) return;
            PeriodicVisibilityCheck();
        }

        #region Public
        public void EnableCombat()
        {
            _isEnable = transform;
            primaryRangedWeapon.StartShooting();
            secondaryRangedWeapon.StartShooting();
        }
        public void DisableCombat()
        {
            _isEnable = false;
            primaryRangedWeapon.StopShooting();
            secondaryRangedWeapon.StopShooting();
            if (fieldOfView.Enable)
            {
                fieldOfView.Value.gameObject.SetActive(false);
            }
        }

        public void Swap()
        {
            HasPrimaryInHand = !HasPrimaryInHand;
        }
        #endregion
       
        #region OnDetection
        private void PeriodicVisibilityCheck()
        {
            _detectionTimer.Tick(Time.deltaTime);
            
            if (_detectionTimer.IsTicking) return;
            
            // here the call back need to be handled by the state machine of the IA or the player itself.
            // this means on detection an ia will need more time to shoot the player
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
        private void OnDetection(List<Transform> detectedTransforms)
        {
            var shortestEnemy = detectedTransforms
                .OrderBy(t => Vector3.Distance(t.position, transform.position))
                .FirstOrDefault();
            CurrentRangedWeapon.Shoot(shortestEnemy, this, _isEnable && shortestEnemy is not null);
        }
        #endregion


    }
}
