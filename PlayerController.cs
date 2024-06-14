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
    Coroutine movingCoroutine;
    void Start()
    {
        laneOffset = MapGenerator.instance.laneOffset;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) 
        && pointFinish > -2*laneOffset)
        {
            MoveHorizontal(-laneChangeSpeed);
        }
        if (Input.GetKeyDown(KeyCode.D)
         && pointFinish < 2*laneOffset)
        {
            MoveHorizontal(laneChangeSpeed);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            MoveLinear();
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
        if (!isJumping) Jump();
    }
    IEnumerator MoveCoroutine(float vectorX)
    {
        isMoving = true;
        if (!isJumping) Jump();
        while (Mathf.Abs(pointStart - transform.position.x) < laneOffset)
        {
            yield return new WaitForFixedUpdate(); 
            rb.velocity = new Vector3(vectorX, rb.velocity.y, 0);
            float x = Mathf.Clamp(transform.position.x, Mathf.Min(pointStart, pointFinish), Mathf.Max(pointStart, pointFinish));
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }
        rb.velocity = Vector3.zero;
        transform.position = new Vector3(pointFinish, transform.position.y, transform.position.z);
        isMoving = false;
    }
    void Jump() 
    {
        if (isJumping) return;
        isJumping = true;
        rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        Physics.gravity = new Vector3(0, jumpGravity, 0);
        StartCoroutine(StopJumpCoroutine());
    }
    IEnumerator StopJumpCoroutine()
    {
        isJumping = true;
        yield return new WaitUntil(() => IsGrounded());
        Physics.gravity = new Vector3(0, realGravity, 0);
        RoadGenerator.Instance.speed = RoadGenerator.Instance.maxSpeed;
        isJumping = false;
        
        // yield return new WaitUntil(() => IsGrounded());
        // Physics.gravity = new Vector3(0, realGravity, 0);
        // RoadGenerator.Instance.speed = RoadGenerator.Instance.maxSpeed;
        // isJumping = false;
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
