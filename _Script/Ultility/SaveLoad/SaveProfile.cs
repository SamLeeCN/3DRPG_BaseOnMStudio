using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
[System.Serializable]
public sealed class SaveProfile<T> where T:SaveProfileData
{
    public string profileName;
    public T saveData;

    private SaveProfile() { }
    public SaveProfile(string profileName, T saveData)
    {
        this.profileName = profileName;
        this.saveData = saveData;
    }
}
public abstract record SaveProfileData { }

public record CharacterSaveData : SaveProfileData
{
    public SerializeVector3 pos;
    public SerializeVector3 agentDestination;
}

public record SundrySaveData : SaveProfileData
{
    public Dictionary<string, SerializeVector3> vector3Dict=new Dictionary<string, SerializeVector3>();
    public Dictionary<string,float> floatSaveData=new Dictionary<string,float>();
    public Dictionary<string,int> intSaveData=new Dictionary<string,int>();
}

public record WorldSaveData : SaveProfileData
{
    
}
public class SerializeVector3
{
    public float x, y, z;
    public SerializeVector3(Vector3 pos)
    {
        x = pos.x;
        y = pos.y;
        z = pos.z;
    }
    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}