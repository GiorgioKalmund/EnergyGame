using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;


public class SettingsManager : MonoBehaviour
{
   public static SettingsManager Instance { get; private set; }

   [Header("GameObjects")] 
   [SerializeField] private GameObject settingsPanel;
   [SerializeField] private GameObject controlsGameObject;

   [SerializeField] private List<SettingsScreenButton> optionButtons = new List<SettingsScreenButton>();

   [Header("Info")] 
   [SerializeField] private string mapSceneName;

   [Header("Confirmation")] 
   [SerializeField] private GameObject confirmationPrompt;
   [SerializeField] private TMP_Text confirmationText;
   [SerializeField] private Button cancelButton;
   [SerializeField] private Button confirmationButton;
   [SerializeField] private string backToMapText = "Zurück zur Karte?";
   [SerializeField] private string restartText = "Runde neu starten?";

   private bool settingsOpen;
   private bool confirmationOpen;

   private void Awake()
   {
      if (Instance && Instance != this)
      {
         Destroy(gameObject);
      }
      else
      {
         Instance = this;
      }
      
      settingsPanel.SetActive(false);
      settingsOpen = false;
   }

   private void Update()
   {
      if (Input.GetKeyDown(KeyCode.Backspace))
      {
         ToggleSettingsPanel();
      }
   }

   private void Start()
   {
      // Set up options buttons
      // Main menu / map button
      var backToMapButton = optionButtons[0];
      backToMapButton.GetButton().onClick.AddListener(AskForMapConfirmation);
      
      // Restart button
      var restartButton = optionButtons[1];
      restartButton.GetButton().onClick.AddListener(AskForRestartConfirmation);
      
      // Controls help button
      var controlsButton = optionButtons[2];
      controlsButton.GetButton().onClick.AddListener(ToggleControlsScreen);
      
      // Cancel button
      cancelButton.onClick.AddListener(HideConfirmation);
      HideConfirmation();
   }


   public void ToggleSettingsPanel()
   {
      settingsPanel.SetActive(!settingsPanel.activeSelf);
      settingsOpen = !settingsOpen;
      
      controlsGameObject.SetActive(false);
      HideConfirmation();
   }

   private void ToggleControlsScreen()
   {
      if (confirmationOpen)
         HideConfirmation();
      
      controlsGameObject.SetActive(!controlsGameObject.activeSelf);
   }

   private void BackToMap()
   {
      SceneManager.LoadScene(mapSceneName);
      //SceneManager.LoadSceneAsync(mapSceneName);
   }

   private void ReloadScene()
   {
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
   }

   public void AskForMapConfirmation()
   {
      Debug.Log("M: " + confirmationText.text);
      if (confirmationText.text.Equals(backToMapText))
      {
         HideConfirmation();
         return;
      }
      
      if (controlsGameObject.activeSelf)
         controlsGameObject.SetActive(false);
      
      confirmationOpen = true;
      confirmationPrompt.SetActive(confirmationOpen);
      confirmationText.text = backToMapText;
      confirmationButton.onClick.RemoveAllListeners();
      confirmationButton.onClick.AddListener(BackToMap);
   }
   
   public void AskForRestartConfirmation()
   {
      Debug.Log("R: " + confirmationText.text);
      if (confirmationText.text.Equals(restartText))
      {
         HideConfirmation();
         return;
      }
      
      if (controlsGameObject.activeSelf)
         controlsGameObject.SetActive(false);
      
      confirmationOpen = true;
      confirmationPrompt.SetActive(confirmationOpen);
      confirmationText.text = restartText;
      confirmationButton.onClick.RemoveAllListeners();
      confirmationButton.onClick.AddListener(ReloadScene);
   }

   private void HideConfirmation()
   {
      confirmationOpen = false;
      confirmationPrompt.SetActive(confirmationOpen);
      confirmationText.text = "";
   }
}