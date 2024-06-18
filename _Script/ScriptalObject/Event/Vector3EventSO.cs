using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(menuName ="Event/Vector3EventSO")]
public class Vector3EventSO : ScriptableObject
{
    public UnityAction<Vector3> OnEventRaised;
    public void RaiseEvent(Vector3 vector3)
    {
        OnEventRaised?.Invoke(vector3);
    }
}
