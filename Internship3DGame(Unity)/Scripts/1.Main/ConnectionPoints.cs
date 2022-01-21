using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class ConnectionPoints : MonoBehaviour
{
    public string brockName;
    public Vector3 pointCoordinate;

    public Vector3 topPoint;
    public Vector3 bottomPoint;
    public Vector3 southPoint;
    public Vector3 northPoint;
    public Vector3 westPoint;
    public Vector3 eastPoint;

    public Vector3 topSouthPoint;
    public Vector3 topNorthPoint;
    public Vector3 topWestPoint;
    public Vector3 topEastPoint;

    public Vector3 bottomSouthPoint;
    public Vector3 bottomNorthPoint;
    public Vector3 bottomWestPoint;
    public Vector3 bottomEastPoint;

    public void initializePoints(Vector3 brockCoordinate, string brockName)
    {
        switch (brockName)
        {
            case "Base":
                setBasePoints(brockCoordinate);
                break;
            case "Double":
                setDoublePoints(brockCoordinate);
                break;
            case "T":
                setTPoints(brockCoordinate);
                break;
            case "1D":
                set1DPoints(brockCoordinate);
                break;
            case "2D":
                set2DPoints(brockCoordinate);
                break;
            case "3D":
                set3DPoints(brockCoordinate);
                break;
            case "4D":
                set4DPoints(brockCoordinate);
                break;
            default:
                break;
        }
    }
    private void setBasePoints(Vector3 brockCoordinate)
    {
        topPoint = new Vector3 (brockCoordinate.x, brockCoordinate.y + 1.0f, brockCoordinate.z);
        bottomPoint = new Vector3(brockCoordinate.x, brockCoordinate.y - 1.0f, brockCoordinate.z);
        northPoint = new Vector3(brockCoordinate.x, brockCoordinate.y, brockCoordinate.z + 1.0f);
        southPoint = new Vector3(brockCoordinate.x, brockCoordinate.y, brockCoordinate.z - 1.0f);
        eastPoint = new Vector3(brockCoordinate.x + 1.0f, brockCoordinate.y, brockCoordinate.z);
        westPoint = new Vector3(brockCoordinate.x - 1.0f, brockCoordinate.y, brockCoordinate.z);
    }
    private void setDoublePoints(Vector3 brockCoordinate)
    {
        topPoint = new Vector3(brockCoordinate.x, brockCoordinate.y + 2.0f, brockCoordinate.z);
        bottomPoint = new Vector3(brockCoordinate.x, brockCoordinate.y - 2.0f, brockCoordinate.z);
        northPoint = new Vector3(brockCoordinate.x, brockCoordinate.y, brockCoordinate.z + 1.0f);
        southPoint = new Vector3(brockCoordinate.x, brockCoordinate.y, brockCoordinate.z - 1.0f);
        eastPoint = new Vector3(brockCoordinate.x + 1.0f, brockCoordinate.y, brockCoordinate.z);
        westPoint = new Vector3(brockCoordinate.x - 1.0f, brockCoordinate.y, brockCoordinate.z);
    }

    private void setTPoints(Vector3 brockCoordinate)
    {
        topPoint = new Vector3(brockCoordinate.x, brockCoordinate.y + 1.0f, brockCoordinate.z);
        bottomPoint = new Vector3(brockCoordinate.x, brockCoordinate.y - 1.0f, brockCoordinate.z);
        northPoint = new Vector3(brockCoordinate.x, brockCoordinate.y, brockCoordinate.z + 1.0f);
        southPoint = new Vector3(brockCoordinate.x, brockCoordinate.y, brockCoordinate.z - 1.0f);
        eastPoint = new Vector3(brockCoordinate.x + 1.0f, brockCoordinate.y, brockCoordinate.z);
        westPoint = new Vector3(brockCoordinate.x - 1.0f, brockCoordinate.y, brockCoordinate.z);
    }
    private void set1DPoints(Vector3 brockCoordinate)
    {
        topPoint = new Vector3(brockCoordinate.x, brockCoordinate.y + 1.0f, brockCoordinate.z);
        bottomPoint = new Vector3(brockCoordinate.x, brockCoordinate.y - 1.0f, brockCoordinate.z);
        northPoint = new Vector3(brockCoordinate.x, brockCoordinate.y, brockCoordinate.z + 1.0f);
        southPoint = new Vector3(brockCoordinate.x, brockCoordinate.y, brockCoordinate.z - 1.0f);
        eastPoint = new Vector3(brockCoordinate.x + 1.0f, brockCoordinate.y, brockCoordinate.z);
        westPoint = new Vector3(brockCoordinate.x - 1.0f, brockCoordinate.y, brockCoordinate.z);
    }
    private void set2DPoints(Vector3 brockCoordinate)
    {
        topPoint = new Vector3(brockCoordinate.x, brockCoordinate.y + 1.0f, brockCoordinate.z);
        bottomPoint = new Vector3(brockCoordinate.x, brockCoordinate.y - 1.0f, brockCoordinate.z);
        northPoint = new Vector3(brockCoordinate.x, brockCoordinate.y, brockCoordinate.z + 1.0f);
        southPoint = new Vector3(brockCoordinate.x, brockCoordinate.y, brockCoordinate.z - 1.0f);
        eastPoint = new Vector3(brockCoordinate.x + 1.0f, brockCoordinate.y, brockCoordinate.z);
        westPoint = new Vector3(brockCoordinate.x - 1.0f, brockCoordinate.y, brockCoordinate.z);
    }
    private void set3DPoints(Vector3 brockCoordinate)
    {
        topPoint = new Vector3(brockCoordinate.x, brockCoordinate.y + 1.0f, brockCoordinate.z);
        bottomPoint = new Vector3(brockCoordinate.x, brockCoordinate.y - 1.0f, brockCoordinate.z);
        northPoint = new Vector3(brockCoordinate.x, brockCoordinate.y, brockCoordinate.z + 1.0f);
        southPoint = new Vector3(brockCoordinate.x, brockCoordinate.y, brockCoordinate.z - 1.0f);
        eastPoint = new Vector3(brockCoordinate.x + 1.0f, brockCoordinate.y, brockCoordinate.z);
        westPoint = new Vector3(brockCoordinate.x - 1.0f, brockCoordinate.y, brockCoordinate.z);
    }
    private void set4DPoints(Vector3 brockCoordinate)
    {
        topPoint = new Vector3(brockCoordinate.x, brockCoordinate.y + 1.0f, brockCoordinate.z);
        bottomPoint = new Vector3(brockCoordinate.x, brockCoordinate.y - 1.0f, brockCoordinate.z);
        northPoint = new Vector3(brockCoordinate.x, brockCoordinate.y, brockCoordinate.z + 1.0f);
        southPoint = new Vector3(brockCoordinate.x, brockCoordinate.y, brockCoordinate.z - 1.0f);
        eastPoint = new Vector3(brockCoordinate.x + 1.0f, brockCoordinate.y, brockCoordinate.z);
        westPoint = new Vector3(brockCoordinate.x - 1.0f, brockCoordinate.y, brockCoordinate.z);
    }
}
