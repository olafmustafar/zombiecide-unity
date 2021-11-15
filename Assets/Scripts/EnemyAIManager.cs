using System.Linq;
using UnityEngine;

public class EnemyAIManager : MonoBehaviour
{
    [Range(0.0f, 1.0f)] public float inertiaWeight = 0.9f;
    public float inertiaWeightDecay = 0.01f;
    [Range(0.01f, 1.0f)] public float minimalInertiaWeight = 0.4f;
    public float cognitiveConstant = 2f;
    public float socialConstant = 2f;

    public Player player;
    public Enemy[] enemies;

    float gBest = -1;
    Vector2 gBestPosition = new Vector2(0, 0);

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        GameObject[] enemiesObjects = GameObject.FindGameObjectsWithTag("Enemy");
        enemies = enemiesObjects.Select(e => e.GetComponent<Enemy>()).ToArray();
    }

    void Update()
    {
        foreach (Enemy e in enemies)
        {
            float fitness = e.getSoundLevel();
            if (fitness > e.pBest)
            {
                e.pBest = fitness;
                e.pBestPosition = e.currentPosition;
            }

            if (fitness > gBest)
            {
                gBest = fitness;
                gBestPosition = e.currentPosition;
            }

            print( e.currentPosition );
            e.targetPosition = e.currentPosition + gBestPosition + e.pBestPosition;


            Vector2 inertia = inertiaWeight * (e.currentPosition - e.targetPosition);
            Vector2 cognitiveComponent = (cognitiveConstant * Random.Range(0, 1) * (e.currentPosition - e.pBestPosition));
            Vector2 socialComponent = (socialConstant * Random.Range(0, 1) * (e.currentPosition - e.pBestPosition));
            e.velocity = (inertia + cognitiveComponent + socialComponent).magnitude;

            inertiaWeight -= inertiaWeightDecay;
        }
    }
}
