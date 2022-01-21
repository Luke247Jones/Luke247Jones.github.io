using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class CubePlacer : MonoBehaviour
{
    // List objects 
    public List<Transform> placedBricks = new List<Transform>();
    public List<GameObject> clickedBricks = new List<GameObject>();
    public List<GameObject> preRenderInstances = new List<GameObject>();
    public List<GameObject> selectedBricks = new List<GameObject>();
    public List<PlaceablePrefarb> placeablePrefarbs;

    // Brick objects
    private GameObject selectedBrick;
    private GameObject preRenderBrick;

    // Grid object
    private Grid grid;

    // Text objects
    public Text edit;
    public Text copy;
    public Text switchButtonText;

    // Sprite/Button objects
    public Sprite rotate;
    public Sprite move;
    public GameObject editButtons;
    public Image editSwitchButton;
    public GameObject rotateButtons;
    public GameObject transformButtons;
    public GameObject editSwitch;
    public GameObject undoButton;
    public GameObject redoButton;
    public List<Image> allBrickImages;
    public CanvasGroup loadingPanel;
    public GameObject allBricks;
    public GameObject allObjects;

    // Challenge object
    public ChallengeSystem challengeSystem;

    // Booleans/States
    public bool isPrecise;
    public static bool isPreRenderMode;
    private bool isChallenge;
    private int mode = 0; // 0 - create, 1 - edit, 2 - delete

    // Global variables
    private Quaternion originalRotation;
    private Vector3 spawnLocation;
    public Vector3 previousMousePosition;

    // Undo / Redo
    public List<UndoAction> allUndos = new List<UndoAction>();
    public int currentUndoPosition = -1;

    // Drag and Drop
    bool isDrag = false;
    bool isDrop = false;
    public List<GameObject> movedBricks = new List<GameObject>();
    public List<GameObject> preRenderMovedBricks = new List<GameObject>();
    GameObject preRenderMovedBrick = null;

    // Delete
    bool isDelete = false;

    // Copy / Paste
    bool isCopy = false;
    bool isPaste = false;
    public List<GameObject> copiedBricks = new List<GameObject>();
    public List<GameObject> preRenderCopiedBricks = new List<GameObject>();
    private void Awake()
    {
        // Initialize asset values
        Application.targetFrameRate = 30;
        allBrickImages[0].color = new Color(0.83f, 0.83f, 0.83f);
        copy.color = new Color(0.2f, 0.2f, 0.2f);
        grid = FindObjectOfType<Grid>();
        spawnLocation = new Vector3(1000f, 1000f, 1000f);
        previousMousePosition = Input.mousePosition;
        loadingPanel.alpha = 0f;

        // Initialize brick values
        selectedBrick = placeablePrefarbs[0].prefarb;
        originalRotation = selectedBrick.transform.localRotation;
        isPreRenderMode = true;
        InitializeSelectedBrick();
        InitializePreRender();

        // Initialize Challenge values
        isChallenge = (PlayerPrefs.GetInt("challengeType") == 3);
        if (isChallenge) { challengeSystem.Setup(); }

        // Check if there is a scene being loaded
        if (SaveSystem.isLoading)
        {
            LoadObjects();
            SaveSystem.isLoading = false;
        }
        
    }

    private void Update()
    {
        // Check for input
        InputListener();

        // Performance: if change in prevoius mouse position > 50 pixels, then skip raycast
        Vector3 currentMousePosition = Input.mousePosition;
        if ((Mathf.Abs(currentMousePosition.x) - Mathf.Abs(previousMousePosition.x)) > 50)
        {
            previousMousePosition = currentMousePosition;
            return;
        }
        if ((Mathf.Abs(currentMousePosition.y) - Mathf.Abs(previousMousePosition.y)) > 50)
        {
            previousMousePosition = currentMousePosition;
            return;
        }

        // Check if pointer is over UI Event System Object
        if (EventSystem.current.IsPointerOverGameObject())
        {
            preRenderBrick.SetActive(false);
            if (preRenderCopiedBricks != null)
            {
                for (int i = 0; i < preRenderCopiedBricks.Count; i++)
                {
                    preRenderCopiedBricks[i].SetActive(false);
                }
            }

            return;
        }

        // Initialize ray for click and hover 
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Run Cubeplacer code based on mode
        if (mode == 0)
        {
            HandleCreateMode(ray);
        }
        else
        {
            HandleSelectMode(ray);
        }

        //Set previous mouse position to current
        previousMousePosition = currentMousePosition;
    }

    // Listen for all inputs
    private void InputListener()
    {
        // Escape to menu
        if (Input.GetKeyDown(KeyCode.Escape)) BackToMainMenu();

        // Pre-Render toggle On/Off
        if (Input.GetKeyDown(KeyCode.P))
        {
            // toggle pre-render mode
            isPreRenderMode = !isPreRenderMode;
            if (isPreRenderMode == false)
            {
                // turn off all pre-render bricks
                preRenderBrick.SetActive(false);
                for (int i = 0; i < preRenderCopiedBricks.Count; i++)
                {
                    preRenderCopiedBricks[i].SetActive(false);
                }
            }
            else
            {
                // turn on all pre-render bricks
                preRenderBrick.SetActive(true);
                for (int i = 0; i < preRenderCopiedBricks.Count; i++)
                {
                    preRenderCopiedBricks[i].SetActive(true);
                }
            }
        }

        // Undo
        if (Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKey(KeyCode.Z))
        {
            Undo();
        }
        //Redo
        if (Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKey(KeyCode.Y))
        {
            Redo();
        }

        //Rotate
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Rotate(1);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Rotate(0);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Rotate(5);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Rotate(4);
        }


        if (mode == 0) { return; }


        // Check all bricks for action
        for (int i = 0; i < placedBricks.Count; i++)
        {
            var brickProperties = placedBricks[i].GetComponent<BrickProperties>();

            // Drag action
            if (brickProperties.isDragging)
            {
                if (isPaste == false)
                {
                    if (!clickedBricks.Contains(placedBricks[i].gameObject))
                    {
                        Select(placedBricks[i].gameObject);
                    }
                    SetupDrag(); // First run              
                }
                else
                {
                    brickProperties.isDragging = false;
                }
                isDrag = true; // keep drag true until brick is dropped

            }
            // Drop action
            else if (brickProperties.isDropped)
            {
                if (isPaste == false)
                {
                    SetupDrop();
                    brickProperties.isDropped = false;
                }
                else
                {
                    brickProperties.isDropped = false;
                }
                isDrop = true;
            }
            // Click action
            if (brickProperties.isClicked)
            {
                if (isPaste == false)
                {
                    Select(placedBricks[i].gameObject);
                    brickProperties.isClicked = false;
                }
                else
                {
                    brickProperties.isClicked = false;
                }
            }

        }

        // Paste
        if (isPaste)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isPreRenderMode = false;
                for (int j = 0; j < preRenderCopiedBricks.Count; j++)
                {
                    preRenderCopiedBricks[j].SetActive(false);
                }
            }
            else
            {
                isPreRenderMode = true;
                for (int j = 0; j < preRenderCopiedBricks.Count; j++)
                {
                    preRenderCopiedBricks[j].SetActive(true);
                }
            }
        }

        //Delete
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            isDelete = true;
        }

        // Copy
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.C))
        {
            SwitchCopyPaste();
        }

    }
    private void ResetInput() // Reset all input values to default
    {
        isCopy = false;
        isDelete = false;
        isPaste = false;
        isDrag = false;
        isDrop = false;

        copiedBricks.Clear();
        preRenderCopiedBricks.Clear();
        clickedBricks.Clear();

        // Check all blocks for action
        for (int i = 0; i < placedBricks.Count; i++)
        {
            var brickProperties = placedBricks[i].GetComponent<BrickProperties>();

            // Drag
            if (brickProperties.isDragging)
            {
                brickProperties.isDragging = false;

            }
            // Drop
            if (brickProperties.isDropped)
            {
                brickProperties.isDropped = false;
            }
            // Select
            if (brickProperties.isClicked)
            {
                brickProperties.isClicked = false;
            }
        }
    }

    // Setup Clicked Bricks for Drag
    private void SetupDrag()
    {
        isDrag = true;
        isPreRenderMode = true;
        movedBricks = clickedBricks;

        // Create the pre render moved bricks for dragging 
        if (preRenderMovedBricks.Count == 0)
        {
            for (int i = 0;  i < clickedBricks.Count; i++)
            {
                var preRenderMoved = Instantiate(clickedBricks[i], spawnLocation, clickedBricks[i].transform.rotation);
                preRenderMoved = ConvertToPreRender(preRenderMoved);
                preRenderMovedBricks.Add(preRenderMoved);
            }

            if (preRenderMovedBricks.Count == 1)
            {
                preRenderMovedBrick = preRenderMovedBricks[0];
                var clickedBrickProperties = clickedBricks[0].GetComponent<BrickProperties>();
                preRenderMovedBrick.GetComponent<BrickProperties>().isRotated = clickedBrickProperties.isRotated;
            }
        }
        

    }

    // Setup Clicked Bricks for Drop
    private void SetupDrop()
    {
        isDrop = true;
        isDrag = false;
        isPreRenderMode = false;

        // Save rotation if pre-render was rotated
        if (clickedBricks.Count == 1)
        {  
            clickedBricks[0].transform.rotation = preRenderMovedBrick.transform.rotation;
        }
        if (clickedBricks.Count > 1)
        {
            for (int i = 0; i < clickedBricks.Count; i++)
            {
                clickedBricks[i].transform.rotation = preRenderMovedBricks[i].transform.rotation;
            }
        }
    }
    private void HandleCreateMode(Ray ray)
    {
        // Create Brick when mouse is clicked
        if (Input.GetMouseButtonDown(0))
        {
            // Set Pre Render mode to false 
            isPreRenderMode = false;
            preRenderBrick.SetActive(false);
            for (int i = 0; i < preRenderCopiedBricks.Count; i++)
            {
                preRenderCopiedBricks[i].SetActive(false);
            }
            InitializeSelectedBrick();
            FilterRay(ray);

        }
        // Pre-Render Brick when mouse is hovering
        else if (isPreRenderMode)
        {
            preRenderBrick.SetActive(true);
            FilterRay(ray);
        }
    }
    private void HandleSelectMode(Ray ray)
    {
        if (isDelete)
        {
            isPreRenderMode = false;
            Delete();
            isDelete = false;
            Debug.Log("delete brick(s)");
        }
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            if (hitInfo.transform.gameObject != null)
            {
                // Get hit object
                var hitObject = hitInfo.transform.gameObject;

                // Get Direction of clicked Object. Calculates direction relative to camera
                Direction direction = GetDirection(hitInfo.normal - Vector3.up);

                if (hitObject.GetComponent<BrickProperties>() != null)
                {
                    var hitObjectProperties = hitObject.GetComponent<BrickProperties>();
                    //direction = GetDirection(hitInfo.normal - hitObjectProperties.localDirectionUp);
                    print("world direction: " + direction);
                }
                //var hitObjectProperties = hitObject.GetComponent<BrickProperties>();
                //var normalizedRotation = hitObject.transform.rotation.normalized;
                //var normalizedVertical = GetLocalCoordinates(normalizedRotation * Vector3.up, hitObjectProperties.isRotatedUp);
                //print("normalized rotation: " + normalizedRotation);
                //print("normalized vertical: " + normalizedVertical);
                //print("hitInfo normal: " + hitInfo.normal);
                
                

                // Get parent if hit object is child Collider
                if (hitObject.name == "Collider")
                {
                    hitObject = hitInfo.transform.parent.gameObject;
                }

                if (isPaste)
                {
                    print("paste");
                    if (copiedBricks.Count == 1)
                    {
                        //var copiedBrickProperties = copiedBricks[0].GetComponent<BrickProperties>();
                        var preRenderCopiedBrick = preRenderCopiedBricks[0];

                        // Remove clamped objects

                        //preRenderCopiedBrick.GetComponent<BrickProperties>().isRotated = copiedBrickProperties.isRotated;

                        // Place pasted brock
                        if (isPreRenderMode)
                        {
                            Place(preRenderCopiedBrick, hitObject, hitInfo.point, direction);
                        }
                        else
                        {
                            var newPastedBrick = Instantiate(preRenderCopiedBricks[0].gameObject, spawnLocation, preRenderCopiedBricks[0].transform.rotation);
                            newPastedBrick = ConvertToSolid(newPastedBrick);
                            Place(newPastedBrick, hitObject, hitInfo.point, direction);
                        }
                    }
                    else
                    {
                        PasteAtPoint(hitInfo.point);
                    }


                }
                else if (isDrag)
                {
                    isPreRenderMode = true;
                    if (movedBricks.Count == 1)
                    { 
                        // Place
                        Place(preRenderMovedBrick, hitObject, hitInfo.point, direction);
                        print(preRenderMovedBrick.transform.rotation);
                    }
                    else
                    {
                        Move(hitInfo.point);
                    }
                    isDrag = false; //*?
                }
                else if (isDrop)
                {

                    if (clickedBricks.Count == 1)
                    {
                        Place(clickedBricks[0], hitObject, hitInfo.point, direction);

                    }
                    else
                    {
                        Move(hitInfo.point);
                    }

                    isDrop = false;
                    for (int i = 0; i < preRenderMovedBricks.Count; i++)
                    {
                        preRenderMovedBricks[i].SetActive(false);
                    }
                    preRenderMovedBrick = null;
                    preRenderMovedBricks.Clear();
                    clickedBricks.Clear();
                    
                }
                else if (!hitObject.CompareTag("brick") && !hitObject.CompareTag("prop"))
                {
                    return;
                }
            }
        }
    }
    private GameObject ConvertToPreRender(GameObject preRenderBrick)
    {
        // Set pre render material
        if (!preRenderBrick.CompareTag("prop"))
        {
            Material transparentMaterial = Resources.Load<Material>("_1_Transparent");
            preRenderBrick.GetComponent<Renderer>().sharedMaterial = transparentMaterial;
        }
        // Set pre render components
        preRenderBrick.GetComponent<BrickProperties>().isPreRender = true;
        preRenderBrick.GetComponent<MeshRenderer>().receiveShadows = false;
        preRenderBrick.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        preRenderBrick.layer = 2;
        foreach (Transform child in preRenderBrick.transform)
        {
            child.gameObject.layer = 2;
        }

        preRenderBrick.name = preRenderBrick.name.Replace("(Clone)", "");

        return preRenderBrick;
    }
    private GameObject ConvertToSolid(GameObject solidBrick)
    {
        // Set solid material
        if (!solidBrick.CompareTag("prop"))
        {
            Material solidMaterial = Resources.Load<Material>("_1_Solid");
            solidBrick.GetComponent<Renderer>().sharedMaterial = solidMaterial;
        }
        // Set solid components
        solidBrick.GetComponent<BrickProperties>().isPreRender = false;
        solidBrick.GetComponent<MeshRenderer>().receiveShadows = true;
        solidBrick.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        solidBrick.layer = 0;
        solidBrick.SetActive(true);

        foreach (Transform child in solidBrick.transform)
        {
            child.gameObject.layer = 0;
        }

        solidBrick.name = solidBrick.name.Replace("(Clone)", "");

        return solidBrick;
    }
    private void Place(GameObject selectBrick, GameObject hitObject, Vector3 hitPoint, Direction direction)
    {

        // Place on plane if clicked Object is not brick/prop
        if (!hitObject.CompareTag("brick") && !hitObject.CompareTag("prop"))
        {
            // Place cub slightly above plane
            PlaceSelectedCubeOnPlane(selectBrick, new Vector3(hitPoint.x, hitPoint.y + 0.1f, hitPoint.z));
            return;
        }
        else
        {
            PlaceSelectedCubeOnObject(selectBrick, hitObject, hitPoint, direction);
        }
    }
    private void PlaceSelectedCubeOnPlane(GameObject selectBrick, Vector3 hitPoint)
    {
        // Check if challenge limit is not exceded
        if (isChallenge && !challengeSystem.CheckForLimit(selectBrick.name)) { return; }

        // Get position based on grid point closest to hit point
        Vector3 coordinates = grid.GetGridCoordinates(hitPoint);
        Vector3 finalPosition = grid.GetNearestPointOnGrid(hitPoint);

        // Original position
        var brickProperties = selectBrick.GetComponent<BrickProperties>();
        var currentCoordinates = brickProperties.coordinates;
        var currentPosition = selectBrick.transform.position;
        var currentRotation = selectBrick.transform.rotation;
        var currentLocalDirection = brickProperties.localDirection;

        // Change position based on prop
        if (selectBrick.CompareTag("prop"))
        {
            coordinates = GetCoordinates(Direction.Down, coordinates, 0.5f);
            finalPosition = grid.CoordinatesToGridPoint(coordinates);
        }
        if (selectBrick.name == "4D" && brickProperties.isRotatedUp == true)
        {
            //coordinates = GetCoordinates(Direction.Up, coordinates, 1.0f);
        }

        // Create brick
        if (isPreRenderMode)
        {
            selectBrick.transform.position = finalPosition;
            if (!brickProperties.isRotated)
            {
                selectBrick.transform.rotation = brickProperties.initialRotation;
            }
        }
        else
        {
            // List to hold Undo
            //List<GameObject> newPlacedBricks = new List<GameObject>();

            selectBrick.name = selectBrick.name.Replace("(Clone)", "");

            // Move new brick coordinate to grid coordinate
            if (brickProperties != null)
            {
                brickProperties.coordinates = coordinates;
            }

            var clickedBrick = selectBrick;
            //var newCoordinates = grid.CoordinatesToGridPoint(coordinates);

            brickProperties.coordinates = coordinates;
            brickProperties.Move(finalPosition, coordinates, false);
            //brickProperties.ClearHighlight();

            //Add to moved bricks
            //newPlacedBricks.Add(clickedBrick);

            //Add to placed bricks
            GameObject placedBrick = selectBrick;

            StartCoroutine(CheckIfSelectedCubeCollides(brickProperties, null, false, placedBrick, currentPosition, currentCoordinates, currentRotation, currentLocalDirection));

        }
        
    }
    
    private void PlaceSelectedCubeOnObject(GameObject selectBrick, GameObject hitObject, Vector3 hitPoint, Direction direction)
    {
        // Get transform rotation
        var hitObjectRotation = hitObject.transform.localRotation;

        // Get properties from clicked Object (clampedObjects, coordinates)
        var hitObjectProperties = hitObject.GetComponent<BrickProperties>();

        List<GameObject> clampedObjects = new List<GameObject>(); //initialize
        Vector3 gridCoordinates = Vector3.one; //initialize

        if (hitObjectProperties != null)
        {
            gridCoordinates = hitObjectProperties.coordinates;
        }

        // Get hit object's face coordinate using hit direction with offset from center: 1 for bricks, 0.5 for props (0.5 = more precision)
        Vector3 newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);

        if (selectBrick.CompareTag("prop"))
        {
            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.5f);
        }

        // Initialize 
        Quaternion objectRotation = hitObjectRotation;
        bool clamp = false;

        // Get local direction
        Direction coordinates = hitObjectProperties.GetLocalDirection(direction);
        print("local direction: " + coordinates);


        // Different directions Object could be in
        Direction cODirectionUp = Direction.Up;
        Direction cODirectionDown = Direction.Down;
        Direction cODirection = Direction.North;
        Direction cODirectionReverse = Direction.South;
        Direction cODirectionRight = Direction.East;
        Direction cODirectionLeft = Direction.West;

        // Get distance from edge to center of Object
        var size = selectBrick.GetComponent<BoxCollider>().size;
        var distanceFromCenter = grid.ConvertToGridDistance(selectBrick.GetComponent<BrickProperties>().GetDistanceFromCenter(size, coordinates));

        // Connecting logic for Selected brick and Hit Object
        switch (hitObject.name)
        {
            case "Base":
                switch (selectBrick.name)
                {
                    case "4D":
                        newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.5f);
                        break;
                    case "3D":
                        newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.5f);
                        break;
                    case "2D":
                        newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.5f);
                        break;
                    case "1D":
                        newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.5f);
                        break;
                    case "T":
                        if (coordinates == Direction.Up || coordinates == Direction.Down)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, hitObject.transform.up) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.67f);
                        }
                        else
                        {
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.5f);
                            Vector3 connectionPointIn1 = GetCoordinates(hitObjectProperties.ConvertVectorToDirection(hitObject.transform.up), newGridCoordinates, 0.0f);
                            Vector3 connectionPointIn2 = GetCoordinates(hitObjectProperties.ConvertVectorToDirection(hitObject.transform.up), newGridCoordinates, 0.5f);
                            Vector3 hitPointCoordinate = grid.GetGridPoint(hitPoint);
                            print("hit point coord: " + hitPointCoordinate);

                            if (IsPointInRadius(hitPointCoordinate, connectionPointIn1, 0.25f) || IsPointInRadius(hitPointCoordinate, connectionPointIn2, 0.25f))
                            {
                                newGridCoordinates = GetCoordinates(hitObjectProperties.ConvertVectorToDirection(hitObject.transform.up), gridCoordinates, 0.5f);
                                objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                                print("coonection point in: " + connectionPointIn2);
                            }
                            else
                            {
                                newGridCoordinates = GetCoordinates(hitObjectProperties.ConvertVectorToDirection(hitObject.transform.up), gridCoordinates, 0.5f);
                                newGridCoordinates = GetCoordinates(direction, newGridCoordinates, 0.67f);
                                objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                                print("coonection point out: " + newGridCoordinates);
                            }
                        }
                        break;
                    case "Double":
                        if (coordinates == Direction.Down)
                        {
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 2.0f);
                        }
                        break;
                    default:
                        break;
                }
                break;

            case "T":
                switch (selectBrick.name)
                {
                    case "T":
                        if (coordinates == Direction.North)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObject.transform.forward) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.0f);
                        }
                        else if (coordinates == Direction.South)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObject.transform.forward) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                        }
                        break;
                    case "1D":
                        if (coordinates == Direction.North)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.up) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.0f);
                        }
                        break;
                    case "2D":
                        if (coordinates == Direction.Up || coordinates == Direction.Down || coordinates == Direction.North)
                        {
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                        }
                        else
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObject.transform.right) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                        }
                        break;
                    case "3D":
                        if (coordinates == Direction.Up || coordinates == Direction.Down || coordinates == Direction.North)
                        {
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                        }
                        else
                        {
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                        }
                        break;
                    case "4D":
                        if (coordinates == Direction.Up || coordinates == Direction.Down || coordinates == Direction.North)
                        {
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                        }
                        else
                        {
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 2.0f);
                        }
                        break;
                    case "Base":
                        if (coordinates == cODirectionUp || coordinates == cODirectionDown)
                        {
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.5f);
                            if (coordinates == cODirectionUp)
                            {
                                if (hitObjectRotation.x > 0.7 && hitObjectRotation.x < 0.8)
                                {
                                    objectRotation = hitObject.transform.rotation * Quaternion.AngleAxis(180, hitObject.transform.up);
                                }
                                else
                                {
                                    objectRotation = hitObject.transform.rotation * Quaternion.AngleAxis(180, hitObject.transform.forward);
                                }
                            }
                        }
                        else if (coordinates == cODirection)
                        {
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.5f);
                            objectRotation = hitObject.transform.rotation * Quaternion.AngleAxis(90, hitObject.transform.right);
                        }
                        else if (coordinates == Direction.East || coordinates == Direction.West)
                        {
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.67f);
                            //objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.up) * objectRotation;

                            Vector3 connectionPointUp = GetCoordinates(hitObjectProperties.ConvertVectorToDirection(hitObject.transform.up), newGridCoordinates, 0.5f);
                            Vector3 connectionPointDown = GetCoordinates(hitObjectProperties.ConvertVectorToDirection(-hitObject.transform.up), newGridCoordinates, 0.5f);
                            Vector3 hitPointCoordinate = grid.GetGridPoint(hitPoint);
                            print("hit point coord: " + hitPointCoordinate);

                            if (IsPointInRadius(hitPointCoordinate, connectionPointUp, 0.5f))
                            {
                                newGridCoordinates = connectionPointUp;
                                objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.up) * objectRotation;
                            }
                            else
                            {
                                newGridCoordinates = connectionPointDown;
                                objectRotation = Quaternion.FromToRotation(hitObject.transform.up, hitObject.transform.up) * objectRotation;
                            }

                        }
                        break;
                    case "Double":
                        if (coordinates == cODirectionUp || coordinates == cODirectionDown)
                        {
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                            if (coordinates == cODirectionUp)
                            {
                                if (hitObjectRotation.x > 0.7 && hitObjectRotation.x < 0.8)
                                {
                                    objectRotation = hitObject.transform.rotation * Quaternion.AngleAxis(180, hitObject.transform.up);
                                }
                                else
                                {
                                    objectRotation = hitObject.transform.rotation * Quaternion.AngleAxis(180, hitObject.transform.forward);
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
                break;

            case "1D":
                switch (selectBrick.name)
                {
                    case "T":
                        if (coordinates == Direction.North)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.up) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.0f);
                        }
                        break;
                    case "1D":
                        if (coordinates == Direction.North)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.up) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.0f);
                        }
                        break;
                    case "2D":
                        if (coordinates == Direction.Up || coordinates == Direction.Down || coordinates == Direction.North)
                        {
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                        }
                        else
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObject.transform.forward) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 2.0f);
                        }
                        break;
                    case "3D":
                        if (coordinates == Direction.Up || coordinates == Direction.Down || coordinates == Direction.North)
                        {
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                        }
                        else
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObject.transform.forward) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 2.0f);
                        }
                        break;
                    case "4D":
                        if (coordinates == Direction.Up || coordinates == Direction.Down || coordinates == Direction.North)
                        {
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                        }
                        else
                        {
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 2.0f);
                        }
                        break;
                    case "Base":
                        if (coordinates == Direction.Up || coordinates == Direction.Down)
                        {
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.5f); // distance from center
                            print("distance from center: " + distanceFromCenter);

                            if (coordinates == Direction.Up)
                            {
                                // Flip rotation from facing up to facing down
                                objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.up) * objectRotation;
                                print("euler rotation: " + objectRotation);
                            }

                            clamp = true;

                            Vector3 connectionPointSouth = newGridCoordinates + (hitObject.transform.forward * -1);
                            Vector3 hitPointCoordinate = grid.GetGridCoordinates(hitPoint);
                            print("hit point coord: " + hitPointCoordinate);

                            if (IsPointInRadius(hitPointCoordinate, connectionPointSouth, 0.7f)) newGridCoordinates = connectionPointSouth;

                        }
                        else if (coordinates == Direction.North)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.up, hitObject.transform.forward) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.5f);
                        }
                        else if (coordinates == Direction.South)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.forward) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                        }
                        break;
                    case "Double":
                        if (coordinates == Direction.Up || coordinates == Direction.Down)
                        {
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f); // distance from center
                            print("distance from center: " + distanceFromCenter);

                            if (coordinates == Direction.Up)
                            {
                                // Flip rotation from facing up to facing down
                                objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.up) * objectRotation;
                                print("euler rotation: " + objectRotation);
                            }

                            clamp = true;

                            Vector3 connectionPointSouth = newGridCoordinates + (hitObject.transform.forward * -1);
                            Vector3 hitPointCoordinate = grid.GetGridCoordinates(hitPoint);
                            print("hit point coord: " + hitPointCoordinate);

                            if (IsPointInRadius(hitPointCoordinate, connectionPointSouth, 0.7f)) newGridCoordinates = connectionPointSouth;

                        }
                        else if (coordinates == Direction.North)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.up, hitObject.transform.forward) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.5f);
                        }
                        else if (coordinates == Direction.South)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.forward) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                        }
                        break;
                    default:
                        break;
                }
                break;

            case "2D":
                switch (selectBrick.name)
                {
                    case "T":
                        if (coordinates == Direction.North)
                        {
                            //objectRotation = Quaternion.FromToRotation(hitObject.transform.up, hitObject.transform.up) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                        }
                        else if (coordinates == Direction.East)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, hitObject.transform.right) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                        }
                        else if (coordinates == Direction.West)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, hitObject.transform.right) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                        }
                        else
                        {
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                        }
                        break;
                    case "1D":
                        if (coordinates != Direction.Up && coordinates != Direction.Down && coordinates != Direction.North && coordinates != Direction.East)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                        }
                        break;
                    case "2D":
                        if (coordinates != Direction.Up && coordinates != Direction.Down)
                        {
                            objectRotation = Quaternion.FromToRotation(-hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 2.0f);
                        }
                        if (coordinates == Direction.North)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                        }
                        if (coordinates == Direction.East)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.right, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                        }
                        break;
                    case "3D":
                        if (coordinates != Direction.Up && coordinates != Direction.Down)
                        {
                            objectRotation = Quaternion.FromToRotation(-hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 2.0f);
                        }
                        if (coordinates == Direction.North)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                        }
                        if (coordinates == Direction.East)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.right, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                        }
                        break;
                    case "4D":
                        if (coordinates != Direction.Up && coordinates != Direction.Down)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 2.0f);
                        }
                        break;
                    case "Base":
                        if (coordinates == Direction.Up || coordinates == Direction.Down)
                        {
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.5f); // distance from center
                            print("distance from center: " + distanceFromCenter);

                            if (coordinates == Direction.Up)
                            {
                                // Flip rotation from facing up to facing down
                                objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.up) * objectRotation;
                                print("euler rotation: " + objectRotation);
                            }

                            clamp = true;

                            Vector3 connectionPointSouth = newGridCoordinates + (hitObject.transform.forward * -1);
                            Vector3 connectionPointWest = newGridCoordinates + (hitObject.transform.right * -1);
                            Vector3 hitPointCoordinate = grid.GetGridCoordinates(hitPoint);
                            print("hit point coord: " + hitPointCoordinate);

                            if (IsPointInRadius(hitPointCoordinate, connectionPointSouth, 0.7f)) newGridCoordinates = connectionPointSouth;
                            else if (IsPointInRadius(hitPointCoordinate, connectionPointWest, 0.7f)) newGridCoordinates = connectionPointWest;

                        }
                        else if (coordinates == Direction.West)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.right) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                        }
                        else if (coordinates == Direction.South)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.forward) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                        }
                        break;
                    case "Double":
                        if (coordinates == Direction.Up || coordinates == Direction.Down)
                        {
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f); // distance from center
                            print("distance from center: " + distanceFromCenter);

                            if (coordinates == Direction.Up)
                            {
                                // Flip rotation from facing up to facing down
                                objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.up) * objectRotation;
                                print("euler rotation: " + objectRotation);
                            }

                            clamp = true;

                            Vector3 connectionPointSouth = newGridCoordinates + (hitObject.transform.forward * -1);
                            Vector3 connectionPointWest = newGridCoordinates + (hitObject.transform.right * -1);
                            Vector3 hitPointCoordinate = grid.GetGridCoordinates(hitPoint);
                            print("hit point coord: " + hitPointCoordinate);

                            if (IsPointInRadius(hitPointCoordinate, connectionPointSouth, 0.7f)) newGridCoordinates = connectionPointSouth;
                            else if (IsPointInRadius(hitPointCoordinate, connectionPointWest, 0.7f)) newGridCoordinates = connectionPointWest;

                        }
                        else if (coordinates == Direction.West)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.right) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                        }
                        else if (coordinates == Direction.South)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.forward) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                        }
                        break;
                }
                break;

            case "3D":
                switch (selectBrick.name)
                {
                    case "T":
                        if (coordinates == Direction.North)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.up, hitObject.transform.up) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                        }
                        else if (coordinates == Direction.East)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObject.transform.right) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                        }
                        else if (coordinates == Direction.West)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, hitObject.transform.right) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                        }
                        else
                        {
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                        }
                        break;
                    case "1D":
                        if (coordinates != Direction.Up && coordinates != Direction.Down && coordinates != Direction.North)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                        }
                        break;
                    case "2D":
                        if (coordinates != Direction.Up && coordinates != Direction.Down)
                        {
                            objectRotation = Quaternion.FromToRotation(-hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 2.0f);
                        }
                        if (coordinates == Direction.North)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                        }
                        break;
                    case "3D":
                        if (coordinates != Direction.Up && coordinates != Direction.Down)
                        {
                            objectRotation = Quaternion.FromToRotation(-hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 2.0f);
                        }
                        if (coordinates == Direction.North)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                        }
                        break;
                    case "4D":
                        if (coordinates != Direction.Up && coordinates != Direction.Down)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 2.0f);
                        }
                        break;
                    case "Base":
                        if (coordinates == Direction.Up || coordinates == Direction.Down)
                        {
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.5f); // distance from center
                            print("distance from center: " + distanceFromCenter);

                            if (coordinates == Direction.Up)
                            {
                                // Flip rotation from facing up to facing down
                                objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.up) * objectRotation;
                                print("euler rotation: " + objectRotation);
                            }

                            clamp = true;

                            Vector3 connectionPointEast = newGridCoordinates + (hitObject.transform.right);
                            Vector3 connectionPointSouth = newGridCoordinates + (hitObject.transform.forward * -1);
                            Vector3 connectionPointWest = newGridCoordinates + (hitObject.transform.right * -1);
                            Vector3 hitPointCoordinate = grid.GetGridCoordinates(hitPoint);
                            print("hit point coord: " + hitPointCoordinate);

                            if (IsPointInRadius(hitPointCoordinate, connectionPointEast, 0.7f)) newGridCoordinates = connectionPointEast;
                            else if (IsPointInRadius(hitPointCoordinate, connectionPointSouth, 0.7f)) newGridCoordinates = connectionPointSouth;
                            else if (IsPointInRadius(hitPointCoordinate, connectionPointWest, 0.7f)) newGridCoordinates = connectionPointWest;

                        }
                        else if (coordinates == Direction.East)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.up, hitObject.transform.right) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                        }
                        else if (coordinates == Direction.West)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.right) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                        }
                        else if (coordinates == Direction.South)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.forward) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                        }
                        break;
                    case "Double":
                        if (coordinates == Direction.Up || coordinates == Direction.Down)
                        {
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f); // distance from center
                            print("distance from center: " + distanceFromCenter);

                            if (coordinates == Direction.Up)
                            {
                                // Flip rotation from facing up to facing down
                                objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.up) * objectRotation;
                                print("euler rotation: " + objectRotation);
                            }

                            clamp = true;

                            Vector3 connectionPointEast = newGridCoordinates + (hitObject.transform.right);
                            Vector3 connectionPointSouth = newGridCoordinates + (hitObject.transform.forward * -1);
                            Vector3 connectionPointWest = newGridCoordinates + (hitObject.transform.right * -1);
                            Vector3 hitPointCoordinate = grid.GetGridCoordinates(hitPoint);
                            print("hit point coord: " + hitPointCoordinate);

                            if (IsPointInRadius(hitPointCoordinate, connectionPointEast, 0.7f)) newGridCoordinates = connectionPointEast;
                            else if (IsPointInRadius(hitPointCoordinate, connectionPointSouth, 0.7f)) newGridCoordinates = connectionPointSouth;
                            else if (IsPointInRadius(hitPointCoordinate, connectionPointWest, 0.7f)) newGridCoordinates = connectionPointWest;

                        }
                        else if (coordinates == Direction.East)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.up, hitObject.transform.right) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                        }
                        else if (coordinates == Direction.West)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.right) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                        }
                        else if (coordinates == Direction.South)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.forward) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                        }
                        break;
                }
                break;

            case "4D":
                switch (selectBrick.name)
                {
                    case "T":
                        if (coordinates == Direction.North)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.up) * objectRotation;
                        }
                        else if (coordinates == Direction.East)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObject.transform.right) * objectRotation;
                        }
                        else if (coordinates == Direction.West)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, hitObject.transform.right) * objectRotation;
                        }
                        
                        newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);

                        break;
                    case "1D":
                        if (coordinates != Direction.Up && coordinates != Direction.Down)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                        }
                        break;
                    case "2D":
                        if (coordinates != Direction.Up && coordinates != Direction.Down)
                        {
                            objectRotation = Quaternion.FromToRotation(-hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 2.0f);
                        }
                        break;
                    case "3D":
                        if (coordinates != Direction.Up && coordinates != Direction.Down)
                        {
                            objectRotation = Quaternion.FromToRotation(-hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 2.0f);
                        }
                        break;
                    case "4D":
                        if (coordinates != Direction.Up && coordinates != Direction.Down)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 2.0f);
                        }
                        break;
                    case "Base":
                        if (coordinates == Direction.Up || coordinates == Direction.Down)
                        {
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.5f); // distance from center
                            print("distance from center: " + distanceFromCenter);

                            if (coordinates == Direction.Up)
                            {
                                // Flip rotation from facing up to facing down
                                objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.up) * objectRotation;
                                print("euler rotation: " + objectRotation);
                            }

                            clamp = true;

                            Vector3 connectionPointNorth = newGridCoordinates + (hitObject.transform.forward);
                            Vector3 connectionPointEast = newGridCoordinates + (hitObject.transform.right);
                            Vector3 connectionPointSouth = newGridCoordinates + (hitObject.transform.forward * -1);
                            Vector3 connectionPointWest = newGridCoordinates + (hitObject.transform.right * -1);
                            Vector3 hitPointCoordinate = grid.GetGridCoordinates(hitPoint);
                            print("hit point coord: " + hitPointCoordinate);

                            if (IsPointInRadius(hitPointCoordinate, connectionPointNorth, 0.7f)) newGridCoordinates = connectionPointNorth;
                            else if (IsPointInRadius(hitPointCoordinate, connectionPointEast, 0.7f)) newGridCoordinates = connectionPointEast;
                            else if (IsPointInRadius(hitPointCoordinate, connectionPointSouth, 0.7f)) newGridCoordinates = connectionPointSouth;
                            else if (IsPointInRadius(hitPointCoordinate, connectionPointWest, 0.7f)) newGridCoordinates = connectionPointWest;

                        }
                        else if (coordinates == Direction.North)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.up, hitObject.transform.forward) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                        }
                        else if (coordinates == Direction.East)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.up, hitObject.transform.right) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                        }
                        else if (coordinates == Direction.West)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.right) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                        }
                        else if (coordinates == Direction.South)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.forward) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                        }
                        break;

                    case "Double":
                        if (coordinates == Direction.Up || coordinates == Direction.Down)
                        {
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f); // distance from center
                            print("distance from center: " + distanceFromCenter);

                            if (coordinates == Direction.Up)
                            {
                                // Flip rotation from facing up to facing down
                                objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.up) * objectRotation;
                                print("euler rotation: " + objectRotation);
                            }

                            clamp = true;

                            Vector3 connectionPointNorth = newGridCoordinates + (hitObject.transform.forward);
                            Vector3 connectionPointEast = newGridCoordinates + (hitObject.transform.right);
                            Vector3 connectionPointSouth = newGridCoordinates + (hitObject.transform.forward * -1);
                            Vector3 connectionPointWest = newGridCoordinates + (hitObject.transform.right * -1);
                            Vector3 hitPointCoordinate = grid.GetGridCoordinates(hitPoint);
                            print("hit point coord: " + hitPointCoordinate);

                            if (IsPointInRadius(hitPointCoordinate, connectionPointNorth, 0.7f)) newGridCoordinates = connectionPointNorth;
                            else if (IsPointInRadius(hitPointCoordinate, connectionPointEast, 0.7f)) newGridCoordinates = connectionPointEast;
                            else if (IsPointInRadius(hitPointCoordinate, connectionPointSouth, 0.7f)) newGridCoordinates = connectionPointSouth;
                            else if (IsPointInRadius(hitPointCoordinate, connectionPointWest, 0.7f)) newGridCoordinates = connectionPointWest;

                        }
                        else if (coordinates == Direction.North)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.up, hitObject.transform.forward) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                        }
                        else if (coordinates == Direction.East)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.up, hitObject.transform.right) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                        }
                        else if (coordinates == Direction.West)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.right) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                        }
                        else if (coordinates == Direction.South)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.forward) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                        }
                        break;
                }
                break;

            case "Double":
                switch (selectBrick.name)
                {
                    case "T":
                        if (coordinates == Direction.Up)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, hitObject.transform.up) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.67f);
                        }
                        else if (coordinates == Direction.Down)
                        {
                            objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObject.transform.up) * objectRotation;
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.67f);
                        }
                        else
                        {
                            Vector3 connectionPointTop = newGridCoordinates + (hitObject.transform.up * 1.5f);
                            Vector3 hitPointCoordinate = grid.GetGridCoordinates(hitPoint);

                            if (IsPointInRadius(hitPointCoordinate, connectionPointTop, 0.7f))
                            {
                                objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                                newGridCoordinates = GetCoordinates(hitObjectProperties.GetWorldDirection(Direction.Up), gridCoordinates, 1.5f);
                            }
                            else
                            {
                                objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                                newGridCoordinates = GetCoordinates(hitObjectProperties.GetWorldDirection(Direction.Down), gridCoordinates, 0.5f);
                            }
                        }
                        break;
                    case "Base":
                        if (coordinates == Direction.Up)
                        {
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 2.0f);
                        }
                        else if (coordinates == Direction.Down)
                        {
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                        }
                        else
                        {
                            Vector3 connectionPointTop = newGridCoordinates + (hitObject.transform.up * 1.0f);
                            Vector3 hitPointCoordinate = grid.GetGridCoordinates(hitPoint);

                            if (IsPointInRadius(hitPointCoordinate, connectionPointTop, 0.7f))
                            {
                                //objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                                newGridCoordinates = GetCoordinates(direction, connectionPointTop, 0.0f);
                            }
                            else
                            {
                                //objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                                newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                            }
                        }
                        break;
                    case "Double":
                        if (coordinates == Direction.Up)
                        {
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 2.0f);
                        }
                        else if (coordinates == Direction.Down)
                        {
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 2.0f);
                        }
                        break;
                    default:
                        if (coordinates == Direction.Up)
                        {
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                        }
                        else if (coordinates == Direction.Down)
                        {
                            newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.5f);
                        }
                        break;
                }
                break;

            default:
                break;
        }

        // Props
        if (hitObject.CompareTag("prop"))
        {
            float propHeight = 0;
            propHeight = hitObject.GetComponent<BoxCollider>().size.y;
            propHeight = grid.ConvertToGridDistance(propHeight);

            newGridCoordinates = GetCoordinates(Direction.Up, gridCoordinates, propHeight);
            print(propHeight);
        }
        else if (selectBrick.CompareTag("prop"))
        {
            newGridCoordinates = GetCoordinates(Direction.Up, gridCoordinates, 0.5f);
            objectRotation = selectBrick.transform.rotation;

            if (hitObject.name == "Double")
            {
                newGridCoordinates = GetCoordinates(Direction.Up, gridCoordinates, 1.5f);
            }
        }

        PlaceSelectedCubeNear(selectBrick, direction, objectRotation, newGridCoordinates, hitObjectProperties, clamp);
    }
    private void PlaceSelectedCubeNear(GameObject selectBrick, Direction direction, Quaternion objectRotation, Vector3 gridCoordinates, BrickProperties clickedObjectProperties, bool clamp)
    {
        // Check if challenge limit is not exceded
        if (isChallenge && !challengeSystem.CheckForLimit(selectBrick.name)) { return; }

        // Get final grid point for clicked Object
        Vector3 finalPosition = grid.CoordinatesToGridPoint(gridCoordinates);

        var brickProperties = selectBrick.GetComponent<BrickProperties>();
        var currentCoordinates = brickProperties.coordinates;
        var currentPosition = selectBrick.transform.position;
        var currentRotation = selectBrick.transform.rotation;
        var currentLocalDirection = brickProperties.localDirection;

        // If user rotated brick, keep rotation 
        if (!brickProperties.isRotated)
        {
            selectBrick.transform.rotation = objectRotation;
        }

        // Create brick
        if (isPreRenderMode)
        {
            selectBrick.transform.position = finalPosition;
        }
        else
        {
            // List for Undo
            List<GameObject> newPlacedBricks = new List<GameObject>();

            selectBrick.name = selectBrick.name.Replace("(Clone)", "");

            // Move new brick coordinate to grid coordinate
            if (brickProperties != null)
            {
                brickProperties.coordinates = gridCoordinates;
            }

            var newPosition = grid.CoordinatesToGridPoint(gridCoordinates);


            brickProperties.coordinates = gridCoordinates;
            brickProperties.localDirection = clickedObjectProperties.localDirection;
            brickProperties.Move(newPosition, gridCoordinates, false);

            //Add to placed bricks
            GameObject placedBrick = selectBrick;

            StartCoroutine(CheckIfSelectedCubeCollides(brickProperties, clickedObjectProperties, clamp, placedBrick, currentPosition, currentCoordinates, currentRotation, currentLocalDirection));

        }
    }

    private IEnumerator CheckIfSelectedCubeCollides(BrickProperties brickProperties, BrickProperties clickedObjectProperties, bool clamp, GameObject placedBrick, Vector3 oldPosition, Vector3 oldCoordinates, Quaternion oldRotation, Direction[] oldLocalDirection)
    {
        yield return new WaitForSeconds(0.1f);

        // Unselect
        brickProperties.ClearHighlight();

        if (!brickProperties.isCollided)
        {
            if (isPaste == true)
            {
                // Create and Save undo
                UndoAction undoAction = new UndoAction
                {
                    Index = currentUndoPosition + 1,
                    Type = 1,
                    Bricks = new List<GameObject>() { placedBrick }
                };
                SaveUndo(undoAction);

                placedBricks.Add(placedBrick.transform);
            }
            else
            {
                // Create and Save undo
                UndoAction undoAction = new UndoAction
                {
                    Index = currentUndoPosition + 1,
                    Type = 0,
                    Bricks = new List<GameObject>() { placedBrick }
                };
                SaveUndo(undoAction);
            }

            // Clamp objects together if clamp is true 
            print("local Direction count: " + brickProperties.previousLocalDirections.Count);
        }
        else
        {
            if (isPaste == true)
            {
                // New brick already collides?
                Destroy(brickProperties.gameObject);
            }
            else
            {
                // Reset brick properties
                placedBrick.transform.position = oldPosition;
                brickProperties.coordinates = oldCoordinates;
                print("Check if Collides: " + placedBrick + "collides. Old rotation = " + oldRotation);
                brickProperties.localDirection = oldLocalDirection;
            }
        }
    }

    private void Select(GameObject clickedObject)
    {
        if (isPaste)
        {
            print("in paste mode still");
            return;
        }

        var clickedObjectProperties = clickedObject.GetComponent<BrickProperties>();

        // If already clicked
        if (clickedBricks.Contains(clickedObject))
        {
            clickedObjectProperties.ClearHighlight();
            clickedBricks.Remove(clickedObject);
            return;
        }

        // Save old color materials
        var previousColors = new List<Color>();
        foreach (Material m in clickedObject.GetComponent<Renderer>().materials)
        {
            previousColors.Add(m.color);
            m.color = Color.green;
        }
        clickedObjectProperties.previousColors = previousColors;
        clickedObjectProperties.EnterEditMode();

        // Add to clicked bricks
        if (!clickedBricks.Contains(clickedObject))
        {
            clickedBricks.Add(clickedObject);
        }
        
        return;
    }
    private void Move(Vector3 clickedPoint)
    {
        Vector3 point = grid.GetGridCoordinates(new Vector3(clickedPoint.x, clickedPoint.y + 0.1f, clickedPoint.z));

        if (preRenderMovedBricks.Count == 0 || movedBricks.Count == 0)
        {
            return;
        }
        else
        {
            // Get max coordinate
            List<Vector3> allCoordinates = placedBricks.Select(go => go.GetComponent<BrickProperties>().coordinates).ToList();
            List<Vector3> allClickedCoordinates = movedBricks.Select(go => go.GetComponent<BrickProperties>().coordinates).ToList();
            Vector3 maxVector = Vector3.zero;
            float maxY = allClickedCoordinates.Max((Vector3 arg) => arg.y) - allClickedCoordinates.Min((Vector3 arg) => arg.y);
            for (int i = 0; i < allClickedCoordinates.Count; i++)
            {
                Vector3 coordinates = allClickedCoordinates[i];
                maxVector = (coordinates.magnitude > maxVector.magnitude) ? coordinates : maxVector;
            }

            // Place objects
            List<Vector3> gridCoordinates = new List<Vector3>();
            for (int i = 0; i < movedBricks.Count; i++)
            {
                var clickedBrick = movedBricks[i];
                var difference = maxVector - clickedBrick.GetComponent<BrickProperties>().coordinates;
                var newGridCoordinates = point - difference;
                newGridCoordinates.y += maxY;

                if (allCoordinates.Contains(newGridCoordinates))
                {
                    isPreRenderMode = true;
                    return;
                }
                else
                {
                    gridCoordinates.Add(newGridCoordinates);
                }
            }

            // Move Pre-Rendered brick
            if (isPreRenderMode)
            {
                // Pre-Render initialization
                for (int i = 0; i < preRenderMovedBricks.Count; i++)
                {
                    // Set pre render material
                    if (!preRenderMovedBricks[i].CompareTag("prop"))
                    {
                        Material transparentMaterial = Resources.Load<Material>("_1_Transparent");
                        preRenderMovedBricks[i].GetComponent<Renderer>().sharedMaterial = transparentMaterial;
                    }
                    // Set pre render components
                    preRenderMovedBricks[i].GetComponent<BrickProperties>().isPreRender = true;
                    preRenderMovedBricks[i].GetComponent<MeshRenderer>().receiveShadows = false;
                    preRenderMovedBricks[i].GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    preRenderMovedBricks[i].layer = 2;
                    foreach (Transform child in preRenderMovedBricks[i].transform)
                    {
                        child.gameObject.layer = 2;
                    }

                    // Move Pre-Render bricks
                    var preRenderBrick = preRenderMovedBricks[i];
                    var newCoordinates = grid.CoordinatesToGridPoint(gridCoordinates[i]);
                    preRenderBrick.transform.position = newCoordinates;
                    preRenderBrick.transform.rotation = preRenderBrick.transform.localRotation;
                }
            }
            // Move Solid brick
            else
            {
                // Solid initialization
                List<GameObject> newMovedBricks = new List<GameObject>();
                for (int i = 0; i < clickedBricks.Count; i++)
                {
                    // Set solid material
                    if (!clickedBricks[i].CompareTag("prop"))
                    {
                        Material solidMaterial = Resources.Load<Material>("_1_Solid");
                        clickedBricks[i].GetComponent<Renderer>().sharedMaterial = solidMaterial;
                    }
                    // Set solid components
                    clickedBricks[i].GetComponent<BrickProperties>().isPreRender = false;
                    clickedBricks[i].GetComponent<MeshRenderer>().receiveShadows = true;
                    clickedBricks[i].GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                    clickedBricks[i].layer = 0;
                    clickedBricks[i].SetActive(true);

                    foreach (Transform child in clickedBricks[i].transform)
                    {
                        child.gameObject.layer = 0;
                    }

                    var clickedBrick = clickedBricks[i];
                    var newCoordinates = grid.CoordinatesToGridPoint(gridCoordinates[i]);

                    var brickProperties = clickedBrick.GetComponent<BrickProperties>();
                    brickProperties.coordinates = gridCoordinates[i];
                    brickProperties.Move(newCoordinates, gridCoordinates[i], false);
                    brickProperties.ClearHighlight();

                    //Add to moved bricks
                    newMovedBricks.Add(clickedBrick);

                }
                // Create and Save undo
                UndoAction undoAction = new UndoAction
                {
                    Index = currentUndoPosition + 1,
                    Type = 0,
                    Bricks = newMovedBricks
                };
                SaveUndo(undoAction);
            }
        }

    }
    public void Rotate(int axe)
    {
        List<GameObject> newRotatedBricks = new List<GameObject>();

        // Rotate Pre-Render brick in Create Mode
        if (mode == 0)
        {
            BrickProperties preRenderBrickProperties = preRenderBrick.GetComponent<BrickProperties>();
            preRenderBrickProperties.Rotate(axe, false);
        }
        // Rotate brick or bricks in Select Mode
        if (mode == 1 && clickedBricks.Count > 0)
        {
            // Rotate Pre-Rendered copied bricks while pasting
            if (isPaste)
            {
                foreach (GameObject preRenderCopiedBrick in preRenderCopiedBricks)
                {
                    BrickProperties preRenderProperties = preRenderCopiedBrick.GetComponent<BrickProperties>();
                    preRenderProperties.Rotate(axe, false);
                    originalRotation = preRenderCopiedBrick.transform.localRotation;
                }
            }
            // Rotate Pre-Rendered moved bricks
            else if (isPreRenderMode)
            {
                foreach (GameObject preRenderMovedBrick in preRenderMovedBricks)
                {
                    BrickProperties preRenderProperties = preRenderMovedBrick.GetComponent<BrickProperties>();
                    preRenderProperties.Rotate(axe, false);
                    originalRotation = preRenderMovedBrick.transform.localRotation;
                }
            }
            // Rotate Selected bricks
            else
            {
                foreach (GameObject clickedBrick in clickedBricks)
                {
                    BrickProperties brickProperties = clickedBrick.GetComponent<BrickProperties>();
                    if (clickedBrick.name == "4D")
                    {
                        var newGridCoordinatesUp = GetCoordinates(Direction.Up, brickProperties.coordinates, 1.0f);
                        var newGridCoordinatesDown = GetCoordinates(Direction.Up, brickProperties.coordinates, -1.0f);
                        brickProperties.AdjustYForRotate(axe, newGridCoordinatesUp, newGridCoordinatesDown);
                    }
                    brickProperties.Rotate(axe, false);
                    originalRotation = clickedBrick.transform.localRotation;

                    if (brickProperties.isEditMode)
                    {
                        newRotatedBricks.Add(clickedBrick);
                    }
                }

                if (newRotatedBricks.Count > 0)
                {
                    // Save as undo
                    UndoAction undoAction = new UndoAction
                    {
                        Index = currentUndoPosition + 1,
                        Type = 0,
                        Bricks = newRotatedBricks
                    };
                    SaveUndo(undoAction);
                }
            }
            
            
        }
    }
    public void Delete()
    {
        // Check for clicked bricks to delete
        if (clickedBricks.Count == 0) return;

        // Save undo delete state
        UndoAction undoAction = new UndoAction
        {
            Index = currentUndoPosition + 1,
            Type = 2,
            Bricks = clickedBricks
        };
        SaveUndo(undoAction);

        for (int i = 0; i < clickedBricks.Count; i++)
        {
            clickedBricks[i].SetActive(false);
            placedBricks.Remove(clickedBricks[i].transform);
        }

        ClearHighlights();



        if (isChallenge) { challengeSystem.UpdateLimits(placedBricks); }
        return;
    }
    private void InitializePreRender()
    {
        // Destroy current pre render brick
        if (preRenderBrick != null)
        {
            Destroy(preRenderBrick);
        }

        // Initialize pre render to selected brick
        preRenderBrick = selectedBrick;

        // Change material to transparent except for prop
        if (!preRenderBrick.CompareTag("prop"))
        {
            Material transparentMaterial = Resources.Load<Material>("_1_Transparent");
            preRenderBrick.GetComponent<Renderer>().sharedMaterial = transparentMaterial;
        }

        // Set pre render components
        preRenderBrick.GetComponent<BrickProperties>().isPreRender = true;
        preRenderBrick.GetComponent<MeshRenderer>().receiveShadows = false;
        preRenderBrick.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        preRenderBrick.layer = 2;
        foreach (Transform child in preRenderBrick.transform)
        {
            child.gameObject.layer = 2;
        }

        // Create pre render brick
        preRenderBrick = Instantiate(preRenderBrick, Vector3.zero, originalRotation);
    }
    private void InitializeSelectedBrick()
    {
        // Set solid attributes
        if (!selectedBrick.CompareTag("prop"))
        {
            Material solidMaterial = Resources.Load<Material>("_1_Solid");
            selectedBrick.GetComponent<Renderer>().sharedMaterial = solidMaterial;
        }
        selectedBrick.GetComponent<BrickProperties>().isPreRender = false;
        selectedBrick.GetComponent<MeshRenderer>().receiveShadows = true;
        selectedBrick.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        selectedBrick.layer = 0;
        foreach (Transform child in selectedBrick.transform)
        {
            child.gameObject.layer = 0;
        }
    }


    // Menu Actions
    public void BackToMainMenu()
    {
        loadingPanel.gameObject.SetActive(true);
        loadingPanel.alpha = 1.0f;
        SceneManager.LoadScene("Menu");
    }
    public void SaveScene()
    {
        Transform[] placedBricksArray = new Transform[placedBricks.Count];

        // Get transform data of all placed bricks and hold in array
        for (int i = 0; i < placedBricks.Count; i++)
        {
            placedBricksArray[i] = placedBricks[i];
        }

        // Save
        var save = SaveSystem.SaveData(placedBricksArray);
        if (save) print("Scene Saved!");
    }
    public void LoadScene()
    {
        // Get transform data from array
        PlacedBrocksData loadedData = SaveSystem.LoadData();

        if (loadedData == null) return;
        else SaveSystem.SetData(loadedData);

        SaveSystem.isLoading = true;
        SceneManager.LoadScene("Main");
        print("Loading scene");
    }
    private void LoadObjects()
    {
        PlacedBrocksData data = SaveSystem.GetData();

        GameObject loadedObject;

        // Place each brick from data into scene
        for (int i = 0; i < data.count; i++)
        {
            if (data.objectName[i] == "Base") loadedObject = placeablePrefarbs[0].prefarb;
            else if (data.objectName[i] == "T") loadedObject = placeablePrefarbs[1].prefarb;
            else if (data.objectName[i] == "1D") loadedObject = placeablePrefarbs[2].prefarb;
            else if (data.objectName[i] == "Double") loadedObject = placeablePrefarbs[3].prefarb;
            else if (data.objectName[i] == "2D") loadedObject = placeablePrefarbs[4].prefarb;
            else if (data.objectName[i] == "3D") loadedObject = placeablePrefarbs[5].prefarb;
            else if (data.objectName[i] == "4D") loadedObject = placeablePrefarbs[6].prefarb;
            else
            {
                Debug.Log("Loaded Object doesn't match prefab name");
                loadedObject = placeablePrefarbs[0].prefarb;
            }

            Vector3 position = new Vector3(data.positionX[i], data.positionY[i], data.positionZ[i]);
            Vector3 rotation = new Vector3(data.rotationX[i], data.rotationY[i], data.rotationZ[i]);
            Vector3 coordinates = new Vector3(data.coordinatesX[i], data.coordinatesY[i], data.coordinatesZ[i]);

            loadedObject = Instantiate(loadedObject, position, Quaternion.Euler(rotation));
            loadedObject = ConvertToSolid(loadedObject);

            var placedBrickProperties = loadedObject.GetComponent<BrickProperties>();
            placedBrickProperties.coordinates = coordinates;

            placedBricks.Add(loadedObject.transform);
        }

        print("Scene Loaded!");
    }


    // Button Actions
    public void SwitchBlocks()
    {
        if (mode == 1)
        {
            SwitchMode();
        }

        if (switchButtonText.text == "PROPS")
        {
            switchButtonText.text = "BROCKS";
            allBricks.SetActive(false);
            allObjects.SetActive(true);
        }
        else
        {
            switchButtonText.text = "PROPS";
            allBricks.SetActive(true);
            allObjects.SetActive(false);
        }
        isPreRenderMode = false;
    }

    public void SwitchMode()
    {
        mode++;

        // Reset count back to 0
        if (mode == 2) { mode = 0; }

        // Turn off Edit buttons
        editButtons.SetActive(false);

        // Change modes
        switch (mode)
        {
            case 1:
                edit.text = "SELECT";
                editButtons.SetActive(true);
                transformButtons.SetActive(false);
                rotateButtons.SetActive(false);
                var c = new Color(0.2f, 0.2f, 0.2f);
                copy.color = c;
                ClearHighlights();
                ResetInput();
                isPreRenderMode = false;
                break;
            default:
                edit.text = "CREATE";
                originalRotation = selectedBrick.transform.localRotation;
                InitializeSelectedBrick();
                InitializePreRender();
                ClearHighlights();
                ResetInput();
                isPreRenderMode = true;
                break;
        }
    }

    public void SwitchBrick(int brick)
    {
        if (mode == 1)
        {
            SwitchMode();
        }

        for (var i = 0; i < allBrickImages.Count; i++)
        {
            // Default highlight color
            allBrickImages[i].color = Color.white;
        }
        // Darken highlight color
        allBrickImages[brick].color = new Color(0.83f, 0.83f, 0.83f);

        // Initialialize Selected brick to correct brick
        selectedBrick = placeablePrefarbs[brick].prefarb;
        originalRotation = selectedBrick.transform.localRotation;

        InitializeSelectedBrick();
        InitializePreRender();
        isPreRenderMode = true;
        
    }
    
    public void SwitchCopyPaste()
    {
        if (clickedBricks.Count == 0) { return; }

        var c = new Color(0.2f, 0.2f, 0.2f);

        if (copy.color == c)
        {
            copy.color = Color.grey;
        }
        else
        {
            copy.color = c;
        }

        isCopy = !isCopy;
        copy.text = (isCopy) ? "COPY" : "PASTE";

        if (isCopy == true) 
        {
            if (clickedBricks.Count > 0)
            {
                isPaste = true;
                isPreRenderMode = true;
                Copy();
            }
            
        }
        else
        {
            isPaste = false;
            isPreRenderMode = false;
            ClearHighlights();
            for (int i = 0; i < preRenderCopiedBricks.Count; i++)
            {
                preRenderCopiedBricks[i].SetActive(false);
            }
            copiedBricks.Clear();
            preRenderCopiedBricks.Clear();
            clickedBricks.Clear();
            
        }
    }


    // Copy
    public void Copy()
    {
        copiedBricks = clickedBricks;

        // Create Pre-Render Copied
        for (int i = 0; i < copiedBricks.Count; i++)
        {
            // Create pre render brick
            var newPreRenderCopied = Instantiate(copiedBricks[i].gameObject, spawnLocation, copiedBricks[i].transform.rotation);
            newPreRenderCopied = ConvertToPreRender(newPreRenderCopied);
            preRenderCopiedBricks.Add(newPreRenderCopied); 
        }
    }

    // Paste
    public void PasteAtPoint(Vector3 clickedPoint)
    {
        Vector3 point = grid.GetGridCoordinates(new Vector3(clickedPoint.x, clickedPoint.y + 0.1f, clickedPoint.z));

        // Check if challenge paste blocks are within the limit
        if (isChallenge && !challengeSystem.CheckPasteLimit(copiedBricks)) { return; }

        // Get max coordinate
        List<Vector3> allCoordinates = placedBricks.Select(go => go.GetComponent<BrickProperties>().coordinates).ToList(); // Calculating for collision
        List<Vector3> allCopiedCoordinates = copiedBricks.Select(go => go.GetComponent<BrickProperties>().coordinates).ToList();
        Vector3 maxVector = Vector3.zero; // initialize
        float maxY = allCopiedCoordinates.Max((Vector3 arg) => arg.y) - allCopiedCoordinates.Min((Vector3 arg) => arg.y); // Calculating difference in all of the Y coordinates
        for (int i = 0; i < allCopiedCoordinates.Count; i++)
        {
            Vector3 coordinates = allCopiedCoordinates[i];
            maxVector = (coordinates.magnitude > maxVector.magnitude) ? coordinates : maxVector;
        }
         
        // Place objects
        List<Vector3> gridCoordinates = new List<Vector3>();
        for (int i = 0; i < copiedBricks.Count; i++)
        {
            var copiedBrick = copiedBricks[i];
            var difference = maxVector - copiedBrick.GetComponent<BrickProperties>().coordinates; // offset
            var newGridCoordinates = point - difference; // clicked point - offset
            newGridCoordinates.y += maxY; // updating gridCoordinates

            if (allCoordinates.Contains(newGridCoordinates)) // Checking for collision
            {
                isPreRenderMode = true;
                return; // collision, no paste
            }
            else
            {
                gridCoordinates.Add(newGridCoordinates); // paste
            }
        }

        if (isPreRenderMode)
        {
            // Pre-Render initialization
            for (int i = 0; i < preRenderCopiedBricks.Count; i++)
            {
                preRenderCopiedBricks[i] = ConvertToPreRender(preRenderCopiedBricks[i]);

                var preRenderBrick = preRenderCopiedBricks[i];
                var newCoordinates = grid.CoordinatesToGridPoint(gridCoordinates[i]);
                preRenderBrick.transform.position = newCoordinates;
            }
        }
        else
        {
            // Solid initialization
            List<GameObject> pastedBricks = new List<GameObject>();
            for (int i = 0; i < copiedBricks.Count; i++)
            {

                // Instantiate
                var copiedBrick = preRenderCopiedBricks[i];
                var newCoordinates = grid.CoordinatesToGridPoint(gridCoordinates[i]); // paste point
                var newBrick = Instantiate(copiedBrick, newCoordinates, preRenderCopiedBricks[i].transform.rotation);
                newBrick = ConvertToSolid(newBrick);
                var brickProperties = newBrick.GetComponent<BrickProperties>();
                brickProperties.coordinates = gridCoordinates[i];


                //Add to pasted bricks
                pastedBricks.Add(newBrick);
                brickProperties.ClearHighlight();

            }
            // Create and Save undo
            UndoAction undoAction = new UndoAction
            {
                Index = currentUndoPosition + 1,
                Type = 1,
                Bricks = pastedBricks
            };
            SaveUndo(undoAction);

            // Place bricks
            foreach (GameObject brick in pastedBricks)
            {
                placedBricks.Add(brick.transform);
            }

            isPreRenderMode = true;
        } 

        if (isChallenge) { challengeSystem.UpdateLimits(placedBricks); }
    }


    // Undo / Redo
    public void ResetUndos()
    {
        if (allUndos.Count > 0)
        {
            List<UndoAction> filteredUndos = allUndos.FindAll(u => u.Index > currentUndoPosition);
            foreach (UndoAction undo in filteredUndos)
            {
                if (undo.Type == 1) // Place, Paste
                {
                    for (int i = 0; i < undo.Bricks.Count; i++)
                    {
                        Destroy(undo.Bricks[i].gameObject);
                    }
                }
            }
            allUndos.RemoveAll(u => u.Index > currentUndoPosition);
        }
    }

    public void SaveUndo(UndoAction undo)
    {
        ResetUndos();
        allUndos.Add(undo);
        currentUndoPosition = allUndos.Count - 1;
    }

    public void Undo()
    {
        if (currentUndoPosition == -1) { return; }
        UndoAction firstUndo = allUndos.First(e => e.Index == currentUndoPosition);

        switch (firstUndo.Type)
        {
            case 0:
                foreach (GameObject Brick in firstUndo.Bricks)
                {
                    BrickProperties brickProperties = Brick.GetComponent<BrickProperties>();
                    brickProperties.Undo();
                }
                break;
            case 2:
                foreach (GameObject Brick in firstUndo.Bricks)
                {
                    Brick.SetActive(true);
                    placedBricks.Add(Brick.transform);
                }
                break;
            default:
                foreach (GameObject Brick in firstUndo.Bricks)
                {
                    Brick.SetActive(false);
                    placedBricks.Remove(Brick.transform);
                }
                break;
        }

        ClearHighlights();

        if (currentUndoPosition > -1) { currentUndoPosition -= 1; }
        if (isChallenge) { challengeSystem.UpdateLimits(placedBricks); }
    }

    public void Redo()
    {
        if (currentUndoPosition < allUndos.Count - 1) { currentUndoPosition += 1; } else { return; }
        UndoAction firstUndo = allUndos.First(e => e.Index == currentUndoPosition);

        switch (firstUndo.Type)
        {
            case 0:
                foreach (GameObject Brick in firstUndo.Bricks)
                {
                    BrickProperties brickProperties = Brick.GetComponent<BrickProperties>();
                    brickProperties.Redo();
                }
                break;
            case 2:
                foreach (GameObject Brick in firstUndo.Bricks)
                {
                    Brick.SetActive(false);
                    placedBricks.Remove(Brick.transform);
                }
                break;
            default:
                foreach (GameObject Brick in firstUndo.Bricks)
                {
                    Brick.SetActive(true);
                    placedBricks.Add(Brick.transform);
                }
                break;
        }
        challengeSystem.UpdateLimits(placedBricks);
    }

    private void ClearHighlights()
    {
        ResetCopy();

        if (placedBricks.Count > 0)
        {
            foreach (Transform placedBrick in placedBricks)
            {
                placedBrick.gameObject.GetComponent<BrickProperties>().ClearHighlight();
                placedBrick.gameObject.GetComponent<BrickProperties>().isClicked = false;
                placedBrick.gameObject.GetComponent<BrickProperties>().isDropped = false;
            }
        }
        clickedBricks = new List<GameObject>();
    }

    private void ResetCopy() //*
    {
        //isCopy = true;
        copy.text = "COPY";

        undoButton.SetActive(true);
        redoButton.SetActive(true);
    }

    private void FilterRay(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            if (hitInfo.transform.gameObject != null)
            {
                // Get hit object
                var hitObject = hitInfo.transform.gameObject;

                // Get parent if hit object is child Collider
                if (hitObject.name == "Collider")
                {
                    hitObject = hitInfo.transform.parent.gameObject;
                }

                // Get position and rotation of clicked Object (hit object)
                var hitPoint = hitInfo.point;
                var hitObjectRotation = hitObject.transform.localRotation;

                // Place on plane if clicked Object is not brick/prop
                if (!hitObject.CompareTag("brick") && !hitObject.CompareTag("prop"))
                {
                    if (mode == 0)
                    {
                        // Place cub slightly above plane
                        PlaceCubeOnPlane(new Vector3(hitPoint.x, hitPoint.y + 0.1f, hitPoint.z));
                        return;
                    }
                    // Ignore if in copy mode
                    if (mode == 2 || mode == 1 && isCopy == true) { return; }
                }

                // Get properties from clicked Object (clampedObjects, coordinates)
                var hitObjectProperties = hitObject.GetComponent<BrickProperties>();
                List<GameObject> clampedObjects = new List<GameObject>(); //initialize
                Vector3 gridCoordinates = Vector3.one; //initialize
                 
                if (hitObjectProperties != null)
                {
                    //clampedObjects = clickedObjectProperties.clampedObjects;
                    gridCoordinates = hitObjectProperties.coordinates;
                }

                // Different directions clicked Object could be in
                Direction cODirectionUp = Direction.Up;
                Direction cODirectionDown = Direction.Down;
                Direction cODirection = Direction.North;
                Direction cODirectionReverse = Direction.South;
                Direction cODirectionRight = Direction.East;
                Direction cODirectionLeft = Direction.West;

                // Get Direction of clicked Object. Calculates direction relative to camera
                Direction direction = GetDirection(hitInfo.normal - Vector3.up);

                // Move new Object Grid coordinate in direction with distance: 1 for bricks, 0.5 for props (0.5 = more precision)
                Vector3 newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1);
                if (selectedBrick.CompareTag("prop"))
                {
                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.5f);
                }

                // Set clicked Object's new coordinate, rotation
                Direction coordinates = hitObjectProperties.GetLocalDirection(direction);
                Quaternion objectRotation = hitObjectRotation;
                Direction worldDirectionTest = hitObjectProperties.GetWorldDirection(Direction.Up);

                //initialize 
                bool clamp = false;

                // Clamp logic, clicked Object rotation and new grid coordinates
                //Debug.Log(direction);
                switch (hitObject.name)
                {
                    case "Base":
                        switch (selectedBrick.name)
                        {
                            case "4D":
                                newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.5f);
                                break;
                            case "3D":
                                newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.5f);
                                break;
                            case "2D":
                                newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.5f);
                                break;
                            case "1D":
                                newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.5f);
                                break;
                            case "T":
                                if (coordinates == Direction.Up || coordinates == Direction.Down)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, hitObject.transform.up) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.67f);
                                }
                                else
                                {
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.5f);
                                    Vector3 connectionPointIn1 = GetCoordinates(hitObjectProperties.ConvertVectorToDirection(hitObject.transform.up), newGridCoordinates, 0.0f);
                                    Vector3 connectionPointIn2 = GetCoordinates(hitObjectProperties.ConvertVectorToDirection(hitObject.transform.up), newGridCoordinates, 0.5f);
                                    //Vector3 connectionPointOut = GetCoordinates(direction, connectionPointIn, 0.67f);
                                    Vector3 hitPointCoordinate = grid.GetGridPoint(hitPoint);
                                    print("hit point coord: " + hitPointCoordinate);

                                    if (IsPointInRadius(hitPointCoordinate, connectionPointIn1, 0.25f) || IsPointInRadius(hitPointCoordinate, connectionPointIn2, 0.25f))
                                    {
                                        newGridCoordinates = GetCoordinates(hitObjectProperties.ConvertVectorToDirection(hitObject.transform.up), gridCoordinates, 0.5f);
                                        objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                                        print("coonection point in: " + connectionPointIn2);
                                    }
                                    else
                                    {
                                        newGridCoordinates = GetCoordinates(hitObjectProperties.ConvertVectorToDirection(hitObject.transform.up), gridCoordinates, 0.5f);
                                        newGridCoordinates = GetCoordinates(direction, newGridCoordinates, 0.67f);
                                        objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                                        print("coonection point out: " + newGridCoordinates);
                                    }
                                }
                                break;
                            case "Double":
                                if (coordinates == Direction.Down)
                                {
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 2.0f);
                                } 
                                break;
                            default:
                                break;
                        }
                        break;

                    case "T":
                        switch (selectedBrick.name)
                        {
                            case "T":
                                if (coordinates == Direction.North)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObject.transform.forward) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.0f);
                                }
                                else if (coordinates == Direction.South)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObject.transform.forward) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                                }
                                break;
                            case "1D":
                                if (coordinates == Direction.North)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.up) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.0f);
                                }
                                break;
                            case "2D":
                                if (coordinates == Direction.Up || coordinates == Direction.Down || coordinates == Direction.North)
                                {
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                                }
                                else
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObject.transform.right) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                                }
                                break;
                            case "3D":
                                if (coordinates == Direction.Up || coordinates == Direction.Down || coordinates == Direction.North)
                                {
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                                }
                                else
                                {
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                                }
                                break;
                            case "4D":
                                if (coordinates == Direction.Up || coordinates == Direction.Down || coordinates == Direction.North)
                                {
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                                }
                                else
                                {
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 2.0f);
                                }
                                break;
                            case "Base":
                                if (coordinates == cODirectionUp || coordinates == cODirectionDown)
                                {
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.5f);
                                    if (coordinates == cODirectionUp)
                                    {
                                        if (hitObjectRotation.x > 0.7 && hitObjectRotation.x < 0.8)
                                        {
                                            objectRotation = hitObject.transform.rotation * Quaternion.AngleAxis(180, hitObject.transform.up);
                                        }
                                        else
                                        {
                                            objectRotation = hitObject.transform.rotation * Quaternion.AngleAxis(180, hitObject.transform.forward);
                                        }
                                    }
                                }
                                else if (coordinates == cODirection)
                                {
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.5f);
                                    objectRotation = hitObject.transform.rotation * Quaternion.AngleAxis(90, hitObject.transform.right);
                                }
                                else if (coordinates == Direction.East || coordinates == Direction.West)
                                {
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.67f);
                                    //objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.up) * objectRotation;

                                    Vector3 connectionPointUp = GetCoordinates(hitObjectProperties.ConvertVectorToDirection(hitObject.transform.up), newGridCoordinates, 0.5f);
                                    Vector3 connectionPointDown = GetCoordinates(hitObjectProperties.ConvertVectorToDirection(-hitObject.transform.up), newGridCoordinates, 0.5f);
                                    Vector3 hitPointCoordinate = grid.GetGridPoint(hitPoint);
                                    print("hit point coord: " + hitPointCoordinate);

                                    if (IsPointInRadius(hitPointCoordinate, connectionPointUp, 0.5f))
                                    {
                                        newGridCoordinates = connectionPointUp;
                                        objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.up) * objectRotation;
                                    }
                                    else
                                    {
                                        newGridCoordinates = connectionPointDown;
                                        objectRotation = Quaternion.FromToRotation(hitObject.transform.up, hitObject.transform.up) * objectRotation;
                                    }
                                    
                                }
                                break;
                            case "Double":
                                if (coordinates == cODirectionUp || coordinates == cODirectionDown)
                                {
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                                    if (coordinates == cODirectionUp)
                                    {
                                        if (hitObjectRotation.x > 0.7 && hitObjectRotation.x < 0.8)
                                        {
                                            objectRotation = hitObject.transform.rotation * Quaternion.AngleAxis(180, hitObject.transform.up);
                                        }
                                        else
                                        {
                                            objectRotation = hitObject.transform.rotation * Quaternion.AngleAxis(180, hitObject.transform.forward);
                                        }
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        break;

                    case "1D":
                        switch (selectedBrick.name)
                        {
                            case "T":
                                if (coordinates == Direction.North)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.up) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.0f);
                                }
                                break;
                            case "1D":
                                if (coordinates == Direction.North)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.up) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.0f);
                                }
                                break;
                            case "2D":
                                if (coordinates == Direction.Up || coordinates == Direction.Down || coordinates == Direction.North)
                                {
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                                }
                                else
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObject.transform.forward) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 2.0f);
                                }
                                break;
                            case "3D":
                                if (coordinates == Direction.Up || coordinates == Direction.Down || coordinates == Direction.North)
                                {
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                                }
                                else
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObject.transform.forward) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 2.0f);
                                }
                                break;
                            case "4D":
                                if (coordinates == Direction.Up || coordinates == Direction.Down || coordinates == Direction.North)
                                {
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                                }
                                else
                                {
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 2.0f);
                                }
                                break;
                            case "Base":
                                if (coordinates == Direction.Up || coordinates == Direction.Down)
                                {
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.5f); // distance from center

                                    if (coordinates == Direction.Up)
                                    {
                                        // Flip rotation from facing up to facing down
                                        objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.up) * objectRotation;
                                        print("euler rotation: " + objectRotation);
                                    }

                                    clamp = true;

                                    Vector3 connectionPointSouth = newGridCoordinates + (hitObject.transform.forward * -1);
                                    Vector3 hitPointCoordinate = grid.GetGridCoordinates(hitPoint);
                                    print("hit point coord: " + hitPointCoordinate);

                                    if (IsPointInRadius(hitPointCoordinate, connectionPointSouth, 0.7f)) newGridCoordinates = connectionPointSouth;

                                }
                                else if (coordinates == Direction.North)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.up, hitObject.transform.forward) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.5f);
                                }
                                else if (coordinates == Direction.South)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.forward) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                                }
                                break;
                            case "Double":
                                if (coordinates == Direction.Up || coordinates == Direction.Down)
                                {
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f); // distance from center

                                    if (coordinates == Direction.Up)
                                    {
                                        // Flip rotation from facing up to facing down
                                        objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.up) * objectRotation;
                                        print("euler rotation: " + objectRotation);
                                    }

                                    clamp = true;

                                    Vector3 connectionPointSouth = newGridCoordinates + (hitObject.transform.forward * -1);
                                    Vector3 hitPointCoordinate = grid.GetGridCoordinates(hitPoint);
                                    print("hit point coord: " + hitPointCoordinate);

                                    if (IsPointInRadius(hitPointCoordinate, connectionPointSouth, 0.7f)) newGridCoordinates = connectionPointSouth;

                                }
                                else if (coordinates == Direction.North)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.up, hitObject.transform.forward) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.5f);
                                }
                                else if (coordinates == Direction.South)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.forward) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                                }
                                break;
                            default:
                                break;
                        }
                        break;

                    case "2D":
                        switch (selectedBrick.name)
                        {
                            case "T":
                                if (coordinates == Direction.North)
                                {
                                    //objectRotation = Quaternion.FromToRotation(hitObject.transform.up, hitObject.transform.up) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                                }
                                else if (coordinates == Direction.East)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, hitObject.transform.right) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                                }
                                else if (coordinates == Direction.West)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, hitObject.transform.right) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                                }
                                else
                                {
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                                }
                                break;
                            case "1D":
                                if (coordinates != Direction.Up && coordinates != Direction.Down && coordinates != Direction.North && coordinates != Direction.East)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                                }
                                break;
                            case "2D":
                                if (coordinates != Direction.Up && coordinates != Direction.Down)
                                {
                                    objectRotation = Quaternion.FromToRotation(-hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 2.0f);
                                }
                                if (coordinates == Direction.North)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                                }
                                if (coordinates == Direction.East)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.right, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                                }
                                break;
                            case "3D":
                                if (coordinates != Direction.Up && coordinates != Direction.Down)
                                {
                                    objectRotation = Quaternion.FromToRotation(-hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 2.0f);
                                }
                                if (coordinates == Direction.North)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                                }
                                if (coordinates == Direction.East)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.right, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                                }
                                break;
                            case "4D":
                                if (coordinates != Direction.Up && coordinates != Direction.Down)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 2.0f);
                                }
                                break;
                            case "Base":
                                if (coordinates == Direction.Up || coordinates == Direction.Down)
                                {
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.5f); // distance from center

                                    if (coordinates == Direction.Up)
                                    {
                                        // Flip rotation from facing up to facing down
                                        objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.up) * objectRotation;
                                        print("euler rotation: " + objectRotation);
                                    }

                                    clamp = true;

                                    Vector3 connectionPointSouth = newGridCoordinates + (hitObject.transform.forward * -1);
                                    Vector3 connectionPointWest = newGridCoordinates + (hitObject.transform.right * -1);
                                    Vector3 hitPointCoordinate = grid.GetGridCoordinates(hitPoint);
                                    print("hit point coord: " + hitPointCoordinate);

                                    if (IsPointInRadius(hitPointCoordinate, connectionPointSouth, 0.7f)) newGridCoordinates = connectionPointSouth;
                                    else if (IsPointInRadius(hitPointCoordinate, connectionPointWest, 0.7f)) newGridCoordinates = connectionPointWest;

                                }
                                else if (coordinates == Direction.West)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.right) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                                }
                                else if (coordinates == Direction.South)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.forward) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                                }
                                break;
                            case "Double":
                                if (coordinates == Direction.Up || coordinates == Direction.Down)
                                {
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f); // distance from center

                                    if (coordinates == Direction.Up)
                                    {
                                        // Flip rotation from facing up to facing down
                                        objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.up) * objectRotation;
                                        print("euler rotation: " + objectRotation);
                                    }

                                    clamp = true;

                                    Vector3 connectionPointSouth = newGridCoordinates + (hitObject.transform.forward * -1);
                                    Vector3 connectionPointWest = newGridCoordinates + (hitObject.transform.right * -1);
                                    Vector3 hitPointCoordinate = grid.GetGridCoordinates(hitPoint);
                                    print("hit point coord: " + hitPointCoordinate);

                                    if (IsPointInRadius(hitPointCoordinate, connectionPointSouth, 0.7f)) newGridCoordinates = connectionPointSouth;
                                    else if (IsPointInRadius(hitPointCoordinate, connectionPointWest, 0.7f)) newGridCoordinates = connectionPointWest;

                                }
                                else if (coordinates == Direction.West)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.right) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                                }
                                else if (coordinates == Direction.South)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.forward) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                                }
                                break;
                        }
                        break;

                    case "3D":
                        switch (selectedBrick.name)
                        {
                            case "T":
                                if (coordinates == Direction.North)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.up, hitObject.transform.up) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                                }
                                else if (coordinates == Direction.East)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObject.transform.right) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                                }
                                else if (coordinates == Direction.West)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, hitObject.transform.right) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                                }
                                else
                                {
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                                }
                                break;
                            case "1D":
                                if (coordinates != Direction.Up && coordinates != Direction.Down && coordinates != Direction.North)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                                }
                                break;
                            case "2D":
                                if (coordinates != Direction.Up && coordinates != Direction.Down)
                                {
                                    objectRotation = Quaternion.FromToRotation(-hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 2.0f);
                                }
                                if (coordinates == Direction.North)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                                }
                                break;
                            case "3D":
                                if (coordinates != Direction.Up && coordinates != Direction.Down)
                                {
                                    objectRotation = Quaternion.FromToRotation(-hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 2.0f);
                                }
                                if (coordinates == Direction.North)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                                }
                                break;
                            case "4D":
                                if (coordinates != Direction.Up && coordinates != Direction.Down)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 2.0f);
                                }
                                break;
                            case "Base":
                                if (coordinates == Direction.Up || coordinates == Direction.Down)
                                {
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.5f); // distance from center

                                    if (coordinates == Direction.Up)
                                    {
                                        // Flip rotation from facing up to facing down
                                        objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.up) * objectRotation;
                                        print("euler rotation: " + objectRotation);
                                    }

                                    clamp = true;

                                    Vector3 connectionPointEast = newGridCoordinates + (hitObject.transform.right);
                                    Vector3 connectionPointSouth = newGridCoordinates + (hitObject.transform.forward * -1);
                                    Vector3 connectionPointWest = newGridCoordinates + (hitObject.transform.right * -1);
                                    Vector3 hitPointCoordinate = grid.GetGridCoordinates(hitPoint);
                                    print("hit point coord: " + hitPointCoordinate);

                                    if (IsPointInRadius(hitPointCoordinate, connectionPointEast, 0.7f)) newGridCoordinates = connectionPointEast;
                                    else if (IsPointInRadius(hitPointCoordinate, connectionPointSouth, 0.7f)) newGridCoordinates = connectionPointSouth;
                                    else if (IsPointInRadius(hitPointCoordinate, connectionPointWest, 0.7f)) newGridCoordinates = connectionPointWest;

                                }
                                else if (coordinates == Direction.East)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.up, hitObject.transform.right) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                                }
                                else if (coordinates == Direction.West)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.right) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                                }
                                else if (coordinates == Direction.South)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.forward) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                                }
                                break;
                            case "Double":
                                if (coordinates == Direction.Up || coordinates == Direction.Down)
                                {
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f); // distance from center

                                    if (coordinates == Direction.Up)
                                    {
                                        // Flip rotation from facing up to facing down
                                        objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.up) * objectRotation;
                                        print("euler rotation: " + objectRotation);
                                    }

                                    clamp = true;

                                    Vector3 connectionPointEast = newGridCoordinates + (hitObject.transform.right);
                                    Vector3 connectionPointSouth = newGridCoordinates + (hitObject.transform.forward * -1);
                                    Vector3 connectionPointWest = newGridCoordinates + (hitObject.transform.right * -1);
                                    Vector3 hitPointCoordinate = grid.GetGridCoordinates(hitPoint);
                                    print("hit point coord: " + hitPointCoordinate);

                                    if (IsPointInRadius(hitPointCoordinate, connectionPointEast, 0.7f)) newGridCoordinates = connectionPointEast;
                                    else if (IsPointInRadius(hitPointCoordinate, connectionPointSouth, 0.7f)) newGridCoordinates = connectionPointSouth;
                                    else if (IsPointInRadius(hitPointCoordinate, connectionPointWest, 0.7f)) newGridCoordinates = connectionPointWest;

                                }
                                else if (coordinates == Direction.East)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.up, hitObject.transform.right) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                                }
                                else if (coordinates == Direction.West)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.right) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                                }
                                else if (coordinates == Direction.South)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.forward) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                                }
                                break;
                        }
                        break;

                    case "4D":
                        switch (selectedBrick.name)
                        {
                            case "T":
                                if (coordinates == Direction.North)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.up) * objectRotation;
                                }
                                else if (coordinates == Direction.East)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObject.transform.right) * objectRotation;
                                }
                                else if (coordinates == Direction.West)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, hitObject.transform.right) * objectRotation;
                                }

                                newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);

                                break;
                            case "1D":
                                if (coordinates != Direction.Up && coordinates != Direction.Down)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                                }
                                break;
                            case "2D":
                                if (coordinates != Direction.Up && coordinates != Direction.Down)
                                {
                                    objectRotation = Quaternion.FromToRotation(-hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 2.0f);
                                }
                                break;
                            case "3D":
                                if (coordinates != Direction.Up && coordinates != Direction.Down)
                                {
                                    objectRotation = Quaternion.FromToRotation(-hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 2.0f);
                                }
                                break;
                            case "4D":
                                if (coordinates != Direction.Up && coordinates != Direction.Down)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 2.0f);
                                }
                                break;
                            case "Base":
                                if (coordinates == Direction.Up || coordinates == Direction.Down)
                                {
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.5f); // distance from center

                                    if (coordinates == Direction.Up)
                                    {
                                        // Flip rotation from facing up to facing down
                                        objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.up) * objectRotation;
                                        print("euler rotation: " + objectRotation);
                                    }

                                    clamp = true;

                                    Vector3 connectionPointNorth = newGridCoordinates + (hitObject.transform.forward);
                                    Vector3 connectionPointEast = newGridCoordinates + (hitObject.transform.right);
                                    Vector3 connectionPointSouth = newGridCoordinates + (hitObject.transform.forward * -1);
                                    Vector3 connectionPointWest = newGridCoordinates + (hitObject.transform.right * -1);
                                    Vector3 hitPointCoordinate = grid.GetGridCoordinates(hitPoint);
                                    print("hit point coord: " + hitPointCoordinate);

                                    if (IsPointInRadius(hitPointCoordinate, connectionPointNorth, 0.7f)) newGridCoordinates = connectionPointNorth;
                                    else if (IsPointInRadius(hitPointCoordinate, connectionPointEast, 0.7f)) newGridCoordinates = connectionPointEast;
                                    else if (IsPointInRadius(hitPointCoordinate, connectionPointSouth, 0.7f)) newGridCoordinates = connectionPointSouth;
                                    else if (IsPointInRadius(hitPointCoordinate, connectionPointWest, 0.7f)) newGridCoordinates = connectionPointWest;

                                }
                                else if (coordinates == Direction.North)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.up, hitObject.transform.forward) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                                }
                                else if (coordinates == Direction.East)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.up, hitObject.transform.right) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                                }
                                else if (coordinates == Direction.West)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.right) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                                }
                                else if (coordinates == Direction.South)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.forward) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                                }
                                break;

                            case "Double":
                                if (coordinates == Direction.Up || coordinates == Direction.Down)
                                {
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f); // distance from center

                                    if (coordinates == Direction.Up)
                                    {
                                        // Flip rotation from facing up to facing down
                                        objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.up) * objectRotation;
                                        print("euler rotation: " + objectRotation);
                                    }

                                    clamp = true;

                                    Vector3 connectionPointNorth = newGridCoordinates + (hitObject.transform.forward);
                                    Vector3 connectionPointEast = newGridCoordinates + (hitObject.transform.right);
                                    Vector3 connectionPointSouth = newGridCoordinates + (hitObject.transform.forward * -1);
                                    Vector3 connectionPointWest = newGridCoordinates + (hitObject.transform.right * -1);
                                    Vector3 hitPointCoordinate = grid.GetGridCoordinates(hitPoint);
                                    print("hit point coord: " + hitPointCoordinate);

                                    if (IsPointInRadius(hitPointCoordinate, connectionPointNorth, 0.7f)) newGridCoordinates = connectionPointNorth;
                                    else if (IsPointInRadius(hitPointCoordinate, connectionPointEast, 0.7f)) newGridCoordinates = connectionPointEast;
                                    else if (IsPointInRadius(hitPointCoordinate, connectionPointSouth, 0.7f)) newGridCoordinates = connectionPointSouth;
                                    else if (IsPointInRadius(hitPointCoordinate, connectionPointWest, 0.7f)) newGridCoordinates = connectionPointWest;

                                }
                                else if (coordinates == Direction.North)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.up, hitObject.transform.forward) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                                }
                                else if (coordinates == Direction.East)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.up, hitObject.transform.right) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                                }
                                else if (coordinates == Direction.West)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.right) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                                }
                                else if (coordinates == Direction.South)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.up, -hitObject.transform.forward) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                                }
                                break;
                        }
                        break;

                    case "Double":
                        switch (selectedBrick.name)
                        {
                            case "T":
                                if (coordinates == Direction.Up)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, hitObject.transform.up) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.67f);
                                }
                                else if (coordinates == Direction.Down)
                                {
                                    objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObject.transform.up) * objectRotation;
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.67f);
                                }
                                else
                                {
                                    Vector3 connectionPointTop = newGridCoordinates + (hitObject.transform.up * 1.5f);
                                    Vector3 hitPointCoordinate = grid.GetGridCoordinates(hitPoint);

                                    if (IsPointInRadius(hitPointCoordinate, connectionPointTop, 0.7f))
                                    {
                                        objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                                        newGridCoordinates = GetCoordinates(hitObjectProperties.GetWorldDirection(Direction.Up), gridCoordinates, 1.5f);
                                    }
                                    else
                                    {
                                        objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                                        newGridCoordinates = GetCoordinates(hitObjectProperties.GetWorldDirection(Direction.Down), gridCoordinates, 0.5f);
                                    }
                                }
                                break;
                            case "Base":
                                if (coordinates == Direction.Up)
                                {
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 2.0f);
                                }
                                else if (coordinates == Direction.Down)
                                {
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                                }
                                else
                                {
                                    Vector3 connectionPointTop = newGridCoordinates + (hitObject.transform.up * 1.0f);
                                    Vector3 hitPointCoordinate = grid.GetGridCoordinates(hitPoint);

                                    if (IsPointInRadius(hitPointCoordinate, connectionPointTop, 0.7f))
                                    {
                                        //objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                                        newGridCoordinates = GetCoordinates(direction, connectionPointTop, 0.0f);
                                    }
                                    else
                                    {
                                        //objectRotation = Quaternion.FromToRotation(hitObject.transform.forward, -hitObjectProperties.ConvertDirectionToVector(direction)) * objectRotation;
                                        newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.0f);
                                    }
                                }
                                break;
                            case "Double":
                                if (coordinates == Direction.Up)
                                {
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 2.0f);
                                }
                                else if (coordinates == Direction.Down)
                                {
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 2.0f);
                                }
                                break;
                            default:
                                if (coordinates == Direction.Up)
                                {
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 1.5f);
                                }
                                else if (coordinates == Direction.Down)
                                {
                                    newGridCoordinates = GetCoordinates(direction, gridCoordinates, 0.5f);
                                }
                                break;
                        }
                        break;

                    default:
                        break;
                }

                // Props
                if (hitObject.CompareTag("prop"))
                {
                    float propHeight = 0;
                    propHeight = hitObject.GetComponent<BoxCollider>().size.y;
                    propHeight = grid.ConvertToGridDistance(propHeight);

                    newGridCoordinates = GetCoordinates(Direction.Up, gridCoordinates, propHeight);
                    print(propHeight);
                }
                else if (selectedBrick.CompareTag("prop"))
                {
                    newGridCoordinates = GetCoordinates(Direction.Up, gridCoordinates, 0.5f);
                    objectRotation = selectedBrick.transform.rotation;

                    if (hitObject.name == "Double")
                    {
                        newGridCoordinates = GetCoordinates(Direction.Up, gridCoordinates, 1.5f);
                    }
                }

                PlaceCubeNear(direction, objectRotation, newGridCoordinates, hitObjectProperties, clamp);
            }
        }
    }

    private void PlaceCubeOnPlane(Vector3 clickPoint)
    {
        // Check if challenge limit is not exceded
        if (isChallenge && !challengeSystem.CheckForLimit(selectedBrick.name)) { return; }

        // Get position based on grid point closest to hit point
        Vector3 coordinates = grid.GetGridCoordinates(clickPoint);
        Vector3 finalPosition = grid.GetNearestPointOnGrid(clickPoint);

        // Change position based on prop
        if (selectedBrick.CompareTag("prop"))
        {
            coordinates = GetCoordinates(Direction.Down, coordinates, 0.5f);
            finalPosition = grid.CoordinatesToGridPoint(coordinates);
        }

        // Create brick
        if (isPreRenderMode)
        {
            preRenderBrick.transform.position = finalPosition;
            var brickProperties = preRenderBrick.GetComponent<BrickProperties>();
            if (brickProperties != null && brickProperties.isRotated != true)
            {
                preRenderBrick.transform.rotation = brickProperties.initialRotation;
            }

        }
        else
        {
            var newBrick = Instantiate(preRenderBrick, finalPosition, preRenderBrick.transform.rotation);
            newBrick = ConvertToSolid(newBrick);

            var newBrickProperties = newBrick.GetComponent<BrickProperties>();

            if (newBrickProperties != null)
            {
                newBrickProperties.coordinates = coordinates;
            }

            _ = StartCoroutine(CheckIfCollides(newBrickProperties, null, false));
        }

        

    }

    private void PlaceCubeNear(Direction direction, Quaternion objectRotation, Vector3 gridCoordinates, BrickProperties clickedObjectProperties = null, bool clamp = false)
    {
        // Check if challenge limit is not exceded
        if (isChallenge && !challengeSystem.CheckForLimit(selectedBrick.name)) { return; }

        // Get final grid point for clicked Object
        Vector3 finalPosition = grid.CoordinatesToGridPoint(gridCoordinates);

        
        // Create brick
        if (isPreRenderMode)
        {
            preRenderBrick.transform.position = finalPosition;
            var preRenderBrickProperties = preRenderBrick.GetComponent<BrickProperties>();
            if (preRenderBrickProperties != null && preRenderBrickProperties.isRotated != true)
            {
                preRenderBrick.transform.rotation = objectRotation;
            }

        }
        else
        {
            // Create new brick from gameObject, final position, rotation
            var newBrick = Instantiate(preRenderBrick, finalPosition, preRenderBrick.transform.rotation);
            newBrick = ConvertToSolid(newBrick);

            var newBrickProperties = newBrick.GetComponent<BrickProperties>();

            // Move new brick coordinate to grid coordinate
            if (newBrickProperties != null)
            {
                newBrickProperties.coordinates = gridCoordinates;
                newBrickProperties.localDirection = clickedObjectProperties.localDirection;
            }

            _ = StartCoroutine(CheckIfCollides(newBrickProperties, clickedObjectProperties, clamp));
        }
        
        

    }
    
    // Check Brick/Prop collision, confirm placement
    private IEnumerator CheckIfCollides(BrickProperties brickProperties, BrickProperties clickedObjectProperties, bool clamp)
    {
        yield return new WaitForSeconds(0.1f);

        GameObject newBrick = brickProperties.gameObject;
        if (!brickProperties.isCollided)
        {
            // Create and Save undo
            UndoAction undoAction = new UndoAction
            {
                Index = currentUndoPosition + 1,
                Type = 1,
                Bricks = new List<GameObject>() { newBrick }
            };
            SaveUndo(undoAction);

            brickProperties.SaveInitialState();

            placedBricks.Add(newBrick.transform);
            if (isChallenge) { challengeSystem.UpdateLimits(placedBricks); }
        }
        else
        {
            // New brick already collides?
            Destroy(brickProperties.gameObject);
        }


        // Reset Pre-Render after Brick is placed
        isPreRenderMode = true;
        preRenderBrick.SetActive(true);

    }

    // Reusable Calculation Functions
    public bool V3Equal(Vector3 a, Vector3 b)
    {
        return Vector3.SqrMagnitude(a - b) < 250;
    }
    private enum Brick { Base, T };
    private Direction GetDirection(Vector3 incomingVec)
    {
        if (incomingVec == new Vector3(0, -1, -1))
            return Direction.South;

        if (incomingVec == new Vector3(0, -1, 1))
            return Direction.North;

        if (incomingVec == new Vector3(0, 0, 0))
            return Direction.Up;

        if (incomingVec == new Vector3(0, -2, 0)) //(0, -2, 0)?
            return Direction.Down;

        if (incomingVec == new Vector3(-1, -1, 0))
            return Direction.West;

        if (incomingVec == new Vector3(1, -1, 0))
            return Direction.East;

        return new Direction();
    }

    private Vector3 GetCoordinates(Direction direction, Vector3 coordinates, float amount)
    {
        switch (direction)
        {
            case Direction.Up:
                return new Vector3(coordinates.x, coordinates.y + amount, coordinates.z); // Up
            case Direction.Down:
                return new Vector3(coordinates.x, coordinates.y - amount, coordinates.z); // Down

            case Direction.East:
                return new Vector3(coordinates.x + amount, coordinates.y, coordinates.z); // East
            case Direction.West:
                return new Vector3(coordinates.x - amount, coordinates.y, coordinates.z); // West

            case Direction.North:
                return new Vector3(coordinates.x, coordinates.y, coordinates.z + amount); // North
            case Direction.South:
                return new Vector3(coordinates.x, coordinates.y, coordinates.z - amount); // South

            default:
                break;
        }
        return Vector3.zero;
    }
    private bool IsPointInRadius(Vector3 hitPoint, Vector3 radiusPoint, float radius)
    {
        if (Vector3.Distance(radiusPoint, hitPoint) < radius)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
