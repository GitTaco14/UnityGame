using System;
using UnityEngine;


public class HealthPlusRed : MonoBehaviour
{
    public float rotateSpeed; // Rotation speed
public float addHealth; // Amount of health to add

private void Update()
{
    // Rotation
    transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
}

// Collision detection
private void OnTriggerEnter(Collider other)
{
    // If the object collides with the player, increase the player's health
    if (other.CompareTag("Player"))
    {
        other.GetComponent<PlayerController>().TakeDamage(-addHealth); // Negative value heals the player
        Destroy(gameObject); // Destroy this object after interaction
    }
}

}
