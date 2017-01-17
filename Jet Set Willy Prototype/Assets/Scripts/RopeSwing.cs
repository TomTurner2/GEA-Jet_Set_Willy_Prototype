using UnityEngine;
using System.Collections.Generic;

public class RopeSwing : MonoBehaviour
{
    public Transform ropeEnd;
    public GameObject ropeSprite;
    [HideInInspector]
    public List<Vector2> ropePoints;//exposed for future climbing feature
    public int nodeCount = 50;
    private int oldNodeCount = 0;


    private void Update()
    {
        if(nodeCount != oldNodeCount)
        {
            generateRope();
        }
    }


    void Start ()
    {
        generateRope();  
    }


    /// <summary>
    /// Creates a rope only if the node count allows it. Keeps track of node count for
    /// regenereation checking.
    /// </summary>
    void generateRope()
    {
        if (nodeCount > 0)
        {
            calculateNodePositions();
            placeRopeNodes();
            oldNodeCount = nodeCount;
        }
    }


    /// <summary>
    /// Evenly distributes node between two points.
    /// </summary>
    void calculateNodePositions()
    {
        float percentage = 1.0f / nodeCount;//percentage for distribution 
        for (float i = 0; i <= 1; i += percentage)
        {
            Vector2 point = Vector2.Lerp(transform.position, ropeEnd.position, i);//increment the percentage between the two points
            ropePoints.Add(point);//add point to the list
        }        
    }


    /// <summary>
    /// Instantiates nodes at pre-calcualted points. Each sprite is
    /// connected to the last using hinge joints.
    /// </summary>
    void placeRopeNodes()
    {
        Rigidbody2D lastPoint = GetComponent<Rigidbody2D>();
        foreach (var ropePoint in ropePoints)
        {
            GameObject rope = Instantiate(ropeSprite) as GameObject;//create rope node
            rope.transform.position = ropePoint;

            Rigidbody2D pointRB = rope.GetComponent<Rigidbody2D>();
            if (lastPoint != pointRB)
            {
                HingeJoint2D joint = rope.AddComponent<HingeJoint2D>();//create hinge joints between rope nodes
                joint.connectedBody = lastPoint;
                joint.enableCollision = false;                
            }
            lastPoint = pointRB;//next link target
            rope.transform.parent = this.transform;//add it as a child
        }
    }
}
