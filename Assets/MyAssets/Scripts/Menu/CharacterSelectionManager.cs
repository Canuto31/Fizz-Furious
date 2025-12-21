using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectionManager : MonoBehaviour
{
    [Header("Character Prefabs")]
    public GameObject[] characterPrefabs;

    [Header("Spawn Points")]
    public Transform player1Spawn;
    public Transform player2Spawn;

    [Header("Rotación visual")]
    public float rotationY_P1;
    public float rotationY_P2;

    public float selectionScale;

    private int p1Index = 0;
    private int p2Index = 0;

    private GameObject p1Model;
    private GameObject p2Model;

    private bool p1Locked = false;
    private bool p2Locked = false;

    public string selectedCharacterP1;
    public string selectedCharacterP2;

    public CanvasGroup startButtonCanvasGroup;

    public AudioClip moveSound;
    public AudioClip confirmSound;
    public AudioClip startMatchSound;

    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration;

    public GameObject uiElementsToHide;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnCharacters();
        startButtonCanvasGroup.alpha = 0.5f;
        startButtonCanvasGroup.interactable = false;
        startButtonCanvasGroup.blocksRaycasts = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!p1Locked)
        {
            if (Input.GetKeyDown(KeyCode.A)) { ChangeCharacter(1, -1); SFXManagerSelection.instance.PlaySFX(moveSound); }
            if (Input.GetKeyDown(KeyCode.D)) { ChangeCharacter(1, 1); SFXManagerSelection.instance.PlaySFX(moveSound); }
            if (Input.GetKeyDown(KeyCode.W)) { LockCharacter(1); SFXManagerSelection.instance.PlaySFX(confirmSound); }
        }

        if (!p2Locked)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) { ChangeCharacter(2, -1); SFXManagerSelection.instance.PlaySFX(moveSound); }
            if (Input.GetKeyDown(KeyCode.RightArrow)) { ChangeCharacter(2, 1); SFXManagerSelection.instance.PlaySFX(moveSound); }
            if (Input.GetKeyDown(KeyCode.UpArrow)) { LockCharacter(2); SFXManagerSelection.instance.PlaySFX(confirmSound); }
        }


        if (p1Locked && p2Locked)
        {
            startButtonCanvasGroup.alpha = 1f;
            startButtonCanvasGroup.interactable = true;
            startButtonCanvasGroup.blocksRaycasts = true;
        }
        else
        {
            startButtonCanvasGroup.alpha = 0.5f;
            startButtonCanvasGroup.interactable = false;
            startButtonCanvasGroup.blocksRaycasts = false;
        }
    }

    void ChangeCharacter(int player, int direction)
    {
        if (player == 1)
        {
            Destroy(p1Model);
            p1Index = (p1Index + direction + characterPrefabs.Length) % characterPrefabs.Length;
            p1Model = Instantiate(characterPrefabs[p1Index], player1Spawn.position, Quaternion.identity);
            p1Model.transform.localScale = Vector3.one * selectionScale;
            SetSelectionRotation(p1Model, rotationY_P1);
        }
        else if (player == 2)
        {
            Destroy(p2Model);
            p2Index = (p2Index + direction + characterPrefabs.Length) % characterPrefabs.Length;
            p2Model = Instantiate(characterPrefabs[p2Index], player2Spawn.position, Quaternion.identity);
            p2Model.transform.localScale = Vector3.one * selectionScale;
            SetSelectionRotation(p2Model, rotationY_P2);
        }
    }

    void SpawnCharacters()
    {
        p1Model = Instantiate(characterPrefabs[p1Index], player1Spawn.position, Quaternion.identity);
        p2Model = Instantiate(characterPrefabs[p2Index], player2Spawn.position, Quaternion.identity);

        p1Model.transform.localScale = Vector3.one * selectionScale;
        p2Model.transform.localScale = Vector3.one * selectionScale;

        SetSelectionRotation(p1Model, rotationY_P1);
        SetSelectionRotation(p2Model, rotationY_P2);
    }

    void LockCharacter(int player)
    {
        if (player == 1)
        {
            p1Locked = true;
            selectedCharacterP1 = characterPrefabs[p1Index].name;
        }
        else if (player == 2)
        {
            p2Locked = true;
            selectedCharacterP2 = characterPrefabs[p2Index].name;
        }
    }

    public void StartMatch()
    {
        StartCoroutine(StartMatchRoutine());
    }

    IEnumerator StartMatchRoutine()
    {
        SFXManagerSelection.instance.PlaySFX(startMatchSound);

        if (uiElementsToHide != null)
        {
            uiElementsToHide.SetActive(false);
        }

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            yield return null;
        }

        SceneManager.LoadScene("FirstScene");
    }


    void SetSelectionRotation(GameObject model, float yRotation)
    {
        model.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
