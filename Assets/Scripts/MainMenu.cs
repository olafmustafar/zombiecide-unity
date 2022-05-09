using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    string[] manualLevels;
    string[] generatedLevels;

    public bool IsGeneratedLevel { get => isGeneratedLevel; set => isGeneratedLevel = value; }
    private bool isGeneratedLevel;

    void Awake()
    {
        string manualLevelsPath = Path.GetFullPath("./levels/manual");
        manualLevels = Directory.GetFiles(manualLevelsPath, "*.json");

        string generatedLevelsPath = Path.GetFullPath("./levels/generated");
        generatedLevels = Directory.GetFiles(generatedLevelsPath, "*.json");
    }

    public void StartGame(int level)
    {
        ScenesState.level = IsGeneratedLevel ? generatedLevels[level] : manualLevels[level];
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
