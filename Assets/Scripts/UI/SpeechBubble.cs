using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;


[RequireComponent(typeof(DialogueContainer))]
public class SpeechBubble : MonoBehaviour
{
    //This is the current string to indicate that something went wrong when getting the dialogue string 
    public const string INVALID_DIALOGUE = "NULL";
    public static float SPEECHBUBBLE_DURATION = 20f;
    [SerializeField] private TMP_Text textbox;
    [SerializeField] public DialogueContainer DialogueContainer;
    [SerializeField] public DialogueObjectSetting dialogueSetting;
    public bool isOpen = false;
    [SerializeField] private float animationTime = 0.3f;

    [SerializeField] private Smiley smiley;
    [SerializeField] private GameObject smileyScaler;
    [SerializeField] private GameObject _lookAtObj;
    [SerializeField] private GameObject _lookAtObj2;

    private void Start()
    {
        transform.localScale = Vector3.zero;
        if (smileyScaler)
            smileyScaler.transform.localScale = Vector3.zero;
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
    
    public async Task OpenSpeechbubble(int index = -1)
    {
        string nextText = GetSpeechBubbleText(index);

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
                
                await appear.Play().AsyncWaitForCompletion();
                break;
            case "$DISABLE":
                Sequence disappear = DOTween.Sequence();
                disappear.Append(smileyScaler.transform.DOScale(Vector3.one*1.5f,0.2f)).SetEase(Ease.InQuad);
                disappear.Append(smileyScaler.transform.DOScale(Vector3.zero,0.2f)).SetEase(Ease.OutQuad);
                
                await disappear.Play().AsyncWaitForCompletion();
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
                transform.DOScale(1f,animationTime);
                isOpen = true;
                textbox.text = nextText;
                await CloseSpeechbubble();
                break;
        }
        
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
        // Kill current delayed close
        DOTween.Kill(transform);
        
        if (!isOpen)
        {
            Debug.LogWarning("Tried to close Speechbubble that was not open.");
            return;
        }

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
