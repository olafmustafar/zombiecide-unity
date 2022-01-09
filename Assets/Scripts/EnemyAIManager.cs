using System.Linq;
using UnityEngine;
using UnityEngine.AI;

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

    [HideInInspector] public float gBest = -1;
    Vector2 gBestPosition = new Vector2(0, 0);
    GameObject gBestPositionTargetInstance;
    GameObject pBestPositionTargetInstance;
    GameObject[] enemies;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        gBestPositionTargetInstance = Instantiate(gBestPositionTarget);
        pBestPositionTargetInstance = Instantiate(pBestPositionTarget);
    }


    void FixedUpdate()
    {
        Agent[] agents = enemies
                           .Where(e => e != null)
                           .Select(e => e.GetComponent<Enemy>())
                           .ToArray();
        Enemy[] agents_ = enemies
                           .Where(e => e != null)
                           .Select(e => e.GetComponent<Enemy>())
                           .ToArray();

        int i = 0;

        foreach (Agent e in agents)
        {
            float fitness = e.getFitness();
            if (fitness >= e.pBest)
            {
                e.pBest = fitness;
                e.pBestPosition = e.currentPosition;
                pBestPositionTargetInstance.transform.position = VectorConverter.Convert(e.pBestPosition, 5);
            }

            if (fitness >= gBest)
            {
                gBest = fitness;
                gBestPosition = e.currentPosition;
                gBestPositionTargetInstance.transform.position = VectorConverter.Convert(gBestPosition, 5);
            }

            // if (e.inertia == Vector2.zero)
            // {
            //     e.inertia = new Vector2(Random.Range(-100f, 100f), Random.Range(-100f, 100f));
            // }
            Vector2 inertia = e.inertia * inertiaW;

            NavMeshPath path = new NavMeshPath();

            Vector2 cognitiveComponent = Vector2.zero;
            if (e.currentPosition != e.pBestPosition)
            {
                VectorConverter.CalculateNavmeshPath(e.currentPosition, e.pBestPosition, path);
                float pathSqrMagnitude = path.corners.Select(x => x.sqrMagnitude).Sum();
                float distance = Mathf.Sqrt(pathSqrMagnitude);

                if (path.corners.Length > 0)
                {
                    cognitiveComponent = cognitiveC * Random.Range(0f, 1f) * distance * (VectorConverter.Convert(path.corners[1]) - e.currentPosition).normalized;
                }

            }

            Vector2 socialComponent = Vector2.zero;
            if (e.currentPosition != gBestPosition)
            {
                VectorConverter.CalculateNavmeshPath(e.currentPosition, gBestPosition, path);
                float pathSqrMagnitude = path.corners.Select(x => x.sqrMagnitude).Sum();
                float distance = Mathf.Sqrt(pathSqrMagnitude);
                if (path.corners.Length > 0)
                {
                    socialComponent = socialC * Random.Range(0f, 1f) * distance * (VectorConverter.Convert(path.corners[1]) - e.currentPosition).normalized;
                }
            }

            e.inertia = inertia + cognitiveComponent + socialComponent;
            e.targetPosition = e.currentPosition + e.inertia;

            e.pBest = Mathf.Max(e.pBest - fitnessDecay, 0);
            i++;

        }

        inertiaW = Mathf.Max(inertiaW - (inertiaWeightDecay * Time.fixedDeltaTime), minimalInertiaWeight);
        gBest = Mathf.Max(gBest - fitnessDecay, 0);
    }
}
