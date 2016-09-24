using System;
using System.Collections.Generic;
using System.Text;

namespace A14_Ex02
{
    public class Board
    {
        public enum eTileStatus
        {
            Empty,
            PlayerA,
            PlayerB,
        }

        private readonly eTileStatus[,] r_Matrix;
        private readonly int[] r_NumOfTokensInColumn;
        private int m_NumRows;
        private int m_NumCols;

        public int NumOfRows
        {
            get { return m_NumRows; }
            set { m_NumRows = value; }
        }

        public int NumOfCols
        {
           get { return m_NumCols; }
           set { m_NumCols = value; }
        }

        public Board(int i_NumRows, int i_NumCols)
        {
            r_Matrix = new eTileStatus[i_NumRows, i_NumCols];
            r_NumOfTokensInColumn = new int[i_NumCols];
            m_NumCols = i_NumCols;
            m_NumRows = i_NumRows;
        }

        public eTileStatus CheckTileStatus(int i_Row, int i_Col)
        {
            return r_Matrix[i_Row, i_Col];
        }

        public void SetTileStatus(int i_Row, int i_Col, eTileStatus status)
        {
            r_Matrix[i_Row, i_Col] = status;
        }

        public int GetNumOfTokensInCol(int i_Col)
        {
            return r_NumOfTokensInColumn[i_Col];
        }

        public void SetNumOfTokensInCol(int i_Col, int value)
        {
            r_NumOfTokensInColumn[i_Col] = value;
        }

        public bool IsThePlayerTokenOnTile(int i_row, int i_Column, eTileStatus i_player)
        {
            bool playerIsOnTile = false;

            if(TileIsValid(i_row, i_Column))
            {
                playerIsOnTile = CheckTileStatus(i_row, i_Column) == i_player;
            }
     
            return playerIsOnTile;
        }

        public bool TileIsValid(int i_Row, int i_Colomn)
        {
            bool isTileValid;

            isTileValid = i_Row < m_NumRows && i_Colomn < m_NumCols && i_Row >= 0 && i_Colomn >= 0;

            return isTileValid;
        }

        public bool IsBoardFull()
        {
            bool isBoardFull = true;

            for (int i = 0; i < m_NumCols && isBoardFull; i++)
            {
                isBoardFull = r_Matrix[0, i] != Board.eTileStatus.Empty;
            }

            return isBoardFull;
        }
    }
}
