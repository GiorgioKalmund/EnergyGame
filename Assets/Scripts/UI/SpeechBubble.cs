using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;
using Unity.Mathematics;
using Random = UnityEngine.Random;
using static System.Net.Mime.MediaTypeNames;


[RequireComponent(typeof(DialogueContainer))]
public class SpeechBubble : MonoBehaviour
{
    //This is the current string to indicate that something went wrong when getting the dialogue string 
    public const string INVALID_DIALOGUE = "NULL";
    public static float SPEECHBUBBLE_DURATION = 20000f;
    [SerializeField] private TMP_Text textbox;
    [SerializeField] public DialogueContainer DialogueContainer;
    [SerializeField] public DialogueObjectSetting dialogueSetting;
    public bool isOpen = false;
    [SerializeField] private float animationTime = 0.3f;

    [SerializeField] private Smiley smiley;
    [SerializeField] private GameObject smileyScaler;
    [SerializeField] private GameObject _lookAtObj;
    [SerializeField] private GameObject _lookAtObj2;



    //ik should be seperated into another script 
    [Header("HintManager")]
    [SerializeField] private GameObject OverlayDropdown;
    [SerializeField] private GameObject Strom;
    [SerializeField] private GameObject CO2;
    [SerializeField] private GameObject Finanzen;
    [SerializeField] private GameObject OL_Sonne;
    [SerializeField] private GameObject OL_Wind;
    [SerializeField] private GameObject OL_Wasser;
    [SerializeField] private GameObject OL_Kohle;
    [SerializeField] private GameObject builderInventory;


    [SerializeField] private string _onLevelCompleteText;
    private void Start()
    {
        transform.localScale = Vector3.zero;
        if (smileyScaler)
            smileyScaler.transform.localScale = Vector3.zero;
        
        if (LevelManager.Instance)
            LevelManager.Instance.LevelCompleted += SayTextOnLevelComplete; //M: keine ahnung warum aber muss hier hin und nicht in OnEnable
    }
    
