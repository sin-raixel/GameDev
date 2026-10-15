using UnityEngine;

public class CoinsBehavior : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.instance.AddCoins(1);
            FindFirstObjectByType<PlayerMovement>().PlayCoinSound();

            if (GameManager.instance.coins >= 20)
            {
                UIManagerScript.instance.ShowWinUI();
            }

            Destroy(gameObject);
        }
    }
}