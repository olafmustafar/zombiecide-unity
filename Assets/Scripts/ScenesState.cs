using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesState : MonoBehaviour
{
    public static Dungeon dungeon;
    public static bool usePSOAI;
    public static QuestionsType nextQuestionType;
    public static Dictionary<QuestionsType, List<string>> formAnswers;
    public static Steps steps;
    public static bool playGeneratedLevelFirst;
}

public enum StepType
{
    MAIN_MENU,
    PROFILE_FORM,
    MANUAL_LEVEL,
    GENERATED_LEVEL,
    MANUAL_LEVEL_FORM,
    GENERATED_LEVEL_FORM,
    TURING_TEST,
    SIMILARITY_FORM,
}

public class Steps
{
    Queue<StepType> steps;

    public Steps()
    {
        steps = new Queue<StepType>();
    }

    public void Add(StepType step)
    {
        steps.Enqueue(step);
    }

    public void Next()
    {
        if (steps.Count <= 0)
        {
            foreach (var ans in ScenesState.formAnswers)
            {
                Debug.Log($"{ans.Key}: ");
                foreach( string answer in ans.Value){
                    Debug.Log($"    {answer}: ");
                }
            }
            return;
        }

        StepType nextStep = steps.Dequeue();
        switch (nextStep)
        {
            case StepType.PROFILE_FORM:
                ScenesState.nextQuestionType = QuestionsType.PROFILE;
                SceneManager.LoadScene("FormScene");
                break;
            case StepType.MANUAL_LEVEL:
                LoadLevel(false, Random.Range(0, 5));
                break;
            case StepType.GENERATED_LEVEL:
                LoadLevel(true, Random.Range(0, 5));
                break;
            case StepType.MANUAL_LEVEL_FORM:
                ScenesState.nextQuestionType = QuestionsType.IMMERSION_MANUAL;
                SceneManager.LoadScene("FormScene");
                break;
            case StepType.GENERATED_LEVEL_FORM:
                ScenesState.nextQuestionType = QuestionsType.IMMERSION_GENERATED;
                SceneManager.LoadScene("FormScene");
                break;
            case StepType.TURING_TEST:
                ScenesState.nextQuestionType = QuestionsType.TURING_TEST;
                SceneManager.LoadScene("FormScene");
                break;
            case StepType.SIMILARITY_FORM:
                ScenesState.nextQuestionType = QuestionsType.SIMILARITY;
                SceneManager.LoadScene("FormScene");
                break;
        }
    }

    void LoadLevel(bool isGenerated, int level)
    {
        ZombieTilesApi ztapi = new ZombieTilesApi();
        ztapi.LoadDungeon(isGenerated, level);
        ScenesState.dungeon = ztapi.dungeon;
        ScenesState.usePSOAI = isGenerated;
        SceneManager.LoadScene("MainScene");
    }
}