using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int p1Wins = 0;
    public int p2Wins = 0;

    public FighterHealth player1;
    public FighterHealth player2;

    public void PlayerDied(FighterHealth deadPlayer)
    {
        if (deadPlayer == player1)
            p2Wins++;
        else
            p1Wins++;

        Debug.Log("Marcador: P1 = " + p1Wins + " | P2 = " + p2Wins);

        ResetRound();
    }

    void ResetRound()
    {
        player1.ResetHealth();
        player2.ResetHealth();

        // Si quieres, también reseteas posiciones:
         player1.transform.position = new Vector3(-3, 0, 0);
         player2.transform.position = new Vector3(3, 0, 0);
    }
}
