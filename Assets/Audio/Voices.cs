using System.Collections;
using System.Collections.Generic;
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


    public void CharacterSpeak(string character)
    {
        if (character.Equals("Bürgermeisterin"))
        {
            int index = Random.Range(0, 8);
            _audioSource.PlayOneShot(bürgermeisterin[index]);
        }
        else if (character.Equals("Bau"))
        {
            int index = Random.Range(0, 7);
            _audioSource.PlayOneShot(bauarbeiter[index]);
        }
        else if (character.Equals("CO2"))
        {
            int index = Random.Range(0, 4);
            _audioSource.PlayOneShot(grüner[index]);
        }
        else if (character.Equals("Money"))
        {
            int index = Random.Range(0, 5);
            _audioSource.PlayOneShot(monopoly[index]);
        }
        else if (character.Equals("Quiote"))
        {
            int index = Random.Range(0, 5);
            _audioSource.PlayOneShot(quiote[index]);
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
