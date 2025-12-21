using UnityEngine;

public class TVlightFlicker : MonoBehaviour
{
    public Light tvLight;
    public float flickerSpeed;
    public float minIntensity;
    public float maxIntensity;
    public float flickerInterval;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= flickerInterval)
        {
            tvLight.intensity = Random.Range(minIntensity, maxIntensity);
            timer = 0f;
        }
    }
}
