using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public CanvasGroup countDownGroup;
    public TextMeshProUGUI countDownText;
    public AudioClip countDownVO;
    public AudioSource audioSource;

    [SerializeField] private PlayerController[] players;

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
        foreach (var player in players)
        {
            player.DisableControl();
        }

        StartCoroutine(StartCoundown());
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

        winner.Celebrate();

        Time.timeScale = 0.6f;
    }
    
    private void DisableLoserCameras(PlayerController winner)
    {
        foreach (var player in players)
        {
            if (player == null || player == winner) continue;

            Camera cam = player.GetComponentInChildren<Camera>();
            if (cam != null)
            {
                cam.gameObject.SetActive(false);
                Destroy(cam.gameObject);
            }
        }
    }

    public IEnumerator StartCoundown()
    {
        countDownGroup.alpha = 1;
        countDownText.text = "";
        audioSource.PlayOneShot(countDownVO);

        string[] steps = { "3", "2", "1", "FIGHT!" };
        foreach (var step in steps)
        {
            countDownText.text = step;
            yield return new WaitForSeconds(1f);
        }

        countDownGroup.alpha = 0;

        foreach (var player in players)
            player.EnableControl();
    }

}
