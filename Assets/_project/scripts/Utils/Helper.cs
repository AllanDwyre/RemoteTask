using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _project.scripts.utils
{
    public static class Helper
    {
        private static Camera _camera;

        public static Camera Camera
        {
            get
            {
                if (_camera == null) _camera = Camera.main;
                return _camera;
            }
        }

        public static Vector2 MousePosition => Mouse.current.position.ReadValue();

        private static WaitForFixedUpdate _waitForFixedUpdate;
        
        /// <summary>
        /// Reduce the garbage collector
        /// </summary>
        public static WaitForFixedUpdate WaitForFixedUpdate
        {
            get { return _waitForFixedUpdate ??= new WaitForFixedUpdate(); }
        }

        private static Dictionary<int, WaitForSeconds> _waitForSeconds = new();
        public static WaitForSeconds WaitForSeconds(int seconds)
        {
            if (_waitForSeconds.TryGetValue(seconds, out var value))
                return value;
            
            var newValue = new WaitForSeconds(seconds);
            _waitForSeconds.Add(seconds, newValue);
            return newValue;
        }
    }
}