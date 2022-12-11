using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShowGeneratedManager : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public Image imageManual;
    public Image imageGenerated;
    public List<Sprite> spriteListManual;
    public List<Sprite> spriteListGenerated;

    void Start(){
        title.text = $"{ScenesState.formAnswers[QuestionsType.TURING_TEST][0]}!";
        description.text = $"O {( ScenesState.playGeneratedLevelFirst ? "primeiro" : "segundo" )} nível foi gerado por computador!";

        imageManual.sprite = spriteListManual[ScenesState.manualIndex];
        imageGenerated.sprite = spriteListGenerated[ScenesState.generatedIndex];
    }

    public void Next(){
        ScenesState.steps.Next();
    }
}
