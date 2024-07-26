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
    }
}