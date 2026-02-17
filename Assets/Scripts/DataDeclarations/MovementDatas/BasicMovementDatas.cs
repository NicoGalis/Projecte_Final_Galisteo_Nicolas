using UnityEngine;

[CreateAssetMenu(fileName = "BasicMovementDatas", menuName = "Scriptable Objects/BasicMovementDatas")]
public class BasicMovementDatas : ScriptableObject
{
    public float speed, jumpForce, blockTime, onAirSpeedDivisor, fallSpeed, landTime;
}
