using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARInteraction : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public Transform fishTransform;
    public int numberOfSpawns = 3;
    public float spawnRadius = 1.0f;
    public float destroyDelay = 5.0f;

    public GameObject foodPrefab;
    public float throwForce = 10f;
    public float foodDestroyDelay = 3f;
    public Transform arCameraTransform;

    private List<GameObject> spawnedPrefabs = new List<GameObject>();
    private List<GameObject> spawnedFood = new List<GameObject>();

    public float collisionDestroyDelay = 0.3f;

    public void SpawnPrefabsFromButton()
    {
        for (int i = 0; i < numberOfSpawns; i++)
        {
            Vector3 randomSpawnPosition = fishTransform.position + Random.insideUnitSphere * spawnRadius;
            randomSpawnPosition.y = fishTransform.position.y;

            GameObject spawnedObject = Instantiate(prefabToSpawn, randomSpawnPosition, Random.rotation);
            spawnedPrefabs.Add(spawnedObject);

            StartCoroutine(DelayedDestroy(spawnedObject, destroyDelay));
        }
    }

    public void ThrowFoodFromCamera()
    {
        GameObject food = Instantiate(foodPrefab, arCameraTransform.position, Quaternion.identity);
        Rigidbody foodRb = food.GetComponent<Rigidbody>();

        Vector3 throwDirection = arCameraTransform.forward;
        foodRb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
        spawnedFood.Add(food);

        StartCoroutine(DelayedDestroy(food, foodDestroyDelay));
    }

    IEnumerator DelayedDestroy(GameObject objectToDestroy, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (objectToDestroy != null)
        {
            Destroy(objectToDestroy);
            spawnedPrefabs.Remove(objectToDestroy);
            spawnedFood.Remove(objectToDestroy);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject otherObject = collision.gameObject;

        if (spawnedPrefabs.Contains(gameObject) && spawnedFood.Contains(otherObject))
        {
            StartCoroutine(DestroyWithDelay(gameObject, collisionDestroyDelay));
            StartCoroutine(DestroyWithDelay(otherObject, collisionDestroyDelay));
        }
        else if (spawnedFood.Contains(gameObject) && spawnedPrefabs.Contains(otherObject))
        {
            StartCoroutine(DestroyWithDelay(gameObject, collisionDestroyDelay));
            StartCoroutine(DestroyWithDelay(otherObject, collisionDestroyDelay));
        }
    }

    IEnumerator DestroyWithDelay(GameObject objectToDestroy, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (objectToDestroy != null)
        {
            Destroy(objectToDestroy);
            spawnedPrefabs.Remove(objectToDestroy);
            spawnedFood.Remove(objectToDestroy);
        }
    }
}
