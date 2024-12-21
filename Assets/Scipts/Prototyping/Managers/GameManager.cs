using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    #if UNITY_WEBPLAYER
        public static string webplayerQuitURL = "https://giorgiokalmund.itch.io/energy-game-prototype";
    #endif

    public static void OpenSpecificLevel(int levelId)
    {
        string levelName = "level " + levelId;
        SceneManager.LoadScene(levelName);
    }
    public static void LoadSceneById(int levelId)
    {
        SceneManager.LoadScene(levelId);
    }

   public static void ReloadEntireScene()
   {
        string currentSceneName = SceneManager.GetActiveScene().name;

        SceneManager.LoadScene(currentSceneName);
   }

   public static void LoadSceneByIdAsync(int sceneIndex)
   {
      SceneManager.LoadSceneAsync(sceneIndex);
   }
   
   public static void Quit()
   {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_WEBPLAYER
                Application.OpenURL(webplayerQuitURL);
        #else
                Application.Quit();
        #endif
   }
    
    
}
