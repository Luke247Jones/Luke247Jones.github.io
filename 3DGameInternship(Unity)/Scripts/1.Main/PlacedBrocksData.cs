using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlacedBrocksData
{
    public int count;
    public string[] objectName;
    public float[] positionX;
    public float[] positionY;
    public float[] positionZ;
    public float[] rotationX;
    public float[] rotationY;
    public float[] rotationZ;
    public float[] coordinatesX;
    public float[] coordinatesY;
    public float[] coordinatesZ;

    public PlacedBrocksData(Transform[] placedBricks)
    {
        count = placedBricks.Length;
        objectName = new string[count];
        positionX = new float[count];
        positionY = new float[count];
        positionZ = new float[count];
        rotationX = new float[count];
        rotationY = new float[count];
        rotationZ = new float[count];
        coordinatesX = new float[count];
        coordinatesY = new float[count];
        coordinatesZ = new float[count];

        Debug.Log("placed count: " + count);

        for (int i = 0; i < placedBricks.Length; i++)
        {
            objectName[i] = placedBricks[i].name;
            positionX[i] = placedBricks[i].position.x;
            positionY[i] = placedBricks[i].position.y;
            positionZ[i] = placedBricks[i].position.z;
            rotationX[i] = placedBricks[i].rotation.eulerAngles.x;
            rotationY[i] = placedBricks[i].rotation.eulerAngles.y;
            rotationZ[i] = placedBricks[i].rotation.eulerAngles.z;
            var placedBrickProperties = placedBricks[i].gameObject.GetComponent<BrickProperties>();
            coordinatesX[i] = placedBrickProperties.coordinates.x;
            coordinatesY[i] = placedBrickProperties.coordinates.y;
            coordinatesZ[i] = placedBrickProperties.coordinates.z;
        }
    }
}