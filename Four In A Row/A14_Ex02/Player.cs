using System;
using System.Collections.Generic;
using System.Text;

namespace A14_Ex02
{
    public class Player
    {
        public enum ePlayerType
        {
            Human,
            Computer
        }

        private int m_Score;
        private ePlayerType m_PlayerType = new ePlayerType();
        private Board.eTileStatus m_PlayerName;
      
        public Player()
        {
        }

        public int Score
        {
            get { return m_Score; }
            set { m_Score = value; }
        }

        public ePlayerType Type
        {
            get { return m_PlayerType; }
            set { m_PlayerType = value; }
        }

        public Board.eTileStatus Name
        {
            get { return m_PlayerName; }
            set { m_PlayerName = value; }    
        }
    }
}