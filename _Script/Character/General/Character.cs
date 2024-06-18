using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
[RequireComponent(typeof(DataDefination))]
public class Character : MonoBehaviour,ISaveable
{
    public CharacterDataSO templateCharacterData;
    public CharacterDataSO characterData;
    public AttackDataSO templateRegularAttackData;
    public AttackDataSO regularAttackData;
    public Animator animator;
    public NavMeshAgent agent;
    public FillBar healthBar;
    public Character lastAttacker;
    public Inventory inventory;
    public RuntimeAnimatorController oneHandAnimationController;
    public RuntimeAnimatorController twoHandAnimationController;
    public RuntimeAnimatorController unarmedAnimationController;
    [Header("Basic Information")]
    public string characterName;
    [Header("State")]
    public bool isAttacking;
    public bool isInvulnerable;
    public bool isDead;
    public bool isCritical;
    [Header("Buff")]
    public bool isFightBack;
    public bool isDizzy;
    public float dizzyRemainTime;
    [Header("Coroutine")]
    private IEnumerator foughtBackCoroutine;
    [Header("SaveLoad")]
    private SaveProfile<CharacterSaveData> saveProfile;
    private string saveFileName;
    private string characterSOKeyName;
    private string characterPosKeyName;
    private string currentHealthKeyName;
    private string regularAttackSOKeyName;
    private string currentWeaponSOKeyName;
    private string currentArmorSOKeyName;

    #region read from character data
    public int currentMaxHealth
    {
        get { if (characterData) return characterData.currentMaxHealth; else return 0; }
        set { characterData.currentMaxHealth = value; }
    }
    public int currentHealth
    {
        get { if (characterData) return characterData.currentHealth; else return 0; }
        set { characterData.currentHealth = value; }
    }
    public int currentDefence
    {
        get { if (characterData) return characterData.currentDefence; else return 0; }
        set { characterData.currentDefence = value; }
    }
    public int currentAttackForce
    {
        get { if (characterData) return characterData.currentAttackForce; else return 0; }
        set { characterData.currentAttackForce = value; }
    }
    #endregion

