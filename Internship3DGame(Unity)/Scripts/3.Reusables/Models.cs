using System;
using System.Collections.Generic;
using UnityEngine;

// API
[Serializable]
public class BlockDTO // Brock info
{
    public int count;
    public string blockName;
}

[Serializable]
public class ChallengeDTO // Challenge info
{
    public string id;
    public string name;
    public string description;
    public string imageUrl;
    public List<BlockDTO> blocks;
}

[Serializable]
public struct ChallengeResponse
{
    public ChallengeDTO[] challenges;
}

// Internal
public struct UndoAction //Undo info
{
    public int Index;
    public int Type;
    public List<GameObject> Bricks;
}

public struct ModelInfo
{
    public int baseNumber;
    public int tNumber;
    public int doubleNumber;
    public int oneDNumber;
    public int twoDNumber;
    public int threeDNumber;
    public int fourDNumber;
}

[Serializable]
public struct PlaceablePrefarb //Prefab object
{
    public string name;
    public GameObject prefarb;
}

[Serializable]
public enum Direction { Up, Down, East, West, South, North };
