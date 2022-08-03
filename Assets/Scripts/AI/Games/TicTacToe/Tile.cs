using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Tile : MonoBehaviour
{
    public TileState State = TileState.Clear;
    private TicTacToeController ticTacToeController;
    public int[,] coord;
    
    // Start is called before the first frame update
    void Start()
    {
        ticTacToeController = GetComponentInParent<TicTacToeController>();
    }

    public void Clicked()
    {
        if (State == TileState.Clear && !ticTacToeController.gameOver)
        {
            State = ticTacToeController.xTurn ? TileState.X : TileState.O;
            
            GetComponent<Image>().color = State == TileState.X ? Color.red : Color.blue;
            
            ticTacToeController.xTurn = !ticTacToeController.xTurn;
            ticTacToeController.clickable = false;
            ticTacToeController.CheckForWin();
        }
        else
        {
            Debug.Log("AlreadyPicked");
        }
    }
    
    public enum TileState
    {
        Clear,
        X,
        O,
    }
}
