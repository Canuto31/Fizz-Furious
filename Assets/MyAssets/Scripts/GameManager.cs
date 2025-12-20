using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController[] players;

    private List<PlayerInput> playersCamera = new List<PlayerInput>();
    [SerializeField]
    private List<Transform> startingPoints;
    [SerializeField]
    private List<LayerMask> playerLayers;

    private PlayerInputManager playerInputManager;

    private void Awake()
    {
        playerInputManager = FindObjectOfType<PlayerInputManager>();
    }


    public void AddPlayer(PlayerInput player)
    {
        playersCamera.Add(player);

        //need to use the parent due to the structure of the prefab
        Transform playerParent = player.transform.parent;
        playerParent.position = startingPoints[playersCamera.Count - 1].position;

        //convert layer mask (bit) to an integer 
        int layerToAdd = (int)Mathf.Log(playerLayers[playersCamera.Count - 1].value, 2);

        //set the layer
        playerParent.GetComponentInChildren<CinemachineCamera>().gameObject.layer = layerToAdd;
        //add the layer
        playerParent.GetComponentInChildren<Camera>().cullingMask |= 1 << layerToAdd;
        //set the action in the custom cinemachine Input Handler
        //playerParent.GetComponentInChildren<InputHandler>().horizontal = player.actions.FindAction("Look");

    }
    private void OnEnable()
    {
        PlayerController.OnPlayerDied += HandlePlayerDied;
    }
    
    private void OnDisable()
    {
        PlayerController.OnPlayerDied -= HandlePlayerDied;
    }

    private void Start()
    {
        players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
    }

    private void HandlePlayerDied(PlayerController deadPlayer)
    {
        foreach (var player in players)
        {
            if (player == null) continue;
            if (player == deadPlayer) continue;
            if (player.IsDead()) continue;

            OnPlayerWin(player);
            break;
        }
    }

    private void OnPlayerWin(PlayerController winner)
    {
        CameraZoomFocus cam = winner.GetComponentInChildren<CameraZoomFocus>();
        cam?.Focus();
        
        DisableLoserCameras(winner);

        Time.timeScale = 0.6f;
    }
    
    private void DisableLoserCameras(PlayerController winner)
    {
        foreach (var player in players)
        {
            if (player == null || player == winner) continue;

            Camera cam = player.GetComponentInChildren<Camera>();
            if (cam != null)
                cam.gameObject.SetActive(false);
        }
    }
}
