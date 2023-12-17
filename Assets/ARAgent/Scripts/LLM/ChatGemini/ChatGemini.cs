using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class ChatGemini : LLM
{
    public ChatGemini()
    {
        url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent";
    }


    [SerializeField] private string api_key;

    [SerializeField] private string m_PromptFuncSign;
    [SerializeField] private string m_PromptNotFuncSign;
    [SerializeField] private string m_PromptForVision;

    private List<Content> m_History = new List<Content>();

    private void Start()
    {

    }


    public override void PostMsg(string _msg, Action<string> _callback)
    {
        base.PostMsg(_msg, _callback);
    }

    public override IEnumerator Request(string _postWord, System.Action<string> _callback)
    {
        stopwatch.Restart();
        string _jsonData = "";
        string _postUrl = url + "?key=" + api_key;
        if (_postWord.IndexOf("base64") != -1)
        {
            _postUrl = _postUrl.Replace("gemini-pro", "gemini-pro-vision");
            _postWord = _postWord.Substring(23);// no prefix
            RequestVisionData _postData = new RequestVisionData();
            _postData.contents.Add(new ContentVision(m_PromptForVision, "image/jpeg", _postWord));
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            _jsonData = JsonConvert.SerializeObject(_postData, settings);
            m_History.Add(new Content("user", m_PromptForVision));
        }
        else
        {
            if (m_History.Count == 0)
            {
                _postWord = "背景：" + m_Prompt +
                " 回答的语言：" + lan +
                " 接下来是我的提问：" + _postWord;
            }
            m_History.Add(new Content("user", _postWord));
            RequestData _postData = new RequestData
            {
                contents = m_History
            };
            _jsonData = JsonUtility.ToJson(_postData);
        }


        using (UnityWebRequest request = new UnityWebRequest(_postUrl, "POST"))
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(_jsonData);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(data);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.responseCode == 200)
            {
                string _msg = request.downloadHandler.text;

                ResponseData response = JsonConvert.DeserializeObject<ResponseData>(_msg);

                string _responseText = response.candidates[0].content.parts[0].text;
                m_History.Add(new Content(response.candidates[0].content.role, _responseText));

                if (_responseText.IndexOf(m_PromptNotFuncSign) != -1)
                {
                    _responseText = _responseText.Replace(m_PromptNotFuncSign, "");
                }

                if (_responseText.IndexOf(m_PromptFuncSign) != -1)
                {
                    _responseText = _responseText.Replace(m_PromptFuncSign, "");
                }

                _callback(_responseText);
            }
            else
            {
                Debug.Log("Error: " + request.error);
                if (request.responseCode != null)
                    Debug.Log("Eror Msg: " + request.downloadHandler.text);
            }

        }


        stopwatch.Stop();
        Debug.Log("Gemini chat time usage：" + stopwatch.Elapsed.TotalSeconds);
    }

    #region Data

    [Serializable]
    public class RequestData
    {
        [SerializeField] public List<Content> contents = new List<Content>();
    }

    [Serializable]
    public class RequestVisionData
    {
        [SerializeField] public List<ContentVision> contents = new List<ContentVision>();
    }

    [Serializable]
    public class Content
    {
        public string role;
        public List<PartData> parts;

        public Content() { }
        public Content(string _role, string _content)
        {
            role = _role;
            parts = new List<PartData>();
            parts.Add(new PartData(_content));
        }
    }

    [Serializable]
    public class ContentVision
    {
        public List<PartVisionData> parts;

        public ContentVision() { }

        public ContentVision(string _content, string _mime_type, string _data)
        {
            parts = new List<PartVisionData>();
            parts.Add(new PartVisionData(_content));
            parts.Add(new PartVisionData(_mime_type, _data));
        }
    }

    [Serializable]
    public class PartData
    {
        public string text;


        public PartData() { }

        public PartData(string _text)
        {
            text = _text;
        }
    }

    [Serializable]
    public class PartVisionData
    {
        public string text;

        public InlineData inline_data;

        public PartVisionData() { }

        public PartVisionData(string _text)
        {
            text = _text;
        }
        public PartVisionData(string _mime_type, string _data)
        {
            inline_data = new InlineData(_mime_type, _data);
        }
    }

    [Serializable]
    public class InlineData
    {
        public string mime_type;

        public string data;

        public InlineData() { }

        public InlineData(string _mime_type, string _data)
        {
            mime_type = _mime_type;
            data = _data;
        }

    }

    [Serializable]
    private class ResponseData
    {
        [SerializeField] public List<Candidate> candidates;

        [SerializeField] public PromptFeedback promptFeedback;
    }

    [Serializable]
    private class Candidate
    {
        public int index;

        public string finishReason;

        public List<SafetyRating> safetyRatings;

        public Content content;
    }

    [Serializable]
    private class PromptFeedback
    {
        public List<SafetyRating> safetyRatings;
    }
    [Serializable]
    private class SafetyRating
    {
        public string category;
        public string probability;
    }



    #endregion

}
