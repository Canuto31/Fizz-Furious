using UnityEngine;

public class FaceSwitcherMenu : MonoBehaviour
{
    public Texture openEyesTexture;
    public Texture closedEyesTexture;
    public float blinkDuration;
    public float maxBlinkInterval;
    public float minBlinkInterval;

    private Renderer rend;
    private float blinkTimer;
    private bool isBlinking;

    void Start()
    {
      rend = GetComponent<Renderer>();
      rend.material.mainTexture = openEyesTexture;
      ResetBlinkTimer();
    }

    void Update()
    {
        blinkTimer -= Time.deltaTime;
        if (!isBlinking && blinkTimer <= 0f)
        {
            StartCoroutine(Blink());
        }
    }

    System.Collections.IEnumerator Blink()
    {
        isBlinking = true;
        rend.material.mainTexture = closedEyesTexture;
        yield return new WaitForSeconds(blinkDuration);
        rend.material.mainTexture = openEyesTexture;
        isBlinking = false;
        ResetBlinkTimer();
    }

    void ResetBlinkTimer()
    {
        blinkTimer = Random.Range(minBlinkInterval, maxBlinkInterval);
    }

}
