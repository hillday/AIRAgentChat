using System;
using UnityEngine;
public class CameraIntrinisics
{
    public CameraIntrinisics(Vector2 focalLength, Vector2 principalPoint, Vector2Int resolution){
        this.focalLength = focalLength;
        this.principalPoint = principalPoint;
        this.resolution = resolution;
    }

    public Vector2 focalLength { get; }
    public Vector2 principalPoint { get; }
    public Vector2Int resolution { get; }
}