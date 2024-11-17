using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;


public class StarSystem : MonoBehaviour
{
    public GameObject starPanel; // Reference to the Star Panel
    public Button startGameButton; // Reference to the Start Game Button
    public Image[] stars; // Array of star images
    private int score = 50; // Temporary fixed score for testing



    void Start()
    {
        starPanel.SetActive(false);

    
        startGameButton.onClick.AddListener(ShowStarPanel);

    }
    void ShowStarPanel()
    {
        
        starPanel.SetActive(true);

        // Determine the number of stars to display based on score
        int starsToShow = CalculateStars(score);

        
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].enabled = i < starsToShow; // Enable stars based on score
        }
    }



    int CalculateStars(int score)
    {
        // Placeholder logic to determine stars
        if (score >= 80) return 3;
        if (score >= 50) return 2;
        if (score > 0) return 1;
        return 0;
    }

}
