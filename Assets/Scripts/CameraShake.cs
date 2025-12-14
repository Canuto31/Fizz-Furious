using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

// Script principal de Camera Shake
public class CameraShake : MonoBehaviour
{
    [Header("Referencias")]
    public Transform camaraTransform;

    private Vector3 posicionOriginal;
    private Quaternion rotacionOriginal;
    private Coroutine shakeCoroutine;

    void Start()
    {
        if (camaraTransform == null)
            camaraTransform = transform;

        posicionOriginal = camaraTransform.localPosition;
        rotacionOriginal = camaraTransform.localRotation;
    }

    // Método principal para activar el shake
    public void Shake(float duracion = 0.3f, float magnitud = 0.2f, float frecuencia = 25f)
    {
        // Si ya hay un shake activo, detenerlo
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeCoroutine(duracion, magnitud, frecuencia));
    }

    // Shake con diferentes intensidades predefinidas
    public void ShakePequeno()
    {
        Shake(0.15f, 0.1f, 30f);
    }

    public void ShakeMedio()
    {
        Shake(0.3f, 0.2f, 25f);
    }

    public void ShakeGrande()
    {
        Shake(0.5f, 0.4f, 20f);
    }

    public void ShakeExplosion()
    {
        Shake(0.7f, 0.6f, 15f);
    }

    // Corrutina que realiza el shake
    private IEnumerator ShakeCoroutine(float duracion, float magnitud, float frecuencia)
    {
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < duracion)
        {
            // Calcular el decay (reducción progresiva)
            float porcentajeCompletado = tiempoTranscurrido / duracion;
            float decayActual = 1f - porcentajeCompletado;

            // Generar offset aleatorio con Perlin Noise para movimiento más suave
            float x = (Mathf.PerlinNoise(Time.time * frecuencia, 0f) - 0.5f) * 2f;
            float y = (Mathf.PerlinNoise(0f, Time.time * frecuencia) - 0.5f) * 2f;
            float z = (Mathf.PerlinNoise(Time.time * frecuencia, Time.time * frecuencia) - 0.5f) * 2f;

            Vector3 offset = new Vector3(x, y, z) * magnitud * decayActual;

            // Aplicar el shake a la posición
            camaraTransform.localPosition = posicionOriginal + offset;

            // También rotar ligeramente (opcional)
            Vector3 rotacionOffset = new Vector3(y, x, z) * 2f * decayActual;
            camaraTransform.localRotation = rotacionOriginal * Quaternion.Euler(rotacionOffset);

            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }

        // Restaurar posición y rotación originales
        camaraTransform.localPosition = posicionOriginal;
        camaraTransform.localRotation = rotacionOriginal;
    }

    // Actualizar la posición original (útil si la cámara se mueve)
    public void ActualizarPosicionOriginal()
    {
        posicionOriginal = camaraTransform.localPosition;
        rotacionOriginal = camaraTransform.localRotation;
    }


  
}
