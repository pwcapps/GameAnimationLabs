using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {

    List<Vector2Int> closedCells;
    float height = 0.5f;
    int gridSize = 22;

    private void CreateGhost(Pathfinding.Algorithm alg, int num, Vector3 pos)
    {
        GameObject ghost = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ghost.name = "Ghost" + num;
        ghost.transform.position = pos; 
        Pathfinding script = ghost.AddComponent<Pathfinding>();
        script.obstacles = closedCells;
        script.gridSize = gridSize;
        script.algorithm = alg;
    }

    private void CreateGrid()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (i == 0 || i == gridSize - 1 || j == 0 || j == gridSize - 1)
                {
                    GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    wall.transform.position = new Vector3(i, height, j);
                    Vector2Int n = new Vector2Int();
                    n.x = i;
                    n.y = j;
                    closedCells.Add(n);
                }
            }
        }
        GameObject cube = GameObject.Find("Cube");
        Vector2Int cPos = new Vector2Int((int)cube.transform.position.x, (int)cube.transform.position.z);
        closedCells.Add(cPos);
        for (int i = 1; i <= 69; i++)
        {
            cube = GameObject.Find("Cube (" + i + ")");
            cPos = new Vector2Int((int)cube.transform.position.x, (int)cube.transform.position.z);
            closedCells.Add(cPos);
        }
    }

    // Use this for initialization
    void Start () {
        closedCells = new List<Vector2Int>();
        CreateGrid();
        CreateGhost(Pathfinding.Algorithm.RandomWalk, 1, new Vector3(1, height, 1));
        CreateGhost(Pathfinding.Algorithm.BreadthFirst, 2, new Vector3(gridSize - 2, height, 1));
        CreateGhost(Pathfinding.Algorithm.BestFirst, 3, new Vector3(1, height, gridSize - 2));
        CreateGhost(Pathfinding.Algorithm.Djikstra, 4, new Vector3(gridSize - 2, height, gridSize - 2));
        CreateGhost(Pathfinding.Algorithm.AStar, 5, new Vector3(10, height, 10));
    }
}
