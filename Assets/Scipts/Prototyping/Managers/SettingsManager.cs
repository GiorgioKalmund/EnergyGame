﻿using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


public class SettingsManager : MonoBehaviour
{
   public static SettingsManager Instance { get; private set; }

   [Header("GameObjects")] 
   [SerializeField] private GameObject settingsPanel;
   [SerializeField] private GameObject controlsGameObject;
   [SerializeField] private GameObject highScoreGameObject;

   [SerializeField] private List<SettingsScreenButton> optionButtons;

   [Header("Info")] 
   [SerializeField] private string mapSceneName;

   private bool settingsOpen;

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

   private void Update()
   {
      if (Input.GetKeyDown(KeyCode.Backspace))
      {
         ToggleSettingsPanel(false);
      }
   }

   private void Start()
   {
      // Set up options buttons
      // Main menu / map button
      var backToMapButton = optionButtons[0];
      backToMapButton.button.onClick.AddListener(delegate { SceneManager.LoadSceneAsync(mapSceneName);});
      
      // Restart button
      var restartButton = optionButtons[1];
      restartButton.button.onClick.AddListener(delegate { SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);});
      
      // Controls help button
      var controlsButton = optionButtons[2];
      controlsButton.button.onClick.AddListener(ToggleControlsScreen);
      
      // High score button
      var highscoreButton = optionButtons[3];
      highscoreButton.button.onClick.AddListener(ToggleHighscoreScreen);
   }


   public void ToggleSettingsPanel(bool freezeTime)
   {
      settingsPanel.SetActive(!settingsPanel.activeSelf);
      Time.timeScale = Time.timeScale.Equals(1f) && freezeTime ? 0f : 1f;
      settingsOpen = !settingsOpen;
      controlsGameObject.SetActive(false);
      highScoreGameObject.SetActive(false);
   }

   private void ToggleControlsScreen()
   {
      if (highScoreGameObject.activeSelf)
      {
         ToggleHighscoreScreen();
      }
      controlsGameObject.SetActive(!controlsGameObject.activeSelf);
   }
   private void ToggleHighscoreScreen()
   {
      if (controlsGameObject.activeSelf)
      {
         ToggleControlsScreen();
      }
      highScoreGameObject.SetActive(!highScoreGameObject.activeSelf);
   }

}