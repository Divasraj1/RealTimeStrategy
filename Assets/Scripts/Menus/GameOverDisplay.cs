using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class GameOverDisplay : MonoBehaviour
{
    [SerializeField] private GameObject gameOverDisplayParent = null;
    [SerializeField] private TMP_Text winnerNameText = null;
    // Start is called before the first frame update
    void Start()
    {
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    // Update is called once per frame
    void OnDestroy()
    {
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }
    public void LeaveGame()
    {
        if(NetworkServer.active && NetworkClient.isConnected)
        {
            //stop hosting
            NetworkManager.singleton.StopHost();
        }
        else
        {
            //stop client
            NetworkManager.singleton.StopClient();
        }
    }
    private void ClientHandleGameOver(string winner)
    {
        winnerNameText.text = $"{winner} has Won!";
        gameOverDisplayParent.SetActive(true);
    }
}
