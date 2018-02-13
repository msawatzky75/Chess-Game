using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPieces
{
    class GamePiece
    {
        //don't need location info, gamepieces will be kept in array
        /// <summary>
        /// The team value
        /// </summary>
        int team = 0;
        /// <summary>
        /// determince the piece type, with value of importance value for any Computer Player
        /// </summary>
        int value = 0;
        bool hasMoved = false;
        #region Constructors
        public GamePiece()
        {
            SetEmpty();
        }
        public GamePiece(int team, int value)
        {
            this.team = team;
            this.value = value;
        }
        #endregion
        #region Properties
        public int Team { get => team; set => team = value; }
        public int Value { get => value; set => this.value = value; }
        public bool HasMoved { get => hasMoved; }
        public bool IsEmpty { get => (team == 0 && value == 0); }
        #endregion
        #region Methods
        #region Public
        /// <summary>
        /// Moves this Piece to provided GamePiece
        /// </summary>
        /// <param name="gp"></param>
        public void MoveTo(GamePiece gp)
        {
            //move
            gp.Team = team;
            gp.Value = value;
            gp.hasMoved = true;
            SetEmpty();
        }
        /// <summary>
        /// String representation of this object
        /// </summary>
        /// <returns></returns>
        override public String ToString()
        {
            String[] names = { "Pawn", "Rook", "Bishop", "Knight", "Queen", "King" };
            if (value == 0)
            {
                return "Empty";
            }
            else if (value <= names.Length)
            {
                return String.Format("{0} on team {1}", names[value - 1], team);
            }
            return null;
        }
        #endregion
        #region Private
        /// <summary>
        /// Sets all values to Zero
        /// </summary>
        private void SetEmpty()
        {
            team = 0;
            value = 0;
            hasMoved = false;
        }
        #endregion
        #endregion
    }
}
