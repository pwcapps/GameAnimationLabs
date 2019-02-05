using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEmitter : MonoBehaviour
{
    ArrayList particles = new ArrayList();
    public int pCount;
    public float gravityForce;
    public GameObject ground;
    public GameObject slope;

    public void Start()
    {
        for (int i = 0; i < pCount; i++)
        {
            Particle p = new Particle();
            p.particle.name = "Particle " + i;
            particles.Add(p);
        }

    }

    public void Update()
    {
        Vector3 slopeNormal = slope.transform.up;
        double slopeD = -(slopeNormal.x * slope.transform.position.x + slopeNormal.y * slope.transform.position.y + slopeNormal.z * slope.transform.position.z);

        foreach (Particle p in particles)
        {
            Vector3 newForce = new Vector3(0, gravityForce, 0);

            if (p.particle.transform.position.y <= ground.transform.position.y)
            {
                p.velocity = new Vector3(p.velocity.x, p.velocity.y * -0.9f, p.velocity.z);
            }

            double planeE = slopeNormal.x * p.particle.transform.position.x + slopeNormal.y * p.particle.transform.position.y + slopeNormal.z * p.particle.transform.position.z + slopeD;
            if (planeE <= 0)
            {
                p.velocity = Vector3.Reflect(p.velocity, slopeNormal) * 0.9f;
            }

            p.ApplyForce(newForce);
        }

        foreach (Particle p in particles)
        {
            p.Update();
            p.ResetForce();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            foreach (Particle p in particles)
            {
                p.colors = Particle.ColorScheme.Red;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            foreach (Particle p in particles)
            {
                p.colors = Particle.ColorScheme.Blue;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            foreach (Particle p in particles)
            {
                p.colors = Particle.ColorScheme.Green;
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            foreach (Particle p in particles)
            {
                p.DecreaseVRange();
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            foreach (Particle p in particles)
            {
                p.IncreaseVRange();
            }
        }

    }

}
