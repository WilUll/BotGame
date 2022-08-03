using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicTacToeController : MonoBehaviour
{
    public bool xTurn = true;
    public bool clickable = true;

    public GameObject tile;
    private int tilesLeft;

    public List<Tile> board;

    public bool gameOver;
    private bool playerWon;
    // Start is called before the first frame update
    void Start()
    {
        board = new List<Tile>();
        SetupGame();
    }

    private void SetupGame()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                var temp = Instantiate(tile, transform);
                Tile tileTemp = temp.GetComponent<Tile>();
                tileTemp.coord = new int[x, y];
                temp.name = "Tile" + "[" + x + "," + y + "]";
                board.Add(tileTemp);
            }
        }

        tilesLeft = 9;
    }

    public void CheckForWin()
    {
        HorizontalCheck();
        VerticalCheck();
        DiagonalCheck();
        OtherPlayersTurn();
    }

    private void OtherPlayersTurn()
    {
        if (!gameOver)
        {
            //CHANGE PLAYER
            if (!xTurn)
            {
                AITurn();

            }

        }
        
    }

    private void AITurn()
    {
        List<Tile> tiles = new List<Tile>();
        foreach (var boardTile in board)
        {
            if (boardTile.State == Tile.TileState.Clear)
            {
                tiles.Add(boardTile);
            }
        }

        
        tiles[Random.Range(0, tiles.Count)].Clicked();
    }

    private void GameOver()
    {
        gameOver = true;
        if (playerWon)
        {
            //PLAYER WIN STUFF
        }
        else
        {
            //AI WIN STUFF
        }
    }

    private void DiagonalCheck()
    {
        if (board[0].State == Tile.TileState.X && board[4].State == Tile.TileState.X &&
            board[8].State == Tile.TileState.X)
        {
            Debug.Log("X Win Diagonal");
            playerWon = true;
            GameOver();
        }
        else if (board[0].State == Tile.TileState.O && board[4].State == Tile.TileState.O &&
                 board[8].State == Tile.TileState.O)
        {
            Debug.Log("O Win Diagonal");
            GameOver();
        }
        
        if (board[2].State == Tile.TileState.X && board[4].State == Tile.TileState.X &&
            board[6].State == Tile.TileState.X)
        {
            Debug.Log("X Win Diagonal");
            playerWon = true;
            GameOver();
        }
        else if (board[2].State == Tile.TileState.O && board[4].State == Tile.TileState.O &&
                 board[6].State == Tile.TileState.O)
        {
            Debug.Log("O Win Diagonal");
            GameOver();
        }
    }

    private void VerticalCheck()
    {
        for (int i = 0; i < 3; i++)
        {
            if (board[0 + i].State == Tile.TileState.X && board[3 + i].State == Tile.TileState.X &&
                board[6 + i].State == Tile.TileState.X)
            {
                Debug.Log("X Win Vertical");
                playerWon = true;
                GameOver();
            }
            else if (board[0 + i].State == Tile.TileState.O && board[3 + i].State == Tile.TileState.O &&
                     board[6 + i].State == Tile.TileState.O)
            {
                Debug.Log("O Win Vertical");
                GameOver();
            }
        }
    }

    private void HorizontalCheck()
    {
        for (int i = 0; i < 3; i++)
        {
            int column = 3 * i;
            if (board[0 + column].State == Tile.TileState.X && board[1 + column].State == Tile.TileState.X &&
                board[2 + column].State == Tile.TileState.X)
            {
                Debug.Log("X Win Vertical");
                playerWon = true;
                GameOver();
            }
            else if (board[0 + column].State == Tile.TileState.O && board[1 + column].State == Tile.TileState.O &&
                     board[2 + column].State == Tile.TileState.O)
            {
                Debug.Log("O Win Vertical");
                GameOver();
            }
        }
    }
}