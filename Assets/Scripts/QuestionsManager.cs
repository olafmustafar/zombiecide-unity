using System.Collections;
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
    public bool isOptions { get; set; }

    public Question(string description)
    {
        this.description = description;
        this.options = new string[] { };
        this.isOptions = false;

    }

    public Question(string description, string[] options)
    {
        this.description = description;
        this.options = options;
        this.isOptions = true;
    }
}

public class QuestionsManager : MonoBehaviour
{
    public GameObject buttonPrefab;
    public GameObject canvas;
    public GameObject inputfieldPrefab;
    public TextMeshProUGUI uiDescription;
    public TextMeshProUGUI uiTitle;
    bool loadingActive;

    QuestionsType questionsType;
    int index;

    Dictionary<QuestionsType, Question[]> questions;

    List<GameObject> instantiatedObjects;

    public QuestionsManager()
    {
        Question[] profileQuestions = new Question[]{
            new Question("Qual a sua idade?", new string[]{"9 ou menos", "10 a 20", "21 a 30", "31 a 40", "41 a 50", "51 a 60", "60 ou mais"}),
            new Question("Quantos anos de experiência você tem como gamer?", new string[]{"0", "1", "2", "3", "4 ou mais"}),
            new Question("Em qual estado você mora?"),
        };

        Question[] immersionQuestions = new Question[]{
            new Question("Na sua opinião, qual foi o nível de dificuldade do nível jogado? (número maior significa mais difícil).", new string[]{"1", "2", "3", "4", "5"}),
            new Question("Na sua opinião, qual foi o nível de imersão do nível jogado? (número maior significa mais imersivo).", new string[]{"1", "2", "3", "4", "5"}),
            new Question("O quanto você gostou de jogar o nível apresentado? (número maior significa mais divertido).", new string[]{"1", "2", "3", "4", "5"}),
            new Question("Qual nota você daria para o balanceamento e distribuição dos inimigos?", new string[]{"1", "2", "3", "4", "5"}),
            new Question("Qual nota você daria para o comportamento dos inimigos dentro do jogo?", new string[]{"1", "2", "3", "4", "5"}),
            new Question("Qual nota você daria para o layout do nível jogado?", new string[]{"1", "2", "3", "4", "5"}),
        };

        Question[] similarityQuestions = new Question[] {
            new Question("Até que ponto o nível gerado se assemelha ao nível manual? (quanto maior, mais difícil de notar a diferença).", new string[]{"1", "2", "3", "4", "5"}),
            new Question("Até que ponto você pensa que o nível gerado foi construído por computador? (1 para pouco e 5 para totalmente).", new string[]{"1", "2", "3", "4", "5"}),
            new Question("O quanto você notou que o nível gerado foi construído por computador? (1 caso não tenha notado, 5 para notou totalmente).", new string[]{"1", "2", "3", "4", "5"}),
        };

        Question[] turingTest = new Question[] {
            new Question("Qual nível você acredita que foi gerado por computador?", new string[] { "1ᵒ nível jogado" , "2ᵒ nível jogado"})
        };

        instantiatedObjects = new List<GameObject>();
        questions = new Dictionary<QuestionsType, Question[]>();
        questions.Add(QuestionsType.PROFILE, profileQuestions);
        questions.Add(QuestionsType.IMMERSION_MANUAL, immersionQuestions);
        questions.Add(QuestionsType.IMMERSION_GENERATED, immersionQuestions);
        questions.Add(QuestionsType.SIMILARITY, similarityQuestions);
        questions.Add(QuestionsType.TURING_TEST, turingTest);

        if (ScenesState.formAnswers == null)
        {
            ScenesState.formAnswers = new Dictionary<QuestionsType, List<string>>();
            ScenesState.formAnswers.Add(QuestionsType.PROFILE, new List<string>());
            ScenesState.formAnswers.Add(QuestionsType.IMMERSION_MANUAL, new List<string>());
            ScenesState.formAnswers.Add(QuestionsType.IMMERSION_GENERATED, new List<string>());
            ScenesState.formAnswers.Add(QuestionsType.SIMILARITY, new List<string>());
            ScenesState.formAnswers.Add(QuestionsType.TURING_TEST, new List<string>());
        }
    }

    IEnumerator SendAnswers()
    {
        InstantiateButtons(new string[] { });
        List<string> data = new List<string>();
        data.Add(System.DateTime.Now.ToString());
        data.AddRange(ScenesState.formAnswers[QuestionsType.PROFILE]);
        data.AddRange(ScenesState.formAnswers[QuestionsType.IMMERSION_MANUAL]);
        data.AddRange(ScenesState.formAnswers[QuestionsType.IMMERSION_GENERATED]);
        data.AddRange(ScenesState.formAnswers[QuestionsType.TURING_TEST]);
        data.AddRange(ScenesState.formAnswers[QuestionsType.SIMILARITY]);
        startLoading();
        yield return SheetsApi.SendDataSheet(data.ToArray());
        stopLoading();
        uiDescription.text = "Obrigado por jogar, por hoje é só!";
        uiTitle.text = "Obrigado!";

    }

