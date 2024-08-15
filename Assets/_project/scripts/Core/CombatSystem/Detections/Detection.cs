using System;
using System.Collections.Generic;
using _project.scripts.Characters;
using UnityEngine;

namespace _project.scripts.Core.CombatSystem.Detections
{
    public class Detection
    {
        private const int MaxCollider = 20;
        private readonly LayerMask _mask;
        private readonly bool _isAi;
        private readonly bool _simplifiedDetection;
        private readonly float _radius;
        public HashSet<Collider2D> DetectedObjects { get; private set; }= new HashSet<Collider2D>();

        public Detection(CircleCollider2D detectionCollider, float radius, LayerMask mask, bool isAi, bool simplifiedDetection = true)
        {
            var collider = detectionCollider;
            collider.isTrigger = true;
            _mask = mask;
            _isAi = isAi;
            _simplifiedDetection = simplifiedDetection;
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

        public void PerformVisibilityChecks(Vector3 from, Action<Transform, float> callback)
        {
            foreach (var detected in DetectedObjects)
            {
                if (_simplifiedDetection && OnSimplifiedDetection(detected.transform, from, _mask))
                {
                    callback(detected.transform, 0.5f);
                }
                if(_simplifiedDetection) continue;
                
                float centerOfTarget = GetCenterOfTarget(detected.transform, from, _mask);
                if (centerOfTarget > 0f)
                {
                    callback(detected.transform, centerOfTarget);
                }
            }
        }

        private bool OnSimplifiedDetection(Transform target, Vector3 from, LayerMask mask)
        {
            var dir = (target.position - from).normalized;
            float distance = Vector2.Distance(target.position, from);

            var hit = Physics2D.Raycast(from, dir, distance, layerMask: mask);
            Debug.DrawLine(from,from + dir * distance, hit.collider == null ? Color.green : Color.red, 0.4f);
            return hit.collider == null && distance <= _radius;
        }
        private static float GetCenterOfTarget(Transform target, Vector3 from, LayerMask mask)
        {
            if (!target.TryGetComponent(out IObstacle objectObstacle))
            {
                return -1f; // No obstacle we consider that as a suppression fire ?? Then we need to adjust the center of fire to the stomach height
            }

            Height objectHeight = objectObstacle.GetHeight();
            var dir = (target.position - from).normalized;
            float distance = Vector2.Distance(target.position, from);

            RaycastHit2D[] results = new RaycastHit2D[MaxCollider];
            var size = Physics2D.RaycastNonAlloc(from, dir, results, distance, layerMask: mask);

            if (size == 0) return (objectHeight.Min + objectHeight.Max) / 2f - objectHeight.Min;

            float minHeight = objectHeight.Min;
            float maxHeight = objectHeight.Max;

            for (int i = 0; i < size; i++)
            {
                if (!results[i].collider.TryGetComponent(out IObstacle obstacle)) continue;

                Height h = obstacle.GetHeight();
                if (h.Min > objectHeight.Max) continue;

                if (h.Max > objectHeight.Max)
                {
                    return -1f; // Don't take the fact of grenade.
                }

                maxHeight = Mathf.Min(maxHeight, h.Min);
                minHeight = Mathf.Max(minHeight, h.Max);
            }
            return (minHeight + maxHeight) / 2f - minHeight;
        }
        private bool IsValidDetection(Collider2D detected)
        {
            //filter statics (environment)
            if (!detected.CompareTag("Enemy") && !detected.CompareTag("Player")) return false;
            
            //is IA enemy
            if (_isAi && detected.CompareTag("Player")) return true;

            //is Player enemy
            bool isEnemyPlayer = detected.TryGetComponent(out Agent character) && !character.IsOwner;
            return !_isAi && (isEnemyPlayer || detected.CompareTag("Enemy"));
        }
    }
}
