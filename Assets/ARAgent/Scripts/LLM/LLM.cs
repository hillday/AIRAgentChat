using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using UnityEngine;

public class LLM : MonoBehaviour
{
    [SerializeField] protected string url;

    [SerializeField] protected string m_Prompt = string.Empty;

    [SerializeField] protected string lan = "中文";
    [SerializeField] protected int m_HistoryKeepCount = 15;

    [SerializeField] public List<SendData> m_DataList = new List<SendData>();

    [SerializeField] protected Stopwatch stopwatch = new Stopwatch();

    public virtual void PostMsg(string _msg, Action<string> _callback)
    {
        CheckHistory();
        /*
        string message = "当前为角色的人物设定：" + m_Prompt +
            " 回答的语言：" + lan +
            " 接下来是我的提问：" + _msg;
            */

        m_DataList.Add(new SendData("user", _msg));

        StartCoroutine(Request(_msg, _callback));
    }

    public virtual IEnumerator Request(string _postWord, System.Action<string> _callback)
    {
        yield return new WaitForEndOfFrame();

    }


    public virtual void CheckHistory()
    {
        if (m_DataList.Count > m_HistoryKeepCount)
        {
            m_DataList.RemoveAt(0);
        }
    }

    [Serializable]
    public class SendData
    {
        [SerializeField] public string role;
        [SerializeField] public string content;
        public SendData() { }
        public SendData(string _role, string _content)
        {
            role = _role;
            content = _content;
        }

    }

}
