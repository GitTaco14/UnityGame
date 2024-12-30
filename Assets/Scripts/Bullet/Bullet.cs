using System;
using UnityEngine;


public class Bullet : MonoBehaviour
{
    public bool targetIsPlayer; // Whether the target is a player
    public float detectionRadius; // Detection radius
    public float lifeTime; // Bullet lifetime
    public GameObject hitEffect; // Hit effect

    private Vector3 moveDir; // Movement direction
    private float moveSpeed; // Movement speed
    private float attackDamage; // Attack damage

    // Initialize the bullet
    public void Init(Vector3 moveDir, float moveSpeed, float attackDamage)
    {
        this.moveDir = moveDir.normalized;
        this.moveSpeed = moveSpeed;
        this.attackDamage = attackDamage;
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime); // Destroy the bullet after its lifetime expires
    }

    private void Update()
    {
        // Record the position from the last frame to check for collisions
        Vector3 lastPos = transform.position;
        transform.position += moveDir * moveSpeed * Time.deltaTime; // Move the bullet

        // Check for collisions
        RaycastHit[] colliders = Physics.SphereCastAll(lastPos, detectionRadius, transform.position - lastPos, Vector3.Distance(transform.position, lastPos));
        foreach (var hit in colliders)
        {
            // If it collides with the player and the target is the player, deal damage to the player
            if (hit.collider.CompareTag("Player") && targetIsPlayer)
            {
                hit.collider.GetComponent<PlayerController>().TakeDamage(attackDamage);
            }
            else
            {
                // If it collides with an enemy and the target is not the player, deal damage to the enemy
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null && !targetIsPlayer)
                {
                    enemy.TakeDamage(attackDamage);
                }
            }
            // Create the hit effect
            GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            Destroy(effect, 1);
            Destroy(gameObject); // Destroy the bullet after it hits
        }
    }
}

