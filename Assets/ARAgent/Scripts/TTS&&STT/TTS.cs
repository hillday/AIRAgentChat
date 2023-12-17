using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TTS : MonoBehaviour
{

    [SerializeField] protected string m_PostURL = string.Empty;

    [SerializeField] protected Stopwatch stopwatch = new Stopwatch();

    public virtual void Speak(string _msg, Action<AudioClip> _callback) { }

    public virtual void Speak(string _msg, Action<AudioClip, string> _callback) { }


}
