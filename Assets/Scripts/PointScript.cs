using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointScript : MonoBehaviour
{
    public Point point;
    public bool canMove = true;

    // Start is called before the first frame update
    void Start()
    {
        point = new Point(gameObject.transform.position.x, gameObject.transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            if (Random.Range(0f, 1f) > 0.8f) move();
        }
        
        point.x = gameObject.transform.position.x;
        point.y = gameObject.transform.position.y;
    }

    public void move()
    {
        float x = gameObject.transform.position.x;
        float y = gameObject.transform.position.y;
        float z = gameObject.transform.position.z;

        gameObject.transform.position += new Vector3(Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f), 0);
        if (x < -8f) gameObject.transform.position = gameObject.transform.position = new Vector3(-8f, y, z);
        if (x > 8f) gameObject.transform.position = gameObject.transform.position = new Vector3(8f, y, z);
        if (y < -4f) gameObject.transform.position = gameObject.transform.position = new Vector3(x, -4f, z);
        if (y > 4f) gameObject.transform.position = gameObject.transform.position = new Vector3(x, 4f, z);
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
