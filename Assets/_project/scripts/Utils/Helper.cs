using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _project.scripts.Utils
{
    public static class Helper
    {
        public const float GravityConst = 9.8f * 0.2f;
        
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

        private static readonly Dictionary<int, WaitForSeconds> WaitForSecondsMap = new();
        public static WaitForSeconds WaitForSeconds(int seconds)
        {
            if (WaitForSecondsMap.TryGetValue(seconds, out var value))
                return value;
            
            var newValue = new WaitForSeconds(seconds);
            WaitForSecondsMap.Add(seconds, newValue);
            return newValue;
        }
        
        /// <summary>
        /// Checks if the given world position is visible on the screen.
        /// </summary>
        /// <param name="worldPosition">The world position to check.</param>
        /// <param name="camera">The camera to check against. If null, Camera.main will be used.</param>
        /// <returns>True if the position is visible on the screen, false otherwise.</returns>
        public static bool IsOnScreen(Vector3 worldPosition, Camera camera = null)
        {
            if (Camera == null)
            {
                Debug.LogError("No active camera in the scene");
                return false;
            }

            // Convert the world position to screen position
            Vector3 screenPosition = Camera.WorldToScreenPoint(worldPosition);

            // Check if the position is within the screen boundaries
            bool isVisible = screenPosition.x >= 0 && screenPosition.x <= Screen.width &&
                             screenPosition.y >= 0 && screenPosition.y <= Screen.height &&
                             screenPosition.z >= 0; // This ensures the object is in front of the camera

            return isVisible;
        }
    
        /// <summary>
        /// Determines whether a specified layer is included in or excluded from a given set of layers.
        /// </summary>
        /// <param name="layerToTest">The layer to be tested, typically obtained from a transform or game object.</param>
        /// <param name="layers">The set of layers to check against, represented as a LayerMask.</param>
        /// <param name="isIncludeLayers">If true, the method checks if the layer is included in the specified layers. If false, it checks if the layer is excluded.</param>
        /// <returns>
        /// Returns true if the layer is included in the specified layers when <paramref name="isIncludeLayers"/> is true, 
        /// or if the layer is not included in the specified layers when <paramref name="isIncludeLayers"/> is false.
        /// </returns>
        public static bool CompareLayer(this LayerMask layerToTest, LayerMask layers, bool isIncludeLayers = true)
        {
            return isIncludeLayers 
                ? ((1 << layerToTest) & layers) != 0
                : ((1 << layerToTest) & layers) == 0;
        }
        /// <summary>
        /// Determines whether a specified layer is included in or excluded from a given set of layers.
        /// </summary>
        /// <param name="layerToTest">The layer to be tested, typically obtained from a transform or game object.</param>
        /// <param name="layers">The set of layers to check against, represented as a LayerMask.</param>
        /// <param name="isIncludeLayers">If true, the method checks if the layer is included in the specified layers. If false, it checks if the layer is excluded.</param>
        /// <returns>
        /// Returns true if the layer is included in the specified layers when <paramref name="isIncludeLayers"/> is true, 
        /// or if the layer is not included in the specified layers when <paramref name="isIncludeLayers"/> is false.
        /// </returns>
        public static bool CompareLayer(this int layerToTest, LayerMask layers, bool isIncludeLayers = true)
        {
            return isIncludeLayers 
                ? ((1 << layerToTest) & layers) != 0
                : ((1 << layerToTest) & layers) == 0;
        }
    }
}