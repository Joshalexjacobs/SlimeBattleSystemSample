using UnityEngine;

public class SoundManager : MonoBehaviour {

  public AudioSource menuSelect;
  
  public AudioSource hit;
    
  public AudioSource miss;

  public AudioSource castSpell;
  
  public AudioSource run;

  public AudioSource battleOver;
  
  public AudioSource battleMusic;

  public void PlaySound(AudioSource audioSource) {
    audioSource.Play();
  }

  public void StopSound(AudioSource audioSource) {
    audioSource.Stop();
  }

}
