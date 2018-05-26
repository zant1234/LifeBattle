using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PhysicsObject {

    public float maxSpeed = 7f;
    public float jumpTakeOffSpeed = 7f;
    public float startingHealth;

    private float health;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        health = startingHealth;
    }

    protected override void ComputeVelocity() {
        Vector2 move = Vector2.zero;

        move.x = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump") && grounded) {
            velocity.y = jumpTakeOffSpeed;
        } else if (Input.GetButtonUp("Jump")) {

            //allows user to cancel jump midair
            if(velocity.y > 0f) {
                velocity.y = velocity.y * 0.5f;
            }
        }

        bool flipSprite = (spriteRenderer.flipX ? (move.x > 0f) : (move.x < 0f));
        if (flipSprite) {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }

        animator.SetBool("grounded", grounded);
        animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
        animator.SetFloat("velocityY", velocity.y / maxSpeed);

        targetVelocity = move * maxSpeed;
    }

    private void OnCollisionEnter2D(Collision2D coll) {
        if(coll.gameObject.tag == "HealthMod") {
            HealthModifier other = coll.gameObject.GetComponent<HealthModifier>();
            if(!other.isHit) {
                this.Heal(other.healthAmount);
                other.Hit();
            }
        }
    }

    private void Heal(float healAmount) {
        health += healAmount;
    }
}
