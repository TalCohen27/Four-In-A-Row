using System;
using System.Collections.Generic;
using System.Text;

namespace A14_Ex02
{
    public class ConsoleUI
    {
        private const char k_QuitGameKey = 'Q';
        private const char k_Yes = 'y';
        private const char k_No = 'n';
        private const string k_EmptySlot = "|    ";
        private const string k_PlayerASlot = "|  O ";
        private const string k_PlayerBSlot = "|  X ";
        private const string k_LeftWall = "|";
        private const string k_Floor = "=====";
        private const string k_UnitFloor = "=";

        private FourInARow gameInstance = new FourInARow();

        public ConsoleUI()
        {    
        }

        public void StartGame()
        {
            askForDimentions();
            askForGameMode();
            Ex02.ConsoleUtils.Screen.Clear();
            drawGameBoard();

            while(!gameInstance.EndOfGame)
            {
                promptForAMove();
                if(gameInstance.AskForAnotherRound)
                {
                    declareEndGameCaseAndShowScore();
                    handleAnotherRoundRequest();
                }
            }
        }

        private void askForDimentions()
        {
            int numRows = 0, numCols = 0;
            bool goodInput = false, logicalInput = false;

            while(!goodInput || !logicalInput)
            {
                Console.WriteLine("Please enter number of rows for the board: {0}", Environment.NewLine);
                goodInput = int.TryParse(Console.ReadLine(), out numRows);
                Console.WriteLine("And number of columns: {0}", Environment.NewLine);
                goodInput = int.TryParse(Console.ReadLine(), out numCols);
                if(!goodInput)
                {
                    Console.WriteLine("Illegal input! Must be an integer. Try again");
                }
                else
                {
                    logicalInput = gameInstance.CheckIfValidDimentions(numRows, numCols);
                    if(!logicalInput)
                    {
                        Console.WriteLine(
                            "Illegal dimention number. Dimentions must be between {0} and {1}. Try again", 
                            FourInARow.k_MinDimention, 
                            FourInARow.k_MaxDimention);
                    }
                }
            }

           gameInstance.InitializeBoard(numRows, numCols);
        }

        private void askForGameMode()
        {
            FourInARow.eGameMode modeChoice;
            char yesOrNo = new char();
            string modeReqmsg = string.Format("Would you like to play in two players mode? (y/n)");

            Console.WriteLine(modeReqmsg);
            getYesOrNoAnswer(out yesOrNo);
            if(yesOrNo == k_Yes)
            {
                modeChoice = FourInARow.eGameMode.TwoPlayers;
            }
            else
            {
                modeChoice = FourInARow.eGameMode.Solo;
            }

            gameInstance.Mode = modeChoice;
            gameInstance.setPlayersTypes();
        }

        private void drawGameBoard()
        {
            StringBuilder matrix = new StringBuilder();
 
            for(int i = 0; i < gameInstance.GetBoardNumRows(); i++)
            {
                if(i == 0)
                {
                    for(int k = 0; k < gameInstance.GetBoardNumCols(); k++)
                    {
                        matrix.Append(string.Format("  {0}  ", k + 1)); 
                    }

                    matrix.Append(Environment.NewLine);
                }

                for(int j = 0; j < gameInstance.GetBoardNumCols(); j++)
                {
                    switch(gameInstance.GetTileStatus(i, j))
                    {
                        case Board.eTileStatus.Empty: matrix.Append(k_EmptySlot); 
                            break;
                        case Board.eTileStatus.PlayerA: matrix.Append(k_PlayerASlot);
                            break;
                        case Board.eTileStatus.PlayerB: matrix.Append(k_PlayerBSlot);
                            break;
                    }
                }

                matrix.Append(k_LeftWall);
                matrix.Append(Environment.NewLine);
                for(int j = 0; j < gameInstance.GetBoardNumCols(); j++)
                {
                    matrix.Append(k_Floor);
                }

                matrix.Append(k_UnitFloor);
                matrix.Append(Environment.NewLine);
            }

            Console.WriteLine(matrix);
        }

