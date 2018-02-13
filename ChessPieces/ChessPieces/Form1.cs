using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace ChessPieces
{
    public partial class Form1 : Form
    {
        public enum Piece
        {
            Empty = 0,
            Pawn = 1,
            Rook = 2,
            Bishop = 3,
            Knight = 4,
            Queen = 5,
            King = 6,
        }
        public enum Team
        {
            None = 0,
            White = 1,
            Black = 2
        }
        int selX = -1, selY = -1;
        GamePiece[,] board;
        Size cellSize = new Size(75, 75);
        Size boardSize = new Size(8, 8);
        Brush[] brushes = { Brushes.White, Brushes.Black, new SolidBrush(Color.FromArgb((int)(255 * .4), Color.Blue)), new SolidBrush(Color.FromArgb((int)(255 * .4), Color.LimeGreen)) };
        ArrayList hints = new ArrayList();
        bool showHints = true;

        #region Initalize
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Setup();
        }
        public void Setup()
        {
            board = new GamePiece[8, 8];
            for (int x = 0; x < boardSize.Width; x++)
                for (int y = 0; y < boardSize.Height; y++)
                    board[x, y] = new GamePiece();

            for (int i = 0; i < boardSize.Width; i++)
            {
                board[i, 1] = new GamePiece((int)Team.Black, (int)Piece.Pawn);
                board[i, 6] = new GamePiece((int)Team.White, (int)Piece.Pawn);
            }
            //black 
            board[0, 0] = new GamePiece((int)Team.Black, (int)Piece.Rook);
            board[1, 0] = new GamePiece((int)Team.Black, (int)Piece.Knight);
            board[2, 0] = new GamePiece((int)Team.Black, (int)Piece.Bishop);
            board[3, 0] = new GamePiece((int)Team.Black, (int)Piece.King);
            board[4, 0] = new GamePiece((int)Team.Black, (int)Piece.Queen);
            board[5, 0] = new GamePiece((int)Team.Black, (int)Piece.Bishop);
            board[6, 0] = new GamePiece((int)Team.Black, (int)Piece.Knight);
            board[7, 0] = new GamePiece((int)Team.Black, (int)Piece.Rook);
            //white
            board[0, 7] = new GamePiece((int)Team.White, (int)Piece.Rook);
            board[1, 7] = new GamePiece((int)Team.White, (int)Piece.Knight);
            board[2, 7] = new GamePiece((int)Team.White, (int)Piece.Bishop);
            board[3, 7] = new GamePiece((int)Team.White, (int)Piece.Queen);
            board[4, 7] = new GamePiece((int)Team.White, (int)Piece.King);
            board[5, 7] = new GamePiece((int)Team.White, (int)Piece.Bishop);
            board[6, 7] = new GamePiece((int)Team.White, (int)Piece.Knight);
            board[7, 7] = new GamePiece((int)Team.White, (int)Piece.Rook);

            ClientSize = new Size(boardSize.Width * cellSize.Width, menuStrip1.Height + statusStrip1.Height + (boardSize.Height * cellSize.Height));
            Text = "Chess";
        }
        #endregion

        #region Paint
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.TranslateTransform(0, menuStrip1.Height);
            DrawBoard(e.Graphics);
            DrawPieces(e.Graphics);
            if (showHints)
                DrawHints(e.Graphics);
        }
        public void DrawBoard(Graphics gr)
        {
            for (int x = 0; x < boardSize.Width; x++)
                for (int y = 0; y < boardSize.Height; y++)
                {
                    Rectangle workingRectangle = new Rectangle(x * cellSize.Width, y * cellSize.Height, cellSize.Width, cellSize.Height);
                    //draw board on the bottom layer
                    if ((x + y) % 2 == 0)
                        gr.FillRectangle(brushes[0], workingRectangle);
                    else
                        gr.FillRectangle(brushes[1], workingRectangle);
                    //draw selected rectangle
                    if ((x == selX) && (y == selY))
                        gr.FillRectangle(brushes[2], workingRectangle);
                    //draw pieces
                }
        }
        public void DrawPieces(Graphics gr)
        {
            for (int x = 0; x < boardSize.Width; x++)
                for (int y = 0; y < boardSize.Height; y++)
                {
                    RectangleF workingRectangle = new RectangleF(x * cellSize.Width, y * cellSize.Height, cellSize.Width, cellSize.Height);
                    if (board[x, y].Value != 0)
                        gr.DrawString(board[x, y].Value.ToString(), new Font(FontFamily.GenericSansSerif, 43f), ((board[x, y].Team == (int)Team.Black) ? Brushes.Red : Brushes.LightGray), workingRectangle);
                }
        }
        public void DrawHints(Graphics gr)
        {
            for (int x = 0; x < boardSize.Width; x++)
                for (int y = 0; y < boardSize.Height; y++)
                {
                    RectangleF workingRectangle = new RectangleF(x * cellSize.Width, y * cellSize.Height, cellSize.Width, cellSize.Height);
                    if (hints.Contains(new Point(x, y)))
                    {
                        gr.FillRectangle(brushes[3], workingRectangle);
                    }

                }
        }
        #endregion

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X / cellSize.Width, y = (e.Y - menuStrip1.Height) / cellSize.Height;

            if (e.Button == MouseButtons.Left)
                //bounds check
                if ((x >= 0 && y >= 0) && (x < boardSize.Width && y < boardSize.Height))
                {
                    //if clicked in the same spot, deselect
                    if (selX == x && selY == y)
                    {
                        Deselect();
                        return;
                    }
                    //move maybe??
                    else if (hints.Contains(new Point(x, y)))
                    {
                        //move
                        board[selX, selY].MoveTo(board[x, y]);
                        Deselect();
                    }
                    else
                    {
                        if (board[x, y].Team == (int)Team.White)
                        {
                            selX = x;
                            selY = y;
                        }

                    }
                    hints = GetHints(selX, selY);
                    Invalidate();
                    WinCheck();
                }
                else if (e.Button == MouseButtons.Right)
                {

                }
        }
        private void WinCheck()
        {
            bool blackKing = false;
            bool whiteKing = false;
            for (int x = 0; x < boardSize.Width; x++)
                for (int y = 0; y < boardSize.Height; y++)
                    if (board[x, y].Value == (int)Piece.King)
                    {
                        if (board[x, y].Team == (int)Team.Black)
                            blackKing = true;
                        if (board[x, y].Team == (int)Team.White)
                            whiteKing = true;
                    }
            if (whiteKing != blackKing)
                MessageBox.Show(String.Format("{0} wins", whiteKing ? "White" : "Black"));
        }
        private void Deselect()
        {
            selX = -1;
            selY = -1;
            hints = new ArrayList();
        }

        #region Hints
        public ArrayList GetHints(int x, int y)
        {
            if (!(x >= 0 && y >= 0 && x < boardSize.Width && y < boardSize.Height))
                return new ArrayList();
            /* 
                for (int i = 0; i < 1; i++)
                    for (int j = 0; j < 2; j++)
                    temp.Add(new Point(x + i, y + j));

             if (!(x + i >= 0 && x + i < boardSize.Width && y - j >= 0 && y - j < boardSize.Height))
                                continue;
            */
            ArrayList temp = new ArrayList();
            //pawn hints
            if (board[x, y].Value == (int)Piece.Pawn)
                PawnHints(x, y, temp);
            if (board[x, y].Value == (int)Piece.Rook)
                RookHints(x, y, temp);
            if (board[x, y].Value == (int)Piece.Bishop)
                BishopHints(x, y, temp);
            if (board[x, y].Value == (int)Piece.Knight)
                KnightHints(x, y, temp);
            return temp;
        }
        /// <summary>
        /// calculates hints for the movements of pawns
        /// </summary>
        /// <param name="x">pawn x coord</param>
        /// <param name="y">pawn y coord</param>
        /// <param name="hints">the list of possible moves</param>
        private void PawnHints(int x, int y, ArrayList hints)
        {
            if (board[x, y].Team == (int)Team.White)
            {
                //i and j are displacement ints
                for (int i = -1; i <= 1; i++)
                    for (int j = 1; j <= 2; j++)
                    {
                        if (!(x + i >= 0 && x + i < boardSize.Width && y - j >= 0 && y - j < boardSize.Height))
                            continue;
                        //if move up is 2, it must be its first move
                        if (j == 2 && i == 0)
                        {
                            if (!board[x, y].HasMoved)
                                hints.Add(new Point(x + i, y - j));
                        }
                        else if (j == 1)
                        {
                            //moving sideways, must take piece
                            if (Math.Abs(i) == 1)
                            {
                                if (board[x + i, y - j].Team != board[x, y].Team && (board[x + i, y - j].Value > 0))
                                    hints.Add(new Point(x + i, y - j));
                            }
                            //not moving sideways
                            else if (i == 0)
                            {
                                if (board[x + i, y - j].IsEmpty)
                                    hints.Add(new Point(x + i, y - j));
                            }
                        }
                    }
            }
            else if (board[x, y].Team == (int)Team.Black)
            {
                //i and j are displacement ints
                for (int i = -1; i <= 1; i++)
                    for (int j = 1; j <= 2; j++)
                    {
                        if (!(x + i >= 0 && x + i < boardSize.Width && y + j >= 0 && y + j < boardSize.Height))
                            continue;
                        //if move up is 2, it must be its first move
                        if (j == 2 && i == 0)
                        {
                            if (!board[x, y].HasMoved)
                                hints.Add(new Point(x + i, y + j));
                        }
                        else if (j == 1)
                        {
                            //moving sideways, must take piece
                            if (Math.Abs(i) == 1)
                            {
                                if (board[x + i, y + j].Team == (int)Team.White)
                                    hints.Add(new Point(x + i, y + j));
                            }
                            //not moving sideways
                            else if (i == 0)
                            {
                                if (board[x + i, y + j].IsEmpty)
                                    hints.Add(new Point(x + i, y + j));
                            }
                        }
                    }
            }//end pawn hints
        }
        private void RookHints(int x, int y, ArrayList hints)
        {
            //up
            for (int i = y - 1; i >= 0; i--)
            {
                if (board[x, i].Value == (int)Piece.Empty)
                    hints.Add(new Point(x, i));
                else if (board[x, i].Team != (int)Team.None)
                {
                    if (board[x, i].Team != board[x, y].Team)
                        hints.Add(new Point(x, i));
                    break;
                }
            }
            //down
            for (int i = y + 1; i < boardSize.Height; i++)
            {
                if (board[x, i].Value == (int)Piece.Empty)
                    hints.Add(new Point(x, i));
                else if (board[x, i].Team != (int)Team.None)
                {
                    if (board[x, i].Team != board[x, y].Team)
                        hints.Add(new Point(x, i));
                    break;
                }
            }
            //right
            for (int i = x + 1; i < boardSize.Width; i++)
            {
                if (board[i, y].Value == (int)Piece.Empty)
                    hints.Add(new Point(i, y));
                else if (board[i, y].Team != (int)Team.None)
                {
                    if (board[i, y].Team != board[x, y].Team)
                        hints.Add(new Point(i, y));
                    break;
                }
            }
            //left
            for (int i = x - 1; i > 0; i--)
            {
                if (board[i, y].Value == (int)Piece.Empty)
                    hints.Add(new Point(i, y));
                else if (board[i, y].Team != (int)Team.None)
                {
                    if (board[i, y].Team != board[x, y].Team)
                        hints.Add(new Point(i, y));
                    break;
                }
            }

            //add casteling??????
        }
        private void BishopHints(int x, int y, ArrayList hints)
        {

        }
        private void KnightHints(int x, int y, ArrayList hints)
        {
            for (int i = -2; i <= 2; i++)
                for (int j = -2; j <= 2; j++)
                    if (Math.Abs(i * j) == 2)
                        if (x + i >= 0 && x + i < boardSize.Width && y + j >= 0 && y + j < boardSize.Height)//bounds check
                            if (board[x, y].Team != board[x + i, y + j].Team)
                                hints.Add(new Point(x + i, y + j));
        }
        #endregion
        //for click and drag (WIP)
        #region Click and Drag
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            int x = e.X / cellSize.Width, y = (e.Y - menuStrip1.Height) / cellSize.Height;
            Invalidate();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            int x = e.X / cellSize.Width, y = (e.Y - menuStrip1.Height) / cellSize.Height;
            if (x >= boardSize.Width || y >= boardSize.Height)
                return;
            if (board[x, y].Team > 0)
            {
                testLabel.Text = board[x, y].ToString();
                coordinatesLabel.Text = String.Format("({0}, {1})", x, y);
            }
            else
            {
                testLabel.Text = "";
                coordinatesLabel.Text = "";
            }
            Invalidate();
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            int x = e.X / cellSize.Width, y = (e.Y - menuStrip1.Height) / cellSize.Height;
            Invalidate();
        }
        #endregion

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dispose();
        }
        private void clearBoardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Setup();
        }
    }
}
