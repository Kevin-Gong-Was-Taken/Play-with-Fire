using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCubeThrower : MonoBehaviour
{
    [SerializeField] private GameObject iceCube;

    [Space]
    [SerializeField] private float width;
    [SerializeField] private float height;

    [Space]
    [SerializeField] private float spawnDelay;

    // Coroutine for spawning the Ice Cube
    private IEnumerator SpawnIceCube()
    {
        Instantiate(iceCube, new Vector2(Random.Range(-width, width), Random.Range(-height, height)) + (Vector2)transform.position, transform.rotation);


        yield return new WaitForSeconds(spawnDelay);
        StartCoroutine(nameof(SpawnIceCube));
    }

    private void Start()
    {
        StartCoroutine(nameof(SpawnIceCube));
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector2(width * 2, height * 2));
    }
}
