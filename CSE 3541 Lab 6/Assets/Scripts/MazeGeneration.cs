using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGeneration : MonoBehaviour {

    bool[,] mazeGrid = new bool[50, 50];

    void RecursiveBacktracker(int cx, int cy)
    {
        int[] DX = new int[] { 0, 0, 2, -2 };
        int[] DY = new int[] { 2, -2, 0, 0 };
        int direction = Random.Range(0, 4);

        for (int i = 0; i < 4; i++)
        {
            int nx = cx + DX[direction];
            int ny = cy + DY[direction];
            direction++;
            if (direction > 3)
                direction = 0;

            if (ny >= 0 && ny < mazeGrid.GetLength(1) && nx >= 0 && nx < mazeGrid.GetLength(0) && mazeGrid[nx, ny] == false)
            {
                mazeGrid[cx, cy] = true;
                mazeGrid[nx, ny] = true;
                mazeGrid[(cx + nx) / 2, (cy + ny) / 2] = true;
                RecursiveBacktracker(nx, ny);
            }
        }
    }

    void BuildMaze()
    {
        for (int i = 0; i < mazeGrid.GetLength(0) - 1; i++)
        {
            for (int j = 0; j < mazeGrid.GetLength(1) - 1; j++)
            {
                if (!mazeGrid[i, j])
                {
                    GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    wall.transform.parent = transform;

                    wall.transform.position = new Vector3(i, 0.5f, j);
                }
            }
        }
    }

    void AddWalls()
    {
        for (int i = -1; i < 50; i++)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.parent = transform;
            wall.transform.position = new Vector3(-1, 0.5f, i);
            wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.parent = transform;
            wall.transform.position = new Vector3(49, 0.5f, i);
        }
        for (int i = 0; i < 50; i++)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.parent = transform;
            wall.transform.position = new Vector3(i, 0.5f, -1);
            wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.parent = transform;
            wall.transform.position = new Vector3(i, 0.5f, 49);
        }
    }

	// Use this for initialization
	void Start () {
        RecursiveBacktracker(0, 0);
        BuildMaze();
        AddWalls();
	}
}
