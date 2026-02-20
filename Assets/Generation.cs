using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

public class Generation : MonoBehaviour
{
    [Header("Forest Variables")]
    public Vector3 forestPos;
    public int treeNum = 5;
    public int treeSpread = 5;
    public float treeSizeVariation = 2;

    [Header("Pyramid Variables")]
    public Vector3 pyramidPos;
    [Range(3, 10)]
    public int pyramidLevels = 3;

    [Header("Celestial Variables")]
    public Vector3 sunPos;
    public float spinRate = 5;
    GameObject sun;
    Renderer sunRenderer;
    Light directionalLight;
    float time;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the directional light for the day-night cycle
        directionalLight = GameObject.Find("Directional Light").GetComponent<Light>();

        CreateGround();
        CreateForest();
        CreatePyramid();
        CreateSun();
    }

    // Create the ground plane and make it big
    void CreateGround()
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.transform.position = new Vector3(0, 0, 0);
        ground.transform.localScale = new Vector3(5, 1, 5);
        ground.name = "Ground";
    }

    // Create the forest
    void CreateForest()
    {
        GameObject treeParent = new GameObject("Tree Parent");
        treeParent.transform.position = forestPos;
        for (int i = 0; i < treeNum; i++)
        {
            // Create a tree, set a random position around forestPos via treeSpread, set a random size via treeSizeVariation, assign treeParent as its parent
            GameObject tree = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tree.transform.position = forestPos + new Vector3(Random.Range(-treeSpread, treeSpread), 0, Random.Range(-treeSpread, treeSpread));
            tree.transform.localScale = new Vector3(Random.Range(-treeSizeVariation, treeSizeVariation), Random.Range(-treeSizeVariation, treeSizeVariation), Random.Range(-treeSizeVariation, treeSizeVariation));
            tree.transform.position = new Vector3(tree.transform.position.x, tree.transform.position.y + Mathf.Abs(tree.transform.localScale.y), tree.transform.position.z);
            tree.GetComponent<Renderer>().material.color = new Color(Random.Range(0.0f, 0.5f), Random.Range(0.8f, 1.0f), Random.Range(0.0f, 0.5f));
            tree.name = "Tree";
            tree.transform.parent = treeParent.transform;
        }
    }

    // Create the pyramid
    void CreatePyramid()
    {
        // Create the parent for the pyramid
        GameObject pyramidParent = new GameObject("Pyramid Parent");
        pyramidParent.transform.position = pyramidPos;
        for (int i = pyramidLevels; i > 0; i--)
        {
            // Make a color for the current level of the pyramid
            Color newColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            GameObject levelParent = new GameObject("Level " + i.ToString());
            levelParent.transform.parent = pyramidParent.transform;

            // Assign a rectTransform to work with the grid layout group
            levelParent.AddComponent<RectTransform>();
            levelParent.GetComponent<RectTransform>().position = new Vector3(pyramidParent.transform.position.x, pyramidParent.transform.position.y + (pyramidLevels - i + .5f), pyramidParent.transform.position.z);
            levelParent.GetComponent<RectTransform>().eulerAngles = new Vector3(gameObject.transform.eulerAngles.x + 75, gameObject.transform.eulerAngles.y, gameObject.transform.eulerAngles.z);
            
            // Add a grid layout group component to automatically arrange the cubes making up this level of the pyramid
            levelParent.AddComponent<GridLayoutGroup>();
            GridLayoutGroup grid = levelParent.GetComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(1, 1);
            grid.spacing = new Vector2(.1f, .1f);
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.childAlignment = TextAnchor.MiddleCenter;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = i;

            // Create cubes depending on the size of the level, assign its parent and color
            for (int cubes = 0; cubes < Mathf.Pow(i, 2); cubes++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.AddComponent<RectTransform>();
                cube.transform.SetParent(levelParent.transform, false);
                cube.GetComponent<Renderer>().material.color = newColor;
            }
        }
    }

    // Create the sun
    void CreateSun()
    {
        // Create a large sphere and get a reference to its renderer and object
        sun = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sun.transform.position = sunPos;
        sun.transform.localScale = new Vector3(5, 5, 5);
        sunRenderer = sun.GetComponent<Renderer>();
        sun.name = "Sun";
    }

    private void Update()
    {
        // Spin the sun and create a day-night cycle based on it
        if (sun)
        {
            // Rotate the sun and add time based on the speed of its rotation
            sun.transform.Rotate(new Vector3(0, spinRate, 0), Space.Self);
            time += Time.deltaTime * spinRate;

            // If time is 6:00 am or later, brighten the sphere to be yellow and the directional light to be white
            if (time > 6)
            {
                sunRenderer.material.color = new Color(Mathf.Lerp(sunRenderer.material.color.r, 1f, Time.deltaTime * spinRate), Mathf.Lerp(sunRenderer.material.color.g, 0.8f, Time.deltaTime * spinRate), 0);
                directionalLight.color = new Color(Mathf.Lerp(directionalLight.color.r, 1, Time.deltaTime * spinRate), Mathf.Lerp(directionalLight.color.g, 1, Time.deltaTime * spinRate), Mathf.Lerp(directionalLight.color.b, 1, Time.deltaTime * spinRate));
            }
            else if (time < 6) // If time is between 5:00 pm and 6:00 am, darken the sphere and directional light
            {
                sunRenderer.material.color = new Color(Mathf.Lerp(sunRenderer.material.color.r, 0, Time.deltaTime * spinRate), Mathf.Lerp(sunRenderer.material.color.g, 0, Time.deltaTime * spinRate), 0);
                directionalLight.color = new Color(Mathf.Lerp(directionalLight.color.r, 0, Time.deltaTime * spinRate), Mathf.Lerp(directionalLight.color.g, 0, Time.deltaTime * spinRate), Mathf.Lerp(directionalLight.color.b, 0, Time.deltaTime * spinRate));
            }

            // If time reaches 5:00 pm, cycle to -7 (6:00 pm) to simulate dusk/night
            if (time > 17)
            {
                time = -7;
            }
        }
    }
}
