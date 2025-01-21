using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
    [Header("Menu")]
    [SerializeField] private Image blackout;
    public string levelSelectionSceneName = "Level Selection Map";
    public string creditSceneName = "Credits";
    
    private async void Start()
    {
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