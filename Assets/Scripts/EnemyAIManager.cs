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
    [Range(0.0f, 1.0f)] public float inertiaWeight = 0.9f;
    public float inertiaWeightDecay = 0.01f;
    [Range(0.01f, 1.0f)] public float minimalInertiaWeight = 0.4f;
    public float cognitiveConstant = 2f;
    public float socialConstant = 2f;
    public float fitnessDecay = 0.01f;

    public GameObject gBestPositionTarget;

    public Player player;
    public Agent[] enemies;

    float gBest = -1;
    Vector2 gBestPosition = new Vector2(0, 0);

    GameObject gBestPositionTargetInstance;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        GameObject[] enemiesObjects = GameObject.FindGameObjectsWithTag("Enemy");
        enemies = enemiesObjects.Select(e => e.GetComponent<Enemy>()).ToArray();
        gBestPositionTargetInstance = Instantiate(gBestPositionTarget);
    }


    void FixedUpdate()
    {
        foreach (Agent e in enemies)
        {
            print($"e.currentPosition = {e.currentPosition.ToString("F8")} | e.targetPosition = {e.targetPosition.ToString("F8")}");
            if (e.currentPosition != e.targetPosition)
            {
                continue;
            }

            float fitness = e.getFitness();
            if (fitness > e.pBest)
            {
                e.pBest = fitness;
                e.pBestPosition = e.currentPosition;
            }

            if (fitness > gBest)
            {
                gBest = fitness;
                gBestPosition = e.currentPosition;
                gBestPositionTargetInstance.transform.position = VectorConverter.Convert(gBestPosition, 5);
            }

            if (e.inertia == Vector2.zero)
            {
                e.inertia = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            }

            // e.pBestPosition = new Vector2(0, 0);
            // gBestPosition = new Vector2(5, 0);

            Vector2 inertia = e.inertia * inertiaWeight;
            Vector2 cognitiveComponent = cognitiveConstant * Random.Range(0f, 1f) * (e.pBestPosition - e.currentPosition);
            Vector2 socialComponent = socialConstant * Random.Range(0f, 1f) * (gBestPosition - e.currentPosition);

            e.inertia = inertia + cognitiveComponent + socialComponent;
            print($"e.inertia{e.inertia} = inertia{inertia} + cognitiveComponent{cognitiveComponent} + socialComponent{socialComponent}");
            e.targetPosition = e.currentPosition + e.inertia;

            inertiaWeight = Mathf.Max(inertiaWeight - inertiaWeightDecay, minimalInertiaWeight);

            gBest -= fitnessDecay;
            e.pBest -= fitnessDecay;
            print($"gBest: {gBest} | pBest: {e.pBest}");
        }
    }
}
