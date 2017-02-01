using UnityEngine;
using System.Collections.Generic;

public class RopeSwing : MonoBehaviour
{
    const float TEXTURE_SCALE_X_MULT = 5;
    const float TEXTURE_SCALE_Y = 0.1f;
  
    private LineRenderer ropeRender = null;
    private float nodeDiameter = 0;
    private List<GameObject> ropePoints = new List<GameObject>();

    public GameObject ropeSprite;
    [Range(0.1f, 1)]
    public float ropeWidth = 0.15f;
    [Tooltip("Not actual length, is the amount of nodes distributed so is determined by prefabs diameter")]
    public int ropeLength = 10;
    public float ropeSwingSpeed = 1;
    public float swingAngle = 90;


    void Start ()
    {
        ropeRender = gameObject.GetComponent<LineRenderer>();
        ropeRender.SetWidth(ropeWidth, ropeWidth);
        nodeDiameter = ropeSprite.GetComponent<CircleCollider2D>().radius;
        generateRope();  
    }


    private void FixedUpdate()
    {
        if (ropeLength > 1)
        {
            swingRope();
            updateRopeRenderer();
            //updateMaterial();
        }
    }


    /// <summary>
    /// Creates a rope only if the rope length (node count) is greater than zero 
    /// </summary>
    void generateRope()
    {
        if (ropeLength > 1)
        {
            calculateNodePositions();
        }
    }


    /// <summary>
    /// Fixes unity line renderer material bug. Prevents texture stretching.
    /// </summary>
    private void updateMaterial()
    {
        //fix material stretch
        float dist = Vector3.Distance(transform.position, ropePoints[ropePoints.Count - 1].transform.position);
        ropeRender.material.mainTextureScale = new Vector2(dist * TEXTURE_SCALE_X_MULT, TEXTURE_SCALE_Y);
    }


    /// <summary>
    /// Updates the ropes visuals by updating the line renderers point positions.
    /// </summary>
    private void updateRopeRenderer()
    {
        for (int i = 1; i < ropePoints.Count; i++)
        {
            ropeRender.SetPosition(i, ropePoints[i].transform.position);//update line points
        }
    }


    /// <summary>
    /// Rotates rope on z axis between swingAngle and -swingAngle using Sine wave.
    /// </summary>
    private void swingRope()
    {
         transform.rotation = Quaternion.Euler(0.0f, 0.0f, swingAngle * Mathf.Sin(Time.time * ropeSwingSpeed));
    }


    /// <summary>
    /// Evenly distributes node between two points.
    /// </summary>
    void calculateNodePositions()
    {
        float percentage = 1.0f / ropeLength;//percentage for distribution 

        Vector2 endPoint = new Vector2(transform.localPosition.x, -(ropeLength * nodeDiameter));//calculate furthest node position
        Rigidbody2D lastPoint = GetComponent<Rigidbody2D>();  

        for (float i = 0; i <= 1; i += percentage)
        {
            GameObject rope = Instantiate(ropeSprite) as GameObject;//create rope node
            rope.transform.position = Vector2.Lerp(transform.position, endPoint, i);
            rope.transform.parent = this.transform;//add it as a child              
            rope.GetComponent<RopeClimbPoint>().setRopeRef(this);//let it know me

            Rigidbody2D pointRB = rope.GetComponent<Rigidbody2D>();
            if (lastPoint != pointRB)
            {
                DistanceJoint2D joint = rope.AddComponent<DistanceJoint2D>();//create hinge joints between rope nodes
                joint.autoConfigureDistance = false;
                joint.maxDistanceOnly = true;
                joint.distance = nodeDiameter * 2;
                joint.connectedBody = lastPoint;
                joint.enableCollision = false;
            }

            lastPoint = pointRB;//next link target
            ropePoints.Add(rope);//add point to the list
        }
        addNodesToRender(); 
    }


    /// <summary>
    /// Adds the calculated node positions into the line renderer.
    /// Also gives the nodes it's connections.
    /// </summary>
    void addNodesToRender()//should be refactored
    {
        ropeRender.SetVertexCount(ropePoints.Count);
        ropeRender.useWorldSpace = true;

        for(int i = 0; i < ropePoints.Count; i++)
        {
            ropeRender.SetPosition(i, ropePoints[i].transform.position);
        }
        
    }


    /// <summary>
    /// Returns a list of the nodes in this rope.
    /// </summary>
    public List<GameObject> getClimbPoints()
    {
        return ropePoints;
    }
}
