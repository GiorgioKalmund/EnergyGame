using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SettingsManager : MonoBehaviour
{
   public static SettingsManager Instance { get; private set; }

   [Header("GameObjects")] 
   [SerializeField] private GameObject settingsPanel;

   public bool settingsOpen;

   private void Awake()
   {
      if (Instance && Instance != this)
      {
         Destroy(this.gameObject);
      }
      else
      {
         Instance = this;
      }
      
      settingsPanel.SetActive(false);
      settingsOpen = false;
   }


   public void ToggleSettingsPanel(bool freezeTime)
   {
      settingsPanel.SetActive(!settingsPanel.activeSelf);
      Time.timeScale = Time.timeScale.Equals(1f) && freezeTime ? 0f : 1f;
      settingsOpen = !settingsOpen;
   }

   public void ReloadEntireScene()
   {
      GameManager.ReloadEntireScene();
   }

   public void LoadSceneByIdAsync(int sceneIndex)
   {
      GameManager.LoadSceneByIdAsync(sceneIndex); 
   }

   public void Quit()
   {
      GameManager.Quit();
   }

}