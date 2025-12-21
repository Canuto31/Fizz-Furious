using UnityEngine;

public class RotateTowardsCamera : MonoBehaviour
{
    public Camera mainCamera;

    private void Start()
    {
    }

    private void Update()
    {
        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
        }
    }
}
