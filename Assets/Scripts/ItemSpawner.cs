using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public Item itemPrefab;
    public Transform parent;
    public float spawnRate = 1f;
    public float moveSpeed = 100f;
    public float spawnZ = 300f;
    public float destroyZ = -20f;
    public float[] laneX = { -2f, 0f, 2f };
    public float groundY = -1f;
    public List<Item> activeItems = new List<Item>();
    private float spawnTimer;

    private void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnRate)
        {
            SpawnItem();
            spawnTimer = 0f;
        }

        for (int i = activeItems.Count - 1; i >= 0; i--)
        {
            Item item = activeItems[i];
            item.itemPosition.z -= moveSpeed * Time.deltaTime;

            if (item.itemPosition.z < destroyZ)
            {
                activeItems.RemoveAt(i);
                Destroy(item.gameObject);
            }
        }

        activeItems.Sort((a, b) => b.itemPosition.z.CompareTo(a.itemPosition.z));

        for (int i = 0; i < activeItems.Count; i++)
        {
            activeItems[i].transform.SetSiblingIndex(i);
        }
    }

    private void SpawnItem()
    {
        Item newItem = Instantiate(itemPrefab, parent);
        int randomLane = Random.Range(0, laneX.Length);
        newItem.itemPosition = new Vector3(laneX[randomLane], groundY, spawnZ);
        activeItems.Add(newItem);
    }
}
