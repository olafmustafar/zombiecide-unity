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
    public static int generatedIndex = -1;

    public void Next(){
        steps.Next();
    }
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
    SHOW_GENERATED_LEVEL,
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
            ScenesState.nextQuestionType = QuestionsType.END_MESSAGE;
            SceneManager.LoadScene("FormScene");
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
                ScenesState.generatedIndex = Random.Range(0,5);
                LoadLevel(true, ScenesState.generatedIndex);
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
            case StepType.SHOW_GENERATED_LEVEL:
                SceneManager.LoadScene("ShowGenerated");
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