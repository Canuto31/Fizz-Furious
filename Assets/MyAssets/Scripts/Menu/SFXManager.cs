using UnityEngine;

public class SFXManager : MonoBehaviour
{
   public static SFXManager Instance;
   public AudioSource sfxSource;
    private void Awake()
    {
         if (Instance == null)
         {
              Instance = this;
              DontDestroyOnLoad(gameObject);
         }
         else
         {
              Destroy(gameObject);
         }
    }

    public void PlaySFX(AudioClip clip)
    {
         if (clip != null)
         {
              sfxSource.PlayOneShot(clip);
        }
    }

}
