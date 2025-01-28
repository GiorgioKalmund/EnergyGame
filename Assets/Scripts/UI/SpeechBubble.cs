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
using System.Threading;



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

    private CancellationTokenSource _flinkCancellationTokenSource;


    [SerializeField] private string _onLevelCompleteText;

    public bool IsInterruped { get; internal set; }

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
    private async void flink(GameObject gameObject)
    {
        if (gameObject != null)
        {
            Transform backdropTransform = gameObject.transform.Find("Backdrop");
            if (backdropTransform == null)
            {
                return;
            }

            var image = backdropTransform.GetComponent<UnityEngine.UI.Image>();
            if (image == null)
            {
                return;
            }

            // Stop any previous blinking
            _flinkCancellationTokenSource?.Cancel();
            _flinkCancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = _flinkCancellationTokenSource.Token;

            Color yellow = Color.yellow;
            Color white = Color.white;
            float blinkDuration = 5f;
            float blinkInterval = 0.2f;

            float elapsedTime = 0f;
            try
            {
                while (elapsedTime < blinkDuration)
                {
                    // Check for cancellation
                    cancellationToken.ThrowIfCancellationRequested();

                    image.color = yellow;
                    await Task.Delay((int)(blinkInterval * 1000), cancellationToken);

                    cancellationToken.ThrowIfCancellationRequested();
                    image.color = white;
                    await Task.Delay((int)(blinkInterval * 1000), cancellationToken);

                    elapsedTime += blinkInterval * 2;
                }

                image.color = white; // Reset color to white
            }
            catch (OperationCanceledException)
            {
                image.color = white; // Ensure the color is reset
            }
        }
    }
    public async Task OpenSpeechbubble(int index = -1)
    {
        string nextText = GetSpeechBubbleText(index);

        ///////////////////////////////////////
        _flinkCancellationTokenSource?.Cancel();


        /////////////////////////////////////////
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
            case "$TUTORIAL":
                IsInterruped = true;
                OpenSpeechbubble();
                break;
            case INVALID_DIALOGUE:
                Debug.LogWarning("Invalid Dialogue String in Speechbubble");
                return;
            default:
                isOpen = true;
                textbox.text = nextText;
                await transform.DOScale(1f,animationTime).SetRecyclable().AsyncWaitForCompletion();
                await Task.Delay(20000);
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
        if(IsInterruped){
            return;
        }
        if (!isOpen)
        {
            Debug.LogWarning("INSTANT: Tried to close Speechbubble that was not open.");
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
