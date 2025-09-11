using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // 이동 속도
    public float rotateSpeed = 180f; // 회전 속도

    private PlayerInput playerInput;

    private Rigidbody playerRigidBody;
    private Animator playerAnimator;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerRigidBody = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
    }
    private void FixedUpdate()
    {
        Rotate();
        Move();

        playerAnimator.SetFloat("Move", playerInput.move);
    }
    
    private void Move() // 이동
    {
        Vector3 moveDistance = playerInput.move * transform.forward * moveSpeed * Time.deltaTime;
        playerRigidBody.MovePosition(playerRigidBody.position + moveDistance);
    }
    private void Rotate() // 회전
    {
        float turn = playerInput.rotate * rotateSpeed * Time.deltaTime;
        playerRigidBody.rotation = playerRigidBody.rotation * Quaternion.Euler(0, turn, 0f);
    }
}
