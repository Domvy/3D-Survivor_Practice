using UnityEngine;

[CreateAssetMenu(fileName = "ZombieData", menuName = "Scriptable Objects/ZombieData")]
public class ZombieData : ScriptableObject
{
    public float damage = 50f;
    public float health = 100;
    public float speed = 2f;
    public Color skinColor = Color.white;
}
