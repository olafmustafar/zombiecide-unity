using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public bool IsGeneratedLevel { get => isGeneratedLevel; set => isGeneratedLevel = value; }
    private bool isGeneratedLevel;

    public void StartGame(int level)
    {
        LoadLevel(IsGeneratedLevel, level);
    }

    void LoadLevel(bool isGenerated, int level)
    {
        ZombieTilesApi ztapi = new ZombieTilesApi();
        ztapi.LoadDungeon(isGenerated, level);
        ScenesState.dungeon = ztapi.dungeon;
        ScenesState.usePSOAI = isGenerated;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
