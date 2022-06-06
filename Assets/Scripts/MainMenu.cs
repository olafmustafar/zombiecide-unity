using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public bool IsGeneratedLevel { get => isGeneratedLevel; set => isGeneratedLevel = value; }
    private bool isGeneratedLevel;

    public void StartGame(int level)
    {
        StartCoroutine( LoadLevel(IsGeneratedLevel, level));
    }

    IEnumerator LoadLevel( bool isGenerated, int level ){
        ZombieTilesApi ztapi = new ZombieTilesApi();
        yield return ztapi.LoadDungeon(isGenerated, level);
        ScenesState.dungeon = ztapi.dungeon;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
