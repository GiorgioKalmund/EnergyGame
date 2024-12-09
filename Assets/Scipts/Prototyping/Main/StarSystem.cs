using UnityEngine;
using UnityEngine.UI;


public class StarSystem : MonoBehaviour
{
    public GameObject endScreen;

    public Image[] stars;
    private int score = 50;
    private float MaxDemand;
    private float currentProduction;
    void Start()
    {
        if (!endScreen)
        {
            Debug.LogWarning("No star panel found in star sytem, aborting creation of it.");
            return;
        }
        MaxDemand = LevelManager.Instance.GetCurrentDemand();
        endScreen.SetActive(false);
        

    }
    private void Update()
    {
        if (!endScreen)
        {
            return;
        }
        currentProduction = LevelManager.Instance.GetCurrentProduction();
        if(currentProduction >= MaxDemand)
        {
            endScreen.SetActive(true);
        }
        
    }
    void ShowStarPanel()
    {
        if (!endScreen)
        {
            return;
        } 
        endScreen.SetActive(true);

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
