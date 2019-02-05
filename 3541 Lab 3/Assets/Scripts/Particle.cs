using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : ScriptableObject
{
    private Vector3 position;
    private Vector3 force;
    private float age;
    private float maxAge;
    private float mass;
    private float maxYSpeed;
    private float vMin, vMax;
    public Vector3 velocity { get; set; }
    public GameObject particle { get; set; }
    public enum ColorScheme
    {
        Red, Blue, Green
    }
    public ColorScheme colors { get; set; }

    public Particle()
    {
        maxAge = 5f;
        age = Random.Range(0f, maxAge * 0.95f);
        mass = 0.2f;
        vMin = 1f;
        vMax = 3f;

        float yPos = 5 + Random.Range(-0.5f, 0.5f);
        float zPos = Random.Range(-0.5f, 0.5f);
        while (System.Math.Pow(System.Math.Sqrt(yPos - 5), 2) + System.Math.Pow(System.Math.Sqrt(zPos), 2) > 0.5)
        {
            yPos = 5 + Random.Range(-0.5f, 0.5f);
            zPos = Random.Range(-0.5f, 0.5f);
        }
        float pDimension = Random.Range(0f, 0.3f);
        float initialXVel = Random.Range(vMin, vMax);
        float initialYVel = Random.Range(-1f, 0f);

        position = new Vector3(0, yPos, zPos);
        velocity = new Vector3(initialXVel, initialYVel, 0);

        particle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        particle.transform.position = position;
        particle.transform.localScale = new Vector3(pDimension, pDimension, pDimension);
    }

    public void Update()
    {
        Vector3 acceleration = force / mass;
        Vector3 newVelocity = velocity + acceleration * Time.deltaTime;
        if (newVelocity.y < -15)
            newVelocity.y = 15;
        position = position + ((newVelocity + velocity) / 2) * Time.deltaTime;
        velocity = newVelocity;

        age += Time.deltaTime;
        if (age >= maxAge)
        {
            Reset();
        }

        if (age < maxAge * (1f / 3))
        {
            switch (colors)
            {
                case ColorScheme.Red:
                    particle.GetComponent<Renderer>().material.color = Color.red;
                    break;
                case ColorScheme.Blue:
                    particle.GetComponent<Renderer>().material.color = Color.blue;
                    break;
                case ColorScheme.Green:
                    particle.GetComponent<Renderer>().material.color = Color.green;
                    break;
                default:
                    break;
            }
        }
        else if (age < maxAge * (2f / 3))
        {
            switch (colors)
            {
                case ColorScheme.Red:
                    particle.GetComponent<Renderer>().material.color = Color.magenta;
                    break;
                case ColorScheme.Blue:
                    particle.GetComponent<Renderer>().material.color = Color.cyan;
                    break;
                case ColorScheme.Green:
                    particle.GetComponent<Renderer>().material.color = Color.yellow;
                    break;
                default:
                    break;
            }
        }
        else if (age < maxAge)
        {
            switch (colors)
            {
                case ColorScheme.Red:
                    particle.GetComponent<Renderer>().material.color = Color.yellow;
                    break;
                case ColorScheme.Blue:
                    particle.GetComponent<Renderer>().material.color = Color.gray;
                    break;
                case ColorScheme.Green:
                    particle.GetComponent<Renderer>().material.color = Color.white;
                    break;
                default:
                    break;
            }
        }

        particle.transform.position = position;
    }

    public void ApplyForce(Vector3 f)
    {
        force = f;
    }

    public void ResetForce()
    {
        force = Vector3.zero;
    }

    public void IncreaseVRange()
    {
        vMin++;
        vMax++;
    }
    public void DecreaseVRange()
    {
        vMin--;
        vMax--;
        if (vMin <= 0)
            vMin = 0;
        if (vMax <= 1)
            vMax = 1;
    }

    public void Reset()
    {
        age = 0;

        float yPos = 5 + Random.Range(-0.5f, 0.5f);
        float zPos = Random.Range(-0.5f, 0.5f);
        while (System.Math.Pow(System.Math.Sqrt(yPos - 5), 2) + System.Math.Pow(System.Math.Sqrt(zPos), 2) > 0.5)
        {
            yPos = 5 + Random.Range(-0.5f, 0.5f);
            zPos = Random.Range(-0.5f, 0.5f);
        }
        float pDimension = Random.Range(0f, 0.3f);
        float initialXVel = Random.Range(vMin, vMax);
        float initialYVel = Random.Range(-1f, 0f);

        position = new Vector3(0, yPos, zPos);
        velocity = new Vector3(initialXVel, initialYVel, 0);

        particle.transform.position = position;
        particle.transform.localScale = new Vector3(pDimension, pDimension, pDimension);
        particle.GetComponent<Renderer>().material.color = Color.blue;
    }
}
