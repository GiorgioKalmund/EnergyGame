using System;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    [SerializeField] private Image blackout;
    
    [Header("Credits")]
    [SerializeField] private GameObject scrollingElement;
    public float startDelay;
    public float durationMultiplier;
    public Ease animationEase;

    [Header("Main Menu")] 
    public string mainMenuSceneName = "Main Menu";
    
    private async void Start()
    {
        int childCount = scrollingElement.transform.childCount;
        float firstChildPos = scrollingElement.transform.GetChild(0).transform.position.y;
        float lastChildPos = scrollingElement.transform.GetChild(childCount - 1).transform.position.y;
        float targetY = 1.6f * (scrollingElement.transform.position.y + Mathf.Abs(lastChildPos - firstChildPos));

        scrollingElement.transform
            .DOLocalMoveY(targetY, durationMultiplier * scrollingElement.transform.childCount)
            .SetEase(animationEase)
            .SetDelay(startDelay);
        if (!blackout)
            Debug.LogWarning("No image connected for blackout!");
        else
            await blackout.DOFade(0f, 1f).AsyncWaitForCompletion();
    }

    public void ShowMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void OnDestroy()
    {
        DOTween.KillAll();
    }
}