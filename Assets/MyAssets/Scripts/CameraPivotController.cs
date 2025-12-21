using System;
using UnityEngine;

public class CameraPivotController : MonoBehaviour
{
    [SerializeField] private Transform player;

    private void LateUpdate()
    {
        Vector3 pos = transform.position;
        pos.x = player.position.x;
        transform.position = pos;
    }
}
