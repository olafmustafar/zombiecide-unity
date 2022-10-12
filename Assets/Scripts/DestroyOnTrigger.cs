using UnityEngine;

public class DestroyOnTrigger : MonoBehaviour
{
    public string sound;
    public GameObject onDestroyEffect;

    void OnTriggerEnter(Collider collider)
    {
        FindObjectOfType<AudioManager>().Play(sound);

        if ( onDestroyEffect ) {
            GameObject effect = Instantiate(onDestroyEffect, gameObject.transform.position, Quaternion.identity);
            Destroy(effect, 5.0f);
        }
        
        Destroy(gameObject);
    }
}
