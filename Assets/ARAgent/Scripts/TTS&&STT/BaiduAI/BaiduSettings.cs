using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BaiduSettings : MonoBehaviour
{
    #region Param

    public string m_API_key = string.Empty;

    public string m_Client_secret = string.Empty;

    [SerializeField] private bool m_GetTokenFromServer = true;

    public string m_Token = string.Empty;

    [SerializeField] private string m_AuthorizeURL = "https://aip.baidubce.com/oauth/2.0/token";
    #endregion


    private void Awake()
    {
        if (m_GetTokenFromServer)
        {
            StartCoroutine(GetToken(GetTokenAction));
        }

    }


    private void GetTokenAction(string _token)
    {
        m_Token = _token;
    }


    private IEnumerator GetToken(System.Action<string> _callback)
    {
        string _token_url = string.Format(m_AuthorizeURL + "?client_id={0}&client_secret={1}&grant_type=client_credentials"
            , m_API_key, m_Client_secret);

        using (UnityWebRequest request = new UnityWebRequest(_token_url, "GET"))
        {
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            yield return request.SendWebRequest();
            if (request.isDone)
            {
                string _msg = request.downloadHandler.text;
                TokenInfo _textback = JsonUtility.FromJson<TokenInfo>(_msg);
                string _token = _textback.access_token;
                _callback(_token);

            }
        }
    }


    [System.Serializable]
    public class TokenInfo
    {
        public string access_token = string.Empty;
    }
}
