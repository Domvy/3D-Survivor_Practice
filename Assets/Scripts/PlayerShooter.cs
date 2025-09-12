using UnityEngine;
using Photon.Pun;

public class PlayerShooter : MonoBehaviourPun
{
    public Gun gun; // 총 오브젝트
    public Transform gunPivot; // 기준 위치
    public Transform leftHandMount; // 왼쪽 손잡이 위치
    public Transform rightHandMount; // 오른쪽 손잡이 위치
    private PlayerInput playerInput;
    private Animator playerAnimator;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerAnimator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        gun.gameObject.SetActive(true);
    }
    private void OnDisable()
    {
        gun.gameObject.SetActive(false);
    }
    void Update()
    {
        if (!photonView.IsMine) return;

        if (playerInput.fire)
        {
            gun.Fire();
        }
        else if (playerInput.reload) 
        {
            if (gun.Reload())
            {
                playerAnimator.SetTrigger("Reload");
            }
        }

        UpdateUI();
    }
    
    private void UpdateUI()
    {
        if (gun != null && UIManager.instance != null)
        {
            UIManager.instance.UpdateAmmoText(gun.magAmmo, gun.ammoRemain);
        }
    }
    private void OnAnimatorIK(int layerIndex)
    {
        gunPivot.position = playerAnimator.GetIKHintPosition(AvatarIKHint.RightElbow); // 총 위치를 오른쪽 팔꿈치로 이동
        /* 왼손의 위치와 회전을 왼쪽 손잡이 위치로 변경 */
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f); // 손 위치를 완전히 IK로 제어 (0f ~ 1f)
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
        playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandMount.position); // 각 손의 위치를 손잡이 위치로
        playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandMount.rotation);
        /* 오른손의 위치와 회전을 오른쪽 손잡이 위치로 변경 */
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
        playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, rightHandMount.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.RightHand, rightHandMount.rotation);

    }
}
