using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed; // Movement speed
    public float jumpForce; // Jump force
    public bool isGrounded; // Whether the player is on the ground
    public Transform groundCheck; // Ground check point
    public float groundCheckRadius; // Ground check radius
    public LayerMask whatIsGround; // Ground layer
    public float rotationSpeed; // Rotation speed
    public float cameraXRotationSpeed; // Camera X-axis rotation speed
    public Vector3 lookAtOffset; // Offset for the camera look-at point
    public Vector2 cameraRotationLimitX; // X-axis rotation limits
    public Transform cameraParentTransform; // Camera parent transform
    public float dithering; // Dithering intensity
    public Transform firPos; // Fire point
    public GameObject bullet; // Bullet prefab
    public float bulletSpeed; // Bullet speed
    public float bulletDamage; // Bullet damage
    public GameObject muzzleEffect; // Muzzle effect
    public float attackInterval; // Attack interval
    public float maxHealth; // Maximum health
    public Slider healthSlider; // Health bar

    private Rigidbody _rigidbody;
    private Animator _animator;
    private Vector3 _localCameraRotation;
    private Transform _cameraTransform;
    private bool _isAttack;
    private float _lastAttackTime;
    private float _currentHealth; // Current health
    public virtual void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        _rigidbody = GetComponent<Rigidbody>(); // Get Rigidbody component
        _animator = GetComponent<Animator>(); // Get Animator component
        _localCameraRotation = cameraParentTransform.localRotation.eulerAngles; // Get local rotation of the camera parent
        _cameraTransform = Camera.main.transform; // Get camera transform
        cameraParentTransform.position = transform.position + Quaternion.Euler(0,transform.rotation.eulerAngles.y,0) * lookAtOffset; // Set camera parent position

        _currentHealth = maxHealth; // Set current health
        healthSlider.maxValue = maxHealth; // Set health bar max value
        healthSlider.value = maxHealth; // Set health bar current value
    }

    public virtual void Update()
    {
        // Toggle cursor lock and visibility with the Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                // Unlock the cursor and make it visible
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                // Lock the cursor and make it invisible again
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        
        if (_currentHealth <= 0 || Time.timeScale == 0)
        {
            return;
        }
        // Check if the player is on the ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, whatIsGround);

        PlayerMove();
        PlayerJump();
        RotatePlayer();
        Attack();
    }

    private void LateUpdate()
    {
        if (_currentHealth <= 0 || Time.timeScale == 0)
        {
            return;
        }
        CameraControl(); // Camera control
    }

    private void PlayerMove()
    {
        // Get horizontal axis input
        float horizontalInput = Input.GetAxis("Horizontal");
        // Get vertical axis input
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputDir = new Vector3(horizontalInput, 0, verticalInput).normalized;
        if (horizontalInput != 0 || verticalInput != 0)
        {
            // Move
            transform.position += Quaternion.Euler(0, _cameraTransform.rotation.eulerAngles.y,0) * inputDir * moveSpeed * Time.deltaTime;
        }
        _animator.SetFloat("X", inputDir.x);
        _animator.SetFloat("Y", inputDir.z);
    }
    
    private void PlayerJump()
    {
        // Press spacebar and check if on the ground
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // Jump
            _rigidbody.AddForce(Vector3.up * jumpForce);
        }
        _animator.SetFloat("AccelerationY", _rigidbody.velocity.y);
        _animator.SetBool("isGrounded", isGrounded);
    }
    
    // Rotate the player
    private void RotatePlayer()
    {
        float horizontalInput = Input.GetAxis("Mouse X");
        transform.Rotate(new Vector3(0, horizontalInput, 0) * rotationSpeed * Time.deltaTime);
    }
    
    // Attack animation event
    private void Attack()
    {
        _isAttack = Input.GetKey(KeyCode.Mouse0) && isGrounded;
        _animator.SetBool("Attack", _isAttack);

        // If attacking and the attack interval has passed, attack
        if (_isAttack)
        {
            if (Time.time - _lastAttackTime > attackInterval)
            {
                _lastAttackTime = Time.time;
                GameObject effect = Instantiate(muzzleEffect, firPos.position, firPos.rotation);
                effect.transform.parent = firPos;
                Destroy(effect, 1);
                GameObject bulletObj = Instantiate(bullet, firPos.position, firPos.rotation);
                bulletObj.GetComponent<Bullet>().Init(firPos.forward, bulletSpeed, bulletDamage);
            }
        }
    }
    
    // Camera control
    private void CameraControl()
    {
        Vector3 ditheringOffset = Vector3.zero;
        // Camera dithering
        if (_isAttack)
        {
            ditheringOffset = new Vector3(UnityEngine.Random.Range(-dithering, dithering), UnityEngine.Random.Range(-dithering, dithering), UnityEngine.Random.Range(-dithering, dithering));
        }
        
        // Camera follow
        cameraParentTransform.position = transform.position + Quaternion.Euler(0,transform.rotation.eulerAngles.y,0) * lookAtOffset + ditheringOffset;
        
        // Get vertical axis input
        float verticalInput = Input.GetAxis("Mouse Y");
        _localCameraRotation.x -= verticalInput * cameraXRotationSpeed * Time.deltaTime; // Rotate
        _localCameraRotation.x = Mathf.Clamp(_localCameraRotation.x, cameraRotationLimitX.x, cameraRotationLimitX.y); // Clamp rotation
        cameraParentTransform.localRotation = Quaternion.Euler(_localCameraRotation); // Set rotation
    }

    // Take damage
    public void TakeDamage(float damage)
    {
        if (_currentHealth <= 0)
        {
            return;
        }
        
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);
        healthSlider.value = _currentHealth;
        if (_currentHealth <= 0)
        {
            _animator.SetTrigger("Die");
            GameManager.Instance.GameOver("You have died. Game over!");
        }
    }
}

