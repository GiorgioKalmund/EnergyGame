using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Menu")]
    [SerializeField] private Image blackout;
    public string levelSelectionSceneName = "Level Selection Map";
    public string creditSceneName = "Credits";

    [SerializeField] private TMP_Text userIdText; 
    public string userId;
    
    private async void Start()
    {
        string receivedId = PlayerPrefs.GetString("user_id");
        if (receivedId.Length == 0)
        {
            userId = RandomUserID.RandomID();
            PlayerPrefs.SetString("user_id", userId);
            userIdText.text = userId;
        }
        else
        {
            userIdText.text = receivedId;
        }
        
        
        if (!blackout)
            Debug.LogWarning("No image connected for blackout!");
        else
            await blackout.DOFade(0f, 1f).AsyncWaitForCompletion();
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene(levelSelectionSceneName);
    }

    public void ShowCredits()
    {
        SceneManager.LoadScene(creditSceneName);
    }
    
    private void OnDestroy()
    {
        DOTween.KillAll();
    }
}