using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{

    private ArrayList prey, predators, obstacles;
    public float numPrey, numPred, preyHeight, predHeight;

    // Use this for initialization
    void Start()
    {
        prey = new ArrayList();
        predators = new ArrayList();
        obstacles = new ArrayList();
        CreatePrey();
        CreatePredators();
        AddObstacles();
    }

    private void AddObstacles()
    {
        for (int i = 1; i < 7; i++)
        {
            obstacles.Add(GameObject.Find("Ob" + i));
        }
    }

    private void CreatePrey()
    {
        for (int i = 0; i < numPrey; i++)
        {
            GameObject p = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            p.transform.parent = transform;
            p.AddComponent<PreyBehavior>();
            p.GetComponent<PreyBehavior>().predators = predators;
            p.GetComponent<PreyBehavior>().prey = prey;
            p.GetComponent<PreyBehavior>().obstacles = obstacles;
            p.name = "Prey" + (i + 1);
            prey.Add(p);
        }
    }

    private void CreatePredators()
    {
        for (int i = 0; i < numPred; i++)
        {
            GameObject p = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            p.transform.parent = transform;
            p.AddComponent<PredatorBehavior>();
            p.GetComponent<PredatorBehavior>().preyList = prey;
            p.GetComponent<PredatorBehavior>().predators = predators;
            p.GetComponent<PredatorBehavior>().obstacles = obstacles;
            p.name = "Predator" + (i + 1);
            predators.Add(p);
        }
    }
}
