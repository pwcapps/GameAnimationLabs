using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreyBehavior : MonoBehaviour
{
    public ArrayList predators, prey, obstacles;
    private GameObject predator;
    private Vector3 goal, velocity, acceleration, force;
    private float mass, maxSpeed, timer, cosAngle, visionDist, yHeight, accFactor, wallPos;
    private enum State { Wander, Flee };
    private State state;

    // Use this for initialization
    void Start()
    {
        ChooseDirection();
        RandomStart();
        gameObject.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        state = State.Wander;
        mass = 2f;
        maxSpeed = 1f;
        timer = 0f;
        yHeight = 0.13f;
        accFactor = 2.5f;
        visionDist = 2.5f;
        cosAngle = -0.1736f;
        wallPos = 8.5f;
    }

    // Update is called once per frame
    private void Update()
    {
        force = Vector3.zero;
        PredatorCollisionDetection();
        CheckForPredator();
        switch (state)
        {
            case State.Wander:
                timer += Time.deltaTime;
                if (timer >= 3f)
                {
                    ChooseDirection();
                    timer = 0f;
                }
                force = force + (goal - gameObject.transform.position).normalized * accFactor;
                gameObject.GetComponent<Renderer>().material.color = Color.blue;
                break;
            case State.Flee:
                force = (gameObject.transform.position - predator.transform.position).normalized * accFactor * 1.5f;
                gameObject.GetComponent<Renderer>().material.color = Color.black;
                break;
            default:
                break;
        }

        CheckAndMoveFromWall();
        AvoidObstacles();
        acceleration = force / mass;
        Vector3 newV = velocity + acceleration * Time.deltaTime;
        newV = Vector3.ClampMagnitude(newV, maxSpeed);
        gameObject.transform.position = gameObject.transform.position + (newV + velocity) / 2 * Time.deltaTime;
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, yHeight, gameObject.transform.position.z);
        velocity = newV;
        gameObject.transform.forward = velocity.normalized;
    }

    private void ChooseDirection()
    {
        float xPos = Random.Range(-9f, 9f);
        float zPos = 10;
        goal = new Vector3(xPos, yHeight, zPos);
    }

    private void RandomStart()
    {
        float xPos = Random.Range(-9f, 9f);
        float zPos = Random.Range(-9f, -7.5f);
        gameObject.transform.position = new Vector3(xPos, yHeight, zPos);
    }

    private void CheckAndMoveFromWall()
    {
        if (gameObject.transform.position.x >= wallPos)
        {
            gameObject.transform.position = new Vector3(wallPos, yHeight, gameObject.transform.position.z);
            goal = new Vector3(0, yHeight, 0);
        }
        if (gameObject.transform.position.x <= -wallPos)
        {
            gameObject.transform.position = new Vector3(-wallPos, yHeight, gameObject.transform.position.z);
            goal = new Vector3(0, yHeight, 0);
        }
        if (gameObject.transform.position.z >= wallPos)
        {
            RandomStart();
        }
        if (gameObject.transform.position.z <= -wallPos)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, yHeight, -wallPos);
            goal = new Vector3(0, yHeight, 0);
        }
    }

    private void CheckForPredator()
    {
        foreach (GameObject p in predators)
        {
            Vector3 predatorPosition = p.transform.position;
            if (Vector3.Distance(predatorPosition, gameObject.transform.position) < visionDist)
            {
                Vector3 agentToEnemy = predatorPosition - gameObject.transform.position;
                if (Vector3.Dot(agentToEnemy.normalized, gameObject.transform.forward.normalized) > cosAngle)
                {
                    state = State.Flee;
                    predator = p;
                    return;
                }
            }
            else
                state = State.Wander;
        }
    }

    private void PredatorCollisionDetection()
    {
        foreach (GameObject pred in predators)
        {
            if (Vector3.Distance(pred.transform.position, gameObject.transform.position) <= 0.45)
            {
                RandomStart();
            }
        }
    }

    private void AvoidObstacles()
    {
        foreach (GameObject o in obstacles)
        {
            if (Vector3.Distance(o.transform.position, gameObject.transform.position) <= 1.5f)
            {
                force = (gameObject.transform.position - o.transform.position) * accFactor;
            }
        }
    }
}
