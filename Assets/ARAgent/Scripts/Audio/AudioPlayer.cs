using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


namespace MillinAR
{
    public class AudioPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource m_AudioSource;

        public delegate void EventDelegate(bool state);

        public static event EventDelegate eventHandler;

        void Start()
        {
            if (m_AudioSource == null)
            {
                m_AudioSource = transform.gameObject.AddComponent<AudioSource>();
            }
        }

        public void PlayAudio(string audioPath, Action<bool, string> callback)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        string filePath=audioPath;
        Debug.Log("AudioPlayer: Audio local path  " + filePath);
#endif

#if UNITY_EDITOR
        string filePath= audioPath;
#endif

            StartCoroutine(LoadAudio(filePath, callback));
        }

        public void PlayAudio(AudioClip audioClip, Action<bool, string> callback)
        {
            if (audioClip != null)
            {
                if (m_AudioSource.isPlaying)
                    m_AudioSource.Stop();

                m_AudioSource.clip = audioClip;
                m_AudioSource.loop = false;
                m_AudioSource.Play();
                callback(true, "Audio Playing");
                Debug.Log("AudioPlayer: Audio play started, audio lenght " + audioClip.length);
                StartCoroutine(AudioPlayFinished(audioClip.length));
            }
            else
            {
                callback(false, "Play Error");
                Debug.LogError("AudioPlayer: Failed to convert audio file to AudioClip.");
            }
        }

        private IEnumerator LoadAudio(string recordPath, Action<bool, string> callback)
        {
            using (WWW www = new WWW("file://" + recordPath))
            {
                yield return www;

                if (www.error != null)
                {
                    Debug.LogError("AudioPlayer: Failed to load audio file: " + www.error);
                    eventHandler(false);
                    yield break;
                }

                AudioClip audioClip = www.GetAudioClip();

                if (audioClip != null)
                {
                    if (m_AudioSource.isPlaying)
                        m_AudioSource.Stop();

                    m_AudioSource.clip = audioClip;
                    m_AudioSource.loop = false;
                    m_AudioSource.Play();
                    callback(true, "Audio Playing");
                    Debug.Log("AudioPlayer: Audio play started, audio lenght " + audioClip.length);
                    StartCoroutine(AudioPlayFinished(audioClip.length));
                }
                else
                {
                    callback(false, "Play Error");
                    Debug.LogError("AudioPlayer: Failed to convert audio file to AudioClip.");
                }
            }
        }

        private IEnumerator AudioPlayFinished(float time)
        {
            yield return new WaitForSeconds(time);
            Debug.Log("AudioPlayer: Audio play finshed");
            eventHandler(true);
        }


    }

}