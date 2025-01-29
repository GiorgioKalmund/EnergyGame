using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour
{

    [Header("Audio Source")] 
    [SerializeField] private AudioSource _audioSource;

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

    public void PlacePower(string powerPlant)
    {
        if (powerPlant.Equals("Atom"))
        {
            _audioSource.PlayOneShot(atom);
        }
        else if (powerPlant.Equals("Kohle"))
        {
            _audioSource.PlayOneShot(kohle);
        }
        else  if (powerPlant.Equals("Wasser"))
        {
            _audioSource.PlayOneShot(wasser);
        }
        else  if (powerPlant.Equals("Photo"))
        {
            _audioSource.PlayOneShot(photo);
        }
        else  if (powerPlant.Equals("Gas"))
        {
            _audioSource.PlayOneShot(gas);
        }
        else  if (powerPlant.Equals("Wind"))
        {
            _audioSource.PlayOneShot(wind);
        }
        else  if (powerPlant.Equals("Turm"))
        {
            _audioSource.PlayOneShot(tower);
        }
    }

    public void DestroySound()
    {
        _audioSource.PlayOneShot(destroy);
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
