using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour {

    public float minGroundNormalY = 0.65f;
    public float gravityModifier = 1f;

    protected bool grounded;
    protected Rigidbody2D rb2d;
    protected Vector2 velocity;
    protected Vector2 targetVelocity;
    protected Vector2 groundNormal;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

    protected const float minMoveDistance = 0.001f;
    protected const float shellRadius = 0.01f;

    private void OnEnable() {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Use this for initialization
    private void Start () {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
	}

    // Update is called once per frame
    private void Update () {
        targetVelocity = Vector2.zero;
        ComputeVelocity();
	}

    protected virtual void ComputeVelocity() {}

    private void FixedUpdate() {
        velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
        velocity.x = targetVelocity.x;
        grounded = false;
        Vector2 deltaPosition = velocity * Time.deltaTime;
        Vector2 moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);
        Vector2 move = moveAlongGround * deltaPosition.x;

        //separate calls for horizontal and vertical movement to make it easier to handle slopes
        Movement(move, false);
        move = Vector2.up * deltaPosition.y;
        Movement(move, true);
    }

    private void Movement(Vector2 move, bool yMovement) {
        float distance = move.magnitude;

        //don't check for collisions if object is not moving
        if(distance > minMoveDistance) {
            int count = rb2d.Cast(move, contactFilter, hitBuffer, distance + shellRadius);

            //copy into list to loop through only hits, not entire ArrayList
            hitBufferList.Clear();
            for (int i=0; i < count; i++) {
                hitBufferList.Add(hitBuffer[i]);
            }

            for (int i=0; i < hitBufferList.Count; i++) {
                Vector2 currentNormal = hitBufferList[i].normal;

                //grounds if the colliding object is within a certain range of angles
                if(currentNormal.y > minGroundNormalY) {
                    grounded = true;
                    if(yMovement) {
                        groundNormal = currentNormal;
                        currentNormal.x = 0f;
                    }
                }

                /* 
                 * Checks the dot product of current velocity and the normal of the colliding object. If it is negative, subtract 
                 * from the current velocity. This is for in the case of colliding with a sloped object so that it doesn't come to a 
                 * dead stop or gets stuck in the other collider, but will instead have the game object "scrape" off of the slope.
                 */
                float projection = Vector2.Dot(velocity, currentNormal);
                if(projection < 0f) {
                    velocity = velocity - projection * currentNormal;
                }

                //make sure that we do not get stuck in other colliders
                float modifiedDistance = hitBufferList[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }
        }
        rb2d.position = rb2d.position + move.normalized * distance;
    }
}
