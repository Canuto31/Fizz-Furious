using System;
using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
    [SerializeField] private float healAmount = 30f;
    [SerializeField] private float maxHealthReduction = 10f;
    
    [Header("Lifetime")]
    [SerializeField] private float lifeTime = 5f;

    public float floatAmplitude;
    public float floatFrequency;
    public float rotationSpeed;

    public AudioClip pickUpSound;

    private Vector3 startPosition;
    private Vector3 randomRotationAxis;


    private void Start()
    {
        startPosition = transform.position;

        randomRotationAxis = new Vector3(
            UnityEngine.Random.Range(-0.2f, 0.2f), 1f,
            UnityEngine.Random.Range(-0.2f, 0.2f)
        ).normalized;

        Destroy(gameObject, lifeTime);
    }

    public void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = startPosition + new Vector3(0, yOffset, 0);

        transform.Rotate(randomRotationAxis, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            player.Heal(healAmount, maxHealthReduction);

            if (pickUpSound != null)
            {
                AudioSource.PlayClipAtPoint(pickUpSound, transform.position);
                Destroy(gameObject);
            }

            
        }
    }
}
