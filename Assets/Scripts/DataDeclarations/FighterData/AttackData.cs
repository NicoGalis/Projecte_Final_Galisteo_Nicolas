using UnityEngine;

[CreateAssetMenu(fileName = "NewAttack", menuName = "Fighter/Attack")]
public class AttackData : ScriptableObject
{
    [Header("Frame Data")]
    public float startup;
    public float active;
    public float recovery;

    [Header("Cancel Window")]
    public float cancelStart;
    public float cancelEnd;

    [Header("Hitbox")]
    public Vector2 hitboxSize = new Vector2(1, 1);
    public Vector2 hitboxOffset = new Vector2(0.5f, 0.5f);

    [Header("Damage")]
    public int damage = 10;

    [Header("Block Damage")]
    public float blockDamage = 10f;   

    [Header("Animation")]
    public string animationName;
}
