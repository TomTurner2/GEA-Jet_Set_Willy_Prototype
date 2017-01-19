using UnityEngine;
using System.Collections.Generic;

public class RopeSwing : MonoBehaviour
{
    const float TEXTURE_SCALE_X_MULT = 5;
    const float TEXTURE_SCALE_Y = 0.1f;
    public GameObject ropeSprite;
    [Range(0.1f, 1)]
    public float ropeWidth = 0.15f;
    private LineRenderer ropeRender = null;
    private float nodeDiameter = 0;
    [HideInInspector]
    private List<GameObject> ropePoints = new List<GameObject>();//convert to game object and place nodes whilst calc
    [Tooltip("Not actual length, is the amount of nodes distributed so is determined by prefabs diameter")]
    public int ropeLength = 10;
    //private int oldNodeCount = 0;


    void Start ()
    {
        ropeRender = gameObject.GetComponent<LineRenderer>();
        ropeRender.SetWidth(ropeWidth, ropeWidth);
        nodeDiameter = ropeSprite.GetComponent<CircleCollider2D>().radius;
        generateRope();  
    }


    /// <summary>
    /// Creates a rope only if the node count allows it. 
    /// </summary>
    void generateRope()
    {
        if (ropeLength > 1)
        {
            calculateNodePositions();
           // oldNodeCount = nodeCount;
        }
    }

    private void Update()
    {
        if (ropeLength > 1)
        {
            ropeRender.SetPosition(0, transform.position);

            for (int i = 1; i < ropePoints.Count; i ++)
            {
                ropeRender.SetPosition(i, ropePoints[i].transform.position);
            }
        }

        //fix material stretch...kinda
        float dist = Vector3.Distance(transform.position, ropePoints[ropePoints.Count - 1].transform.position);
        ropeRender.material.mainTextureScale = new Vector2(dist * TEXTURE_SCALE_X_MULT, TEXTURE_SCALE_Y);
    }

    /// <summary>
    /// Evenly distributes node between two points.
    /// </summary>
    void calculateNodePositions()
    {
        float percentage = 1.0f / ropeLength;//percentage for distribution 

        Vector2 endPoint = new Vector2(transform.position.x, -(ropeLength * nodeDiameter));
        Rigidbody2D lastPoint = GetComponent<Rigidbody2D>();  
      
       

        for (float i = 0; i <= 1; i += percentage)
        {
            GameObject rope = Instantiate(ropeSprite) as GameObject;//create rope node
            rope.transform.position = Vector2.Lerp(transform.localPosition, endPoint, i);
            rope.transform.parent = this.transform;//add it as a child              

            Rigidbody2D pointRB = rope.GetComponent<Rigidbody2D>();
            if (lastPoint != pointRB)
            {
                HingeJoint2D joint = rope.AddComponent<HingeJoint2D>();//create hinge joints between rope nodes
                joint.connectedBody = lastPoint;
                joint.enableCollision = false;
            }

            lastPoint = pointRB;//next link target
            ropePoints.Add(rope);//add point to the list
        }
        addNodesToRender(); 
    }

    void addNodesToRender()//could remove loop for optimisation
    {
        ropeRender.SetVertexCount(ropePoints.Count);
        ropeRender.useWorldSpace = true;
        int pos = 0;

        for(int i = 0; i < ropePoints.Count; i++)
        {
            ropeRender.SetPosition(pos, ropePoints[i].transform.position);
            pos++;
        }
        
    }
}
