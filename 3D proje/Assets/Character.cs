using UnityEngine;

public class Character : MonoBehaviour
{
    public float moveSpeed;
    public float jumpForce;
    public Transform groundCheckPoint;
    public LayerMask groundLayer; //zemin katı mı su mu

    private bool isGrounded;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    public void FixedUpdate()
    {
        isGrounded = Physics.Raycast(groundCheckPoint.position, Vector3.down, 0.1f, groundLayer);

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;

        if (moveDirection.magnitude >= 0.1f)
        {
            rb.MovePosition(transform.position + moveDirection * moveSpeed * Time.deltaTime);
        }

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}