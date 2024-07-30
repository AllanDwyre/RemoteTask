using System.Collections;
using _project.scripts.utils;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace _project.scripts.Characters
{
    public class HealthComponent : NetworkBehaviour
    {
        [SerializeField] private HealthSettings _settings;
        public  NetworkVariable<int> Health { get; private set; } = new();
        public  int  MaxHealth{ get; private set; }

        public override void OnNetworkSpawn()
        {
            MaxHealth = _settings.health;
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
            StartCoroutine(HealUntilMaxCorountine(healStep, healingTime));
        }

        private IEnumerator HealUntilMaxCorountine(int healStep, int healingTime)
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
            Debug.Log($"<color=red>Agent is dead</color>");
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void ChangeHealthServerRpc(int value)
        {
            Health.Value += value;
            if (Health.Value <= 0)
            {
                DeathClientRpc();
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void SetHealthServerRpc(int value)
        {
            Health.Value = value;
        }
    }
}