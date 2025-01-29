using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Voices : MonoBehaviour
{
    [Header("Audio Source")] 
    [SerializeField] private AudioSource _audioSource;

    [Header("Speaking")] 
    [SerializeField] private AudioClip[] bürgermeisterin;
    [SerializeField] private AudioClip[] bauarbeiter;
    [SerializeField] private AudioClip[] grüner;
    [SerializeField] private AudioClip[] monopoly;
    [SerializeField] private AudioClip[] quiote;
    
    [Header("Meckern")]
    [SerializeField] private AudioClip[] grünerM;
    [SerializeField] private AudioClip[] monopolyM;


    public void CharacterSpeak(Smileys smileys)
    {
        Debug.Log(smileys.ToSafeString() + " speaking now!");
        int index = 0;
        switch (smileys)
        {
            case Smileys.Bürgermeisterin:
                index = Random.Range(0, 8);
                _audioSource.PlayOneShot(bürgermeisterin[index]);
                break;
            case Smileys.Baumeister:
                index = Random.Range(0, 7);
                _audioSource.PlayOneShot(bauarbeiter[index]);
                break;
            case Smileys.Greta:
                index = Random.Range(0, 4);
                _audioSource.PlayOneShot(grüner[index]);
                break;
            case Smileys.Monopoly:
                index = Random.Range(0, 5);
                _audioSource.PlayOneShot(monopoly[index]);
                break;
            case Smileys.Don:
                index = Random.Range(0, 5);
                _audioSource.PlayOneShot(quiote[index]);
                break;
                
        }
    }

    public void CharacterGemecker(string character)
    {
        if (character.Equals("CO2"))
        {
            int index = Random.Range(0, 2);
            _audioSource.PlayOneShot(grünerM[index]);
        }
        else if (character.Equals("Money"))
        {
            int index = Random.Range(0, 3);
            _audioSource.PlayOneShot(monopolyM[index]);
        }
    }
}
