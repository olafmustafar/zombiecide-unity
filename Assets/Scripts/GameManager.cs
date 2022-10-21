using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public BoardManager boardScript;
    public EnemyAIManager enemyAIManager;
    public TextMeshProUGUI GUIText;
    public GameObject player;
    public GameObject defeatScreen;
    public GameObject victoryScreen;
    public GameObject canvas;
    public SectorManager sm;
    public int score;

    public void Next()
    {
        ScenesState.steps.Next();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private bool gameIsOver = false;

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

        boardScript = GetComponent<BoardManager>();

        InitGame();
    }

    private void InitGame()
    {
        boardScript.SetupScene();
    }

    void Start()
    {
        GUIText = GameObject.Find("GUIText").GetComponent<TextMeshProUGUI>();
        player = GameObject.FindGameObjectWithTag("Player");
        enemyAIManager = GetComponentInChildren<EnemyAIManager>();
        canvas = GameObject.Find("Canvas");
        sm = GetComponentInParent<SectorManager>();
    }

    void Update()
    {

        if (gameIsOver)
        {
            return;
        }

        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            handleVictory();
            return;
        }

        float health = player.GetComponent<Player>().health;
        if (health < 0)
        {
            player.SetActive(false);
            handleDefeat();
            return;
        }

        GUIText.SetText($"Life: {health}\nCoins: {score}");
    }

    void handleDefeat()
    {
        GameObject instance = Instantiate(defeatScreen, canvas.transform);
        FindObjectOfType<AudioManager>().Play("PlayerDeath");
        gameIsOver = true;
    }

    void handleVictory()
    {
        GameObject instance = Instantiate(victoryScreen, canvas.transform);
        gameIsOver = true;
    }

    public void IncreaseScore(int score)
    {
        this.score += score;
    }
}
