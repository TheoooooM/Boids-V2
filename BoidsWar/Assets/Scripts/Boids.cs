using System.Collections;
using System.Collections.Generic;
using SO;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boids : MonoBehaviour
{
    private const float SeparateWeight = 4;
    private const float AlignWeight = .5f;
    private const float CohesionWeight = 1;
    
    
    public BoidsSO so;
    [Header("Gizmos")] 
    [SerializeField] private bool displayRotationLines;
    [SerializeField] private float rotationLinesMag = .2f;
    [Space]
    [SerializeField] private bool displayDetectionDistance;
    
    public BoidsFlock parentFlock { get; set; }
    public Vector3Int QuadrantPosition; //{ get; set; }
    

    private Quaternion targetRotation;

    private void Update()
    {
        //List<Transform> neighbors = parentFlock.GetBoids(); //Get Flock Boids
        List<Transform> neighbors = parentFlock.GetQuadrantNeighbor(QuadrantPosition); //Get Flock Boids
        neighbors.Remove(transform); //Remove Self
        ComputeNeighbors(neighbors);
        ComputeFlockBorder();
        
        LerpRotation(); //Rotate
        Move(); // Move
        
        
    }
    
    /// <summary>
    /// Calculate the target direction by neighbors distance
    /// </summary>
    void ComputeNeighbors(List<Transform> neighbors)
    {
        Vector3 deltaDir = Vector3.zero;
        foreach (Transform neighbor in neighbors)
        {
            Vector3 neighborPosition = neighbor.position;
            Vector3 position = transform.position;
            float distance = Vector3.Distance(neighborPosition, position);

            if (distance < so.SeparateDistance) deltaDir += ((position - neighborPosition)*SeparateWeight) / distance; //Add opposite direction to Avoid
            else if (distance < so.AlignDistance) deltaDir += neighbor.forward * AlignWeight; // Add neighbor move direction to align
            else if (distance < so.CohesionDistance) deltaDir += ((neighborPosition - position) * CohesionWeight) / distance; //Add neighbor direction to bring together

        }

        targetRotation = Quaternion.LookRotation(deltaDir);
    }

    void ComputeFlockBorder()
    {
        Vector3 flockCenterDirection = parentFlock.transform.position- transform.position;
        if (flockCenterDirection.magnitude < parentFlock.BoidsMaxDistance) return;
        Vector3 newDirection = ((targetRotation * Vector3.forward + flockCenterDirection) / 2).normalized;
        targetRotation = Quaternion.LookRotation(newDirection);
    }

    void LerpRotation()
    {
        var deltaAngle = so.AngleSpeed * Time.deltaTime;
        if (Quaternion.Angle(transform.rotation, targetRotation) < deltaAngle) transform.rotation = targetRotation;
        else transform.rotation = Quaternion.RotateTowards(transform.rotation,targetRotation,deltaAngle);
    }
    void Move()
    {
        transform.position += transform.forward * so.MoveSpeed * Time.deltaTime;
    }
    
    

    private void OnDrawGizmos()
    {
        if (displayRotationLines)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.forward * rotationLinesMag);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, targetRotation * Vector3.forward * rotationLinesMag);
        }
        if(so == null)return;
        if (displayDetectionDistance)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, so.SeparateDistance);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, so.AlignDistance);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, so.CohesionDistance);
        }
    }

    IEnumerator UpdateTarget()
    {
        while (true)
        {
            targetRotation = Quaternion.LookRotation(Random.onUnitSphere);
            yield return new WaitForSeconds(1);
        }

    }
}