using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class Enemy : MonoBehaviour
{
    public float attackRange; // Attack range
    public float attackInterval; // Attack interval
    public float attackDamage; // Attack damage
    public bool isRemote; // Whether it is a ranged attack

    public GameObject remoteAttackBullet; // Ranged attack bullet
    public Transform attackPoint; // Ranged attack point
    public float bulletSpeed; // Bullet speed

    public float detectionRadius; // Detection radius
    public GameObject hitEffect; // Hit effect

    public float maxHealth; // Maximum health
    public Slider healthSlider; // Health bar

    private Animator _animator;
    private Transform _playerTransform;
    private float _lastAttackTime;
    private NavMeshAgent _meshAgent;
    private float _currentHealth; // Current health

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _playerTransform = GameObject.FindWithTag("Player").transform;
        _meshAgent = GetComponent<NavMeshAgent>();

        _currentHealth = maxHealth; // Set current health
        healthSlider.maxValue = maxHealth; // Set health bar max value
        healthSlider.value = maxHealth; // Set health bar current value
    }

    private void Update()
    {
        // If health is zero or less, or the game is paused, stop moving
        if (_currentHealth <= 0 || Time.timeScale == 0)
        {
            _meshAgent.isStopped = true;
            _animator.SetBool("Move", false);
            return;
        }

        // If the player is within attack range, stop moving and play attack animation
        if (Vector3.Distance(transform.position, _playerTransform.position) < attackRange)
        {
            _meshAgent.isStopped = true;
            _animator.SetBool("Move", false);
            if (Time.time - _lastAttackTime > attackInterval) // If attack interval has elapsed, attack
            {
                _animator.SetTrigger("Attack");
                _lastAttackTime = Time.time;
                Vector3 dir = _playerTransform.position - transform.position;
                dir.y = 0;
                transform.rotation = Quaternion.LookRotation(dir);
            }
        }
        else
        {
            // If the player is out of attack range, play move animation and move toward the player
            bool isAttacking = _animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");
            _meshAgent.isStopped = isAttacking;
            _animator.SetBool("Move", !isAttacking);
            if (!isAttacking)
            {
                _meshAgent.SetDestination(_playerTransform.position);
            }
        }
    }

    private void LateUpdate()
    {
        // Health bar always faces the camera
        healthSlider.transform.LookAt(Camera.main.transform);
    }

    // Attack animation event
    public void Attack()
    {
        // If it is a ranged attack, spawn bullets
        if (isRemote)
        {
            // Spawn bullets
            GameObject bullet = Instantiate(remoteAttackBullet, attackPoint.position, attackPoint.rotation);
            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            bulletComponent.Init(_playerTransform.position - transform.position + Vector3.up / 2, bulletSpeed, attackDamage);
        }
        else
        {
            // Raycast detection
            RaycastHit[] hits;
            hits = Physics.SphereCastAll(attackPoint.position, detectionRadius, transform.forward, 0);
            foreach (RaycastHit hit in hits)
            {
                // If the player is detected, deal damage to the player
                if (hit.collider.CompareTag("Player"))
                {
                    PlayerController playerController = hit.collider.GetComponent<PlayerController>();
                    playerController.TakeDamage(attackDamage);

                    // Create hit effect
                    GameObject effect = Instantiate(hitEffect, playerController.transform.position, Quaternion.identity);
                    Destroy(effect, 1);
                }
            }
        }
    }

    // Take damage
    public void TakeDamage(float damage)
    {
        if (_currentHealth <= 0)
        {
            return;
        }

        // Create hit effect
        _currentHealth -= damage;
        healthSlider.value = _currentHealth;
        if (_currentHealth <= 0)
        {
            // Play death animation, stop moving, destroy after 3 seconds
            _animator.SetTrigger("Die");
            _meshAgent.isStopped = true;
            Destroy(gameObject, 3);
            GameManager.Instance.EnemyDead();
        }
    }
}

