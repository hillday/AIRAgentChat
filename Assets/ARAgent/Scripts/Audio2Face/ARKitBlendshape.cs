using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MillinAR
{
    [System.Serializable]
    public class BlendRecord
    {

        public long startTime;
        public long endTime;

        public long fps;

        public ARKitBlendshapeWeight blendshapes;

        public List<HeadRotation> rotations;
    }

    [System.Serializable]
    public class HeadRotation
    {
        public float x;
        public float y;

        public float z;

        public string order;

    }

    [System.Serializable]
    public class ARKitBlendshapeWeight
    {
        public float[] _neutral;
        public float[] browDownLeft;
        public float[] browDownRight;
        public float[] browInnerUp;
        public float[] browOuterUpLeft;
        public float[] browOuterUpRight;
        public float[] cheekPuff;

        public float[] cheekSquintLeft;

        public float[] cheekSquintRight;

        public float[] eyeBlinkLeft;

        public float[] eyeBlinkRight;
        public float[] eyeLookDownLeft;

        public float[] eyeLookDownRight;
        public float[] eyeLookInLeft;
        public float[] eyeLookInRight;

        public float[] eyeLookOutLeft;

        public float[] eyeLookOutRight;
        public float[] eyeLookUpLeft;
        public float[] eyeLookUpRight;

        public float[] eyeSquintLeft;
        public float[] eyeSquintRight;

        public float[] eyeWideLeft;

        public float[] eyeWideRight;
        public float[] jawForward;
        public float[] jawLeft;
        public float[] jawOpen;
        public float[] jawRight;

        public float[] mouthClose;

        public float[] mouthDimpleLeft;

        public float[] mouthDimpleRight;
        public float[] mouthFrownLeft;
        public float[] mouthFrownRight;
        public float[] mouthFunnel;
        public float[] mouthLeft;
        public float[] mouthLowerDownLeft;

        public float[] mouthLowerDownRight;

        public float[] mouthPressLeft;

        public float[] mouthPressRight;
        public float[] mouthPucker;

        public float[] mouthRight;

        public float[] mouthRollLower;
        public float[] mouthRollUpper;

        public float[] mouthShrugLower;

        public float[] mouthShrugUpper;

        public float[] mouthSmileLeft;

        public float[] mouthSmileRight;

        public float[] mouthStretchLeft;
        public float[] mouthStretchRight;

        public float[] mouthUpperUpLeft;

        public float[] mouthUpperUpRight;

        public float[] noseSneerLeft;
        public float[] noseSneerRight;
        public float[] tongueOut;

        public Dictionary<string, float[]> blendMap;

        public void CreateMap()
        {
            blendMap = new Dictionary<string, float[]>();
            blendMap.Add("_neutral", _neutral);
            blendMap.Add("browDownLeft", browDownLeft);
            blendMap.Add("browDownRight", browDownRight);
            blendMap.Add("browInnerUp", browInnerUp);
            blendMap.Add("browOuterUpLeft", browOuterUpLeft);
            blendMap.Add("browOuterUpRight", browOuterUpRight);
            blendMap.Add("cheekPuff", cheekPuff);
            blendMap.Add("cheekSquintLeft", cheekSquintLeft);
            blendMap.Add("cheekSquintRight", cheekSquintRight);
            blendMap.Add("eyeBlinkLeft", eyeBlinkLeft);
            blendMap.Add("eyeBlinkRight", eyeBlinkRight);
            blendMap.Add("eyeLookDownLeft", eyeLookDownLeft);
            blendMap.Add("eyeLookDownRight", eyeLookDownRight);
            blendMap.Add("eyeLookInLeft", eyeLookInLeft);
            blendMap.Add("eyeLookInRight", eyeLookInRight);
            blendMap.Add("eyeLookOutLeft", eyeLookOutLeft);
            blendMap.Add("eyeLookOutRight", eyeLookOutRight);
            blendMap.Add("eyeLookUpLeft", eyeLookUpLeft);
            blendMap.Add("eyeLookUpRight", eyeLookUpRight);
            blendMap.Add("eyeSquintLeft", eyeSquintLeft);
            blendMap.Add("eyeSquintRight", eyeSquintRight);
            blendMap.Add("eyeWideLeft", eyeWideLeft);
            blendMap.Add("eyeWideRight", eyeWideRight);
            blendMap.Add("jawForward", jawForward);
            blendMap.Add("jawLeft", jawLeft);
            blendMap.Add("jawOpen", jawOpen);
            blendMap.Add("jawRight", jawRight);
            blendMap.Add("mouthClose", mouthClose);
            blendMap.Add("mouthDimpleLeft", mouthDimpleLeft);
            blendMap.Add("mouthDimpleRight", mouthDimpleRight);
            blendMap.Add("mouthFrownLeft", mouthFrownLeft);
            blendMap.Add("mouthFrownRight", mouthFrownRight);
            blendMap.Add("mouthFunnel", mouthFunnel);
            blendMap.Add("mouthLeft", mouthLeft);
            blendMap.Add("mouthLowerDownLeft", mouthLowerDownLeft);
            blendMap.Add("mouthLowerDownRight", mouthLowerDownRight);
            blendMap.Add("mouthPressLeft", mouthPressLeft);
            blendMap.Add("mouthPressRight", mouthPressRight);
            blendMap.Add("mouthPucker", mouthPucker);
            blendMap.Add("mouthRight", mouthRight);
            blendMap.Add("mouthRollLower", mouthRollLower);
            blendMap.Add("mouthRollUpper", mouthRollUpper);
            blendMap.Add("mouthShrugLower", mouthShrugLower);
            blendMap.Add("mouthShrugUpper", mouthShrugUpper);
            blendMap.Add("mouthSmileLeft", mouthSmileLeft);
            blendMap.Add("mouthSmileRight", mouthSmileRight);
            blendMap.Add("mouthStretchLeft", mouthStretchLeft);
            blendMap.Add("mouthStretchRight", mouthStretchRight);
            blendMap.Add("mouthUpperUpLeft", mouthUpperUpLeft);
            blendMap.Add("mouthUpperUpRight", mouthUpperUpRight);
            blendMap.Add("noseSneerLeft", noseSneerLeft);
            blendMap.Add("noseSneerRight", noseSneerRight);
            blendMap.Add("tongueOut", tongueOut);
        }


    }
}