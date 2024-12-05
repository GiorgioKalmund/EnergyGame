using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMenu : MonoBehaviour
{
    public void OpenLevel(int levelId)
    {
        string levelName = "level " + levelId;
        SceneManager.LoadScene(levelName);
    }
    public void OpenSceneById(int levelId)
    {
        SceneManager.LoadScene(levelId);
    }
    
}
