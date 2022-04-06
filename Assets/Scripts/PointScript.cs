using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointScript : MonoBehaviour
{
    public Point point;

    // Start is called before the first frame update
    void Start()
    {
        point = new Point(gameObject.GetComponent<Transform>().position.x, gameObject.GetComponent<Transform>().position.y);
    }

    // Update is called once per frame
    void Update()
    {
        point.x = gameObject.GetComponent<Transform>().position.x;
        point.y = gameObject.GetComponent<Transform>().position.y;
    }
}

public class Point
{
    public float x;
    public float y;

    public Point(float a, float b)
    {
        x = a;
        y = b;
    }
}
