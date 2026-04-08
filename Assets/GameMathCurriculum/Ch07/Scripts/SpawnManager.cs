using UnityEngine;
using UnityEngine.UIElements;

public class SpawnManager : MonoBehaviour
{
    public GameObject sphere;
    public Transform startField;
    public Transform endField;
    public int spawnCount = 10;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnMovers();
        }
    }

    void SpawnMovers()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            GameObject s = Instantiate(sphere, startField.position, Quaternion.identity);
            BezierManager bz = s.AddComponent<BezierManager>();

            float randomTime = Random.Range(1f, 3f);

            bz.Init(startField.position, endField.position, randomTime);
        }
    }
}
