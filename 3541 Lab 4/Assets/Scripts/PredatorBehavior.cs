using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorBehavior : MonoBehaviour
{
    public ArrayList preyList, predators, obstacles;
    private GameObject prey;
    private Vector3 goal, velocity, acceleration, force;
    private float mass, maxSpeed, timer, cosAngle, visionDist, yHeight, accFactor, wallPos;
    private enum State { Wander, Chase };
    private State state;

    // Use this for initialization
    void Start()
    {
        ChooseDirection();
        RandomStart();
        gameObject.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        state = State.Wander;
        mass = 2f;
        maxSpeed = 1f;
        timer = 0f;
        yHeight = 0.3f;
        accFactor = 2.5f;
        visionDist = 5f;
        cosAngle = 0.766f;
        wallPos = 8.5f;
    }

    // Update is called once per frame
    void Update()
    {
        force = Vector3.zero;
        CheckForPrey();
        switch (state)
        {
            case State.Wander:
                timer += Time.deltaTime;
                if (timer >= 3f)
                {
                    ChooseDirection();
                    timer = 0f;
                }
                gameObject.GetComponent<Renderer>().material.color = Color.yellow;
                break;
            case State.Chase:
                goal = new Vector3(prey.transform.position.x, yHeight, prey.transform.position.z);
                gameObject.GetComponent<Renderer>().material.color = Color.red;
                break;
            default:
                break;
        }

        force = force + (goal - gameObject.transform.position).normalized * accFactor;
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
        float zPos = Random.Range(0f, 8f);
        goal = new Vector3(xPos, yHeight, zPos);
    }

    private void RandomStart()
    {
        float xPos = Random.Range(-9f, 9f);
        float zPos = Random.Range(-2f, 8f);
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
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, yHeight, wallPos);
            goal = new Vector3(0, yHeight, 0);
        }
        if (gameObject.transform.position.z <= -wallPos)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, yHeight, -wallPos);
            goal = new Vector3(0, yHeight, 0);
        }
    }

    private void CheckForPrey()
    {
        foreach (GameObject p in preyList)
        {
            Vector3 preyPosition = p.transform.position;
            if (Vector3.Distance(preyPosition, gameObject.transform.position) < visionDist)
            {
                Vector3 agentToEnemy = preyPosition - gameObject.transform.position;
                if (Vector3.Dot(agentToEnemy.normalized, gameObject.transform.forward.normalized) > cosAngle)
                {
                    state = State.Chase;
                    gameObject.GetComponent<Renderer>().material.color = Color.yellow;
                    prey = p;
                    return;
                }
            }
            else
            {
                state = State.Wander;
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
