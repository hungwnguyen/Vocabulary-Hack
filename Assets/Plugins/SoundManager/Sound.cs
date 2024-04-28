using System.Collections;
using UnityEngine;

public class Sound : MonoBehaviour, IPooledObject
{
    private AudioSource audioSource;
    
    public void OnObjectSpawn()
    {
        this.audioSource = this.GetComponent<AudioSource>();
        if (audioSource.isPlaying){
            StopAllCoroutines();
            audioSource.Stop();
        }
        audioSource.Play();
        DeactivateGameObject();
    }

    public void DeactivateGameObject()
    {
        StartCoroutine(deactivateGameObject());
    }

    IEnumerator deactivateGameObject(){
        yield return new WaitForSeconds(audioSource.clip.length);
        gameObject.SetActive(false);
    }
}
