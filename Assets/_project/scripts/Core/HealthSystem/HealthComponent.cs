using System;
using System.Collections;
using _project.scripts.Utils;
using Unity.Netcode;
using UnityEngine;

namespace _project.scripts.Core.HealthSystem
{
    public class HealthComponent : NetworkBehaviour
    {
        [SerializeField] private HealthSettings _settings;
        public  NetworkVariable<int> Health { get; private set; } = new();
        public  int  MaxHealth{ get; private set; }
        
        public EHealthStatus HealthStatus { get; private set; }

        public event Action OnDeath;

        public override void OnNetworkSpawn()
        {
            // please remove the error
            MaxHealth = _settings.health;
            HealthStatus = EHealthStatus.Alive;
            if (!IsServer) return;
            Health.Value = _settings.health;
        }

        public void InstantKill()
        {
            SetHealthServerRpc(-1);
            DeathClientRpc();
        }
        
        public void SetDamage(int damageValue)
        {
            if (HealthStatus == EHealthStatus.Dead) return;
            ChangeHealthServerRpc(-damageValue);
        }

        public void Heal(int healAmount)
        {
            if (Health.Value <= 0) return;
            ChangeHealthServerRpc(healAmount);
        }
        
        public void HealUntilMax(int healStep, int healingTime)
        {
            if (Health.Value <= 0) return;
            StartCoroutine(HealUntilMaxCoroutine(healStep, healingTime));
        }

        private IEnumerator HealUntilMaxCoroutine(int healStep, int healingTime)
        {
            while (Health.Value != MaxHealth)
            {
                int healAmount = Mathf.Min(MaxHealth, Health.Value + healStep);
                // TODO : add a progress bar indicator helperSystem using also a coroutine
                yield return Helper.WaitForSeconds(healingTime);
                ChangeHealthServerRpc(healAmount);
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void DeathServerRpc()
        {
            DeathClientRpc();
        }
        
        [ClientRpc]
        private void DeathClientRpc()
        {
            HealthStatus = EHealthStatus.Dead;
            OnDeath?.Invoke();
            Debug.Log($"<color=red>{name} is dead</color>");
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void ChangeHealthServerRpc(int value)
        {
            Health.Value += value;
            if (Health.Value <= 0)
            {
                DeathClientRpc();
            }

            if (value < 0)
            {
               // _damageFlash.StartDamageFlash();
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void SetHealthServerRpc(int value)
        {
            Health.Value = value;
        }
    }
}