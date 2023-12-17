using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WOV : MonoBehaviour
{

    protected Action<string> OnKeywordRecognizer;

    public virtual void OnBindAwakeCallBack(Action<string> _callback)
    {
        OnKeywordRecognizer += _callback;
    }

    public virtual void StartRecognizer()
    {

    }

    public virtual void StopRecognizer()
    {

    }

    protected virtual void OnAwakeOnVoice(string _msg)
    {
        if (OnKeywordRecognizer == null)
            return;

        OnKeywordRecognizer(_msg);
    }




}
