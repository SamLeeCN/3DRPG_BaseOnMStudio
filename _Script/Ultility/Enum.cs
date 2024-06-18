using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#region Character Item Enum
public enum CharacterEnum
{
    Null,PlayerDog, Slime, Turtle, Grunt, Golem
}
public enum ItemEnum
{
    Null,Sword, Apple, Hammer, WoodenShield
}
#endregion

public enum EnemyStates{
    Guard,Patrol,Chase,Skill,Dead
}
public enum SceneType
{
    Location,Menu,Inventoy
}
public enum TeleportType
{
    Trans,Pos
}
public enum PersistentType
{
    ReadWrite,DoNotPersist
}
public enum ItemType
{
    Useable,Weapon,Armor
}
public enum SlotType
{
    Bag,Weapon,Armor,Action,Display
}
public enum WeaponType
{
    OneHand,TwoHand,Remote
}
public enum QuestRequireType
{
    DefeatCharacter,CollectItem
}
