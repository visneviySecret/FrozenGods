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
    bool isMoving = false;
    bool isJumping = false;
    float jumpPower = 6;
    float jumpGravity = -35;
    float realGravity = -9.8f;
    public float jumpForce = 0.125f;
    private bool isGrounded;
    Coroutine movingCoroutine;

    void Start()
    {
        laneOffset = MapGenerator.instance.laneOffset;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
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

    void MoveHorizontal(float speed)
    {
        pointStart = pointFinish;
        pointFinish += Mathf.Sign(speed) * laneOffset;

        if (!isMoving)
        {
            StartCoroutine(MoveCoroutine(speed));
        }
        // if (isMoving) { StopCoroutine(movingCoroutine); isMoving = false; }
        // movingCoroutine = StartCoroutine(MoveCoroutine(speed));
    }

    void MoveLinear()
    {
        AccelerateSpeed();
        if (!isJumping)
            Jump();
    }

    IEnumerator MoveCoroutine(float vectorX)
    {
        isMoving = true;
        if (!isJumping)
            Jump();
        while (Mathf.Abs(pointStart - transform.position.x) < laneOffset)
        {
            yield return new WaitForFixedUpdate();
            rb.velocity = new Vector3(vectorX, rb.velocity.y, 0);
            float x = Mathf.Clamp(
                transform.position.x,
                Mathf.Min(pointStart, pointFinish),
                Mathf.Max(pointStart, pointFinish)
            );
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }
        rb.velocity = Vector3.zero;
        transform.position = new Vector3(pointFinish, transform.position.y, transform.position.z);
        isMoving = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
    }

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
