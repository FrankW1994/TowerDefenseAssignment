using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class UiBehaviour : MonoBehaviourPun
{

    
    [SerializeField] private GameState gameState;
    [SerializeField] private TMP_Text _kills;
    [SerializeField] private TMP_Text _score;
    [SerializeField] private TMP_Text _currency;
    [SerializeField] private TMP_Text _finalScore;
    [SerializeField] private GameObject _gameOverPanel;

    private void OnEnable(){
        GameState.OnZeroCurrency += HandleGameOver;
    }

    private void OnDisable()
    {
        GameState.OnZeroCurrency -= HandleGameOver;
    }



    private void Start(){
        
        gameState = FindObjectOfType<GameState>();
        //_gameOverPanel.SetActive(false);
       
    }

    private void FixedUpdate(){
        
            _kills.text = gameState.kills.ToString();
            _score.text = gameState.sharedScore.ToString();
            _currency.text = gameState.GetCurrency().ToString();
        

    }

    private void HandleGameOver(){
        _finalScore.text = _score.text;
        _gameOverPanel.SetActive(true);
        
        
       // Debug.Log("Show Panel GO");
    }

}
