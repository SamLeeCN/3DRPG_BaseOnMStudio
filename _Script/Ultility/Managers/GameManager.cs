using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
[DefaultExecutionOrder(-500)]
public class GameManager : Singleton<GameManager>
{
    public Character playerCharacter;
    public PlayerControler playerControler;
    public Inventory playerInventory;
    public bool isPlayerDead;
    public bool isGameOver=false;

    [Header("Sundries")]
    public float timeScaleRecord = 1f;
    public float itemThrowSpeed=2f;
    public float itemThrowHeight = 3f;
    #region ID Lists
    private static List<ItemIDPair> itemList = new List<ItemIDPair>()
    {
        new ItemIDPair(0,ItemEnum.Sword),
        new ItemIDPair (1,ItemEnum.Apple),
        new ItemIDPair(2,ItemEnum.Hammer),
        new ItemIDPair(3,ItemEnum.WoodenShield),
    };
    private static List<CharacterIDPair> characterList = new List<CharacterIDPair>()
    {
        new CharacterIDPair(0,CharacterEnum.PlayerDog),
        new CharacterIDPair(1,CharacterEnum.Slime),
        new CharacterIDPair(2,CharacterEnum.Turtle),
        new CharacterIDPair(3,CharacterEnum.Grunt),
        new CharacterIDPair(4,CharacterEnum.Golem),
    };
    public static ItemEnum ItemIdToEnum(int id)
    {
        ItemIDPair pair = itemList.Find(p => p.id == id);
        if (pair != null) return pair.itemEnum;
        else return ItemEnum.Null;
    }
    public static int ItemEnumToID(ItemEnum _enum)
    {
        ItemIDPair pair = itemList.Find(p => p.itemEnum == _enum);
        if (pair != null) return pair.id;
        else return -1;
    }
    public static CharacterEnum CharacterIdToEnum(int id)
    {
        CharacterIDPair pair = characterList.Find(p => p.id == id);
        if (pair != null) return pair.characterEnum;
        else return CharacterEnum.Null;
    }
    public static int CharacterEnumToID(CharacterEnum _enum)
    {
        CharacterIDPair pair = characterList.Find(p => p.characterEnum==_enum);
        if (pair != null) return pair.id;
        else return -1;
    }
    #endregion
    protected override void Awake()
    {
        base.Awake();
    }
    
    void Start()
    {

    }

    void Update()
    {
        ControlTimeScale();
        JudgeGameOver();
    } 
    public void RegisterPlayerCharacter(Character playerCharacter)
    {
        this.playerCharacter = playerCharacter;
    }
    public void RegisterPlayerController(PlayerControler playerControler)
    {
        this.playerControler = playerControler;
    }
    public void ControlTimeScale()
    {
        bool isSettingPanelOpen = UIManager.Instance.isSettingPanelOpen;
        bool isDialoguePanelOpen = UIManager.Instance.isDialoguePanelOpen;
        bool isTaskPanelOpen=UIManager.Instance.isTaskPanelOpen;
        
        bool toPause=isSettingPanelOpen||isDialoguePanelOpen||isTaskPanelOpen;
        if (Time.timeScale != 0)
        {
            
            if(toPause)
            {
                timeScaleRecord=Time.timeScale;
                Time.timeScale = 0;
            }
        }
        else
        {
            if (!toPause)
            {
                Time.timeScale = timeScaleRecord;
            }
        }
    }
    public void JudgeGameOver()
    {
        isPlayerDead = playerCharacter.isDead;
        if(isPlayerDead&&!isGameOver)
        {
            EventManager.Instance.gameOverEvent.RaiseEvent();
            isGameOver = true;
        }
    }
    public void SetPlayerEnablity(bool enablity)
    {
        playerCharacter.gameObject.SetActive(enablity);
    }
    public bool IsPlayerEnable()
    {
        return playerCharacter.gameObject.activeInHierarchy;
    }
    public void NewGame()
    {
        EventManager.Instance.RaiseNewGameEvent();
    }
    public void Continue()
    {
        DataManager.Instance.LoadAll();
    }
    public void ExitGame()
    {
        //TODO:ExitGame
    }
}
public class ItemIDPair
{
    public int id;
    public ItemEnum itemEnum;
    public ItemIDPair(int id, ItemEnum itemEnum)
    {
        this.id = id;
        this.itemEnum = itemEnum;
    }
}
public class CharacterIDPair
{
    public int id;
    public CharacterEnum characterEnum;
    public CharacterIDPair(int id, CharacterEnum characterEnum)
    {
        this.id = id;
        this.characterEnum = characterEnum;
    }
}