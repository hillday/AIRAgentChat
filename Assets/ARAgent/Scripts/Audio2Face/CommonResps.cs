using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MillinAR
{
    [System.Serializable]
    public class MessageReps<T>
    {
        public int code;

        public string message;

        public T data;
    }
    [System.Serializable]
    public class StorageFile
    {
        public string user_id;
        public string uuid;
        public string bucket;
        public string key;
        public string entity;
        public string created;
        public string format;
        public string item_type;
        public string presigned_url;

    }
}

