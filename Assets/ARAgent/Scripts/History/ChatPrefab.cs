using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChatPrefab : MonoBehaviour
{
    [SerializeField] private Text m_Text;

    public void SetText(string _msg)
    {
        m_Text.text = _msg;
    }
}
