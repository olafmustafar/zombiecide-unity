using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public bool IsGeneratedLevel { get => isGeneratedLevel; set => isGeneratedLevel = value; }
    private bool isGeneratedLevel;

    public void StartGame()
    {
        bool playGeneratedLevelFirst = Random.Range(0, 1) > 0.5f;
        ScenesState.playGeneratedLevelFirst = playGeneratedLevelFirst;

        Steps steps = new Steps();
        steps.Add(StepType.PROFILE_FORM);

        if (playGeneratedLevelFirst)
        {
            steps.Add(StepType.GENERATED_LEVEL);
            steps.Add(StepType.GENERATED_LEVEL_FORM);
            steps.Add(StepType.MANUAL_LEVEL);
            steps.Add(StepType.MANUAL_LEVEL_FORM);
        }
        else
        {
            steps.Add(StepType.MANUAL_LEVEL);
            steps.Add(StepType.MANUAL_LEVEL_FORM);
            steps.Add(StepType.GENERATED_LEVEL);
            steps.Add(StepType.GENERATED_LEVEL_FORM);
        }

        steps.Add(StepType.TURING_TEST);
        steps.Add(StepType.SIMILARITY_FORM);
        ScenesState.steps = steps;

        ScenesState.steps.Next();
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
