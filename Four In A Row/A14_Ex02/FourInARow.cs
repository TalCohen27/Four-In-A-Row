using System;
using System.Collections.Generic;
using System.Text;

namespace A14_Ex02
{
    public class FourInARow
    {
        public enum eGameMode
        {
            Solo,
            TwoPlayers
        }

         public const int k_MaxDimention = 8;
         public const int k_MinDimention = 4;
         private const int k_NumOfPlayers = 2;
         private const int k_FirstPlayer = 0;
         private const int k_SecondPlayer = 1;
         private const int k_FirstRowAndColIndex = 0;
         private const int k_HowManyInARowToWin = 4;

        private readonly Player[] r_Players = new Player[k_NumOfPlayers] { new Player(), new Player() };
        private Board.eTileStatus? m_Winner = null;
        private eGameMode? m_Mode = null;
        private Board m_GameBoard;
        private bool m_AskForAnotherRound;
        private bool m_EndOfGame;
        private int m_TurnsCounter;

        public FourInARow()
        {
            r_Players[k_FirstPlayer].Type = Player.ePlayerType.Human;
            r_Players[k_FirstPlayer].Name = Board.eTileStatus.PlayerA;
            r_Players[k_SecondPlayer].Name = Board.eTileStatus.PlayerB;
        }

        public int GetBoardNumRows()
        {
            return m_GameBoard.NumOfRows;
        }

        public int GetBoardNumCols()
        {
            return m_GameBoard.NumOfCols;
        }

        public eGameMode? Mode
        {
            get { return m_Mode; }
            set { m_Mode = value; }
        }

        public Board.eTileStatus GetTileStatus(int i_Row, int i_Col)
        {
            return m_GameBoard.CheckTileStatus(i_Row, i_Col);
        }

        public bool AttemptAMove(int i_ColIndex)
        {
            bool columnIsFree = false;
            Player.ePlayerType playerType = GetCurrentPlayerType();

            if(playerType == Player.ePlayerType.Computer)
            {
                Random number = new Random();
                i_ColIndex = number.Next(k_FirstRowAndColIndex, m_GameBoard.NumOfCols);
            }

            columnIsFree = m_GameBoard.CheckTileStatus(k_FirstRowAndColIndex, i_ColIndex) == Board.eTileStatus.Empty;

            if (columnIsFree)
            {
                makeAMove(i_ColIndex);
            }

            return columnIsFree;
        }

        private void makeAMove(int i_ColIndex)
        {
            bool isWin = false;
            int numOfTokensInCol = m_GameBoard.GetNumOfTokensInCol(i_ColIndex);
            int RowIndex = m_GameBoard.NumOfRows - numOfTokensInCol - 1;
            Board.eTileStatus playerName = GetCurrentPlayerName();

            m_GameBoard.SetTileStatus(RowIndex, i_ColIndex, playerName);
            m_AskForAnotherRound = isWin = doWeHaveAWinner(RowIndex, i_ColIndex);
            numOfTokensInCol++;
            m_GameBoard.SetNumOfTokensInCol(i_ColIndex, numOfTokensInCol);
            if (!isWin)
            {
                m_AskForAnotherRound = m_GameBoard.IsBoardFull();
            }
            else
            {
                r_Players[(int)playerName - 1].Score++;
                m_Winner = playerName;
            }

            m_TurnsCounter++;
        }

        public void InitializeBoard(int i_Rows, int i_Cols)
        {
            m_GameBoard = new Board(i_Rows, i_Cols);
        }

        public void StartNewRound()
        {
            m_GameBoard = new Board(m_GameBoard.NumOfRows, m_GameBoard.NumOfCols);
            m_EndOfGame = false;
            m_AskForAnotherRound = false;
            m_Winner = null;
            m_TurnsCounter = 0;
        }

        public void QuitRequest()
        {
            m_AskForAnotherRound = true;
        }

        public bool AskForAnotherRound
        {
            get { return m_AskForAnotherRound; }
            set { m_AskForAnotherRound = value; }
        }

