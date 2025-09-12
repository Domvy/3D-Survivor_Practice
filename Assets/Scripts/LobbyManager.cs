using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1"; // 게임 버전 구분용

    public Text connectionInfoText;
    public Button joinButton;

    private void Start()
    {
        joinButton.interactable = false; // 접속 버튼 상호작용 일시적 비활성화
        PhotonNetwork.GameVersion = gameVersion; // 버전 설정
        PhotonNetwork.ConnectUsingSettings(); // 서버 접속 시도
        connectionInfoText.text = "서버에 접속 중...";
    }

    public void Connect()
    {
        joinButton.interactable = false;

        if (PhotonNetwork.IsConnected) // 서버 연결완료 시
        {
            PhotonNetwork.JoinRandomRoom(); // 대기방 랜덤 입장
            connectionInfoText.text = "룸에 입장...";
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            connectionInfoText.text = "서버와 연결되지 않음\n접속 재시도 중...";
        }
    }

    public override void OnConnectedToMaster() // 접속 성공 시 콜백 함수
    {
        joinButton.interactable = true;
        connectionInfoText.text = "서버와 연결됨";
    }
    public override void OnDisconnected(DisconnectCause cause) // 접속 실패 & 연결 끊김 시 콜백 함수
    {
        joinButton.interactable = false;
        PhotonNetwork.ConnectUsingSettings(); // 재접속 시도
        connectionInfoText.text = "서버와 연결되지 않음\n접속 재시도 중...";
    }
    public override void OnJoinRandomFailed(short returnCode, string message) // 방 참가 실패 시 콜백 함수
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 }); // 4명 수용 가능한 방 생성
        connectionInfoText.text = "새로운 방 생성중...";
    }
    public override void OnJoinedRoom() // 방 참가 성공 시 콜백 함수
    {
        PhotonNetwork.LoadLevel("Main"); // 참가자들에게 Main 씬 로드 명령
        connectionInfoText.text = "방 참가 성공";
    }
}
