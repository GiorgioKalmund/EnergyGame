using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ambience : MonoBehaviour
{
    [Header("Audio Source Menu")]
    [SerializeField] private AudioSource startScreen;
    [SerializeField] private AudioSource bayern;
    [SerializeField] private AudioSource baustelle;
    
    [Header(("Audio Source Level"))]
    [SerializeField] private AudioSource level;
    [SerializeField] private AudioSource wind;
    [SerializeField] private AudioSource sonne;
    [SerializeField] private AudioSource kohle;
    [SerializeField] private AudioSource wasser;

    [Header("Screen")] 
    public bool isStartScreen;

    public bool isBaustell;

    [Header("Music")] 
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioClip mMusic;
    [SerializeField] private AudioClip gMusic;

    private void Start()
    {
        bayern.mute = true;
        baustelle.mute = true;
        wind.mute = true;
        sonne.mute = true;
        kohle.mute = true;
        wasser.mute = true;
        
        if (isStartScreen)
        {
            level.mute = true;
            music.clip = mMusic;
            music.Play();
        }
        else
        {
            startScreen.mute = true;
            music.clip = gMusic;
            music.Play();
        }

        if (PlayerPrefs.GetInt("levels_completed") <= 7 && isBaustell)
        {
            ChangeScreenSound("baustelle");
        } else if (PlayerPrefs.GetInt("levels_completed") > 7 && isBaustell)
        {
            ChangeScreenSound("bayern");
        }
    }

    public void ChangeScreenSound(string screen)
    {
        if (screen.Equals("start"))
        {
            bayern.mute = true;
            baustelle.mute = true;
        }else if (screen.Equals("bayern"))
        {
            bayern.mute = false;
            baustelle.mute = true;
            startScreen.mute = true;
        }else if (screen.Equals("baustelle"))
        {
            bayern.mute = true;
            baustelle.mute = false;
            startScreen.mute = true;
        }
    }


    public void ChangeLevelSound(string overlay)
    {
        if (overlay.Equals("none"))
        {
            wind.mute = true;
            sonne.mute = true;
            kohle.mute = true;
            wasser.mute = true;
        }else if (overlay.Equals("sonne"))
        {
            wind.mute = true;
            sonne.mute = !sonne.mute;
            kohle.mute = true;
            wasser.mute = true;
        }else if (overlay.Equals("sonne2"))
        {
            wind.mute = true;
            sonne.mute = false;
            kohle.mute = true;
            wasser.mute = true;
        }else if (overlay.Equals("wind"))
        {
            wind.mute = !wind.mute;
            sonne.mute = true;
            kohle.mute = true;
            wasser.mute = true;
        }else if (overlay.Equals("wind2"))
        {
            wind.mute = false;
            sonne.mute = true;
            kohle.mute = true;
            wasser.mute = true;
        }else if (overlay.Equals("wasser"))
        {
            wind.mute = true;
            sonne.mute = true;
            kohle.mute = true;
            wasser.mute = !wasser.mute;
        }else if (overlay.Equals("wasser2"))
        {
            wind.mute = true;
            sonne.mute = true;
            kohle.mute = true;
            wasser.mute = false;
        }else if (overlay.Equals("kohle"))
        {
            wind.mute = true;
            sonne.mute = true;
            kohle.mute = !kohle.mute;
            wasser.mute = true;
        }else if (overlay.Equals("kohle2"))
        {
            wind.mute = true;
            sonne.mute = true;
            kohle.mute = false;
            wasser.mute = true;
        }
    }
}
