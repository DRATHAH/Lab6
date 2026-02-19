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
    [Range(3, 5)]
    public int pyramidLevels = 3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateGround();
        CreateForest();
        CreatePyramid();
    }

    void CreateGround()
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.transform.position = new Vector3(0, 0, 0);
        ground.transform.localScale = new Vector3(5, 1, 5);
        ground.name = "Ground";
    }

    void CreateForest()
    {
        GameObject treeParent = new GameObject("Tree Parent");
        treeParent.transform.position = forestPos;
        for (int i = 0; i < treeNum; i++)
        {
            GameObject tree = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tree.transform.position = forestPos + new Vector3(Random.Range(-treeSpread, treeSpread), 0, Random.Range(-treeSpread, treeSpread));
            tree.transform.localScale = new Vector3(Random.Range(-treeSizeVariation, treeSizeVariation), Random.Range(-treeSizeVariation, treeSizeVariation), Random.Range(-treeSizeVariation, treeSizeVariation));
            tree.transform.position = new Vector3(tree.transform.position.x, tree.transform.position.y + Mathf.Abs(tree.transform.localScale.y), tree.transform.position.z);
            tree.GetComponent<Renderer>().material.color = Color.green;
            tree.name = "Tree";
            tree.transform.parent = treeParent.transform;
        }
    }

    void CreatePyramid()
    {
        GameObject pyramidParent = new GameObject("Pyramid Parent");
        for (int i = pyramidLevels; i > 0; i--)
        {
            GameObject levelParent = new GameObject("Level " + i.ToString());
            levelParent.AddComponent<RectTransform>();
            levelParent.GetComponent<RectTransform>().position = new Vector3(pyramidParent.transform.position.x, pyramidParent.transform.position.y + (pyramidLevels - i), pyramidParent.transform.position.z);
            levelParent.GetComponent<RectTransform>().eulerAngles = new Vector3(gameObject.transform.eulerAngles.x + 75, gameObject.transform.eulerAngles.y, gameObject.transform.eulerAngles.z);
            levelParent.AddComponent<GridLayoutGroup>();
            GridLayoutGroup grid = levelParent.GetComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(1, 1);
            grid.spacing = new Vector2(.1f, .1f);
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.childAlignment = TextAnchor.MiddleCenter;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = i;
            LayoutRebuilder.ForceRebuildLayoutImmediate(levelParent.GetComponent<RectTransform>());

            for (int cubes = 0; cubes < Mathf.Pow(i, 2); cubes++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.parent = levelParent.transform;
            }
            Debug.Log(levelParent.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(levelParent.GetComponent<RectTransform>());
        }
    }
}
