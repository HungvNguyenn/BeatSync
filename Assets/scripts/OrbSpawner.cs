using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbSpawner : MonoBehaviour
{
    public GameObject orbPrefab;

    public float spawnRate = 1f;
    public float spawnDistance = 6f;
    public float horizontalRange = 2f;
    public float verticalRange = 1f;

    float timer;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnRate)
        {
            spawnOrb();
            timer = 0;
        }
    }

    void spawnOrb()
    {
        Vector3 spawnPos = new Vector3(
        Random.Range(-horizontalRange, horizontalRange),
        Random.Range(1f, 1f + verticalRange),
        spawnDistance
        );

        Instantiate(orbPrefab, spawnPos, Quaternion.identity);
    }
}
