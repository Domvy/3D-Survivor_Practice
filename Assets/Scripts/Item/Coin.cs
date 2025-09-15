using UnityEngine;
using Photon.Pun;

public class Coin : MonoBehaviourPun, IItem
{
    public int score = 200;

    public void Use(GameObject target)
    {
        GameManager.instance.AddScore(score);
        PhotonNetwork.Destroy(gameObject);
    }
}
