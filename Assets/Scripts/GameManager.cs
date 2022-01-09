using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public BoardManager boardScript;
    public EnemyAIManager enemyAIManager;
    public TextMeshProUGUI GUIText;
    public Player player;

    private int level = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        boardScript = GetComponent<BoardManager>();
        InitGame();
    }

    private void InitGame()
    { 
        boardScript.SetupScene(level);
    }

    void Start()
    {
        GUIText = GameObject.Find("GUIText").GetComponent<TextMeshProUGUI>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        enemyAIManager = GetComponentInChildren<EnemyAIManager>();
    }

    void Update()
    {
        GUIText.SetText($"GBest: {enemyAIManager.gBest}\nLife : {player.health}");
    }

}
