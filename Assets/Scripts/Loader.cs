using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{

    public GameObject gameManager;

    void Awake(){
        if( GameManager.instance == null ){
            GameObject instance = Instantiate( gameManager );
            instance.name = "GameManager";
        }
    }

}
