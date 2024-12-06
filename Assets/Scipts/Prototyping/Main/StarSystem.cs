using UnityEngine;
using UnityEngine.UI;


public class StarSystem : MonoBehaviour
{
    public GameObject starPanel; 
    public Button startGameButton; 
    public Image[] stars;
    private int score = 50;
    
    void Start()
    {
        if (!starPanel)
        {
            Debug.LogWarning("No star panel found in star sytem, aborting creation of it.");
            return;
        }
        
        starPanel.SetActive(false);
        startGameButton.onClick.AddListener(ShowStarPanel);

    }
    void ShowStarPanel()
    {
        if (!starPanel)
        {
            return;
        } 
        starPanel.SetActive(true);

        int starsToShow = CalculateStars(score);

        
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].enabled = i < starsToShow; 
        }
    }

    int CalculateStars(int score)
    {
        // TODO: Get score (potential from the current LevelManager) and work with that
        if (score >= 80) return 3;
        if (score >= 50) return 2;
        if (score > 0) return 1;
        return 0;
    }


}
