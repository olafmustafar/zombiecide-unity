using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum QuestionsType
{
    PROFILE,
    IMMERSION_MANUAL,
    IMMERSION_GENERATED,
    TURING_TEST,
    SIMILARITY,
    END_MESSAGE
}

public struct Question
{
    public string description { get; set; }
    public string[] options { get; set; }

    public Question(string description, string[] options)
    {
        this.description = description;
        this.options = options;
    }
}

public class QuestionsManager : MonoBehaviour
{
    public GameObject buttonPrefab;
    public GameObject canvas;
    public TextMeshProUGUI tmPro;

    QuestionsType questionsType;
    int index;

    Dictionary<QuestionsType, Question[]> questions;

    List<GameObject> buttonList;

    public QuestionsManager()
    {
        Question[] profileQuestions = new Question[]{
            new Question("Qual a sua idade?", new string[]{"9 ou menos", "10 a 20", "21 a 30", "31 a 40", "41 a 50", "51 a 60", "60 ou mais"}),
            new Question("Você já jogou ou joga videogame?", new string[]{"Sim", "Não"}),
        };

        Question[] immersionQuestions = new Question[]{
            new Question("Na sua opinião, qual foi o nível de difículdade do nível jogado? (número maior significa mais difícil).", new string[]{"1", "2", "3", "4", "5"}),
            new Question("Na sua opinião, qual foi o nível de imersão do nível jogado? (número maior significa mais imersivo).", new string[]{"1", "2", "3", "4", "5"}),
            new Question("Na sua opinião, qual foi o nível de diversão do nível jogado? (número maior significa mais divertido).", new string[]{"1", "2", "3", "4", "5"}),
        };

        Question[] similarityQuestions = new Question[] {
            new Question("Até que ponto o nível gerado se assemelha ao nível manual? (quanto maior, mais difícil de notar a difença).", new string[]{"1", "2", "3", "4", "5"}),
            new Question("Até que ponto você acha que os nível gerado foi gerado por computador? (1 para poucas partes geradas e 5 para totalmente gerado).", new string[]{"1", "2", "3", "4", "5"}),
            new Question("Até que ponto você notou que o nível gerado foi gerado por computador? (5 para caso tenha notado que os níveis foram gerados).", new string[]{"1", "2", "3", "4", "5"}),
        };

        Question[] turingTest = new Question[] {
            new Question("Qual nível você acredita que foi gerado por computador?", new string[] { "1ᵒ nível jogado" , "2ᵒ nível jogado"})
        };

        buttonList = new List<GameObject>();
        questions = new Dictionary<QuestionsType, Question[]>();
        questions.Add(QuestionsType.PROFILE, profileQuestions);
        questions.Add(QuestionsType.IMMERSION_MANUAL, immersionQuestions);
        questions.Add(QuestionsType.IMMERSION_GENERATED, immersionQuestions);
        questions.Add(QuestionsType.SIMILARITY, similarityQuestions);
        questions.Add(QuestionsType.TURING_TEST, turingTest);
        
        if( ScenesState.formAnswers == null){
            ScenesState.formAnswers = new Dictionary<QuestionsType, List<string>>();
            ScenesState.formAnswers.Add(QuestionsType.PROFILE, new List<string>());
            ScenesState.formAnswers.Add(QuestionsType.IMMERSION_MANUAL, new List<string>());
            ScenesState.formAnswers.Add(QuestionsType.IMMERSION_GENERATED, new List<string>());
            ScenesState.formAnswers.Add(QuestionsType.SIMILARITY, new List<string>());
            ScenesState.formAnswers.Add(QuestionsType.TURING_TEST, new List<string>());
        }
    }

    void Start()
    {
        Debug.Log(ScenesState.nextQuestionType);
        SetQuestions(ScenesState.nextQuestionType);
        Next();
    }

    void SetQuestions(QuestionsType newQuestionsType)
    {
        questionsType = newQuestionsType;
        index = -1;
    }

    void Next(string currentAnswer = "")
    {
        if (currentAnswer.Length > 0)
        {
            if (questionsType == QuestionsType.TURING_TEST)
            {
                if ((currentAnswer == "1ᵒ nível jogado" && ScenesState.playGeneratedLevelFirst) || (currentAnswer == "2ᵒ nível jogado" && !ScenesState.playGeneratedLevelFirst))
                {
                    ScenesState.formAnswers[questionsType].Add("Acertou");
                }
                else
                {
                    ScenesState.formAnswers[questionsType].Add("Errou");
                }
            } else {
                ScenesState.formAnswers[questionsType].Add(currentAnswer);
            }
        }

        index++;
        //No more questions
        if (index >= questions[questionsType].Length)
        {
            ScenesState.steps.Next();
            return;
        }

        tmPro.text = questions[questionsType][index].description;
        InstantiateButtons(questions[questionsType][index].options);
    }

    void InstantiateButtons(string[] options)
    {
        foreach (GameObject button in buttonList)
        {
            Destroy(button);
        }
        buttonList.Clear();

        float buttonLength = 100;
        float buttonSpacing = 10;
        float increment = buttonLength + buttonSpacing;
        float originPos = ((options.Length * increment) - (buttonSpacing + increment)) * -0.5f;

        foreach (string option in options)
        {
            GameObject button = Instantiate(buttonPrefab);
            button.transform.SetParent(canvas.transform);
            RectTransform btnTransform = button.transform as RectTransform;
            btnTransform.anchorMax = new Vector2(0.5f, 0.5f);
            btnTransform.anchorMin = new Vector2(0.5f, 0.5f);
            btnTransform.pivot = new Vector2(0.5f, 0.5f);
            btnTransform.sizeDelta = new Vector2(buttonLength, 100);
            btnTransform.anchoredPosition = new Vector2(originPos, 0);

            TextMeshProUGUI textMesh = button.GetComponentInChildren<TextMeshProUGUI>();
            textMesh.text = option;

            Button btnComponent = button.GetComponent<Button>();
            btnComponent.onClick.AddListener(() => this.Next(option));

            originPos += increment;
            buttonList.Add(button);
        }
    }
}