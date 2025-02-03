using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    [Header("Menu")]
    [SerializeField] private Image blackout;
    public string levelSelectionSceneName = "Level Selection Map";
    public string creditSceneName = "Credits";

    [SerializeField] private TMP_Text userIdText; 
    [SerializeField] private Button resetUserIdButton; 
    public string userId;

    private async void Start()
    {
        string receivedId = PlayerPrefs.GetString("user_id");
        if (receivedId.Length == 0)
            NewUserId(); 
        else
            userIdText.text = receivedId;
        
        
        if (!blackout)
            Debug.LogWarning("No image connected for blackout!");
        else
            await blackout.DOFade(0f, 1f).AsyncWaitForCompletion();
    }

    public void NewUserId(int length = 5)
    {
        userId = RandomUserID.RandomID(length);
        PlayerPrefs.SetString("user_id", userId);
        userIdText.text = userId;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(levelSelectionSceneName);
    }

    public void ShowCredits()
    {
        SceneManager.LoadScene(creditSceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    
    private void OnDestroy()
    {
        DOTween.KillAll();
    }

    void Update(){
        if(Input.GetKey(KeyCode.RightControl)){
            if(Input.GetKey(KeyCode.L)){
                PlayerPrefs.DeleteAll();
                NewUserId();
            }
        }
    }
}