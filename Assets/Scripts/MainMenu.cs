using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string manualLevelsPath;
    public string generatedLevelsPath;

    string[] manualLevels;
    string[] generatedLevels;

    public bool IsGeneratedLevel { get => isGeneratedLevel; set => isGeneratedLevel = value; }
    private bool isGeneratedLevel;

    void Awake()
    {
        manualLevels = Directory.GetFiles(manualLevelsPath, "*.json");
        generatedLevels = Directory.GetFiles(generatedLevelsPath, "*.json");
    }

    public void StartGame(int level)
    {
        ScenesState.level = IsGeneratedLevel ? generatedLevels[level] : manualLevels[level];
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
