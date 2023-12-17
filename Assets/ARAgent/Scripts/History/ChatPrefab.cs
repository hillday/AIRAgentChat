using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChatPrefab : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]private Text m_Text;

    public void SetText(string _msg){
        m_Text.text=_msg;
    }
}