    void Start()
    {
        if (ScenesState.nextQuestionType == QuestionsType.END_MESSAGE)
        {
            StartCoroutine(SendAnswers());
            return;
        }
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
            }
            else
            {
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

        uiDescription.text = questions[questionsType][index].description;
        if (questions[questionsType][index].isOptions)
        {
            InstantiateButtons(questions[questionsType][index].options);
        }
        else
        {
            InstantiateTextfield();
        }
    }

    void InstantiateTextfield()
    {
        foreach (GameObject objects in instantiatedObjects)
        {
            Destroy(objects);
        }
        instantiatedObjects.Clear();

        GameObject inputfield = Instantiate(inputfieldPrefab);
        inputfield.transform.SetParent(canvas.transform);
        RectTransform ifTransform = inputfield.transform as RectTransform;
        ifTransform.anchorMax = new Vector2(0.5f, 0.5f);
        ifTransform.anchorMin = new Vector2(0.5f, 0.5f);
        ifTransform.pivot = new Vector2(0.5f, 0.5f);
        ifTransform.anchoredPosition = new Vector2(0f, 0f);
        instantiatedObjects.Add(inputfield);

        TMP_InputField tmpInputField = inputfield.GetComponent<TMP_InputField>();
        tmpInputField.contentType = TMP_InputField.ContentType.Name;
        tmpInputField.onSubmit.AddListener((string text) =>
        {
            if (text.Length > 3)
            {
                this.Next(text);
            }
            else
            {
                uiDescription.text = questions[questionsType][index].description + "\nO nome do estado deve conter mais de 3 letras!";
            }
        });

        GameObject button = Instantiate(buttonPrefab);
        button.transform.SetParent(canvas.transform);
        RectTransform btnTransform = button.transform as RectTransform;
        btnTransform.anchorMax = new Vector2(0.5f, 0.5f);
        btnTransform.anchorMin = new Vector2(0.5f, 0.5f);
        btnTransform.pivot = new Vector2(0.5f, 0.5f);
        btnTransform.sizeDelta = ifTransform.sizeDelta;
        btnTransform.anchoredPosition = new Vector2(0f, -ifTransform.sizeDelta.y);
        instantiatedObjects.Add(button);

        Button btnComponent = button.GetComponent<Button>();
        btnComponent.onClick.AddListener(() => tmpInputField.onSubmit.Invoke(tmpInputField.text));
    }

    void InstantiateButtons(string[] options)
    {
        foreach (GameObject button in instantiatedObjects)
        {
            Destroy(button);
        }
        instantiatedObjects.Clear();

        float buttonLength = 200;
        float buttonHeight = 100;
        float spacing = 10;
        Vector2 originPos = new Vector2(0, 0);
        Vector2 increment = new Vector2(buttonLength + spacing, buttonHeight + spacing);

        originPos.x = ((Mathf.Min(options.Length, 3) * increment.x) - (spacing + increment.x)) * -0.5f;
        if (options.Length > 3)
        {
            originPos.y = (increment.y - spacing) * 0.5f;
        }
        int i = 0;

        foreach (string option in options)
        {
            GameObject button = Instantiate(buttonPrefab);
            button.transform.SetParent(canvas.transform);
            RectTransform btnTransform = button.transform as RectTransform;
            btnTransform.anchorMax = new Vector2(0.5f, 0.5f);
            btnTransform.anchorMin = new Vector2(0.5f, 0.5f);
            btnTransform.pivot = new Vector2(0.5f, 0.5f);
            btnTransform.sizeDelta = new Vector2(buttonLength, buttonHeight);
            btnTransform.anchoredPosition = originPos;

            TextMeshProUGUI textMesh = button.GetComponentInChildren<TextMeshProUGUI>();
            textMesh.text = option;

            Button btnComponent = button.GetComponent<Button>();
            btnComponent.onClick.AddListener(() => this.Next(option));

            originPos.x += increment.x;
            if (i > 0 && (((i + 1) % 3) == 0))
            {
                originPos.x = ((Mathf.Min(options.Length - 1 - i, 3) * increment.x) - (spacing + increment.x)) * -0.5f;
                originPos.y -= increment.y;
            }
            i++;
            instantiatedObjects.Add(button);
        }
    }

    void startLoading()
    {
        loadingActive = true;
        StartCoroutine(RenderLoadingText());
    }

    void stopLoading()
    {
        loadingActive = false;
    }

    IEnumerator RenderLoadingText()
    {
        string loading = "Aguarde";
        string[] dots = { ".", "..", "..." };

        int i = 0;
        while (loadingActive)
        {
            uiTitle.text = loading + dots[i % dots.Length];
            uiDescription.text = "";
            i++;
            yield return new WaitForSeconds(1f);
        }

        yield return null;
    }
}
