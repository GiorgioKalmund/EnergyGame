using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMapManager : MonoBehaviour
{
    public static  LevelMapManager Instance { get; private set; }

    [SerializeField] private int unlockUntilLevel = 1;
    
    public List<LevelMapMarker> markers;
    /**
     *  IMPORTANT: This needs to be identical to the real amount of markers present
     *  If not set correctly, the behaviour might not be as expected!
     */
    public int maxMarkerCount;
    public LevelMapMarker CurrentlySelectedMarker { get; set; }

    public LevelMapType currentMap;

    [Header("Path")]
    [SerializeField] private GameObject pathGameObject;
    [SerializeField] private GameObject pathParent;
    public float pathScale = 0.5f;

    [Header("Backdrop")] 
    [SerializeField] private Button backdropResetButtonBayern;
    [SerializeField] private Button backdropResetButtonBaustelle;

    [Header("Movement")] 
    [SerializeField] private GameObject movingElement;
    [SerializeField] private Image blackoutImage;
    [SerializeField] private GameObject arrowLeft;
    [SerializeField] private GameObject arrowRight;

    public float moveAnimationTime;
    public Ease animationEase = Ease.InOutCubic;

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
        markers = new List<LevelMapMarker>();
        if (!pathParent)
        {
            Debug.LogError("No parent for path elements in Map Manager");
        }

        if (!backdropResetButtonBayern || !backdropResetButtonBaustelle)
        {
            Debug.LogWarning("Backdrop Reset Button not set in LevelMapManager, this could cause unwanted behaviour!");
        }
    }

    private void Start()
    {
        // Backdrop should act as a 'click away to close' area
        backdropResetButtonBayern.onClick.AddListener(CloseCurrentlySelectedMarker);
        backdropResetButtonBaustelle.onClick.AddListener(CloseCurrentlySelectedMarker);
        
        arrowLeft.GetComponent<Button>().onClick.AddListener(ShowBaustelle);
        arrowRight.GetComponent<Button>().onClick.AddListener(ShowBayern);
        
        // Set animation time 0 so the initial transition (if needed) is instant
        float animTime = moveAnimationTime;
        moveAnimationTime = 0f;
        
        // Default to Bayern, as this is what is reflected in the scene 
        arrowRight.SetActive(false);
        
        // Reset animation time back to original value
        moveAnimationTime = animTime;
    }

    private void OnEnable()
    {
        LoadCurrentMap();
        
        if (currentMap == LevelMapType.BAUSTELLE)
            ShowBaustelle();
        else
            ShowBayern();
        
        StartCoroutine(WaitForAllMarkers());
        blackoutImage.DOFade(0f, moveAnimationTime / 2);
    }

    private void LoadCurrentMap()
    {
       currentMap = LevelMapType.BAUSTELLE;
       if (PlayerPrefs.GetInt("levels_completed") > 7)
           currentMap = LevelMapType.BAYERN;
    }

    public void ProgressLockAll()
    {
        PlayerPrefs.SetInt("levels_completed", 0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void ProgressUnlockAll()
    {
        PlayerPrefs.SetInt("levels_completed", maxMarkerCount - 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AddMarker(LevelMapMarker marker)
    {
       markers.Add(marker);
    }

    private void LinkMarkers()
    {
        if (markers.Count != maxMarkerCount)
        {
            Debug.LogWarning("Actual marker count does not correspond to the value entered. Please modify 'maxMarkerCount' to be the amount of markers visible on the field");
            return;
        }
        //Debug.Log("Linking "+markers.Count + " markers.");
        markers.Sort((marker0, marker1) => marker0.markerID.CompareTo(marker1.markerID));
        for (int index = 0; index < markers.Count; index++)
        {
            // Calculate backward, meaning that the first marker doesn't have a prev 
            if (index != 0)
            {
                markers[index].SetPrev(markers[index-1]);
            }
            
            // Calculate forward, meaning that the last marker doesn't have a next
            if (index != markers.Count - 1)
            {
                markers[index].SetNext(markers[index+1]);
            }
        }
    }
    /*
     *  IMPORTANT: This method uses 1-based indexing for consistency with level names
     */
    private void UnlockUntilMarkerId(int markerId)
    {
        if (markerId < 0)
        {
            Debug.LogWarning(markerId+" is not a valid level Index");
            return;
        }
        
        if (markerId > maxMarkerCount)
        {
            markerId = maxMarkerCount;
        }
        markers[Math.Min(markers.Count - 1, markerId)].Unlock();
    }

    public GameObject GetPathObject()
    {
        return pathGameObject;
    }

    public GameObject GetPathParent()
    {
        return pathParent;
    }
    
    private IEnumerator WaitForAllMarkers()
    {
        yield return new WaitUntil(() => markers.Count >= maxMarkerCount);

        LinkMarkers();
        unlockUntilLevel = PlayerPrefs.GetInt("levels_completed");
        UnlockUntilMarkerId(unlockUntilLevel);
    }

    private void CloseCurrentlySelectedMarker()
    {
        if (CurrentlySelectedMarker)
        {
            CurrentlySelectedMarker.ClosePopup();
        }
    }

    public async void ShowBayern()
    {
            arrowRight.SetActive(false);

            float localXPos = movingElement.transform.localPosition.x;
            
            if (moveAnimationTime >= 0.05f)
                await blackoutImage.DOFade(1f, moveAnimationTime / 2).AsyncWaitForCompletion();
            
            if (movingElement.transform.localPosition.x >= 959)
                await movingElement.transform.DOLocalMoveX(localXPos - 1920, 0.1f).SetEase(animationEase).SetRecyclable().AsyncWaitForCompletion();
            await blackoutImage.DOFade(0f, moveAnimationTime / 2).AsyncWaitForCompletion();

            arrowLeft.SetActive(true);

            currentMap = LevelMapType.BAYERN;
    }

    public async void ShowBaustelle()
    {
            arrowLeft.SetActive(false);

            float localXPos = movingElement.transform.localPosition.x;
            
            if (moveAnimationTime >= 0.05f)
                await blackoutImage.DOFade(1f, moveAnimationTime / 2).SetEase(animationEase).SetRecyclable().AsyncWaitForCompletion();
            if (movingElement.transform.localPosition.x <= -959)
                await movingElement.transform.DOLocalMoveX(localXPos + 1920, 0.0f).SetEase(animationEase).SetRecyclable().AsyncWaitForCompletion();
            await blackoutImage.DOFade(0f, moveAnimationTime / 2).SetEase(animationEase).SetRecyclable().AsyncWaitForCompletion();

            arrowRight.SetActive(true);

            currentMap = LevelMapType.BAUSTELLE;
    }

    public void GoHome(string homeName)
    {
        SceneManager.LoadScene(homeName);
    }

    private void OnDestroy()
    {
        DOTween.Kill(movingElement.transform);
        DOTween.Kill(blackoutImage);
    }
}

public enum LevelMapType
{
    BAUSTELLE, BAYERN
}