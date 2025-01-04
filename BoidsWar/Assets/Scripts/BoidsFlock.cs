using System;
using System.Collections.Generic;
using System.Linq;
using SO;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class BoidsFlock : MonoBehaviour
{
    [SerializeField]private Boids boidsPrefab;
    [SerializeField]private BoidsSO defaultSO;
    [Space]
    [SerializeField]private Vector3 startZoneSize;
    [SerializeField]private int quadrantSize = 20;
    [SerializeField] private float boidsMaxDistance;
    public float BoidsMaxDistance=> boidsMaxDistance;
    [Space]
    [SerializeField]private int boidsAmount = 1;
    [Header("Gizmos")] 
    [SerializeField] private bool displayStartBorder;
    [SerializeField] private bool displayMaxDistance;

    private Dictionary<Vector3Int, List<Boids>> _boidsQuadrant = new();

    private List<Boids> _boids = new();

    private void Start()
    {
        for (int i = 0; i < boidsAmount; i++)
        {
            CreateBoids(defaultSO,GetRandomStartZonePosition());
        }
    }

    private void Update()
    {
        UpdateBoidsQuadrants();
    }

    //Boids
    void CreateBoids(BoidsSO so, Vector3 position)
    {
        Boids boids = Instantiate(boidsPrefab, position, Quaternion.LookRotation(Random.onUnitSphere),transform);
        boids.so = so;
        boids.parentFlock = this;
        Vector3Int quadrant = GetQuadrant(position);
        boids.QuadrantPosition = quadrant;
        //Check if quadrant list exist
        if (_boidsQuadrant.TryGetValue(quadrant, out _)) _boidsQuadrant[quadrant].Add(boids); // Add boids if exist 
        else _boidsQuadrant.Add(quadrant, new(){boids});
        _boids.Add(boids);
    }

    void UpdateBoidsQuadrants()
    {
        foreach (var boids in _boids)
        {
            
            Vector3Int previousQuadrant = boids.QuadrantPosition;
            Vector3Int currentQuadrant = GetQuadrant(boids.transform.position);
            if (currentQuadrant == previousQuadrant) continue;

            _boidsQuadrant[previousQuadrant].Remove(boids);
            boids.QuadrantPosition = currentQuadrant;
            //Check if quadrant list exist
            if (_boidsQuadrant.TryGetValue(currentQuadrant, out _)) _boidsQuadrant[currentQuadrant].Add(boids); // Add boids if exist 
            else _boidsQuadrant.Add(currentQuadrant, new(){boids}); // Else create new list with boids in it
        }
    }
    
    
    //Utilities
    Vector3 GetRandomStartZonePosition()
    {
        float x, y, z;
        x = Random.Range(-startZoneSize.x / 2, startZoneSize.x / 2);
        y = Random.Range(-startZoneSize.y / 2, startZoneSize.y / 2);
        z = Random.Range(-startZoneSize.z / 2, startZoneSize.z / 2);

        return new(x, y, z);
    }

    Vector3Int GetQuadrant(Vector3 worldPosition)
    {
        int x, y, z;
        x = (int)(worldPosition.x / quadrantSize);
        y = (int)(worldPosition.y / quadrantSize);
        z = (int)(worldPosition.z / quadrantSize);
        return new(x,y,z);
    }

    public List<Transform> GetBoids() => _boids.Select(boids =>boids.transform).ToList();
    
    public List<Transform> GetQuadrantNeighbor(Vector3Int quadrantPosition)
    {
        List<Transform> neighbors = new();
        for (int z = quadrantPosition.z-1; z <= quadrantPosition.z+1; z++)
        {
            for (int y = quadrantPosition.y-1; y <= quadrantPosition.y+1; y++)
            {
                for (int x = quadrantPosition.x-1; x <= quadrantPosition.x+1; x++)
                {
                    if (_boidsQuadrant.TryGetValue(new(x, y, z), out List<Boids> neighborsBoids))
                    {
                        List<Transform> neighborsTransforms = neighborsBoids.Select(neighbor=> neighbor.transform).ToList();
                        neighbors.AddRange(neighborsTransforms);
                    }
                }
            }
        }

        return neighbors;
    }


    
    
    private void OnDrawGizmos()
    {
        if(displayStartBorder) Gizmos.DrawWireCube(transform.position, startZoneSize);
        if(displayMaxDistance) Gizmos.DrawWireSphere(transform.position, boidsMaxDistance);
    }
    
    
}