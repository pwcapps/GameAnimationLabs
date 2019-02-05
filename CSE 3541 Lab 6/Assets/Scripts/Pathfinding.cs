using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public enum Algorithm
    {
        RandomWalk, BreadthFirst, BestFirst, Djikstra, AStar
    }
    public Algorithm algorithm;
    public List<Vector2Int> obstacles;
    public int gridSize;
    Vector2Int goalCell, next;
    List<Vector2Int> path;
    float speed;
    float height;
    int i;
    GameObject target;

    private void NewGoal()
    {
        do
        {
            goalCell = new Vector2Int(Random.Range(1, gridSize - 1), Random.Range(1, gridSize - 1));
            target.transform.position = new Vector3(goalCell.x, height, goalCell.y);
        } while (obstacles.Contains(goalCell));
    }

    private int ManhattanDistance(Vector2Int a, Vector2Int b)
    {
        return System.Math.Abs(b.x - a.x) + System.Math.Abs(b.y - a.y);
    }

    private Vector2Int RandomWalk()
    {
        Vector2Int start = new Vector2Int((int)transform.position.x, (int)transform.position.z);
        int[] DX = new int[] { 1, -1, 0, 0 };
        int[] DZ = new int[] { 0, 0, 1, -1 };
        System.Random r = new System.Random();
        foreach (int i in Enumerable.Range(0, 4).OrderBy(x => r.Next()))
        {
            Vector2Int step = new Vector2Int(start.x + DX[i], start.y + DZ[i]);
            //bool distanceCheck = ManhattanDistance(step, goalCell) < ManhattanDistance(start, goalCell) || Random.Range(0f, 100f) > 98f;
            if (step.x > 0 && step.x < gridSize - 1 && step.y > 0 && step.y < gridSize - 1 && !obstacles.Contains(step))
            {
                return step;
            }
        }

        return start;
    }

    private void BreadthFirstSearch()
    {
        path.Clear();
        Vector2Int start = new Vector2Int((int)transform.position.x, (int)transform.position.z);
        Queue<Vector2Int> open = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> closed = new Dictionary<Vector2Int, Vector2Int>();
        open.Enqueue(start);
        closed.Add(start, start);
        while (open.Count != 0)
        {
            Vector2Int currentCell = open.Dequeue(); // Get from open list
            if (currentCell.Equals(goalCell))
                break;
            Vector2Int currentLeft = new Vector2Int(currentCell.x - 1, currentCell.y);
            Vector2Int currentRight = new Vector2Int(currentCell.x + 1, currentCell.y);
            Vector2Int currentDown = new Vector2Int(currentCell.x, currentCell.y - 1);
            Vector2Int currentUp = new Vector2Int(currentCell.x, currentCell.y + 1);
            if (currentLeft.x > 0 && !closed.ContainsKey(currentLeft) && !obstacles.Contains(currentLeft))
            {
                open.Enqueue(currentLeft);
                closed.Add(currentLeft, currentCell);
            }
            if (currentRight.x < gridSize - 1 && !closed.ContainsKey(currentRight) && !obstacles.Contains(currentRight))
            {
                open.Enqueue(currentRight);
                closed.Add(currentRight, currentCell);
            }
            if (currentDown.y > 0 && !closed.ContainsKey(currentDown) && !obstacles.Contains(currentDown))
            {
                open.Enqueue(currentDown);
                closed.Add(currentDown, currentCell);
            }
            if (currentUp.y < gridSize - 1 && !closed.ContainsKey(currentUp) && !obstacles.Contains(currentUp))
            {
                open.Enqueue(currentUp);
                closed.Add(currentUp, currentCell);
            }
        }

        Vector2Int current = goalCell;
        while (!current.Equals(start))
        {
            path.Add(current);
            closed.TryGetValue(current, out current);
        }
        path.Reverse();
    }

    private void BestFirstSearch()
    {
        path.Clear();
        Vector2Int start = new Vector2Int((int)transform.position.x, (int)transform.position.z);
        List<KeyValuePair<Vector2Int, int>> openPriority = new List<KeyValuePair<Vector2Int, int>>();
        Dictionary<Vector2Int, Vector2Int> closed = new Dictionary<Vector2Int, Vector2Int>();
        openPriority.Add(new KeyValuePair<Vector2Int, int>(start, ManhattanDistance(start, goalCell)));
        closed.Add(start, start);
        while (openPriority.Count != 0)
        {
            openPriority.Sort((x, y) => x.Value.CompareTo(y.Value));
            Vector2Int currentCell = openPriority[0].Key; // Get from open list
            openPriority.RemoveAt(0);
            if (currentCell.Equals(goalCell))
                break;
            Vector2Int currentLeft = new Vector2Int(currentCell.x - 1, currentCell.y);
            Vector2Int currentRight = new Vector2Int(currentCell.x + 1, currentCell.y);
            Vector2Int currentDown = new Vector2Int(currentCell.x, currentCell.y - 1);
            Vector2Int currentUp = new Vector2Int(currentCell.x, currentCell.y + 1);
            if (currentLeft.x > 0 && !closed.ContainsKey(currentLeft) && !obstacles.Contains(currentLeft))
            {
                openPriority.Add(new KeyValuePair<Vector2Int, int>(currentLeft, ManhattanDistance(currentLeft, goalCell)));
                closed.Add(currentLeft, currentCell);
            }
            if (currentRight.x < gridSize - 1 && !closed.ContainsKey(currentRight) && !obstacles.Contains(currentRight))
            {
                openPriority.Add(new KeyValuePair<Vector2Int, int>(currentRight, ManhattanDistance(currentRight, goalCell)));
                closed.Add(currentRight, currentCell);
            }
            if (currentDown.y > 0 && !closed.ContainsKey(currentDown) && !obstacles.Contains(currentDown))
            {
                openPriority.Add(new KeyValuePair<Vector2Int, int>(currentDown, ManhattanDistance(currentDown, goalCell)));
                closed.Add(currentDown, currentCell);
            }
            if (currentUp.y < gridSize - 1 && !closed.ContainsKey(currentUp) && !obstacles.Contains(currentUp))
            {
                openPriority.Add(new KeyValuePair<Vector2Int, int>(currentUp, ManhattanDistance(currentUp, goalCell)));
                closed.Add(currentUp, currentCell);
            }
        }

        Vector2Int current = goalCell;
        while (!current.Equals(start))
        {
            path.Add(current);
            closed.TryGetValue(current, out current);
        }
        path.Reverse();
    }

    private void DjikstraSearch()
    {
        path.Clear();
        Vector2Int start = new Vector2Int((int)transform.position.x, (int)transform.position.z);
        List<KeyValuePair<Vector2Int, int>> openPriority = new List<KeyValuePair<Vector2Int, int>>();
        Dictionary<Vector2Int, Vector2Int> closed = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, int> cost = new Dictionary<Vector2Int, int>();
        openPriority.Add(new KeyValuePair<Vector2Int, int>(start, 0));
        closed.Add(start, start);
        cost.Add(start, 0);
        while (openPriority.Count != 0)
        {
            openPriority.Sort((x, y) => x.Value.CompareTo(y.Value));
            openPriority.Reverse();
            Vector2Int currentCell = openPriority[0].Key; // Get from open list
            openPriority.RemoveAt(0);
            if (currentCell.Equals(goalCell))
                break;

            int[] DX = new int[] { 1, -1, 0, 0 };
            int[] DZ = new int[] { 0, 0, 1, -1 };
            System.Random r = new System.Random();
            foreach (int i in Enumerable.Range(0, 4).OrderBy(x => r.Next()))
            {
                Vector2Int step = new Vector2Int(currentCell.x + DX[i], currentCell.y + DZ[i]);
                int c;
                cost.TryGetValue(currentCell, out c);
                int stepCost = c++;
                int c2;
                cost.TryGetValue(step, out c2);
                if (step.x > 0 && step.x < gridSize - 1 && step.y > 0 && step.y < gridSize - 1 && !obstacles.Contains(step) && (!cost.ContainsKey(step) || stepCost < c2))
                {
                    cost.Remove(step);
                    cost.Add(step, stepCost);
                    openPriority.Add(new KeyValuePair<Vector2Int, int>(step, stepCost));
                    closed.Add(step, currentCell);
                }
            }
        }

        Vector2Int current = goalCell;
        while (!current.Equals(start))
        {
            path.Add(current);
            closed.TryGetValue(current, out current);
        }
        path.Reverse();
    }

    private void AStarSearch()
    {
        path.Clear();
        Vector2Int start = new Vector2Int((int)transform.position.x, (int)transform.position.z);
        List<KeyValuePair<Vector2Int, int>> openPriority = new List<KeyValuePair<Vector2Int, int>>();
        Dictionary<Vector2Int, Vector2Int> closed = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, int> cost = new Dictionary<Vector2Int, int>();
        openPriority.Add(new KeyValuePair<Vector2Int, int>(start, 0));
        closed.Add(start, start);
        cost.Add(start, 0);
        while (openPriority.Count != 0)
        {
            openPriority.Sort((x, y) => x.Value.CompareTo(y.Value));
            Vector2Int currentCell = openPriority[0].Key; // Get from open list
            openPriority.RemoveAt(0);
            if (currentCell.Equals(goalCell))
                break;

            int[] DX = new int[] { 1, -1, 0, 0 };
            int[] DZ = new int[] { 0, 0, 1, -1 };
            System.Random r = new System.Random();
            foreach (int i in Enumerable.Range(0, 4).OrderBy(x => r.Next()))
            {
                Vector2Int step = new Vector2Int(currentCell.x + DX[i], currentCell.y + DZ[i]);
                int c;
                cost.TryGetValue(currentCell, out c);
                int stepCost = c++;
                int c2;
                cost.TryGetValue(step, out c2);
                if (step.x > 0 && step.x < gridSize - 1 && step.y > 0 && step.y < gridSize - 1 && !obstacles.Contains(step) && (!cost.ContainsKey(step) || stepCost < c2))
                {
                    cost.Remove(step);
                    cost.Add(step, stepCost);
                    openPriority.Add(new KeyValuePair<Vector2Int, int>(step, stepCost + ManhattanDistance(goalCell, step)));
                    closed.Add(step, currentCell);
                }
            }
        }

        Vector2Int current = goalCell;
        while (!current.Equals(start))
        {
            path.Add(current);
            closed.TryGetValue(current, out current);
        }
        path.Reverse();
    }

    void Start()
    {
        transform.localScale = new Vector3(1, 0.5f, 1);
        i = 0;
        speed = .05f;
        height = 0.5f;
        path = new List<Vector2Int>();
        target = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        NewGoal();
        switch (algorithm)
        {
            case Algorithm.RandomWalk:
                GetComponent<Renderer>().material.color = Color.black;
                target.GetComponent<Renderer>().material.color = Color.black;
                next = RandomWalk();
                break;
            case Algorithm.BreadthFirst:
                GetComponent<Renderer>().material.color = Color.blue;
                target.GetComponent<Renderer>().material.color = Color.blue;
                BreadthFirstSearch();
                break;
            case Algorithm.BestFirst:
                GetComponent<Renderer>().material.color = Color.red;
                target.GetComponent<Renderer>().material.color = Color.red;
                BestFirstSearch();
                break;
            case Algorithm.Djikstra:
                GetComponent<Renderer>().material.color = Color.green;
                target.GetComponent<Renderer>().material.color = Color.green;
                DjikstraSearch();
                break;
            case Algorithm.AStar:
                GetComponent<Renderer>().material.color = Color.yellow;
                target.GetComponent<Renderer>().material.color = Color.yellow;
                AStarSearch();
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (algorithm != Algorithm.RandomWalk)
        {
            Vector3 nextPos = new Vector3(path[i].x, height, path[i].y);

            if (transform.position.Equals(nextPos))
            {
                i++;
            }

            transform.Translate(Vector3.ClampMagnitude((nextPos - transform.position), speed));

            Vector3 goal = new Vector3(goalCell.x, height, goalCell.y);
            if (transform.position.Equals(goal))
            {
                NewGoal();
                if (algorithm == Algorithm.BreadthFirst)
                {
                    BreadthFirstSearch();
                }
                else if (algorithm == Algorithm.BestFirst)
                {
                    BestFirstSearch();
                }
                else if (algorithm == Algorithm.Djikstra)
                {
                    DjikstraSearch();
                }
                else
                {
                    AStarSearch();
                }
                i = 0;
            }
        }
        else
        {
            Vector2Int current = new Vector2Int((int)transform.position.x, (int)transform.position.z);
            if (current == goalCell)
            {
                NewGoal();
                RandomWalk();
            }
            else if (current == next)
            {
                next = RandomWalk();
            }
            else
            {
                Vector3 nextPos = new Vector3(next.x, height, next.y);
                transform.Translate(Vector3.ClampMagnitude(nextPos - transform.position, speed));
            }
        }
    }
}
