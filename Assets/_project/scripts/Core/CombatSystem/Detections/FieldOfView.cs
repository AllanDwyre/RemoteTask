using UnityEngine;

namespace _project.scripts.Core.CombatSystem.Detections
{
    //TODO : My first Idea is to have different settings based on the angle (front, side, back) of the player.
    //TODO : I think it's better to make something more simple to be able to refactor it.
    [RequireComponent(typeof(MeshFilter))]
    public class FieldOfView : MonoBehaviour
    {
        // [Serializable]
        // private struct Fov
        // {
        //     public float minAngle; 
        //     public float maxAngle;
        //     public Optional<float> distanceRatio;
        //      [tooltip("Number of degree between each ray")
        //     public Optional<int> rayStep;
        // }

        // [SerializeField] private Fov frontalFov;
        // [SerializeField] private Fov peripheralFov;
        // [SerializeField] private Fov backFov;
        [SerializeField] private float maxDistance;
        [SerializeField] private int rayCount;

        [SerializeField] private LayerMask obstacleMask;

        private Mesh _fieldMesh;
        private void Start()
        {
            _fieldMesh = new Mesh();
            GetComponent<MeshFilter>().mesh = _fieldMesh;
        }

        public void SetMaxDistance(float newDistance)
        {
            maxDistance = newDistance;
        }
        private void LateUpdate()
        {
            GenerateFieldOfView();
        }

        private void GenerateFieldOfView()
        {
            Vector3 origin = Vector3.zero;
            
            Vector3[] vertices = new Vector3[rayCount + 2];
            Vector2[] uv = new Vector2[vertices.Length];
            int[] triangles = new int[rayCount * 3];

            vertices[0] = origin;
            float step = 360f / rayCount;

            int triangleIndex = 0;

            for (int i = 0; i <= rayCount; i++)
            {
                var angle = step * i;
                Vector3 worldVertex = GetPhysicPosition(transform.position, angle, maxDistance);
                Vector3 localVertex = transform.InverseTransformPoint(worldVertex);
                vertices[i + 1] = localVertex;

                if (i <= 0) continue;
                
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = i + 1;
                triangles[triangleIndex + 2] = i;
                triangleIndex += 3;
            }
            
            _fieldMesh.vertices = vertices;
            _fieldMesh.uv = uv;
            _fieldMesh.triangles = triangles;
            _fieldMesh.RecalculateBounds();
        }
    
        private Vector3 GetPhysicPosition(Vector3 origin, float angle, float distance)
        {
            var dir = AngleToDirection(angle);
            RaycastHit2D hit = Physics2D.Raycast(origin, dir, distance, obstacleMask);
            return hit.collider == null ? origin + dir * distance : hit.point;
        }
        
        private static Vector3 AngleToDirection(float angle)
        {
            float rad = angle * (Mathf.PI / 180f);
            return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);
        }
        
    }
}