    void OnDisable() {
        if (LevelManager.Instance)
            LevelManager.Instance.LevelCompleted -= SayTextOnLevelComplete;
    }
    public async Task Talk()
    {
        if (!DialogueContainer)
            return;
        
        while (DialogueContainer.HasNextLine())
        {
            await OpenSpeechbubble();
            await transform.DOLocalMoveX(transform.localPosition.x,0).SetDelay(2*animationTime).AsyncWaitForCompletion(); //Schlimmster hack in diesem gesamten Projekt 
        } 
    }
    private async void flink(GameObject gameobject)
    {
        if(gameobject != null)
        {
            
            float blinkDuration = 5f; // Blinkdauer in Sekunden
            float blinkInterval = 0.2f; // Intervall, wie oft es blinken soll (in Sekunden)

            float elapsedTime = 0f;
            while (elapsedTime < blinkDuration)
            {

                gameobject.SetActive(false);
               
                await Task.Delay((int)(blinkInterval * 1000));


                gameobject.SetActive(true);
            
                await Task.Delay((int)(blinkInterval * 1000));

                elapsedTime += blinkInterval * 2;
            }


            gameobject.SetActive(true);
     

        }
    }
    public async Task OpenSpeechbubble(int index = -1)
    {
        string nextText = GetSpeechBubbleText(index);


        switch (nextText.Trim())
        {
            case "Klick mal links auf das Strom-Symbol!":
                OverlaysDropdown.Instance.Expand();
                BuilderInventory.Instance.ShowInventory();
                flink(Strom);
                break;
          
            default:
                break;
        }


        if (nextText.Contains("Öffne"))
        {
            if (OverlaysDropdown.Instance)
                OverlaysDropdown.Instance.Expand();
        }
        switch(nextText.Trim()){
            case "$ENABLE":
                Sequence appear = DOTween.Sequence();
                appear.Append(smileyScaler.transform.DOScale(Vector3.one*1.5f,0.2f)).SetEase(Ease.InQuad);
                appear.Append(smileyScaler.transform.DOScale(Vector3.one,0.2f)).SetEase(Ease.OutQuad);
                
                await appear.Play().SetRecyclable().AsyncWaitForCompletion();
                break;
            case "$DISABLE":
                Sequence disappear = DOTween.Sequence();
                disappear.Append(smileyScaler.transform.DOScale(Vector3.one*1.5f,0.2f)).SetEase(Ease.InQuad);
                disappear.Append(smileyScaler.transform.DOScale(Vector3.zero,0.2f)).SetEase(Ease.OutQuad);
                
                await disappear.Play().SetRecyclable().AsyncWaitForCompletion();
                break;
            case "$SMILE":
                smiley.Expression = Expression.Smile;
                await transform.DOLocalMoveX(transform.localPosition.x,0).SetDelay(animationTime).AsyncWaitForCompletion(); //Schlimmster hack in diesem gesamten Projekt 
                break;
            case "$NEUTRAL":
                smiley.Expression = Expression.Neutral;
                await transform.DOLocalMoveX(transform.localPosition.x,0).SetDelay(animationTime).AsyncWaitForCompletion();
                break;
            case "$FROWN":
                smiley.Expression = Expression.Frown;
                await transform.DOLocalMoveX(transform.localPosition.x,0).SetDelay(animationTime).AsyncWaitForCompletion();
                break;
            case "$LOOKAT":
                smiley.cutsceneLookAt(_lookAtObj);
                break;
            case "$LOOKAT2":
                smiley.cutsceneLookAt(_lookAtObj2);
                break;
            case "$STOPLOOKAT":
                smiley.cutsceneLookAt();
                break;
            case INVALID_DIALOGUE:
                Debug.LogWarning("Invalid Dialogue String in Speechbubble");
                return;
            default:
                isOpen = true;
                textbox.text = nextText;
                await transform.DOScale(1f,animationTime).SetRecyclable().AsyncWaitForCompletion();
                await CloseSpeechbubble();
                break;
        }
        
    }
    public async Task OpenSpeechbubbleWithCustomText(string text){
        
        await transform.DOScale(1f,animationTime).SetRecyclable().AsyncWaitForCompletion();
        isOpen = true;
        textbox.text = text;
        await CloseSpeechbubble();
    }

    public async Task CloseSpeechbubble()
    {
        if (!isOpen)
        {
            Debug.LogWarning("Tried to close Speechbubble that was not open.");
            return;
        }

        await transform.DOScale(0f, animationTime).SetDelay(SPEECHBUBBLE_DURATION).AsyncWaitForCompletion();
        isOpen = false;
        textbox.text = "";
    }
    public void CloseSpeechBubbleInstantly()
    {
        if (!isOpen)
        {
            Debug.LogWarning("Tried to close Speechbubble that was not open.");
            return;
        }
        // Kill current delayed close
        DOTween.Kill(transform);
        
        

        transform.DOScale(0f, animationTime);
        isOpen = false;
        textbox.text = "";
        
    }

    //Call this in the smiley button
    public void ToggleSpeechBubble()
    {
        if (!isOpen)
        {
            OpenSpeechbubble();

        }
        else
        {
            CloseSpeechbubble();
        }
    }

    private string GetSpeechBubbleText(int index = -1) //optional
    {
        switch (dialogueSetting)
        {
            case DialogueObjectSetting.LINEAR:
                return DialogueContainer.GetNextLineOfDialogue();
            case DialogueObjectSetting.FIRSTONLY:
                return DialogueContainer.GetFirstLine();
            case DialogueObjectSetting.RANDOM:
                return DialogueContainer.GetRandom();
            case DialogueObjectSetting.INDEX:
                if(index != -1) return DialogueContainer.GetAtIndex(index);
                break;
        }
        return INVALID_DIALOGUE;
    }

    private void SayTextOnLevelComplete(){
        if(_onLevelCompleteText != "")
            OpenSpeechbubbleWithCustomText(_onLevelCompleteText);
        
    }
    private void OnDestroy()
    {
        DOTween.Kill(transform);
    }
}
public enum DialogueObjectSetting
{
    LINEAR,
    FIRSTONLY,
    RANDOM,
    INDEX
}
