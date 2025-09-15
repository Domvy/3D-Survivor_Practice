using UnityEngine;
using Photon.Pun;

public class HealthPack : MonoBehaviourPun, IItem
{
    public float health = 50f;

    public void Use(GameObject target)
    {
        LivingEntity livingEntity = target.GetComponent<LivingEntity>();

        if (livingEntity != null)
        {
            livingEntity.RestoreHealth(health);
        }

        PhotonNetwork.Destroy(gameObject);
    }
}
