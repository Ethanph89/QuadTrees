using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTreeScript : MonoBehaviour
{
    public GameObject dot;

    private QuadTree qtree;
    private List<Point> allPoints = new List<Point>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            Debug.Log(mousePos);
            allPoints.Add(new Point(mousePos.x, mousePos.y));
            Instantiate(dot, mousePos, Quaternion.identity);
        }

        qtree = new QuadTree(new Rectangle(new Point(0f, 0f), 8, 4));

        for (int i = 0; i < allPoints.Count; i++)
        {
            qtree.insert(allPoints[i]);
        }

        qtree.drawTree();
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

public class Rectangle
{
    public Point center;
    public float wd;
    public float ht;

    public Rectangle(Point point, float w, float h)
    {
        center = point;
        wd = w;
        ht = h;
    }

    public bool containsPoint(Point point)
    {
        return !(point.x > center.x + wd ||
            point.x < center.x - wd ||
            point.y > center.y + ht ||
            point.y < center.y - ht);
    }

    //TODO
    public bool intersectsRectangle(Rectangle rect)
    {
        return true;
    }
}

public class QuadTree
{
    public Rectangle boundary;
    public const int cap = 4;
    public List<Point> points = new List<Point>();

    public QuadTree? nw = null;
    public QuadTree? ne = null;
    public QuadTree? sw = null;
    public QuadTree? se = null;

    public QuadTree(Rectangle rect)
    {
        boundary = rect;
    }

    public bool insert(Point point)
    {
        //Debug.Log("Inserting point: (" + point.x + ", " + point.y + ")");

        if (!boundary.containsPoint(point)) {
            //Debug.Log("Does not contain point");
            return false; 
        }

        if (points.Count < cap && nw == null)
        {
            //Debug.Log("Adding to points index: " + points.Count);
            points.Add(point);
            return true;
        }

        //Debug.Log("Cap hit");

        if (nw == null)
        {
            //Debug.Log("Dividing");
            subdivide();
        }

        if (nw.insert(point)) return true;
        if (ne.insert(point)) return true;
        if (sw.insert(point)) return true;
        if (se.insert(point)) return true;

        Debug.Log("ERROR");

        return false;
    }

    public void subdivide()
    {
        nw = new QuadTree(new Rectangle(new Point(boundary.center.x - (boundary.wd / 2), boundary.center.y + (boundary.ht / 2)), boundary.wd / 2, boundary.ht / 2));
        ne = new QuadTree(new Rectangle(new Point(boundary.center.x + (boundary.wd / 2), boundary.center.y + (boundary.ht / 2)), boundary.wd / 2, boundary.ht / 2));
        sw = new QuadTree(new Rectangle(new Point(boundary.center.x - (boundary.wd / 2), boundary.center.y - (boundary.ht / 2)), boundary.wd / 2, boundary.ht / 2));
        se = new QuadTree(new Rectangle(new Point(boundary.center.x + (boundary.wd / 2), boundary.center.y - (boundary.ht / 2)), boundary.wd / 2, boundary.ht / 2));

        for (int i = 0; i < points.Count; i++)
        {
            if (nw.boundary.containsPoint(points[i])) nw.insert(points[i]);
            else if (ne.boundary.containsPoint(points[i])) ne.insert(points[i]);
            else if (sw.boundary.containsPoint(points[i])) sw.insert(points[i]);
            else if (se.boundary.containsPoint(points[i])) se.insert(points[i]);
        }
    }

    public void drawTree()
    {
        Debug.DrawLine(new Vector3(boundary.center.x - boundary.wd, boundary.center.y + boundary.ht, 0), new Vector3(boundary.center.x + boundary.wd, boundary.center.y + boundary.ht, 0), Color.white); // Top
        Debug.DrawLine(new Vector3(boundary.center.x - boundary.wd, boundary.center.y - boundary.ht, 0), new Vector3(boundary.center.x + boundary.wd, boundary.center.y - boundary.ht, 0), Color.white); // Bottom
        Debug.DrawLine(new Vector3(boundary.center.x - boundary.wd, boundary.center.y + boundary.ht, 0), new Vector3(boundary.center.x - boundary.wd, boundary.center.y - boundary.ht, 0), Color.white); // Left
        Debug.DrawLine(new Vector3(boundary.center.x + boundary.wd, boundary.center.y + boundary.ht, 0), new Vector3(boundary.center.x + boundary.wd, boundary.center.y - boundary.ht, 0), Color.white); // Right

        if (nw != null) nw.drawTree();
        if (ne != null) ne.drawTree();
        if (sw != null) sw.drawTree();
        if (se != null) se.drawTree();
    }

    //TODO
    public Point[] queryRange()
    {
        Point[] points = {};
        return points;
    }
}