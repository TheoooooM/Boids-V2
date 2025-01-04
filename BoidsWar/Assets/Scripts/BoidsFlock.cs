using System;
using System.Collections.Generic;
using System.Linq;
using SO;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoidsFlock : MonoBehaviour
{
    [SerializeField]private Boids boidsPrefab;
    [SerializeField]private Vector3 startZoneSize;
    [SerializeField] private float boidsMaxDistance;
    public float BoidsMaxDistance=> boidsMaxDistance;
    [Space]
    [SerializeField]private int boidsAmount = 1;
    [Space]
    [SerializeField]private BoidsSO defaultSO;
    [Header("Gizmos")] 
    [SerializeField] private bool displayStartBorder;
    [SerializeField] private bool displayMaxDistance;

    private List<Boids> _boids = new();

    private void Start()
    {
        for (int i = 0; i < boidsAmount; i++)
        {
            CreateBoids(defaultSO,GetRandomStartZonePosition());
        }
    }

    //Boids
    void CreateBoids(BoidsSO so, Vector3 position)
    {
        Boids boids = Instantiate(boidsPrefab, position, Quaternion.LookRotation(Random.onUnitSphere),transform);
        boids.so = so;
        boids.parentFlock = this;
        _boids.Add(boids);
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

    public List<Transform> GetBoids() => _boids.Select(boids =>boids.transform).ToList();


    
    
    private void OnDrawGizmos()
    {
        if(displayStartBorder) Gizmos.DrawWireCube(transform.position, startZoneSize);
        if(displayMaxDistance) Gizmos.DrawWireSphere(transform.position, boidsMaxDistance);
    }
    
    
}