using System;
using System.Collections.Generic;
using _project.scripts.Characters;
using _project.scripts.Core.HealthSystem;
using UnityEngine;

namespace _project.scripts.Core.CombatSystem.Detections
{
    public class Detection
    {
        private const int MaxCollider = 20;
        private readonly LayerMask _mask;
        private readonly bool _isAi;
        private readonly float _radius;
        public HashSet<Collider2D> DetectedObjects { get; private set; }= new HashSet<Collider2D>();

        public Detection(CircleCollider2D detectionCollider, float radius, LayerMask mask, bool isAi, bool simplifiedDetection = true)
        {
            var collider = detectionCollider;
            collider.isTrigger = true;
            _mask = mask;
            _isAi = isAi;
            collider.radius = radius;
            _radius = radius;
        }

        public void AddDetectedObject(Collider2D other)
        {
            if(other.isTrigger || DetectedObjects.Contains(other) || !IsValidDetection(other)) return;
            
            DetectedObjects.Add(other);
        }

        public void RemoveDetectedObject(Collider2D other)
        {
            DetectedObjects.Remove(other);
        }

        public void PerformVisibilityChecks(Vector3 from, Action<List<Transform>> callback)
        {
            DetectedObjects.RemoveWhere(
                x =>
                        !x.TryGetComponent(out HealthComponent healthComponent)
                     || healthComponent.HealthStatus is EHealthStatus.Dead
                );
            List<Transform> visible = new List<Transform>();
            foreach (var detected in DetectedObjects)
            {
                if (OnSimplifiedDetection(detected.transform, from, _mask))
                {
                    visible.Add(detected.transform);
                }
            }
            callback(visible);
        }

        private bool OnSimplifiedDetection(Transform target, Vector3 from, LayerMask mask)
        {
            var dir = (target.position - from).normalized;
            float distance = Vector2.Distance(target.position, from);

            var hit = Physics2D.Raycast(from, dir, distance, layerMask: mask);
            Debug.DrawLine(from,from + dir * distance, hit.collider == null ? Color.green : Color.red, 0.4f);
            return hit.collider == null && distance <= _radius;
        }
        
        private bool IsValidDetection(Collider2D detected)
        {
            //detect only enemy or player
            if (!detected.CompareTag("Enemy") && !detected.CompareTag("Player")) return false;
            
            // is dead or have not healthComponent
            if (!detected.TryGetComponent(out HealthComponent healthComponent) || healthComponent.HealthStatus is EHealthStatus.Dead)
            {
                return false;
            }
            
            //is IA enemy
            if (_isAi && detected.CompareTag("Player")) return true;

            //is Player enemy
            bool isEnemyPlayer = detected.TryGetComponent(out Agent character) && !character.IsOwner;
            return !_isAi && (isEnemyPlayer || detected.CompareTag("Enemy"));
        }
    }
}
