using Cinemachine;
using Pooling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance;

    private void CreateSingleton()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    #endregion

    [Header("Game Settings")]
    public GameState currentGameState;
    [Space, Header("UI Settings")]
    public TextMeshProUGUI currentLevel;
    public TextMeshProUGUI finishText;
    public Slider levelSlider;

    [HideInInspector] public int level = 1;
    [HideInInspector] public BladesHolder bladesHolder;
    [HideInInspector] public CinemachineTrackedDolly cameraTrackingDolly;
    [HideInInspector] public Transform enemiesPlaceHolder;

    private CanvasGroup pauseCanvasGroup;
    private CanvasGroup finishCanvasGroup;
    private Animator virtualCameraAnimator;
    private float timer;
    private bool isTouchedFirstTime;
    private EnemySpawner[] enemySpawners;

    public enum GameState
    {
        MENU,
        GAME,
        VICTORY,
        DEFEAT,
    }

    public enum EnemyTags
    {
        Light,
        Heavy,
    }

    private void Awake()
    {
        CreateSingleton();
        
        cameraTrackingDolly = Camera.main.GetComponent<CinemachineVirtualCamera>()
            .GetCinemachineComponent<CinemachineTrackedDolly>();
        bladesHolder = FindObjectOfType<BladesHolder>();
        pauseCanvasGroup = transform.GetChild(3).GetComponent<CanvasGroup>();
        finishCanvasGroup = transform.GetChild(0).GetComponent<CanvasGroup>();
        virtualCameraAnimator = Camera.main.gameObject.GetComponent<Animator>();
        enemySpawners = virtualCameraAnimator.transform.GetComponentsInChildren<EnemySpawner>();
        enemiesPlaceHolder = virtualCameraAnimator.transform.GetChild(0);
        LoadPlayerData();
    }

    private void Update()
    {
        if (currentGameState == GameState.MENU) return;
        if (currentGameState == GameState.VICTORY || currentGameState == GameState.DEFEAT)
        {
            if (finishCanvasGroup.alpha < 1)
                finishCanvasGroup.alpha = Mathf.SmoothStep(finishCanvasGroup.alpha, 1, 10 * Time.unscaledDeltaTime);
            return;
        }
        var enemiesCount = enemiesPlaceHolder.childCount;
        if (levelSlider.value >= 1 && enemiesCount == 0)
        {
            if (currentGameState != GameState.VICTORY)
                FinishLevel(GameState.VICTORY);
        }
        levelSlider.value = cameraTrackingDolly.m_PathPosition;
        if (Input.touchCount > 0 && !isTouchedFirstTime)
            isTouchedFirstTime = true;
        else if (Input.touchCount.Equals(0) && isTouchedFirstTime)
        {
            timer += Time.deltaTime;
            if (!(timer > 1.25f)) return;
            Time.timeScale = 0.02f;
            Time.fixedDeltaTime = Time.timeScale * .02f;
            pauseCanvasGroup.alpha = Mathf.SmoothStep(pauseCanvasGroup.alpha, 1,  10 * Time.unscaledDeltaTime);
        }
        else
        {
            timer = 0.0f;
            Time.timeScale = 1;
            pauseCanvasGroup.alpha = 0;
        }
    }

    private void LoadPlayerData()
    {
        if (PlayerPrefs.HasKey("Level"))
            level = PlayerPrefs.GetInt("Level");
        currentLevel.text = level.ToString();
        var parent = currentLevel.transform.parent;
        parent.GetChild(0).GetComponent<TextMeshProUGUI>().text = (level - 1).ToString();
        parent.GetChild(2).GetComponent<TextMeshProUGUI>().text = (level + 1).ToString();
    }

    public void OnClickStartGame()
    {
        levelSlider.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = level.ToString();
        levelSlider.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = (level + 1).ToString();
        levelSlider.transform.GetChild(3).GetComponent<Image>().color =
            levelSlider.transform.GetChild(0).GetComponent<Image>().color; 
        isTouchedFirstTime = false;
        virtualCameraAnimator.SetTrigger("Start");
        currentGameState = GameState.GAME;
        foreach (var enemySpawner in enemySpawners)
        {
            enemySpawner.StartSpawn();
        }
    }


    public void FinishLevel(GameState finishState)
    {
        if (finishState == GameState.VICTORY)
        {
            SavePlayerLevel();
            levelSlider.transform.GetChild(3).GetComponent<Image>().color =
                levelSlider.transform.GetChild(2).GetComponent<Image>().color;
        }
        virtualCameraAnimator.speed = 0;
        finishCanvasGroup.blocksRaycasts = true;
        string result = finishState.ToString();
        finishText.text = $"{result}\n<color=white>{result}</color>";
        currentGameState = finishState;
        foreach (var enemySpawner in enemySpawners)
        {
            enemySpawner.StopSpawn();
        }
    }

    private void SavePlayerLevel()
    {
        PlayerPrefs.SetInt("Level", ++level);
    }
    
    public void GoBackToMenu()
    {
        //Set default values
        LoadPlayerData();
        ObjectPooler.Instance.DisableAllPooledObjects();
        finishCanvasGroup.blocksRaycasts = isTouchedFirstTime = false;
        levelSlider.value = finishCanvasGroup.alpha = cameraTrackingDolly.m_PathPosition = 0;
        FindObjectOfType<PlayerController>().transform.position = transform.position;
        bladesHolder.SetupBlades(6);
        bladesHolder.GetComponent<Animator>().speed = 1;
        virtualCameraAnimator.speed = 1;
        virtualCameraAnimator.SetTrigger("End");
        currentGameState = GameState.MENU;
    }

}
