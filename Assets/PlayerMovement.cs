using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cameraTransform ;
    private Animator animator;
    [Header("Movement")]
    public float ms = 3f;
    public float rotationSpeed = 10f;
    public float sprintSpeed = 10f;
    bool isSprinting;
    bool isMoving;
    bool isCrouching;
    bool wasGrounded;

    [Header("Jumping")]
    public float jumpHeight = 1f;
    public float grav = -9.81f;
    public float height = 2f;
    
    public float crouchHeight = 1f;
    private float normalHeight;
    private Vector3 velocity;
    [Header("Health")]
    public int health = 100;
    public Slider healthBar;
    private float damageTimer = 0f;
    public bool isDead = false;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip walkGround;
    public AudioClip sprintGround;
    public AudioClip walkGravel;
    public AudioClip sprintGravel;
    public AudioClip walkGrass;
    public AudioClip sprintGrass;
    public AudioClip jumpSound;
    public AudioClip landingSound;
    public AudioClip landingGrassSound;
    public AudioClip landingGravelSound;
    public AudioClip coinSound;

    public float footStepTimer;

    void Start()
    {
          animator = GetComponent<Animator>();
    normalHeight = controller.height;

        healthBar.maxValue = 100;
        healthBar.value = health;
    }

    void Update()
{
    if (isDead)
        return;

    MovePlayer();

    if (damageTimer > 0)
        damageTimer -= Time.deltaTime;
}

    private void MovePlayer()
    {
        bool grounded = controller.isGrounded;
        if (grounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float horizontal = Input.GetAxis("Horizontal");
        
        float vertical = Input.GetAxis("Vertical");

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 move = forward* vertical+ right* horizontal;    

        isMoving = move.magnitude > 0.1f;
        isSprinting = isMoving && Input.GetKey(KeyCode.LeftShift);
        isCrouching =  Input.GetKey(KeyCode.LeftControl);

        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isSprinting", isSprinting);
        animator.SetBool("isJumping", !grounded);
        animator.SetBool("isCrouching", isCrouching);

        if (isMoving)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            footStepTimer -= Time.deltaTime;
            if(footStepTimer <= 0)
            {
                PlayFootSteps(isSprinting);

                if(isSprinting)
                {
                    footStepTimer = 0.30f;

                }
                else
                {
                    footStepTimer = 0.75f;
                }
            }
        }
        else
        {
            footStepTimer = 0f;
        }
        
        controller.Move(move.normalized * ms * Time.deltaTime);
        
        if (Input.GetButtonDown("Jump") && grounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * grav);
            audioSource.PlayOneShot(jumpSound);

        } 

        if (Input.GetKey(KeyCode.LeftShift)) {
            ms = sprintSpeed;
        }
        else
        {
            ms = 3f;

        }
       if (Input.GetKey(KeyCode.LeftControl))
        {
            controller.height = crouchHeight;
            controller.center = new Vector3(0, crouchHeight / 2f, 0);
        }
        else
        {
            controller.height = normalHeight;
            controller.center = new Vector3(0, normalHeight / 2f, 0);
        }
        velocity.y += grav * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime); 

        bool landed = !wasGrounded && controller.isGrounded;

        if (landed)
        {
            PlayLandingSound();
        }

        wasGrounded = controller.isGrounded;
      
    }
     public void OnControllerColliderHit(ControllerColliderHit hit)
    {
       if (hit.gameObject.CompareTag("Lava") && damageTimer <= 0)
    {
        TakeDamage(10);
        damageTimer = 1f;
    }
        if (hit.gameObject.CompareTag("Boulder")){
                    TakeDamage(100);

        }
    }
    private void TakeDamage(int v)
    {
        health -= v;
        healthBar.value = health;

        if (health <= 0)
        {
            Die();
        }
    }

        private void Die()
        {
            isDead = true;
            animator.SetTrigger("isDead");
            StartCoroutine(DeathSequence());
        }
        private System.Collections.IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(2f);

        UIManagerScript.instance.ShowDeathUI();
        gameObject.SetActive(false);
    }

    void PlayFootSteps(bool isRunning)
    {
        isRunning = isSprinting;
        RaycastHit hit;
        Vector3 origin = transform.position + controller.center;
        origin.y -= controller.height / 2f;
        if(Physics.Raycast(origin, Vector3.down, out hit, 0.5f))
        {
            Debug.Log("Hit: " + hit.collider.tag);
            if(hit.collider.CompareTag("Ground"))
            {
                if(isRunning)
                {
                    audioSource.PlayOneShot(sprintGround);
                }
                else
                {
                    audioSource.PlayOneShot(walkGround);
                }
            }
            else if(hit.collider.CompareTag("Gravel")) 
            {
                if(isRunning)
                {
                    audioSource.PlayOneShot(sprintGravel);
                }
                else
                {
                    audioSource.PlayOneShot(walkGravel);
                }
            }
            else if(hit.collider.CompareTag("Grass")) 
            {
                if(isRunning)
                {
                    audioSource.PlayOneShot(sprintGrass);
                }
                else
                {
                    audioSource.PlayOneShot(walkGrass);
                }
            }
            else
            {
                Debug.Log("No ground detected.");
            }

            
        }
        
        
    }
    void PlayLandingSound()
{
    RaycastHit hit;

    Vector3 origin = transform.position + controller.center;
    origin.y -= controller.height / 2f;

    if (Physics.Raycast(origin, Vector3.down, out hit, 1f))
    {
        if (hit.collider.CompareTag("Ground"))
        {
            audioSource.PlayOneShot(landingSound);
        }
        else if (hit.collider.CompareTag("Gravel"))
        {
            audioSource.PlayOneShot(landingGravelSound);
        }
        else if (hit.collider.CompareTag("Grass"))
        {
            audioSource.PlayOneShot(landingGrassSound);
        }
    }
}
            public void PlayCoinSound()
        {
            if (coinSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(coinSound);
            }
        }
}
