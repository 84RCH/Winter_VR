using UnityEngine;

public class RandomPlacement : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnItem
    {
        public GameObject prefab;
        public Vector3 rotationOffset;   // 個別補正
    }

    public SpawnItem[] items;

    public Terrain terrain;
    public int count = 50;

    void Start()
    {
        SpawnAll();
    }

    void SpawnAll()
    {
        float w = terrain.terrainData.size.x;
        float l = terrain.terrainData.size.z;

        for (int i = 0; i < count; i++)
        {
            SpawnOne();
        }
    }

    void SpawnOne()
    {
        // アイテムをランダム選択
        SpawnItem item = items[Random.Range(0, items.Length)];

        float x = Random.Range(0f, terrain.terrainData.size.x);
        float z = Random.Range(0f, terrain.terrainData.size.z);

        Vector3 pos = new Vector3(x, 0, z);
        pos.y = terrain.SampleHeight(pos);

        // 個別の回転補正を反映
        Quaternion rot = Quaternion.Euler(item.rotationOffset);

        Instantiate(item.prefab, pos, rot);
    }
}
