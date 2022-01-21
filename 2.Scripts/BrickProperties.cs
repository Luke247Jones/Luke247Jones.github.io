using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class BrickProperties : MonoBehaviour
{
    public bool isCollided = false; //Changed by ColliderDetect
    public bool isEditMode = false; //Changed by CubePlacer
    public bool isSelected = false;
    public bool isPreRender = false;
    public bool isDragging = false;
    public bool isDropped = false;
    public bool isClicked = false;
    private bool drag = false;
    private bool click = false;
    public GameObject clampedObject;
    public List<GameObject> clampedObjects;
    public List<Color> previousColors;

    public Vector3 coordinates;
    public Quaternion initialRotation;

    // Private
    private int ediState; // 0 - Rotation, 1 - Move
    private Vector3 previousPosition;
    private Vector3 previousGridCoordinate;
    private Direction[] previousLocalDirection;
    private int previousAxe;

    // Redo / Undo
    public int currentUndoPosition; // State of object
    public List<Quaternion> previousRotations = new List<Quaternion>();
    public List<Vector3> previousPositions = new List<Vector3>();
    public List<Vector3> previousGridCoordinates = new List<Vector3>();

    [SerializeField]
    //public List<Direction[]> previousLocalDirections = new List<Direction[]>();
    public List<DirectionWrapper> previousLocalDirections = new List<DirectionWrapper>();

    [System.Serializable]
    public class DirectionWrapper
    {
        public Direction[] myDirection;
    }

    DirectionWrapper newDirection = new DirectionWrapper();

    //Rotate
    public Quaternion currentRotation;
    public Quaternion originalRotation;
    public bool isRotated = false;
    public bool isRotatedUp = false;
    //private float yAdjustment = 0.2f;
    public Direction[] localDirection = new Direction[] { Direction.Up, Direction.Down, Direction.North, Direction.East, Direction.South, Direction.West };
    //public Vector3 localDirectionUp;
    private Grid grid;
    

    public void Start()
    {
        initialRotation = transform.localRotation;

        previousPositions.Add(transform.localPosition);
        previousRotations.Add(transform.localRotation);
        previousGridCoordinates.Add(coordinates);
        
        newDirection.myDirection = localDirection;
        previousLocalDirections.Add(newDirection);

        grid = FindObjectOfType<Grid>();
    }

    public void createConnectionPoints()
    {
        /*switch (gameObject.name)
        {
            
                
        }*/
    }
    public void Triggered()
    {
        if (!isCollided && isEditMode)
        {
            print("Triggered");
            StopAllCoroutines();
            isCollided = true;
            if (ediState == 0)
            {
                // reset to previous rotation
                transform.rotation = currentRotation;
                localDirection = previousLocalDirection;
                if (transform.name == "4D" && (previousAxe == 4 || previousAxe == 5))
                {
                    Move(previousPosition, previousGridCoordinate, true);
                    isRotatedUp = !isRotatedUp;
                }
                print("reset rotate");
                //ResetCollision();
            }
            else
            {
                transform.rotation = currentRotation;
                // reset to previous position
                //transform.rotation = currentRotation;
                //localDirection = previousLocalDirection;
                Move(previousPosition, previousGridCoordinate, true);
                print("reset position");
            }
        }
    }

    public void Rotate(int axe, bool delay)
    {
        currentRotation = transform.rotation;
        previousLocalDirection = localDirection;
        
        previousAxe = axe;
        isRotated = true;
        

        switch (axe)
        {
            case 0:
                transform.Rotate(0f, 90f, 0f, Space.World);
                RotateLocalDirection(axe);
                break;
            case 1:
                transform.Rotate(0f, -90f, 0f, Space.World);
                RotateLocalDirection(axe);
                break;
            case 2:
                transform.Rotate(0f, 0f, 90f, Space.World);
                break;
            case 3:
                transform.Rotate(0f, 0f, -90f, Space.World);
                break;
            case 4:
                transform.Rotate(-90f, 0f, 0f, Space.World);
                RotateLocalDirection(axe);
                break;
            case 5:
                transform.Rotate(90f, 0f, 0f, Space.World);
                RotateLocalDirection(axe);
                break;
            default:
                break;
        }

        ediState = 0;
        if (delay == true)
        {
            ResetCollision();
        }
        else
        {
            isCollided = false;

            if (!isPreRender)
            {
                StartCoroutine(Save());
                print("rotate");
            }
        }
    }
    public void RotateLocalDirection(int axe)
    {
        previousLocalDirection = localDirection;


        if (axe == 0) // Rotate Right
        {
            Direction[] newLocalDirection = new Direction[6];
            newLocalDirection[2] = localDirection[5]; // North = West
            newLocalDirection[3] = localDirection[2]; // East = North
            newLocalDirection[4] = localDirection[3]; // South = East
            newLocalDirection[5] = localDirection[4]; // West = South

            newLocalDirection[0] = localDirection[0]; // Up = Up
            newLocalDirection[1] = localDirection[1]; // Down = Down
            localDirection = newLocalDirection;
        }
        if (axe == 1) // Rotate Left
        {
            Direction[] newLocalDirection = new Direction[6];
            newLocalDirection[2] = localDirection[3]; // North = East
            newLocalDirection[3] = localDirection[4]; // East = South
            newLocalDirection[4] = localDirection[5]; // South = West
            newLocalDirection[5] = localDirection[2]; // West = North

            newLocalDirection[0] = localDirection[0]; // Up = Up
            newLocalDirection[1] = localDirection[1]; // Down = Down
            localDirection = newLocalDirection;
        }
        if (axe == 5) // Rotate forward 
        {
            Direction[] newLocalDirection = new Direction[6];
            newLocalDirection[0] = localDirection[4]; // Up = South
            newLocalDirection[2] = localDirection[0]; // North = Up
            newLocalDirection[1] = localDirection[2]; // Down = North
            newLocalDirection[4] = localDirection[1]; // South = Down

            newLocalDirection[3] = localDirection[3]; // East = East
            newLocalDirection[5] = localDirection[5]; // West = West
            localDirection = newLocalDirection;
        }
        if (axe == 4) // Rotate backward
        {
            Direction[] newLocalDirection = new Direction[6];
            newLocalDirection[0] = localDirection[2]; // Up = North
            newLocalDirection[2] = localDirection[1]; // North = Down
            newLocalDirection[1] = localDirection[4]; // Down = South
            newLocalDirection[4] = localDirection[0]; // South = Up

            newLocalDirection[3] = localDirection[3]; // East = East
            newLocalDirection[5] = localDirection[5]; // West = West
            localDirection = newLocalDirection;
        }
    }
    public void AdjustYForRotate(int axe, Vector3 gridCoordinatesUp, Vector3 gridCoordinatesDown)
    {
        previousPosition = transform.localPosition;
        previousGridCoordinate = coordinates;

        if (axe == 4 || axe == 5)
        {
            isRotatedUp = !isRotatedUp;
            //float yAdjustmentCoord = 0.0f;
            Vector3 adjustedPosUp = grid.CoordinatesToGridPoint(new Vector3(gridCoordinatesUp.x, gridCoordinatesUp.y, gridCoordinatesUp.z));
            Vector3 adjustedPosDown = grid.CoordinatesToGridPoint(new Vector3(gridCoordinatesDown.x, gridCoordinatesDown.y, gridCoordinatesDown.z));

            if (isRotatedUp)
            {
                transform.localPosition = adjustedPosUp;
                coordinates = gridCoordinatesUp;
                //print(yAdjustmentCoord);
            }
            else
            {
                transform.localPosition = adjustedPosDown;
                coordinates = gridCoordinatesDown;
                //print(yAdjustmentCoord);
            }
        }

        StartCoroutine(Save());
    }
    public Direction GetLocalDirection(Direction worldDirection)
    {
        /*switch (worldDirection)
        {
            case Direction.Up:
                return localDirection[0];
            case Direction.Down:
                return localDirection[1];
            case Direction.North:
                return localDirection[2];
            case Direction.East:
                return localDirection[3];
            case Direction.South:
                return localDirection[4];
            case Direction.West:
                return localDirection[5];
            default:
                Debug.Log("No local direction matched. Direction must be: Up,Down,North,East,South,West");
                return localDirection[0];
        }*/

        Vector3 worldDirectionVector = ConvertDirectionToVector(worldDirection);

        if (worldDirectionVector == transform.up) return Direction.Up;
        else if (worldDirectionVector == -transform.up) return Direction.Down;
        else if (worldDirectionVector == transform.forward) return Direction.North;
        else if (worldDirectionVector == transform.right) return Direction.East;
        else if (worldDirectionVector == -transform.forward) return Direction.South;
        else if (worldDirectionVector == -transform.right) return Direction.West;
        else
        {
            print("No Direction matched for :" + worldDirectionVector);
            return Direction.Up;
        }
            
    }
    public Direction GetWorldDirection(Direction locDirection)
    {
        /*int worldDirectionIndex = 0;

        for (int i = 0; i < localDirection.Count(); i++)
        {
            if (localDirection[i] == locDirection)
            {
                worldDirectionIndex = i;
            }
        }

        switch (worldDirectionIndex)
        {
            case 0:
                return Direction.Up;
            case 1:
                return Direction.Down;
            case 2:
                return Direction.North;
            case 3:
                return Direction.East;
            case 4:
                return Direction.South;
            case 5:
                return Direction.West;
            default:
                Debug.Log("No world direction matched. Direction must be: Up,Down,North,East,South,West");
                return Direction.Up;
        }*/

        Vector3 localDirectionVector = ConvertDirectionToVector(locDirection);

        if (localDirectionVector == transform.up) return Direction.Up;
        else if (localDirectionVector == -transform.up) return Direction.Down;
        else if (localDirectionVector == transform.forward) return Direction.North;
        else if (localDirectionVector == transform.right) return Direction.East;
        else if (localDirectionVector == -transform.forward) return Direction.South;
        else if (localDirectionVector == -transform.right) return Direction.West;
        else
        {
            print("No Direction matched for :" + localDirectionVector);
            return Direction.Up;
        }
    }

    public float GetDistanceFromCenter(Vector3 locSize, Direction locDirection)
    {
        if (locDirection == Direction.Up || locDirection == Direction.Down)
        {
            print("size y: " + (locSize.y/2));
            return (locSize.y / 2);
        }
        else if (locDirection == Direction.East || locDirection == Direction.West)
        {
            return locSize.x / 2;
        }
        else if (locDirection == Direction.North || locDirection == Direction.South)
        {
            return locSize.z / 2;
        }
        else
        {
            Debug.Log("No local direction matched. Direction must be: Up,Down,North,East,South,West");
            return 1.0f;
        }
    }
    private Vector3[] ConvertDirectionArrayToVectorArray(Direction[] directionArray)
    {
        Vector3[] vectorArray = new Vector3[6];
        for (int i = 0; i < directionArray.Count(); i++)
        {
            vectorArray[i] = ConvertDirectionToVector(directionArray[i]);
        }

        return vectorArray;
    }
    private Direction[] ConvertVectorArrayToDirectionArray(Vector3[] vectorArray)
    {
        Direction[] directionArray = new Direction[6];
        for (int i = 0; i < vectorArray.Count(); i++)
        {
            directionArray[i] = ConvertVectorToDirection(vectorArray[i]);
        }

        return directionArray;
    }
    public Vector3 ConvertDirectionToVector(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return Vector3.up;
            case Direction.Down:
                return Vector3.down;
            case Direction.North:
                return Vector3.forward;
            case Direction.East:
                return Vector3.right;
            case Direction.South:
                return Vector3.back;
            case Direction.West:
                return Vector3.left;
            default:
                return Vector3.up;
        }
    }
    public Direction ConvertVectorToDirection(Vector3 vector)
    {
        vector = vector.normalized;

        if (vector == Vector3.up) return Direction.Up;
        if (vector == Vector3.down) return Direction.Down;
        if (vector == Vector3.forward) return Direction.North;
        if (vector == Vector3.right) return Direction.East;
        if (vector == Vector3.back) return Direction.South;
        if (vector == Vector3.left) return Direction.West;
        else
        {
            print("No match vector: " + vector);
            return Direction.Up;
        }
    }
    public void Move(Vector3 position, Vector3 gridCoordinates, bool delay)
    {
        previousPosition = transform.localPosition;
        previousGridCoordinate = coordinates;


        coordinates = gridCoordinates;
        transform.localPosition = position;

        ediState = 1;
        if (delay == true)
        {
            ResetCollision();
        }
        else
        {
            isCollided = false;
            if (!isPreRender) StartCoroutine(Save());
        }
    }

    public void SaveInitialState()
    {
        StartCoroutine(Save());
    }
    public void Undo()
    {
        if (currentUndoPosition > 0)
        {
            currentUndoPosition--; //*
            UpdateObject();
        }
    }

    public void Redo()
    {
        if (currentUndoPosition < previousPositions.Count - 1)
        {
            currentUndoPosition++;
            UpdateObject();
        }
    }

    public void Reset()
    {
        isEditMode = false;
    }

    public void EnterEditMode()
    {
        isEditMode = true;
    }

    public void ClearHighlight()
    {
        for (int i = 0; i < previousColors.Count; i++)
        {
            GetComponent<Renderer>().materials[i].color = previousColors[i];
        }
        isEditMode = false;
        isSelected = false;
    }

    // Private

    // Move Object to previous location, rotation, coordinate
    private void UpdateObject()
    {
        print("previous positions: " + previousPositions.Count);
        print("previous rotations: " + previousRotations.Count);
        print("previous gridCoordinates: " + previousGridCoordinates.Count);
        print("previous localDirections: " + previousLocalDirections.Count);

        transform.localPosition = previousPositions[currentUndoPosition];
        transform.localRotation = previousRotations[currentUndoPosition];
        coordinates = previousGridCoordinates[currentUndoPosition];
        
        newDirection = previousLocalDirections[currentUndoPosition];
        localDirection = newDirection.myDirection;
    }

    IEnumerator ResetCollision()
    {
        yield return new WaitForSeconds(0.1f);
        isCollided = false;
    }

    // Save Object state data for location, rotation, coordinate 
    IEnumerator Save()
    {
        yield return new WaitForSeconds(0.1f);
 
        if (currentUndoPosition < previousGridCoordinates.Count - 1)
        {
            previousPositions.RemoveRange(currentUndoPosition + 1, previousPositions.Count - currentUndoPosition - 1);
            previousRotations.RemoveRange(currentUndoPosition + 1, previousRotations.Count - currentUndoPosition - 1);
            previousGridCoordinates.RemoveRange(currentUndoPosition + 1, previousGridCoordinates.Count - currentUndoPosition - 1);
            previousLocalDirections.RemoveRange(currentUndoPosition + 1, previousGridCoordinates.Count - currentUndoPosition - 1);
        }

        previousGridCoordinates.Add(coordinates);
        previousPositions.Add(transform.localPosition);
        previousRotations.Add(transform.localRotation);
        newDirection.myDirection = localDirection;
        previousLocalDirections.Add(newDirection);

        currentRotation = transform.rotation;

        currentUndoPosition = previousPositions.Count - 1; //*
    }
    
    void OnMouseDrag()
    {
        StartCoroutine(DelayDrag());
    }
    void OnMouseDown()
    {
        drag = true;
        click = true;
        print("click down");
            
    }
    void OnMouseUp()
    {
        drag = false;
        isDragging = false;
        if (click)
        {
            isClicked = true;
        }
        else
        {
            isDropped = true;
        }
        click = false;
        print("click up");

    }
    IEnumerator DelayDrag()
    {
        yield return new WaitForSeconds(0.5f);
        if (drag)
        {
            isDragging = true;
            click = false;
            print("drag");
        }
    }

    //public enum Direction { Up, Down, East, West, South, North };
}
