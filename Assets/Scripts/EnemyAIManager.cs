using System.Linq;
using UnityEngine;

public interface Agent
{
    Vector2 targetPosition { get; set; }
    Vector2 currentPosition { get; set; }
    float pBest { get; set; }
    Vector2 pBestPosition { get; set; }
    Vector2 inertia { get; set; }
    float getFitness();
};

public class EnemyAIManager : MonoBehaviour
{
    [Range(0.0f, 1.0f)] public float inertiaW = 1f;
    public float inertiaWeightDecay = 0.00f;
    [Range(0.01f, 1.0f)] public float minimalInertiaWeight = 0.4f;
    public float cognitiveC = 2f;
    public float socialC = 2f;
    public float fitnessDecay = 0.01f;

    public GameObject gBestPositionTarget;
    public GameObject pBestPositionTarget;

    public Player player;
    public Agent[] enemies;

    float gBest = -1;
    Vector2 gBestPosition = new Vector2(0, 0);
    GameObject gBestPositionTargetInstance;
    GameObject pBestPositionTargetInstance;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        GameObject[] enemiesObjects = GameObject.FindGameObjectsWithTag("Enemy");
        enemies = enemiesObjects.Select(e => e.GetComponent<Enemy>()).ToArray();
        gBestPositionTargetInstance = Instantiate(gBestPositionTarget);
        pBestPositionTargetInstance = Instantiate(pBestPositionTarget);
    }


    void FixedUpdate()
    {
        int i = 0;
        foreach (Agent e in enemies)
        {
            ++i;

            float fitness = e.getFitness();
            if (fitness > e.pBest)
            {
                e.pBest = fitness;
                e.pBestPosition = e.currentPosition;
                pBestPositionTargetInstance.transform.position = VectorConverter.Convert(e.pBestPosition, 5);
            }

            if (fitness > gBest)
            {
                gBest = fitness;
                gBestPosition = e.currentPosition;
                gBestPositionTargetInstance.transform.position = VectorConverter.Convert(gBestPosition, 5);
            }

            if (e.inertia == Vector2.zero)
            {
                e.inertia = new Vector2(Random.Range(-100f, 100f), Random.Range(-100f, 100f));
            }

            // e.pBestPosition = new Vector2(5, 0);
            // gBestPosition = new Vector2(5, 0);

            Vector2 inertia = e.inertia * inertiaW;
            Vector2 cognitiveComponent = cognitiveC * Random.Range(0f, 1f) * (e.pBestPosition - e.currentPosition);
            Vector2 socialComponent = socialC * Random.Range(0f, 1f) * (gBestPosition - e.currentPosition);

            e.inertia = inertia + cognitiveComponent + socialComponent;
            e.targetPosition = e.currentPosition + e.inertia;

            e.pBest = Mathf.Max(e.pBest - fitnessDecay, 0);
        }

        inertiaW = Mathf.Max(inertiaW - (inertiaWeightDecay * Time.fixedDeltaTime), minimalInertiaWeight);
        gBest = Mathf.Max(gBest - fitnessDecay, 0);
        print($"{i}: inertiaW = {inertiaW} | inertiaWeightDecay = {(inertiaWeightDecay * Time.fixedDeltaTime)}");
    }
}