    private void OnEnable()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveable();
    }
    private void OnDisable()
    {
        ISaveable saveable = this;
        saveable.UnregisterSaveable();
        
    }
    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        inventory = GetComponent<Inventory>();
        healthBar = GetComponentInChildren<FillBar>();
        


        SetFileName();
        characterData = Instantiate(templateCharacterData);
        regularAttackData = Instantiate(templateRegularAttackData);
        regularAttackData.ApplyCharacterData(characterData);
        saveProfile = new SaveProfile<CharacterSaveData>(saveFileName, new CharacterSaveData());
    }
    

    private void Start()
    {
        currentHealth = currentMaxHealth;
        UIManager.Instance.UpdateCharacterHealthUI(healthBar, currentHealth, currentMaxHealth);
    }
    
    void Update()
    {
        isDead = currentHealth == 0;
        if (dizzyRemainTime > 0)
        {
            isDizzy = true;
            dizzyRemainTime -= Time.deltaTime;
        }
        else
        {
            isDizzy = false;
        }
        if (gameObject.activeInHierarchy&&agent.enabled)
        {
            if (!isFightBack&&(isAttacking || isDizzy))
            {
                agent.isStopped = true;
            }
            else
            {
                agent.isStopped = false;
            }
        }
    }
    #region hurt
    public void TakeDamage(Character attacker, Character defencer, AttackDataSO attackData)
    {
        if (defencer.isInvulnerable) return;
        if (!attacker) attacker = lastAttacker; else lastAttacker = attacker;
        float coreDamage = Random.Range(attackData.minAttackForce, attackData.maxAttackForce);
        if (attacker.isCritical)
        {
            coreDamage *= attackData.criticalMultiplier;
            defencer.animator.SetTrigger("Hurt");
        }
        int damage;
        if (attackData.isDefenceable) damage = Mathf.Max((int)coreDamage + attacker.currentAttackForce - defencer.currentDefence, 1);
        else damage = Mathf.Max((int)coreDamage + attacker.currentAttackForce, 1);
        defencer.currentHealth = Mathf.Clamp(defencer.currentHealth - damage, 0, defencer.currentMaxHealth);
        UIManager.Instance.UpdateCharacterHealthUI(healthBar, currentHealth, currentMaxHealth);
        //TODO:Add buff the attack attachs to the defencer
        if (defencer.currentHealth == 0)
        {
            defencer.OnDie();
            attacker.characterData.GainExp(defencer.characterData.defeatExp);
            if (attacker == GameManager.Instance.playerCharacter)
            {
                TaskManager.Instance.AddDefeatCharacterProgress(characterData.characterID);
            }
        }
    }
    public void FoughtBack(Vector3 fightBackDir, float fightBackForce, float duration = 0.3f)
    {
        agent.ResetPath();
        foughtBackCoroutine = FoughtBackEnumerator(fightBackDir, fightBackForce, duration);
        StartCoroutine(foughtBackCoroutine);
    }
    private IEnumerator FoughtBackEnumerator(Vector3 fightBackDir, float fightBackForce, float duration)
    {
        isFightBack = true;
        agent.velocity = fightBackDir * fightBackForce;
        yield return new WaitForSeconds(duration);
        isFightBack = false;
    }
    public void OnDie()
    {
        isDead = true;
        if (GetComponent<LootItemSpawner>())
        {
            GetComponent<LootItemSpawner>().SpawnLootItems();
        }
    }
    #endregion
    #region buff apply
    public void AddBuffDizzy(float duration)
    {
        if (dizzyRemainTime < duration)
        {
            dizzyRemainTime = duration;
        }
    }
    public void ApplyHealthRecovery(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, currentMaxHealth);
    }
    public void ApplyHealthPercentageRecovery(float percentage)
    {
        currentHealth = Mathf.Clamp(currentHealth + (int)(currentMaxHealth*percentage), 0, currentMaxHealth);
    }
    #endregion
    #region Save Load
    public DataDefination GetID()
    {
        return GetComponent<DataDefination>();
    }
    public void SetFileName()
    {
        saveFileName = GetID().ID + "Character";
        characterSOKeyName = GetID().ID+ "CharacterSO";
        regularAttackSOKeyName = GetID().ID + "RegularAttackSO";
        characterPosKeyName = GetID().ID + "characterPos";
        currentHealthKeyName = GetID().ID + "health";
    }
    public void SaveData()
    {
        //Save SO
        DataManager.Instance.SaveSO(characterData,characterSOKeyName);
        DataManager.Instance.SaveSO(regularAttackData,regularAttackSOKeyName);
        //Save profile
        saveProfile.saveData.pos = new SerializeVector3(transform.position);
        saveProfile.saveData.agentDestination = new SerializeVector3(agent.destination);
        DataManager.Instance.Save(saveProfile);
        //Save sundries
        SundrySaveData sundrySaveData = DataManager.Instance.sundrySaveProfile.saveData;
        if(sundrySaveData.vector3Dict.ContainsKey(GetID().ID))
        {
            sundrySaveData.vector3Dict[characterPosKeyName]=new SerializeVector3(transform.position);
            sundrySaveData.intSaveData[currentHealthKeyName] = currentHealth;
        }
    }
    public void LoadData()
    {
        //Load SO
        if (!DataManager.Instance.LoadSO(characterData, characterSOKeyName)) 
            characterData = Instantiate(templateCharacterData);
        if (!DataManager.Instance.LoadSO(regularAttackData, regularAttackSOKeyName)) 
            regularAttackData = Instantiate(templateRegularAttackData);

        regularAttackData.ApplyCharacterData(characterData);
        //Load profile
        saveProfile = DataManager.Instance.Load<CharacterSaveData>(saveFileName);
        if (saveProfile == null)
        {
            saveProfile = new SaveProfile<CharacterSaveData>(saveFileName, new CharacterSaveData());
        }
        else
        {
            transform.position = saveProfile.saveData.pos.ToVector3();
            if(saveProfile.saveData.agentDestination!=null) agent.destination=saveProfile.saveData.agentDestination.ToVector3();
        }
        //Load sundries
        SundrySaveData sundrySaveData = DataManager.Instance.sundrySaveProfile.saveData;
        if (sundrySaveData.vector3Dict.ContainsKey(characterPosKeyName))
        {
            transform.position = sundrySaveData.vector3Dict[characterPosKeyName].ToVector3();
            currentHealth = sundrySaveData.intSaveData[currentHealthKeyName];
        }
        //Update attached UI with this character
        healthBar.UpdateFillUI(currentHealth, currentMaxHealth);
    }
    public void DeleteData()
    {
        //Delete SO
        DataManager.Instance.DeleteSO(characterSOKeyName);
        //Delete profile
        DataManager.Instance.Delete(saveFileName);
        //Delete sundries from variable
        SundrySaveData sundrySaveData = DataManager.Instance.sundrySaveProfile.saveData;
        sundrySaveData.vector3Dict.Remove(characterPosKeyName);
        sundrySaveData.intSaveData.Remove(currentHealthKeyName);
    }
    #endregion

    
}