        public bool CheckIfValidDimentions(int i_NumRows, int i_NumCols)
        {
            return i_NumRows >= k_MinDimention && i_NumCols <= k_MaxDimention &&
               i_NumRows >= k_MinDimention && i_NumRows <= k_MaxDimention;
        }

        public bool CheckIfValidColNum(int i_ColNum)
        {
            return i_ColNum >= 1 && i_ColNum <= m_GameBoard.NumOfCols;
        }

        public bool EndOfGame
        {
            get { return m_EndOfGame; }
            set { m_EndOfGame = value; }   
        }

        private bool doWeHaveAWinner(int i_LastMoveRow, int i_LastMoveColumn)
        {
            int checkingRow = i_LastMoveRow;
            int checkingColumn = i_LastMoveColumn;
            int numOfTilesToSkip = k_HowManyInARowToWin - 1;
            const int k_NegOffset = -1, k_PosOffset = 1, k_NeutOffset = 0;
            bool winnerFound = false;

            // vertical
            checkingRow += numOfTilesToSkip;
            winnerFound = lookForWinningSequence(checkingRow, checkingColumn, k_NegOffset, k_NeutOffset);

            // horizontal
            if(!winnerFound)
            {
                checkingRow = i_LastMoveRow;
                checkingColumn = i_LastMoveColumn - numOfTilesToSkip;
                winnerFound = lookForWinningSequence(checkingRow, checkingColumn, k_NeutOffset, k_PosOffset);
            }

            // left to right diagonal
            if(!winnerFound)
            {
                checkingColumn = i_LastMoveColumn - numOfTilesToSkip;
                checkingRow = i_LastMoveRow + numOfTilesToSkip;
                winnerFound = lookForWinningSequence(checkingRow, checkingColumn, k_NegOffset, k_PosOffset);
            }

            // right to left diagonal
            if(!winnerFound)
            {
                checkingColumn = i_LastMoveColumn + numOfTilesToSkip;
                checkingRow = i_LastMoveRow + numOfTilesToSkip;
                winnerFound = lookForWinningSequence(checkingRow, checkingColumn, k_NegOffset, k_NegOffset);
            }

            return winnerFound;
        }

        private bool lookForWinningSequence(int i_CheckingRow, int i_CheckingColumn, int i_RowOffset, int i_ColOffset)
        {
            bool sequenceFound = false;
            int checkRange = (k_HowManyInARowToWin * 2) - 1;
            int playerTokensCounter = 0;
            Board.eTileStatus playerName = GetCurrentPlayerName();

            for (int i = 0; (i < checkRange) && (!sequenceFound); i++)
            {
                if(m_GameBoard.IsThePlayerTokenOnTile(i_CheckingRow, i_CheckingColumn, playerName))
                {
                    playerTokensCounter++;
                    sequenceFound = playerTokensCounter == k_HowManyInARowToWin;
                }
                else
                {
                    playerTokensCounter = 0;
                }

                i_CheckingRow += i_RowOffset;
                i_CheckingColumn += i_ColOffset;
            }

            return sequenceFound;
        }
    
        public void setPlayersTypes()
        {
            switch(m_Mode)
            {
                case eGameMode.TwoPlayers: r_Players[k_SecondPlayer].Type = Player.ePlayerType.Human;
                    break;
                case eGameMode.Solo: r_Players[k_SecondPlayer].Type = Player.ePlayerType.Computer;
                    break;
            }
        }

        public Board.eTileStatus? Winner
        {
             get { return m_Winner; }
        }

        public int[] GetPlayersScore()
        {
            int[] playersScore = new int[k_NumOfPlayers];

            playersScore[k_FirstPlayer] = r_Players[k_FirstPlayer].Score;
            playersScore[k_SecondPlayer] = r_Players[k_SecondPlayer].Score;

            return playersScore;
        }

        public Board.eTileStatus GetCurrentPlayerName()
        {
            return r_Players[m_TurnsCounter % k_NumOfPlayers].Name;
        }

        public Player.ePlayerType GetCurrentPlayerType()
        {
            return r_Players[m_TurnsCounter % k_NumOfPlayers].Type;
        }

        public bool IsGameBoardFull()
        {
            return m_GameBoard.IsBoardFull();
        }
    }
}
