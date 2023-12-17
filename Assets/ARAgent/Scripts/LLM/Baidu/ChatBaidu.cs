using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ChatBaidu : LLM
{

    public ChatBaidu()
    {
        url = "https://aip.baidubce.com/rpc/2.0/ai_custom/v1/wenxinworkshop/chat/eb-instant";
    }

    void Awake()
    {
        OnInit();
    }


    [SerializeField] private BaiduSettings m_Settings;

    private List<message> m_History = new List<message>();

    public ModelType m_ModelType = ModelType.ERNIE_Bot_turbo;

    private void OnInit()
    {
        m_Settings = this.GetComponent<BaiduSettings>();
        GetEndPointUrl();
    }


    public override void PostMsg(string _msg, Action<string> _callback)
    {
        base.PostMsg(_msg, _callback);
    }

    public override IEnumerator Request(string _postWord, System.Action<string> _callback)
    {
        stopwatch.Restart();

        string _postUrl = url + "?access_token=" + m_Settings.m_Token;
        m_History.Add(new message("user", _postWord));
        RequestData _postData = new RequestData
        {
            messages = m_History
        };

        using (UnityWebRequest request = new UnityWebRequest(_postUrl, "POST"))
        {
            string _jsonData = JsonUtility.ToJson(_postData);
            byte[] data = System.Text.Encoding.UTF8.GetBytes(_jsonData);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(data);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.responseCode == 200)
            {
                string _msg = request.downloadHandler.text;
                ResponseData response = JsonConvert.DeserializeObject<ResponseData>(_msg);

                string _responseText = response.result;
                m_History.Add(new message("assistant", response.result));

                m_DataList.Add(new SendData("assistant", response.result));

                _callback(response.result);

            }

        }


        stopwatch.Stop();
        Debug.Log("chat time usage：" + stopwatch.Elapsed.TotalSeconds);
    }

    private void GetEndPointUrl()
    {
        url = "https://aip.baidubce.com/rpc/2.0/ai_custom/v1/wenxinworkshop/chat/" + CheckModelType(m_ModelType);
    }


    private string CheckModelType(ModelType _type)
    {
        if (_type == ModelType.ERNIE_Bot_4)
        {
            return "completions_pro";
        }
        if (_type == ModelType.ERNIE_Bot)
        {
            return "completions";
        }
        if (_type == ModelType.ERNIE_Bot_turbo)
        {
            return "eb-instant";
        }
        if (_type == ModelType.BLOOMZ_7B)
        {
            return "bloomz_7b1";
        }
        if (_type == ModelType.Qianfan_BLOOMZ_7B_compressed)
        {
            return "qianfan_bloomz_7b_compressed";
        }
        if (_type == ModelType.ChatGLM2_6B_32K)
        {
            return "chatglm2_6b_32k";
        }
        if (_type == ModelType.Llama_2_7B_Chat)
        {
            return "llama_2_7b";
        }
        if (_type == ModelType.Llama_2_13B_Chat)
        {
            return "llama_2_13b";
        }
        if (_type == ModelType.Llama_2_70B_Chat)
        {
            return "llama_2_70b";
        }
        if (_type == ModelType.Qianfan_Chinese_Llama_2_7B)
        {
            return "qianfan_chinese_llama_2_7b";
        }
        if (_type == ModelType.AquilaChat_7B)
        {
            return "aquilachat_7b";
        }
        return "";
    }


    #region Data

    [Serializable]
    private class RequestData
    {
        public List<message> messages = new List<message>();
        public bool stream = false;
        public string user_id = string.Empty;
    }
    [Serializable]
    private class message
    {
        public string role = string.Empty;
        public string content = string.Empty;
        public message() { }
        public message(string _role, string _content)
        {
            role = _role;
            content = _content;
        }
    }


    [Serializable]
    private class ResponseData
    {
        public string id = string.Empty;
        public int created;
        public int sentence_id;
        public bool is_end;
        public bool is_truncated;
        public string result = string.Empty;
        public bool need_clear_history;
        public int ban_round;
        public Usage usage = new Usage();
    }
    [Serializable]
    private class Usage
    {
        public int prompt_tokens;
        public int completion_tokens;
        public int total_tokens;
    }

    #endregion


    public enum ModelType
    {
        ERNIE_Bot_4,
        ERNIE_Bot,
        ERNIE_Bot_turbo,
        BLOOMZ_7B,
        Qianfan_BLOOMZ_7B_compressed,
        ChatGLM2_6B_32K,
        Llama_2_7B_Chat,
        Llama_2_13B_Chat,
        Llama_2_70B_Chat,
        Qianfan_Chinese_Llama_2_7B,
        AquilaChat_7B,
    }


}
