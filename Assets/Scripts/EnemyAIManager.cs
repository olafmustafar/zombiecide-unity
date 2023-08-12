using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public interface Agent
{
    //PSO AI
    Vector2 targetPosition { get; set; }
    Vector2 position { get; set; }
    float pBest { get; set; }
    Vector2 pBestPosition { get; set; }
    Vector2 inertia { get; set; }

    float getFitness();

    //common AI
    bool triggered { get; set; }
    float randomMovmentCooldown { get; set; }
};

public class EnemyAIManager : MonoBehaviour
{
    [Range(0.0f, 1.0f)] public float inertiaW = 1f;
    public float inertiaWeightDecay = 0.00f;
    [Range(0.01f, 1.0f)] public float minimalInertiaWeight = 0.4f;
    [Range(0, 20)] public int detectionDistance = 13;
    public float cognitiveConst = 2f;
    public float socialConst = 2f;

    public float pFitnessDecay = 0.01f;
    public float gFitnessDecay = 0.01f;
    public float minimalVelocityMult = 1.0f;
    public float maxVisionDistance = 50;
    public float maxSoundDistance = 30;
    public float drag = 0;

    public bool randomMovmentGeneration = true;

    public GameObject gBestPositionTarget;
    public GameObject pBestPositionTarget;
    public Player player;

    [HideInInspector] public float gBest = float.MaxValue;
    Vector2 gBestPosition = new Vector2(0, 0);
    GameObject gBestPositionTargetInstance;

    GameObject pBestPositionTargetInstance;
    GameObject[] enemies;
    SectorManager sectorManager;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (ScenesState.usePSOAI)
        {
            gBestPositionTargetInstance = Instantiate(gBestPositionTarget);
            pBestPositionTargetInstance = Instantiate(pBestPositionTarget);
        }
        sectorManager = GetComponentInParent<SectorManager>();
    }

    void FixedUpdate()
    {
        if (ScenesState.usePSOAI)
        {
            PSOAI();
        }
        else
        {
            CommonAI();
        }
    }

    void PSOAI()
    {
        Enemy[] agents = enemies
                           .Where(e => e != null)
                           .Select(e => e.GetComponent<Enemy>())
                           .ToArray();

        foreach (GameObject e in enemies)
        {
            if (e != null)
            {
                e.GetComponent<Rigidbody>().drag = drag;
            }
        }

        UpdateBestValues(agents);

        foreach (Enemy e in agents)
        {

            Vector2 cognitiveComponentToShow = cognitiveConst * CalcVectorToPosition(e, e.pBestPosition);
            Vector2 socialComponentToShow = socialConst * CalcVectorToPosition(e, gBestPosition);
            Vector2 velocityToShow = (cognitiveComponentToShow + socialComponentToShow) / 10;
            Vector2 targetPosToShow = e.position + (velocityToShow.normalized * 2);
            Debug.DrawLine(VectorConverter.Convert(e.position), VectorConverter.Convert(targetPosToShow), Color.green);
            Debug.DrawLine(VectorConverter.Convert(gBestPosition), VectorConverter.Convert(gBestPosition, 5), Color.red);

            float distance = Vector2.Distance(player.position, gBestPosition);
            Vector2 cognitiveComponent = cognitiveConst * Random.Range(0f, 1f) * CalcVectorToPosition(e, e.pBestPosition);
            Vector2 socialComponent = distance > detectionDistance ? Vector2.zero : socialConst * Random.Range(0f, 1f) * CalcVectorToPosition(e, gBestPosition);
            Vector2 velocity = cognitiveComponent + socialComponent;

            float mag = velocity.magnitude;
            if (mag > 0)
            {
                float minimalVelocity = distance * minimalVelocityMult;
                velocity *= (Mathf.Max(mag, minimalVelocity) / mag);
            }

            Vector2 targetPos = e.position + velocity;
            e.targetPosition = targetPos;
            // e.targetPosition = CalcVectorToPosition(e, player.position);
        }

        inertiaW = Mathf.Max(inertiaW - (inertiaWeightDecay * Time.fixedDeltaTime), minimalInertiaWeight);
    }

    void UpdateBestValues(Agent[] agents)
    {
        float newGBest = float.MaxValue;

        Vector2 newGBestPos = Vector2.zero;
        foreach (Agent e in agents)
        {
            float fitness = CalcFitness(e);

            float oldPBest = e.pBest + (pFitnessDecay * Time.fixedDeltaTime);
            if (fitness < oldPBest)
            {
                e.pBest = fitness;
                e.pBestPosition = e.position;
            }
            else
            {
                e.pBest = oldPBest;
            }

            if (fitness < newGBest)
            {
                newGBest = fitness;
                newGBestPos = e.position;
            }
        }

        float oldGBest = gBest + (gFitnessDecay * Time.fixedDeltaTime);
        if (newGBest < oldGBest)
        {
            gBest = newGBest;
            gBestPosition = newGBestPos;
        }
        else
        {
            gBest = oldGBest;
        }

    }

    Vector2 CalcVectorToPosition(Agent e, Vector2 position)
    {
        NavMeshPath path = new NavMeshPath();

        VectorConverter.CalculateNavmeshPath(e.position, position, path);
        if (path.corners.Length < 2)
        {
            return Vector2.zero;
        }

        float pathSqrMagnitude = path.corners.Select(x => x.sqrMagnitude).Sum();
        float distance = Mathf.Sqrt(pathSqrMagnitude);

        Vector2 target = VectorConverter.Convert(path.corners[1]) - e.position;

        return distance * target.normalized;
    }

    float CalcFitness(Agent agent)
    {
        float fitness = float.MaxValue;
        if (!player.isActiveAndEnabled)
        {
            return fitness;
        }

        Vector3 pos = VectorConverter.Convert(agent.position);
        Vector3 target = player.rb.position - pos;
        float distance = Vector2.Distance(player.position, agent.position);

        float visionScore = maxVisionDistance;
        {
            //vision 
            if (Physics.Raycast(pos, target, out RaycastHit hit) && hit.collider.CompareTag("Player"))
            {
                visionScore = Mathf.Min(distance, maxVisionDistance);
            }
        }

        float soundScore = maxSoundDistance;
        {
            //sound
            RaycastHit[] raycastHits = Physics.RaycastAll(pos, target.normalized, target.magnitude);
            int nonPlayerObjects = 0;
            foreach (RaycastHit hit in raycastHits)
            {
                if (!hit.collider.CompareTag("Player"))
                {
                    nonPlayerObjects++;
                }
            }
            soundScore = Mathf.Min(distance, maxSoundDistance);
        }

        return visionScore + soundScore;
    }

    void CommonAI()
    {
        Agent[] agents = enemies
                           .Where(e => e != null)
                           .Select(e => e.GetComponent<Enemy>())
                           .ToArray();

        foreach (Agent e in agents)
        {
            NavMeshPath path = new NavMeshPath();
            VectorConverter.CalculateNavmeshPath(e.position, player.position, path);

            if (path.corners.Length == 2 && (player.position - e.position).magnitude < 50)
            {
                e.triggered = true;
            }

            if (!e.triggered)
            {
                if (randomMovmentGeneration && e.randomMovmentCooldown <= 0)
                {
                    e.targetPosition = e.position + new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f));
                    e.randomMovmentCooldown = Random.Range(0f, 0.5f);
                }
                e.randomMovmentCooldown -= Time.fixedDeltaTime;
                continue;
            }

            if (path.corners.Length >= 2)
            {
                e.targetPosition = VectorConverter.Convert(path.corners[1]);
            }
        }
    }
}
