using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SFX : MonoBehaviour
{

    [Header("Audio Source")] 
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioSource _powerPlantSource;
    [SerializeField] private Ambience ambience;

    [Header("SFX UI")] 
    [SerializeField] private AudioClip start;

    [SerializeField] private AudioClip back;
    [SerializeField] private AudioClip click;

    [SerializeField] private AudioClip buildMenu1;
    [SerializeField] private AudioClip buildMenu2;

    [SerializeField] private AudioClip level1;
    [SerializeField] private AudioClip level2;
    private bool isBuildMenu;
    private bool isOverlay;

    [Header("SFX Cable")] 
    [SerializeField] private AudioClip cableStart;
    [SerializeField] private AudioClip cableEnd;

    [Header("SFX Power Plants")] 
    [SerializeField] private AudioClip atom;
    [SerializeField] private AudioClip gas;
    [SerializeField] private AudioClip wind;
    [SerializeField] private AudioClip photo;
    [SerializeField] private AudioClip wasser;
    [SerializeField] private AudioClip kohle;
    [SerializeField] private AudioClip tower;

    [SerializeField] private AudioClip destroy;

    [Header("Anzeigen")] 
    [SerializeField] private AudioClip cash;
    [SerializeField] private AudioClip co2;
    [SerializeField] private AudioClip strom;
    [SerializeField] private AudioClip cityHappy;

    public void StartGameSound()
    {
        _audioSource.PlayOneShot(start);
    }

    public void Click()
    {
        _audioSource.PlayOneShot(click);
    }

    public void BackGAmeSound()
    {
        _audioSource.PlayOneShot(back);
    }

    public void LevelSelect()
    {
        _audioSource.PlayOneShot(level1);
    }

    public void LevelUnselect()
    {
        _audioSource.PlayOneShot(level2);
    }

    public void BuildMenu()
    {
        if (isBuildMenu)
        {
            _audioSource.PlayOneShot(buildMenu2);
        }
        else
        {
            _audioSource.PlayOneShot(buildMenu1);
        }
    }
    public void OverlayMenu()
    {
        if (isOverlay)
        {
            _audioSource.PlayOneShot(buildMenu2);
        }
        else
        {
            _audioSource.PlayOneShot(buildMenu1);
        }
    }

    public void PlacePower(PowerPlantType type)
    {
        switch (type)
        {
            case PowerPlantType.NUCLEAR:
                _powerPlantSource.PlayOneShot(atom);
                break;
            case PowerPlantType.WINDMILL:
                _powerPlantSource.PlayOneShot(wind);
                break;
            case PowerPlantType.SOLARPANEL:
                _powerPlantSource.PlayOneShot(photo);
                break;
            case PowerPlantType.GAS:
                _powerPlantSource.PlayOneShot(gas);
                break;
            case PowerPlantType.HYDROPOWER:
                _powerPlantSource.PlayOneShot(wasser);
                break;
            case PowerPlantType.COALPLANT:
                _powerPlantSource.PlayOneShot(kohle);
                break;
            case PowerPlantType.NOTSELECTED:
                _powerPlantSource.PlayOneShot(tower);
                break;
        }
        
        ambience.ChangeLevelSound("none");
    }
    
    public void PowerMap(PowerPlantType type)
    {
        switch (type)
        {
            case PowerPlantType.WINDMILL:
                ambience.ChangeLevelSound("wind2");
                break;
            case PowerPlantType.SOLARPANEL:
                ambience.ChangeLevelSound("sonne2");
                break;
            case PowerPlantType.HYDROPOWER:
                ambience.ChangeLevelSound("wasser2");
                break;
            case PowerPlantType.COALPLANT:
                ambience.ChangeLevelSound("kohle2");
                break;
        }
    }

    public void CitySatisfied()
    {
        _audioSource.PlayOneShot(cityHappy);
    }

    public void CableNew()
    {
        _audioSource.PlayOneShot(cableStart);
    }
    public void CableDone()
    {
        _audioSource.PlayOneShot(cableEnd);
    }

    public void DestroySound()
    {
        _powerPlantSource.PlayOneShot(destroy);
    }

    public void Anzeige(string name)
    {
        if (name.Equals("cash"))
        {
            _audioSource.PlayOneShot(cash);
        }
        else if (name.Equals("co2"))
        {
            _audioSource.PlayOneShot(co2);
        }
        else if (name.Equals("strom"))
        {
            _audioSource.PlayOneShot(strom);
        }
    }
}
