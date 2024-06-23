using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float laneOffset;
    float laneChangeSpeed = 7;
    float accelerateSpeed = 1;
    float pointStart;
    float pointFinish;
    Rigidbody rb;
    Vector3 targetVelocity;
    float jumpPower = 6;
    private bool isGrounded;
    public Transform groundCheck;
    public float groundDistance = 0.4f;

    // public float groundDistance = 0.4f;
    public LayerMask groundMask;

    void Start()
    {
        laneOffset = MapGenerator.instance.laneOffset;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        isGrounded = true;
        // isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        Vector3 checkPosition = groundCheck.position;

        // Создаем луч вниз от позиции groundCheck
        Ray ray = new Ray(checkPosition, Vector3.down);
        bool isGround = Physics.Raycast(ray, groundDistance, groundMask);

        // TODO разабраться с отрабатыванием isGround
        // Debug.Log("groundCheck.position" + groundCheck.position);
        // Debug.Log("groundMask" + groundMask);
        // Debug.Log("isGrounded" + isGrounded);
        // Debug.Log("isGround" + isGround);

        if (Input.GetKeyDown(KeyCode.A) && pointFinish > -2 * laneOffset)
        {
            MoveHorizontal(-laneChangeSpeed);
        }
        if (Input.GetKeyDown(KeyCode.D) && pointFinish < 2 * laneOffset)
        {
            MoveHorizontal(laneChangeSpeed);
        }
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            Jump();
        }
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
    }

    void MoveHorizontal(float speed)
    {
        if (!isGrounded)
        {
            return;
        }
        pointStart = pointFinish;
        pointFinish += Mathf.Sign(speed) * laneOffset;
        StartCoroutine(MoveCoroutine(speed));
    }

    IEnumerator MoveCoroutine(float vectorX)
    {
        Jump();
        while (Mathf.Abs(pointStart - transform.position.x) < laneOffset)
        {
            yield return new WaitForFixedUpdate();
            rb.velocity = new Vector3(vectorX / 3.4f, rb.velocity.y, 0);
            float x = Mathf.Clamp(
                transform.position.x,
                Mathf.Min(pointStart, pointFinish),
                Mathf.Max(pointStart, pointFinish)
            );
            // TODO к перемещению добавить ещё вращение
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }
        rb.velocity = Vector3.zero;
        transform.position = new Vector3(pointFinish, transform.position.y, transform.position.z);
    }

    // private void OnCollisionEnter(Collision collision)
    // {
    //     Debug.Log($"Collided with {collision.gameObject.tag}");
    //     // Проверка на столкновение с землей
    //     if (collision.gameObject.CompareTag("Ground"))
    //     {
    //         isGrounded = true;
    //     }
    // }

    // private void OnCollisionExit(Collision collision)
    // {
    //     Debug.Log($"Exited collision with {collision.gameObject.tag}");
    //     // Проверка на выход из контакта с землей
    //     if (collision.gameObject.CompareTag("Ground"))
    //     {
    //         isGrounded = false;
    //     }
    // }

    bool IsGrounded()
    {
        return transform.position.y == 0;
    }

    void AccelerateSpeed()
    {
        RoadGenerator.Instance.speed += accelerateSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            rb.constraints |= RigidbodyConstraints.FreezePositionZ;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            rb.constraints &= ~RigidbodyConstraints.FreezePositionZ;
        }
    }
}
