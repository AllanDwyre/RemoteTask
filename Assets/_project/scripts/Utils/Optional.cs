using System;
using UnityEngine;

namespace _project.scripts.Utils
{
    [Serializable]
    public class Optional<T>
    {
        [SerializeField] private T value;
        [SerializeField] private bool enable;

        public bool Enable => enable;
        public T Value => value;

        public Optional(T initialValue)
        {
            value = initialValue;
            enable = true;
        }

    }
}