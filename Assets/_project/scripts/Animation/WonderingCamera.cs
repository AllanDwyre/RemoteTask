using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _project.scripts.utils;
using UnityEngine;

namespace _project.scripts.Animation
{
    public class WonderingCamera : MonoBehaviour
    {
        [SerializeField] private List<Transform> _path;
        [SerializeField] private float _step;
        [SerializeField] private float _speed;


        private void Start()
        {
            StartCoroutine(MoveAlongPath(_path.Select(g => g.position + Vector3.back).ToList()));
        }

        private IEnumerator MoveAlongPath(List<Vector3> path)
        {
            int currentWaypointIndex = 0;

            while (true)
            {
                var moveToPosition = path[currentWaypointIndex % path.Count];

                while (transform.position != moveToPosition)
                {
                    float step = _speed * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, moveToPosition , step);

                    yield return Helper.WaitForFixedUpdate;
                }

                currentWaypointIndex++;
            }
        }
    }
}