        private void askForColumnNum(out int o_ColNum)
        {
            bool goodInput = false, logicalInput = false;
            char inChar = new char();
            string chosenColNumStr;

            o_ColNum = 0;
            while((!goodInput || logicalInput == false) && !gameInstance.AskForAnotherRound)
            {
                chosenColNumStr = Console.ReadLine();
                goodInput = int.TryParse(chosenColNumStr, out o_ColNum);

                if(goodInput)
                {
                    logicalInput = gameInstance.CheckIfValidColNum(o_ColNum);

                    if (!logicalInput)
                    {
                        Console.WriteLine("Illegal colunm number. Try again");
                    }
                }
                else
                {
                    Console.WriteLine("Illegal input! Must be an integer. Try again");
                }

                if(char.TryParse(chosenColNumStr, out inChar) && !char.IsNumber(inChar))
                {
                    if (inChar == k_QuitGameKey)
                    {
                        gameInstance.QuitRequest();
                    }
                }
            }    
        }
        
        private void handleColumnChoice()
        {
            bool isFreeCol = false;
            int chosenColNum = 0;
           
            Player.ePlayerType playerType = gameInstance.GetCurrentPlayerType();
            while(!isFreeCol && !gameInstance.AskForAnotherRound)
            {
                if(playerType != Player.ePlayerType.Computer)
                {
                    askForColumnNum(out chosenColNum);
                    chosenColNum--;
                }

                if(!gameInstance.AskForAnotherRound)
                {
                    isFreeCol = gameInstance.AttemptAMove(chosenColNum);
                    if(!isFreeCol)
                    {
                        Console.WriteLine("Choose another column, this one is full");
                    }
                }
            }
        }
    
    private void handleAnotherRoundRequest()
    {
        char yesOrNo;

            Console.WriteLine("Would you like to play a new round? (y/n)");
            getYesOrNoAnswer(out yesOrNo);
            if(yesOrNo == k_Yes)
            {
                gameInstance.StartNewRound();
                Ex02.ConsoleUtils.Screen.Clear();
                drawGameBoard();
            }
            else
            {
                gameInstance.EndOfGame = true;
            }
        }

        private void getYesOrNoAnswer(out char o_YesOrNo)
        {
            do
            {
                char.TryParse(Console.ReadLine(), out o_YesOrNo);
                if(o_YesOrNo != k_Yes && o_YesOrNo != k_No)
                {
                    Console.WriteLine("Please enter either y or n");
                }
            }
            while(o_YesOrNo != k_Yes && o_YesOrNo != k_No);
        }

        private void promptForAMove()
        {
            string promptMsg = null;
            Board.eTileStatus playerName = gameInstance.GetCurrentPlayerName();
       
            switch(playerName)
            {
                case Board.eTileStatus.PlayerA: promptMsg = "Player A, please choose a column";
                    break;
                case Board.eTileStatus.PlayerB: promptMsg = "Player B, please choose a column";
                    break;
            }

            Console.WriteLine(promptMsg);
            handleColumnChoice();
            Ex02.ConsoleUtils.Screen.Clear();
            drawGameBoard();
        }

        private void declareEndGameCaseAndShowScore()
        {
            int[] playersScores = gameInstance.GetPlayersScore();
            StringBuilder scoresMsg = new StringBuilder();
            string endOfGameMsg = null;

            if (gameInstance.Winner != null)
            {
                switch (gameInstance.Winner)
                {
                    case Board.eTileStatus.PlayerA: endOfGameMsg = "Player A wins!";
                        break;
                    case Board.eTileStatus.PlayerB: endOfGameMsg = "Player B wins!";
                        break;
                }
            }
            else
            {
                if (gameInstance.IsGameBoardFull())
                {
                    endOfGameMsg = "It's a tie!";
                }
            }

            Console.WriteLine(endOfGameMsg);

            scoresMsg.AppendFormat(
@"Player A score: {0}
Player B score: {1}", 
playersScores[0], 
playersScores[1]);

            Console.WriteLine(scoresMsg);
        }
    }
}