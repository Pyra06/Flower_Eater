using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoximonController : MonoBehaviour
{
    public CharacterController controller;
    public new Transform camera;
    public float runMultiplier;

    public float moveSpeed, walkSpeed, runSpeed;
    public float turnSoomthTime = 0.1f;
    float turnSmoothVelocity;

    public Animator animator;

    private int scoreCount = 0;
    private int lifeCount = 3;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI lifeText;
    public TextMeshProUGUI vicText;
    public TextMeshProUGUI defText;

    public AudioSource playSoundEat, playSoundVictory, playSoundDie;
    public float restartTiming;

    private void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        handleMovement();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Flowers"))
        {
            Eat();
            playSoundEat.Play();
            other.gameObject.SetActive(false);
            scoreCount++;
            countText.text = "SCORE : " + (scoreCount * 100).ToString();
            if (scoreCount >= 9)
            {
                vicText.gameObject.SetActive(true);
                defText.gameObject.SetActive(false);
                playSoundVictory.Play();
                Invoke("Restart", restartTiming);
            }
        }
        else if (other.gameObject.CompareTag("Mushroom"))
        {
            other.gameObject.SetActive(false);
            lifeCount--;
            lifeText.text = "Life : " + (lifeCount).ToString();
            if (lifeCount == 0)
            {
                Die();
                playSoundDie.Play();
                defText.gameObject.SetActive(true);
                vicText.gameObject.SetActive(false);
                Invoke("Restart", restartTiming);
            }
        }
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void handleMovement()
    {
        bool run = Input.GetKey(KeyCode.LeftShift);
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");


        Vector3 direction = new Vector3(horizontal, 0.0f, vertical).normalized;

        if (direction != Vector3.zero && run == false)
        {
            Walk();
        }
        else if (direction != Vector3.zero && run == true)
        {
            Run();
        }
        else if (direction == Vector3.zero)
        {
            Idle();
        }

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSoomthTime);
            transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);

            Vector3 moveDirection = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;
            controller.Move(moveDirection.normalized * moveSpeed * Time.deltaTime);
        }
    }

    void Idle()
    {
        //Idle
        animator.SetFloat("Speed", 0.0f, 0.1f, Time.deltaTime);
    }

    void Walk()
    {
        //Walk
        moveSpeed = walkSpeed;
        animator.SetFloat("Speed", 0.5f, 0.1f, Time.deltaTime);
    }

    void Run()
    {
        moveSpeed = runSpeed;
        animator.SetFloat("Speed", 1.0f, 0.1f, Time.deltaTime);
    }

    void Eat()
    {
        animator.SetTrigger("Eat");
    }

    void Die()
    {
        animator.SetTrigger("Die");
    }
}
