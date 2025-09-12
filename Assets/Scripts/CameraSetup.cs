using UnityEngine;
using Unity.Cinemachine;
using Photon.Pun;

public class CameraSetup : MonoBehaviourPun
{
    private void Start()
    {
        if (photonView.IsMine) // 로컬 플레이어일 경우
        {
            /* 씬의 카메라를 찾아 현재 오브젝트를 비추도록 변경 */
            var followcam = FindFirstObjectByType<CinemachineCamera>();
            followcam.Follow = transform;
            followcam.LookAt = transform;
        }
    }
}
