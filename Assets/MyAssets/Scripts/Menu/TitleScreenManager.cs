using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    public CanvasGroup fadeCanvas;
    public float fadeDuration = 1.0f;
    public string sceneToLoad = "MainMenu";
    
    public AudioSource blipSound;
    public AudioSource voiceOver;

    private bool isFading = false;
    public Transform pressEnterText;

    private void Update()
    {
        if (!isFading && Input.anyKeyDown)
        {
            StartCoroutine(FadeAndLoadScene());
        }
    }

    System.Collections.IEnumerator FadeAndLoadScene()
    {
        isFading = true;
         if (blipSound != null)
            blipSound.Play();
         if (voiceOver != null)
            voiceOver.Play();

         AnimatePressText();

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }

        SceneManager.LoadScene(sceneToLoad);
    }

    public void AnimatePressText() 
    {
        if (pressEnterText != null) 
        {
            StartCoroutine(ScaleDown());
        }
    
    }

    IEnumerator ScaleDown()
    {
        float t = 0f;

        Vector3 startScale = pressEnterText.localScale;
        Vector3 endScale = Vector3.zero;

        while (t < 1f) 
        {
            t += Time.deltaTime; 
            pressEnterText.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

    }
}
