using UnityEngine;

public class HideIfNotInSameRoom : MonoBehaviour
{
    private GameObject player;
    private SectorManager sm;

    void Start()
    {
        sm = GameObject.Find("GameManager").GetComponent<SectorManager>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        gameObject.SetActive(sm.SectorDistanceOf(player.transform.position, gameObject.transform.position) > 1);
    }
}
