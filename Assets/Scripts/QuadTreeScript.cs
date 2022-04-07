using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTreeScript : MonoBehaviour
{
    public GameObject dot;

    private QuadTree qtree;
    private GameObject[] allObjects;
    private Vector3 mousePos;
    private Rectangle mouseRange;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        qtree = new QuadTree(new Rectangle(new Point(0f, 0f), 8, 4)); // Clear qtree
        allObjects = GameObject.FindGameObjectsWithTag("Dot"); // Set obj array

        // Create and draw mouse range
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        mouseRange = new Rectangle(new Point(mousePos.x, mousePos.y), 2, 1);
        mouseRange.drawRectangle();

        // Insert points into qtree and draw
        if (allObjects.Length > 0)
        {
            foreach (GameObject obj in allObjects)
            {
                qtree.insert(obj.GetComponent<PointScript>().point);    
            }
        }

        qtree.drawTree();

        // Add dot to screen when MLB is clicked and there is not already a dot there
        if (Input.GetMouseButton(0))
        {
            bool canAdd = true;
            if (allObjects.Length > 0)
            {
                foreach (GameObject obj in allObjects)
                {
                    if (obj.GetComponent<PointScript>().point.x == mousePos.x && obj.GetComponent<PointScript>().point.y == mousePos.y) canAdd = false;
                }
            }
             if (canAdd == true) Instantiate(dot, mousePos, Quaternion.identity);
        }
    }

    private void OnGUI()
    {
        // Write query to screen
        string countStr = qtree.queryRange(mouseRange).Count.ToString();
        GUI.Label(new Rect(100, 50, 20, 50), countStr);
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

    public bool intersectsRectangle(Rectangle other)
    {
        bool xIntersects = false;
        bool yIntersects = false;
        float xL = center.x - wd;
        float xR = center.x + wd;
        float yT = center.y + ht;
        float yB = center.y - ht;
        float othxL = other.center.x - other.wd;
        float othxR = other.center.x + other.wd;
        float othyT = other.center.y + other.ht;
        float othyB = other.center.y - other.ht;

        if ((othxL >= xL && othxL <= xR) ||
            (othxR >= xL && othxR <= xR) ||
            (xL >= othxL && xL <= othxR) ||
            (xR >= othxL && xR <= othxR))
        {
            xIntersects = true;
        }

        if ((othyB >= yB && othyB <= yT) ||
            (othyT >= yB && othyT <= yT) ||
            (yB >= othyB && yB <= othyT) ||
            (yT >= othyB && yT <= othyT))
        {
            yIntersects = true;
        }

        return (xIntersects && yIntersects);
    }

    public void drawRectangle()
    {
        Debug.DrawLine(new Vector3(center.x - wd, center.y + ht, 0), new Vector3(center.x + wd, center.y + ht, 0), Color.green); // Top
        Debug.DrawLine(new Vector3(center.x - wd, center.y - ht, 0), new Vector3(center.x + wd, center.y - ht, 0), Color.green); // Bottom
        Debug.DrawLine(new Vector3(center.x - wd, center.y + ht, 0), new Vector3(center.x - wd, center.y - ht, 0), Color.green); // Left
        Debug.DrawLine(new Vector3(center.x + wd, center.y + ht, 0), new Vector3(center.x + wd, center.y - ht, 0), Color.green); // Right
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

    public List<Point> queryRange(Rectangle range) 
    {
        List<Point> pointsFound = new List<Point>();

        if (!(boundary.intersectsRectangle(range))) {
            return pointsFound;
        }

        if (nw == null)
        {
            for (int i = 0; i < points.Count; i++)
            {
                if (range.containsPoint(points[i])) {
                    pointsFound.Add(points[i]);
                }
            }
        }

        if (nw == null)
        {
            return pointsFound;
        }

        pointsFound.AddRange(nw.queryRange(range));
        pointsFound.AddRange(ne.queryRange(range));
        pointsFound.AddRange(sw.queryRange(range));
        pointsFound.AddRange(se.queryRange(range));

        return pointsFound;
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
}