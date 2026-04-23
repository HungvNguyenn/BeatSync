using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DodgeSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;

    public float spawnDistance = 5f;
    public float spawnRate = 2.5f;
    public float horizontalRange = 2f;

    float timer;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnRate)
        {
            SpawnObstacle();
            timer = 0f;
        }
    }

    void SpawnObstacle()
    {
        Vector3 spawnPos = new Vector3(
            Random.Range(-horizontalRange, horizontalRange),
            1.5f,
            spawnDistance
            );

        Instantiate(obstaclePrefab, spawnPos, Quaternion.identity );
    }
}
