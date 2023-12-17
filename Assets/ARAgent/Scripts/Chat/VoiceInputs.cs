using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;


public class VoiceInputs : MonoBehaviour
{
        internal void PermissionCallbacks_PermissionDeniedAndDontAskAgain(string permissionName)
        {
                Debug.Log($"{permissionName} PermissionDeniedAndDontAskAgain");
        }

        internal void PermissionCallbacks_PermissionGranted(string permissionName)
        {
                Debug.Log($"{permissionName} PermissionCallbacks_PermissionGranted");
                RecordAudio();
        }

        internal void PermissionCallbacks_PermissionDenied(string permissionName)
        {
                Debug.Log($"{permissionName} PermissionCallbacks_PermissionDenied");
        }


        [SerializeField] public int m_RecordingLength = 5;

        public AudioClip recording;

        public void StartRecordAudio()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (Permission.HasUserAuthorizedPermission(Permission.Microphone)){
                        Debug.Log("Microphone permission has been granted.");
                        RecordAudio();
                }
                else{
                        var callbacks = new PermissionCallbacks();
                        callbacks.PermissionDenied += PermissionCallbacks_PermissionDenied;
                        callbacks.PermissionGranted += PermissionCallbacks_PermissionGranted;
                        callbacks.PermissionDeniedAndDontAskAgain += PermissionCallbacks_PermissionDeniedAndDontAskAgain;
                        Permission.RequestUserPermission(Permission.Microphone, callbacks);
                }
#else
                RecordAudio();
#endif
        }

        private void RecordAudio()
        {
                recording = Microphone.Start(null, false, m_RecordingLength, 16000);
                if (recording == null)
                {
                        Debug.Log("Recording is null, devices num:" + Microphone.devices.Length);
                }
        }



        public void StopRecordAudio(Action<AudioClip> _callback)
        {

                Microphone.End(null);
                _callback(recording);

        }

}
