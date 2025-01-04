using UnityEngine;

namespace SO
{
    [CreateAssetMenu(fileName = "New BoidsSO", menuName = "BoidsSO", order = 0)]
    public class BoidsSO : ScriptableObject
    {
        [Header("Speed")]
        [SerializeField]private float angleSpeed = 100f;
        public float AngleSpeed => angleSpeed;
        
        [SerializeField]private float moveSpeed = .2f;
        public float MoveSpeed => moveSpeed;
        
        
        [Header("Detection")]
        [SerializeField]private float separateDistance = 1f;
        public float SeparateDistance => separateDistance;
        
        [SerializeField]private float alignDistance = 2f;
        public float AlignDistance => alignDistance;
        
        [SerializeField]private float cohesionDistance = 3f;
        public float CohesionDistance => cohesionDistance;
    }
}