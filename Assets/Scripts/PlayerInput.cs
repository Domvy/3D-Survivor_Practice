using UnityEngine;
using Photon.Pun;

public class PlayerInput : MonoBehaviourPun
{
    public string moveAxisName = "Vertical";
    public string rotateAxisName = "Horizontal";
    public string fireButtonName = "Fire1";
    public string reloadButtonName = "Reload";

    public float move { get; private set; }
    public float rotate { get; private set; }
    public bool fire { get; private set; }
    public bool reload { get; private set; }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) return; // 로컬 플레이어가 아닌 경우 바로 종료

        if (GameManager.instance != null && GameManager.instance.isGameOver)
        {
            move = 0;
            rotate = 0;
            fire = false;
            reload = false;
            return;
        }

        move = Input.GetAxis(moveAxisName);
        rotate = Input.GetAxis(rotateAxisName);
        fire = Input.GetButton(fireButtonName);
        reload = Input.GetButtonDown(reloadButtonName);
    }

    
}
