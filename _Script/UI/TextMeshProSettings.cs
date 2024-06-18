using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class TextMeshProSettings : MonoBehaviour
{
    TextMeshProUGUI m_TextMeshPro;
    public float outlineWidth;
    public Color outlineColor;
    private void OnValidate()
    {
        m_TextMeshPro=GetComponent<TextMeshProUGUI>();
        m_TextMeshPro.outlineWidth=outlineWidth;
        m_TextMeshPro.outlineColor = outlineColor;
        Debug.Log(m_TextMeshPro.outlineWidth);
    }
    private void Awake()
    {
        m_TextMeshPro = GetComponent<TextMeshProUGUI>();
        m_TextMeshPro.outlineWidth = outlineWidth;
        m_TextMeshPro.outlineColor = outlineColor;
    }
}
