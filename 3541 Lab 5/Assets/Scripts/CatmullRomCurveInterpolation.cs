using UnityEngine;
using System.Collections.Generic;

public class CatmullRomCurveInterpolation : MonoBehaviour
{

    const int NumberOfPoints = 8;
    Vector3[] controlPoints;

    const int MinX = -5;
    const int MinY = -5;
    const int MinZ = 0;

    const int MaxX = 5;
    const int MaxY = 5;
    const int MaxZ = 5;

    double time = 0;
    double s = 0;
    const double DT = 0.0025;

    double[,] segULength = new double[101 * NumberOfPoints, 3];

    /* Returns a point on a cubic Catmull-Rom/Blended Parabolas curve
	 * u is a scalar value from 0 to 1
	 * segment_number indicates which 4 points to use for interpolation
	 */
    Vector3 ComputePointOnCatmullRomCurve(double u, int segmentNumber)
    {
        Vector3 point = new Vector3();

        float t = 0.5f;
        Vector3 pMinusTwo = controlPoints[CheckPointNumber(segmentNumber - 2)];
        Vector3 pMinusOne = controlPoints[CheckPointNumber(segmentNumber - 1)];
        Vector3 p = controlPoints[segmentNumber];
        Vector3 pPlusOne = controlPoints[CheckPointNumber(segmentNumber + 1)];

        Vector3 c3 = (-t) * pMinusTwo + (2 - t) * pMinusOne + (t - 2) * p + t * pPlusOne;
        Vector3 c2 = 2 * t * pMinusTwo + (t - 3) * pMinusOne + (3 - 2 * t) * p + (-t) * pPlusOne;
        Vector3 c1 = (-t) * pMinusTwo + t * p;
        Vector3 c0 = pMinusOne;

        point = c3 * (float)System.Math.Pow(u, 3) + c2 * (float)System.Math.Pow(u, 2) + c1 * (float)u + c0;

        return point;
    }

    void GenerateControlPointGeometry()
    {
        for (int i = 0; i < NumberOfPoints; i++)
        {
            GameObject tempcube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tempcube.transform.localScale -= new Vector3(0.8f, 0.8f, 0.8f);
            tempcube.transform.position = controlPoints[i];
        }
    }

    private void Supersampling()
    {
        double totalLength = 0;
        Vector3 lastPoint = controlPoints[0];
        Vector3 nextPoint = Vector3.zero;
        int index = 0;

        for (int i = 0; i < NumberOfPoints; i++)
        {
            for (double j = 0; j <= 1.0; j += 0.01)
            {
                nextPoint = ComputePointOnCatmullRomCurve(j, i);
                totalLength += (double)Vector3.Distance(lastPoint, nextPoint);
                segULength[index, 0] = i;
                segULength[index, 1] = j;
                segULength[index, 2] = totalLength;
                lastPoint = nextPoint;
                index++;
            }
        }

        for (int i = 0; i < segULength.GetLength(0); i++)
        {
            segULength[i, 2] = segULength[i, 2] / totalLength;
        }
    }

    private int CheckPointNumber(int num)
    {
        if (num < 0)
        {
            num = num + NumberOfPoints;
        }
        if (num > NumberOfPoints - 1)
        {
            num = 0;
        }

        return num;
    }

    private int BinarySearchSamples(double[,] array, double key)
    {
        int min = 0;
        int max = array.GetLength(0) - 1;
        int index = 0;

        while (min <= max)
        {
            int midPoint = (min + max) / 2;

            if (key == array[midPoint, 2])
            {
                return midPoint;
            }
            else if (key < array[midPoint, 2])
            {
                index = max;
                max = midPoint - 1;
            }
            else
            {
                index = min;
                min = midPoint + 1;
            }
        }

        if (min <= array.GetLength(0) - 1 && max >= 0)
        {
            if (array[min, 2] - key < (key - array[max, 2]))
            {
                index = min;
            }
            else
            {
                index = max;
            }
        }

        return index;
    }

    private double Ease(double time, double t1, double t2)
    {
        //// Ease function from Computer Animation: Algorithms & Techniques 3rd edition - by Dr. Rick Parent Figure 3.18
        double distance, v;
        v = 2 / (t2 - t1 + 1);
        if (time < t1)
            distance = v * System.Math.Pow(time, 2) / 2 / t1;
        else if (time < t2)
            distance = v * t1 / 2 + v * (time - t1);
        else
            distance = v * t1 / 2 + v * (t2 - t1) + (v - (v * (time - t2) / (1 - t2)) / 2) * (time - t2);

        return distance;
    }

    // Use this for initialization
    void Start()
    {
        controlPoints = new Vector3[NumberOfPoints];

        controlPoints[0] = new Vector3(0, 0, 0);
        for (int i = 1; i < NumberOfPoints; i++)
        {
            controlPoints[i] = new Vector3(Random.Range(MinX, MaxX), Random.Range(MinY, MaxY), Random.Range(MinZ, MaxZ));
        }

        GenerateControlPointGeometry();

        Supersampling();
    }

    // Update is called once per frame
    void Update()
    {
        time += DT;
        if (time > 1.0)
            time = 0;

        double u = 0;
        int segmentNumber = 0;

        s = Ease(time, 0.35, 0.65);
       
        int index = BinarySearchSamples(segULength, s);
        u = segULength[index, 1];
        segmentNumber = (int)segULength[index, 0];

        Vector3 temp = ComputePointOnCatmullRomCurve(u, segmentNumber);
        transform.position = temp;
    }
}
