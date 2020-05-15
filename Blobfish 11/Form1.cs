using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Blobfish_11
{
    public partial class Form1 : Form
    {
        PictureBox[,] Falt = new PictureBox[8, 8];
        Position currentPosition;
        int[] firstSquare = { -1, -1 };
        public Form1()
        {
            InitializeComponent();

            int squareSize = 50;
            boardPanel.AutoSize = true;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    PictureBox picBox = new PictureBox();
                    Falt[i, j] = picBox;
                    boardPanel.Controls.Add(picBox);

                    picBox.MinimumSize = new Size(squareSize, squareSize);
                    picBox.MaximumSize = new Size(squareSize, squareSize);
                    picBox.Dock = DockStyle.Fill;
                    picBox.SizeMode = PictureBoxSizeMode.Zoom;
                    picBox.Margin = new Padding(0);
                    picBox.Padding = new Padding(0);
                    picBox.Image = Image.FromFile("null.png");
                    picBox.MouseClick += new MouseEventHandler(squareClick);
                    if ((i + j) % 2 == 0)
                        picBox.BackColor = Color.WhiteSmoke;
                    else
                        picBox.BackColor = Color.SandyBrown;
                }
            }
            Position startingPostition = new Position("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            display(startingPostition);
        }
        private void display(Position pos)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    char piece = pos.board[i, j].piece;
                    string picName = "null.png";
                    if (piece != '\0')
                    {
                        if (piece > 'Z')
                        {
                            picName = "B" + piece + ".png";
                        }
                        else
                        {
                            picName = "W" + piece + ".png";
                        }
                    }

                    Falt[i, j].Image = Image.FromFile(picName);
                }
            }
            string temp = pos.getMovesString();
            textBox1.Text = temp;
            currentPosition = pos;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //TODO: Remove arguments?
            if (fenBox.Text.ToLower() == "test")
            {
                bool successfulTests = runTests();
                if (!successfulTests)
                    fenBox.Text = "Tests failed!";
            }
            else
            {
                Position pos = new Position(fenBox.Text);
                evalBox.Text = "";
                double value = pos.eval();
                evalBox.Text += "Evaluering:\n";
                evalBox.Text += "Pjäser: " + Math.Round(pos.material, 2) + "\n";
                evalBox.Text += "Bönder: " + Math.Round(pos.pawnValues[1] - pos.pawnValues[0], 2) + "\n";
                evalBox.Text += "    Vita: " + Math.Round(pos.pawnValues[1], 2) + "\n";
                evalBox.Text += "    Svarta: " + Math.Round(pos.pawnValues[0], 2) + "\n";
                evalBox.Text += "Totalt: " + Math.Round(value, 2) + "\n";
                display(pos);
            }
        }
        private void squareClick(object sender, MouseEventArgs e)
        {
            int xVal = ((PictureBox)sender).Location.X; //a-h
            int yVal = ((PictureBox)sender).Location.Y; //1-8
            xVal = xVal / (boardPanel.Size.Width / 8);
            yVal = yVal / (boardPanel.Size.Height / 8);
            int[] newSquare = { yVal, xVal };
            if (firstSquare[0] == -1)  //-1 indikerar att ingen ruta tidigare markerats.
            {
                firstSquare = newSquare;
                moveLabel.Text = (char)(xVal + 'a') + (8 - yVal).ToString();
            }
            else
            {
                foreach (Move item in this.currentPosition.allMoves)
                {
                    if (firstSquare[0] == item.from[0] && firstSquare[1] == item.from[1] &&
                        newSquare[0] == item.to[0] && newSquare[1] == item.to[1])
                    {
                        Square[,] newPosition = item.execute(this.currentPosition.board);
                    }
                }
                moveLabel.Text = (char)(firstSquare[1] + 'a') + (8 - firstSquare[0]).ToString() + "-" +
                    (char)(xVal + 'a') + (8 - yVal).ToString();
                firstSquare[0] = -1;
                firstSquare[1] = -1;
            }
        }
        private void fenBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
                button1_Click(null, null);
        }
        private bool runTests()
        {
            string fullResult = "";
            int testCounter = 1;
            bool testNumberOfMoves(string FEN, int moves)
            {
                Position pos = new Position(FEN);
                pos.eval();
                bool success = pos.allMoves.Count == moves;
                if (success) fullResult+= "Test " + testCounter.ToString() + ": Success\n";
                else fullResult += "Test " + testCounter.ToString() + ": Fail\n";
                testCounter++;
                return success;
            }
            testNumberOfMoves("8/8/1b6/8/1k6/8/3rP1K1/8 w - - 0 1", 6); //Spikad vit bonde.
            testNumberOfMoves("kb5q/8/8/8/5R1B/8/r5PK/8 w - - 0 1", 4); //Tre spikar på vit
            testNumberOfMoves("K6Q/2B5/8/8/7r/6n1/R5pk/8 b - - 0 1", 8); //Tre spikar på svart
            testNumberOfMoves("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 20); //Utgångsställningen
            testNumberOfMoves("rnbqkbnr/pp2pppp/8/2ppP3/8/8/PPPP1PPP/RNBQKBNR w KQkq d6 0 3", 31); //En passant

            evalBox.Text = fullResult;
            return true;
        }
    }
    public class Position
    {
        private readonly string FEN;
        public bool whiteToMove;
        private int halfMoveClock = 0;
        private int moveCounter = 0;
        private int[] enPassantSquare = new int[2];
        private bool[] castlingRights = new bool[4]; //KQkq
        public double material = 0; //TODO: Städa upp
        public double[] pawnValues = new double[2];
        public Square[,] board = new Square[8, 8];
        int[,] kingPositions = new int[2, 2]; //Bra att kunna komma åt snabbt. 0=svart, 1=vit
        int[] checkingPieces = { -1, -1, -1, -1 };
        public List<Move> allMoves = new List<Move>();

        public Position(string FEN)
        {
            this.FEN = FEN;
            int column = 0, row = 0;
            string boardString = FEN.Substring(0, FEN.IndexOf(' '));
            foreach (char tkn in boardString)
            {
                switch (tkn)
                {
                    case 'p':
                        board[row, column].piece = tkn;
                        column++;
                        break;
                    case 'P':
                        board[row, column].piece = tkn;
                        column++;
                        break;

                    case 'n':
                        board[row, column].piece = tkn;
                        column++; break;
                    case 'N':
                        board[row, column].piece = tkn;
                        column++; break;

                    case 'b':
                        board[row, column].piece = tkn;
                        column++; break;
                    case 'B':
                        board[row, column].piece = tkn;
                        column++; break;

                    case 'r':
                        board[row, column].piece = tkn;
                        column++; break;
                    case 'R':
                        board[row, column].piece = tkn;
                        column++; break;

                    case 'k':
                        board[row, column].piece = tkn;
                        kingPositions[0, 0] = row;
                        kingPositions[0, 1] = column;
                        column++; break;
                    case 'K':
                        board[row, column].piece = tkn;
                        kingPositions[1, 0] = row;
                        kingPositions[1, 1] = column;
                        column++; break;

                    case 'q':
                        board[row, column].piece = tkn;
                        column++; break;
                    case 'Q':
                        board[row, column].piece = tkn;
                        column++; break;

                    case '/': column = 0; row++; break;
                    case '1': column += 1; break;
                    case '2': column += 2; break;
                    case '3': column += 3; break;
                    case '4': column += 4; break;
                    case '5': column += 5; break;
                    case '6': column += 6; break;
                    case '7': column += 7; break;
                    case '8': break;
                    default:
                        column++;
                        break;
                }
            }
            if (FEN[FEN.IndexOf(' ') + 1] == 'w') this.whiteToMove = true;
            else this.whiteToMove = false;
            int temp = FEN.IndexOf(' ');
            string infoString = FEN.Substring(FEN.IndexOf(' ') + 3, FEN.Length - FEN.IndexOf(' ') - 3);
            temp = infoString.IndexOf(' ');
            string castlingString = infoString.Substring(0, infoString.IndexOf(' ')); //Till exempel: KQkq
            #region castlingRights
            if (castlingString == "-")
            {
                castlingRights = new bool[] { false, false, false, false };
            }
            else
            {
                if (castlingString.Contains('K'))
                {
                    castlingRights[0] = true;
                }
                if (castlingString.Contains('Q'))
                {
                    castlingRights[1] = true;
                }
                if (castlingString.Contains('k'))
                {
                    castlingRights[2] = true;
                }
                if (castlingString.Contains('q'))
                {
                    castlingRights[3] = true;
                }
            }
            #endregion
            string EPString = infoString.Substring(infoString.IndexOf(' ') + 1, infoString.Length - infoString.IndexOf(' ') - 1);
            //Till exempel: c6 0 2
            if (EPString[0] == '-')
            {
                enPassantSquare = new int[] { -1, -1 };
            }
            else
            {
                int EPcolumn = EPString[0] - 'a';
                int EProw = EPString[1] - '1';
                if (EPcolumn < 0 || EPcolumn > 7 || EProw < 0 || EProw > 7)
                {
                    throw new Exception("Felaktig FEN."); //TODO: Hantera
                }
                enPassantSquare[0] = EProw;
                enPassantSquare[1] = EPcolumn; //Eftersom ordingen är omvänd i FEN.
            }
            string lastString = EPString.Substring(EPString.IndexOf(' ') + 1, EPString.Length - EPString.IndexOf(' ') - 1);
            //Till exempel: "1 2".
            string clockString = lastString.Substring(0, lastString.IndexOf(' '));
            //Till exempel: "1".
            this.halfMoveClock = int.Parse(clockString);
            string moveString = lastString.Substring(lastString.IndexOf(' '), lastString.Length - lastString.IndexOf(' '));
            this.moveCounter = int.Parse(moveString);
            //Till exempel: "2".

        }
        public Position(Square[,] board, bool whiteToMove, bool[] castlingRights, int[] enPassantSquare,
            int halfMoveClock, int moveCounter)
        {
            this.board = board;
            this.FEN = "";
            this.whiteToMove = whiteToMove;
            this.castlingRights = castlingRights;
            this.halfMoveClock = halfMoveClock;
            this.moveCounter = moveCounter;
            this.enPassantSquare = enPassantSquare;
        }
        public string getMovesString()
        {
            if (allMoves.Count == 0)
            {
                this.eval();
            }

            string text = "";
            foreach (Move item in allMoves)
            {
                text += item.toString(board) + Environment.NewLine;
            }
            return text;
        }
        public double eval()
        {
            calculateControl();
            List<Move> moves = calculateMoves();
            allMoves = moves;
            int result = decisiveResult(moves); //1=vit vinst, -1=svart vinst, 0=remi, -2=oklart.

            return numericEval();
        }
        private int[] setControl(int row, int column, bool byWhite)
        {
            /*
             * Sätter wControl respektive bControl till true på ett givet fält, beroende på byWhite
             * Returnerar en int[] med storlek 4, med koordinater för den första och andra schackande pjäsen
             * om sådana finns, annars sätts de till -1.
             * 
             * Kastar undantag om det finns fler än 2 schackande pjäser.
             */
            if (byWhite)
            {
                board[row, column].wControl = true;
                if (row == kingPositions[0, 0] && column == kingPositions[0, 1])
                {
                    if (checkingPieces[0] == -1) //Om detta är den första pjäsen som schackar
                    {
                        checkingPieces[0] = row;
                        checkingPieces[1] = column;
                    }
                    else if (checkingPieces[2] == -1) //Om detta är den andra pjäsen som schackar
                    {
                        checkingPieces[2] = row;
                        checkingPieces[3] = column;
                    }
                    else
                    {
                        throw new Exception("Fler än 2 schackande pjäser!");
                    }
                }
            }
            else
            {
                board[row, column].bControl = true;
                if (row == kingPositions[1, 0] && column == kingPositions[1, 1])
                {
                    if (checkingPieces[0] == -1) //Om detta är den första pjäsen som schackar
                    {
                        checkingPieces[0] = row;
                        checkingPieces[1] = column;
                    }
                    else if (checkingPieces[2] == -1) //Om detta är den andra pjäsen som schackar
                    {
                        checkingPieces[2] = row;
                        checkingPieces[3] = column;
                    }
                    else
                    {
                        throw new Exception("Fler än 2 schackande pjäser!");
                    }
                }
            }
            return checkingPieces;
        }
        public double numericEval()
        {
            /* 
             * [row, column]
             * row==0       -> rad 8.
             * row==7       -> rad 1.
             * column==0    -> a-linjen.
             * column==7    -> h-linjen.
             */
            int[] numberOfPawns = new int[2];
            double[] posFactor = { 1f, 1f };
            int[,] pawns = new int[2, 8]; //0=black, 1=white.
            //bool whiteSquare = true; //TODO: ordna med färgkomplex.
            //int column = 0, row = 0;
            double[] pieceValues = { 3, 3, 5, 9 };
            double pieceValue = 0;
            for (int row = 0; row < 8; row++)
            {
                //TODO: Clean up
                for (int column = 0; column < 8; column++)
                {
                    switch (board[row, column].piece)
                    {
                        case 'p':
                            //board[row, column].piece = tkn;
                            numberOfPawns[0]++;
                            pawns[0, column]++;
                            posFactor[0] += Placement.pawn[0, row, column];
                            break;

                        case 'P':
                            //board[row, column].piece = tkn;
                            numberOfPawns[1]++;
                            pawns[1, column]++;
                            posFactor[1] += Placement.pawn[1, row, column];
                            break;

                        case 'n':
                            //board[row, column].piece = tkn;
                            pieceValue -= pieceValues[0] * Placement.knight[row, column];
                            break;
                        case 'N':
                            //board[row, column].piece = tkn;
                            pieceValue += pieceValues[0] * Placement.knight[row, column];
                            break;

                        case 'b':
                            //board[row, column].piece = tkn;
                            pieceValue -= pieceValues[1] * Placement.bishop[row, column];
                            break;
                        case 'B':
                            //board[row, column].piece = tkn;
                            pieceValue += pieceValues[1] * Placement.bishop[row, column];
                            break;

                        case 'r':
                            //board[row, column].piece = tkn;
                            pieceValue -= pieceValues[2] * Placement.rook[row, column];
                            break;
                        case 'R':
                            //board[row, column].piece = tkn;
                            pieceValue += pieceValues[2] * Placement.rook[row, column];
                            break;

                        case 'k':
                            //board[row, column].piece = tkn;
                            kingPositions[0, 0] = row;
                            kingPositions[0, 1] = column;
                            break;
                        case 'K':
                            //board[row, column].piece = tkn;
                            kingPositions[1, 0] = row;
                            kingPositions[1, 1] = column;
                            break;

                        case 'q':
                            //board[row, column].piece = tkn;
                            pieceValue -= pieceValues[3] * Placement.queen[row, column];
                            break;
                        case 'Q':
                            //board[row, column].piece = tkn;
                            pieceValue += pieceValues[3] * Placement.queen[row, column];
                            break;
                        default:
                            break;
                    }
                }
            }

            //calculateControl();
            for (int i = 0; i < 2; i++)
            {
                if (numberOfPawns[i] == 0)
                    posFactor[i] = 0f;
                else
                    posFactor[i] /= numberOfPawns[i];
            }
            double pawnValue = evalPawns(numberOfPawns, posFactor, pawns);
            this.material = pieceValue;
            return pieceValue + pawnValue;
        }
        private int decisiveResult(List<Move> moves)
        {
            /*
            int c = 0;
            if (whiteToMove) c = 1;
            if (whiteToMove)
            {
                for (int i = 0; i < moves.Count; i++)
                {
                    if (moves[i].from[0] == kingPositions[c, 0] && moves[i].from[1] == kingPositions[c, 1]
                    && board[moves[i].to[0], moves[i].to[1]].bControl) //Tar bort olagliga kungsdrag för vit
                    {
                        moves.RemoveAt(i);
                    }
                }
            }
            else //Om det är svarts drag
            {
                for (int i = 0; i < moves.Count; i++)
                {
                    if (moves[i].from[0] == kingPositions[c, 0] && moves[i].from[1] == kingPositions[c, 1]
                    && board[moves[i].to[0], moves[i].to[1]].wControl) //Tar bort olagliga kungsdrag för svart
                    {
                        moves.RemoveAt(i);
                    }
                }
            }
            */ //TODO: Denna verkar värdelös?


            if (checkingPieces[0] != -1) //Schack
            {
                bool doubleCheck = (checkingPieces[2] != -1);
                if (doubleCheck)
                {

                }
                else //Enkelschack
                {
                    //TODO: Fixa!
                }

            }
            else
            {

            }

            if(this.moveCounter > 100)
            {
                return 0; //Femtiodragsregeln.
            }

            return -2;
        }
        private void calculateControl()
        {
            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    switch (board[row, column].piece)
                    {
                        case 'p':
                            #region blackPawnControl
                            if (column > 0)
                            {
                                setControl(row + 1, column - 1, false);
                            }
                            if (column < 7)
                            {
                                setControl(row + 1, column + 1, false);
                            }
                            #endregion
                            break;
                        case 'P':
                            #region whitePawnControl
                            if (column > 0)
                            {
                                setControl(row - 1, column - 1, true);
                            }
                            if (column < 7)
                            {
                                setControl(row - 1, column + 1, true);
                            }
                            #endregion
                            break;
                        case 'k':
                            #region blackKingControl

                            if (column > 0)
                            {
                                setControl(row, column - 1, false);
                                if (row > 0)
                                {
                                    setControl(row - 1, column - 1, false);
                                    setControl(row - 1, column, false);
                                }
                                if (row < 7)
                                {
                                    setControl(row + 1, column - 1, false);
                                    setControl(row + 1, column, false);
                                }
                            }
                            if (column < 7)
                            {
                                setControl(row, column + 1, false);
                                if (row > 0)
                                {
                                    setControl(row - 1, column + 1, false);
                                    setControl(row - 1, column, false);
                                }
                                if (row < 7)
                                {
                                    setControl(row + 1, column + 1, false);
                                    setControl(row + 1, column, false);
                                }
                            }
                            #endregion
                            break;
                        case 'K':
                            #region whiteKingControl
                            if (column > 0)
                            {
                                setControl(row, column - 1, true);
                                if (row > 0)
                                {
                                    setControl(row - 1, column - 1, true);
                                    setControl(row - 1, column, true);
                                }
                                if (row < 7)
                                {
                                    setControl(row + 1, column - 1, true);
                                    setControl(row + 1, column, true);
                                }
                            }
                            if (column < 7)
                            {
                                setControl(row, column + 1, true);
                                if (row > 0)
                                {
                                    setControl(row - 1, column + 1, true);
                                    setControl(row - 1, column, true);
                                }
                                if (row < 7)
                                {
                                    setControl(row + 1, column + 1, true);
                                    setControl(row + 1, column, true);
                                }
                            }
                            #endregion
                            break;
                        case 'n': //TODO: Förbättra!
                            #region blackKnightControl
                            if (row + 2 < 8 && column + 1 < 8)
                                setControl(row + 2, column + 1, false);
                            if (row + 2 < 8 && column - 1 >= 0)
                                setControl(row + 2, column - 1, false);
                            if (row + 1 < 8 && column + 2 < 8)
                                setControl(row + 1, column + 2, false);
                            if (row + 1 < 8 && column - 2 >= 0)
                                setControl(row + 1, column - 2, false);
                            if (row - 1 >= 0 && column + 2 < 8)
                                setControl(row - 1, column + 2, false);
                            if (row - 1 >= 0 && column - 2 >= 0)
                                setControl(row - 1, column - 2, false);
                            if (row - 2 >= 0 && column + 1 < 8)
                                setControl(row - 2, column + 1, false);
                            if (row - 2 >= 0 && column - 1 >= 0)
                                setControl(row - 2, column - 1, false);
                            #endregion
                            break;
                        case 'N': //TODO: Förbättra!
                            #region whiteKnightControl
                            if (row + 2 < 8 && column + 1 < 8)
                                setControl(row + 2, column + 1, true);
                            if (row + 2 < 8 && column - 1 >= 0)
                                setControl(row + 2, column - 1, true);
                            if (row + 1 < 8 && column + 2 < 8)
                                setControl(row + 1, column + 2, true);
                            if (row + 1 < 8 && column - 2 >= 0)
                                setControl(row + 1, column - 2, true);
                            if (row - 1 >= 0 && column + 2 < 8)
                                setControl(row - 1, column + 2, true);
                            if (row - 1 >= 0 && column - 2 >= 0)
                                setControl(row - 1, column - 2, true);
                            if (row - 2 >= 0 && column + 1 < 8)
                                setControl(row - 2, column + 1, true);
                            if (row - 2 >= 0 && column - 1 >= 0)
                                setControl(row - 2, column - 1, true);
                            #endregion
                            break;
                        case 'b':
                            #region blackBishopControl
                            int i = 1;
                            bool done = false;
                            while (row + i < 8 && column + i < 8 && !done)
                            {
                                setControl(row + i, column + i, false);
                                if (board[row + i, column + i].piece != '\0')
                                {
                                    done = true;
                                }
                                i++;
                            }
                            i = 1;
                            done = false;
                            while (row - i > 0 && column + i < 8 && !done)
                            {
                                setControl(row - i, column + i, false);
                                if (board[row - i, column + i].piece != '\0')
                                {
                                    done = true;
                                }
                                i++;
                            }
                            i = 1;
                            done = false;
                            while (row + i < 8 && column - i > 0 && !done)
                            {
                                setControl(row + i, column - i, false);
                                if (board[row + i, column - i].piece != '\0')
                                {
                                    done = true;
                                }
                                i++;
                            }
                            i = 1;
                            done = false;
                            while (row - i > 0 && column - i > 0 && !done)
                            {
                                setControl(row - i, column - i, false);
                                if (board[row - i, column - i].piece != '\0')
                                {
                                    done = true;
                                }
                                i++;
                            }
                            #endregion
                            break;
                        case 'B':
                            #region whiteBishopControl
                            i = 1;
                            done = false;
                            while (row + i < 8 && column + i < 8 && !done)
                            {
                                setControl(row + i, column + i, true);
                                if (board[row + i, column + i].piece != '\0')
                                {
                                    done = true;
                                }
                                i++;
                            }
                            i = 1;
                            done = false;
                            while (row - i > 0 && column + i < 8 && !done)
                            {
                                setControl(row - i, column + i, true);
                                if (board[row - i, column + i].piece != '\0')
                                {
                                    done = true;
                                }
                                i++;
                            }
                            i = 1;
                            done = false;
                            while (row + i < 8 && column - i > 0 && !done)
                            {
                                setControl(row + i, column - i, true);
                                if (board[row + i, column - i].piece != '\0')
                                {
                                    done = true;
                                }
                                i++;
                            }
                            i = 1;
                            done = false;
                            while (row - i > 0 && column - i > 0 && !done)
                            {
                                setControl(row - i, column - i, true);
                                if (board[row - i, column - i].piece != '\0')
                                {
                                    done = true;
                                }
                                i++;
                            }
                            #endregion
                            break;
                        case 'r':
                            #region blackRookControl
                            i = 1;
                            done = false;
                            while (row + i < 8 && !done)
                            {
                                setControl(row + i, column, false);
                                if (board[row + i, column].piece != '\0')
                                {
                                    done = true;
                                }
                                i++;
                            }
                            i = 1;
                            done = false;
                            while (row - i > 0 && !done)
                            {
                                setControl(row - i, column, false);
                                if (board[row - i, column].piece != '\0')
                                {
                                    done = true;
                                }
                                i++;
                            }
                            i = 1;
                            done = false;
                            while (column + i < 8 && !done)
                            {
                                setControl(row, column + i, false);
                                if (board[row, column + i].piece != '\0')
                                {
                                    done = true;
                                }
                                i++;
                            }
                            i = 1;
                            done = false;
                            while (column - i > 0 && !done)
                            {
                                setControl(row, column - i, false);
                                if (board[row, column - i].piece != '\0')
                                {
                                    done = true;
                                }
                                i++;
                            }
                            #endregion
                            break;
                        case 'R':
                            #region whiteRookControl
                            i = 1;
                            done = false;
                            while (row + i < 8 && !done)
                            {
                                setControl(row + i, column, true);
                                if (board[row + i, column].piece != '\0')
                                {
                                    done = true;
                                }
                                i++;
                            }
                            i = 1;
                            done = false;
                            while (row - i > 0 && !done)
                            {
                                setControl(row - i, column, true);
                                if (board[row - i, column].piece != '\0')
                                {
                                    done = true;
                                }
                                i++;
                            }
                            i = 1;
                            done = false;
                            while (column + i < 8 && !done)
                            {
                                setControl(row, column + i, true);
                                if (board[row, column + i].piece != '\0')
                                {
                                    done = true;
                                }
                                i++;
                            }
                            i = 1;
                            done = false;
                            while (column - i > 0 && !done)
                            {
                                setControl(row, column - i, true);
                                if (board[row, column - i].piece != '\0')
                                {
                                    done = true;
                                }
                                i++;
                            }
                            #endregion
                            break;
                        case 'q': //TODO: Förbättra
                            #region blackQueenControl
                            i = 1;
                            done = false;
                            while (row + i < 8 && !done)
                            {
                                setControl(row + i, column, false);
                                if (board[row + i, column].piece != '\0')
                                {
                                    done = true;
                                }
                                i++;
                            }
                            i = 1;
                            done = false;
                            while (row - i > 0 && !done)
                            {
                                setControl(row - i, column, false);
                                if (board[row - i, column].piece != '\0')
                                {
                                    done = true;
                                }
                                i++;
                            }
                            i = 1;
                            done = false;
                            while (column + i < 8 && !done)
                            {
                                setControl(row, column + i, false);
                                if (board[row, column + i].piece != '\0')
                                {
                                    done = true;
                                }
                                i++;
                            }
                            i = 1;
                            done = false;
                            while (column - i > 0 && !done)
                            {
                                setControl(row, column - i, false);
                                if (board[row, column - i].piece != '\0')
                                {
                                    done = true;
                                }
                                i++;
                            }
                            i = 1;
                            done = false;
                            while (row + i < 8 && column + i < 8 && !done)
                            {
                                setControl(row + i, column + i, false);
                                if (board[row + i, column + i].piece != '\0')
                                {
                                    done = true;
                                }
                                i++;
                            }
                            i = 1;
                            done = false;
                            while (row - i > 0 && column + i < 8 && !done)
                            {
                                setControl(row - i, column + i, false);
                                if (board[row - i, column + i].piece != '\0')
                                {
                                    done = true;
                                }
                                i++;
                            }
                            i = 1;
                            done = false;
                            while (row + i < 8 && column - i > 0 && !done)
                            {
                                setControl(row + i, column - i, false);
                                if (board[row + i, column - i].piece != '\0')
                                {
                                    done = true;
                                }
                                i++;
                            }
                            i = 1;
                            done = false;
                            while (row - i > 0 && column - i > 0 && !done)
                            {
                                setControl(row - i, column - i, false);
                                if (board[row - i, column - i].piece != '\0')
                                {
                                    done = true;
                                }
                                i++;
                            }
                            #endregion
                            break;
                        case 'Q': //TODO: Förbättra
                            #region whiteQueenControl
                            i = 1;
                            done = false;
                            while (row + i < 8 && !done)
                            {
                                setControl(row + i, column, true);
                                if (board[row + i, column].piece != '\0')
                                {
                                    done = true;
                                }
                                i++;
                            }
                            i = 1;
                            done = false;
                            while (row - i > 0 && !done)
                            {
                                setControl(row - i, column, true);
                                if (board[row - i, column].piece != '\0')
                                {
                                    done = true;
                                }
                                i++;
                            }
                            i = 1;
                            done = false;
                            while (column + i < 8 && !done)
                            {
                                setControl(row, column + i, true);
                                if (board[row, column + i].piece != '\0')
                                {
                                    done = true;
                                }
                                i++;
                            }
                            i = 1;
                            done = false;
                            while (column - i > 0 && !done)
                            {
                                setControl(row, column - i, true);
                                if (board[row, column - i].piece != '\0')
                                {
                                    done = true;
                                }
                                i++;
                            }
                            i = 1;
                            done = false;
                            while (row + i < 8 && column + i < 8 && !done)
                            {
                                setControl(row + i, column + i, true);
                                if (board[row + i, column + i].piece != '\0')
                                {
                                    done = true;
                                }
                                i++;
                            }
                            i = 1;
                            done = false;
                            while (row - i > 0 && column + i < 8 && !done)
                            {
                                setControl(row - i, column + i, true);
                                if (board[row - i, column + i].piece != '\0')
                                {
                                    done = true;
                                }
                                i++;
                            }
                            i = 1;
                            done = false;
                            while (row + i < 8 && column - i > 0 && !done)
                            {
                                setControl(row + i, column - i, true);
                                if (board[row + i, column - i].piece != '\0')
                                {
                                    done = true;
                                }
                                i++;
                            }
                            i = 1;
                            done = false;
                            while (row - i > 0 && column - i > 0 && !done)
                            {
                                setControl(row - i, column - i, true);
                                if (board[row - i, column - i].piece != '\0')
                                {
                                    done = true;
                                }
                                i++;
                            }
                            #endregion
                            break;
                        default: break;
                    }
                }
            }
        }
        private List<Move> calculateMoves()
        {
            //TODO: Fixa olagliga kungsdrag när fältet bakom kungen inte räknas som kontrollerat.
            List<Move> moves = new List<Move>();
            List<Move> newMoves = new List<Move>();
            if (whiteToMove)
            {
                for (int row = 0; row < 8; row++)
                {
                    for (int column = 0; column < 8; column++)
                    {
                        List<int[]> legalSquares = squaresIfPinned(row, column, true);
                        switch (board[row, column].piece)
                        {
                            case 'P':
                                #region whitePawnMoves
                                if (column > 0)
                                {
                                    if (board[row - 1, column - 1].piece > 'Z'
                                        || (enPassantSquare[0] == row + 1 && enPassantSquare[1] == column - 1)) //Svart pjäs eller passant
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row - 1, column - 1 }));
                                    }
                                }
                                if (column < 7)
                                {
                                    if (board[row - 1, column + 1].piece > 'Z'
                                        || (enPassantSquare[0] == row + 1 && enPassantSquare[1] == column + 1)) //Svart pjäs eller passant
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row - 1, column + 1 }));
                                    }
                                }
                                if (board[row - 1, column].piece == '\0') //Tomt fält
                                {
                                    newMoves.Add(new Move(new int[] { row, column }, new int[] { row - 1, column }));
                                    if (row == 6 && board[row - 2, column].piece == '\0')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row - 2, column }));
                                    }
                                }
                                #endregion
                                break;
                            case 'K':
                                #region whiteKingMoves
                                int r = row, c = column - 1;
                                if (c >= 0 && !board[r, c].bControl && accessableFor(true, board[r, c].piece))
                                {
                                    newMoves.Add(new Move(new int[] { row, column }, new int[] { r, c }));
                                }
                                c = column + 1;
                                if (c < 8 && !board[r, c].bControl && accessableFor(true, board[r, c].piece))
                                {
                                    newMoves.Add(new Move(new int[] { row, column }, new int[] { r, c }));
                                }

                                r = row + 1; c = column - 1;
                                if (r < 8 && c >= 0 && !board[r, c].bControl && accessableFor(true, board[r, c].piece))
                                {
                                    newMoves.Add(new Move(new int[] { row, column }, new int[] { r, c }));
                                }
                                c = column + 1;
                                if (r < 8 && c < 8 && !board[r, c].bControl && accessableFor(true, board[r, c].piece))
                                {
                                    newMoves.Add(new Move(new int[] { row, column }, new int[] { r, c }));
                                }
                                c = column;
                                if (r < 8 && !board[r, c].bControl && accessableFor(true, board[r, c].piece))
                                {
                                    newMoves.Add(new Move(new int[] { row, column }, new int[] { r, c }));
                                }

                                r = row - 1; c = column - 1;
                                if (r >= 0 && c >= 0 && !board[r, c].bControl && accessableFor(true, board[r, c].piece))
                                {
                                    newMoves.Add(new Move(new int[] { row, column }, new int[] { r, c }));
                                }
                                c = column + 1;
                                if (r >= 0 && c < 8 && !board[r, c].bControl && accessableFor(true, board[r, c].piece))
                                {
                                    newMoves.Add(new Move(new int[] { row, column }, new int[] { r, c }));
                                }
                                c = column;
                                if (r >= 0 && !board[r, c].bControl && accessableFor(true, board[r, c].piece))
                                {
                                    newMoves.Add(new Move(new int[] { row, column }, new int[] { r, c }));
                                }

                                if (column == 4 && row == 7)
                                {
                                    //Kort rockad
                                    if (castlingRights[0] && board[7, 5].piece == '\0' && board[7, 6].piece == '\0'
                                        && !board[7, 5].bControl && !board[7, 6].bControl)
                                    {
                                        newMoves.Add(new Castle(new int[] { column, row }, new int[] { 7, 6 },
                                            new int[] { 7, 7 }, new int[] { 7, 5 }));
                                    }

                                    //Lång rockad
                                    if (castlingRights[1] && board[7, 3].piece == '\0' && board[7, 2].piece == '\0'
                                        && board[7, 1].piece == '\0' && !board[7, 3].bControl && !board[7, 2].bControl)
                                    {
                                        newMoves.Add(new Castle(new int[] { column, row }, new int[] { 7, 2 },
                                            new int[] { 7, 0 }, new int[] { 7, 3 }));
                                    }
                                }

                                #endregion
                                break;
                            case 'N':
                                #region whiteKnightMoves
                                r = row + 2; c = column + 1;
                                if (r < 8 && c < 8 && (board[r, c].piece > 'Z' || board[r, c].piece == '\0'))
                                    newMoves.Add(new Move(new int[] { row, column }, new int[] { r, c }));

                                c = column - 1;
                                if (r < 8 && c >= 0 && (board[r, c].piece > 'Z' || board[r, c].piece == '\0'))
                                    newMoves.Add(new Move(new int[] { row, column }, new int[] { r, c }));

                                r = row + 1; c = column + 2;
                                if (r < 8 && c < 8 && (board[r, c].piece > 'Z' || board[r, c].piece == '\0'))
                                    newMoves.Add(new Move(new int[] { row, column }, new int[] { r, c }));

                                c = column - 2;
                                if (r < 8 && c >= 0 && (board[r, c].piece > 'Z' || board[r, c].piece == '\0'))
                                    newMoves.Add(new Move(new int[] { row, column }, new int[] { r, c }));

                                r = row - 1; c = column + 2;
                                if (r >= 0 && c < 8 && (board[r, c].piece > 'Z' || board[r, c].piece == '\0'))
                                    newMoves.Add(new Move(new int[] { row, column }, new int[] { r, c }));

                                c = column - 2;
                                if (r >= 0 && c >= 0 && (board[r, c].piece > 'Z' || board[r, c].piece == '\0'))
                                    newMoves.Add(new Move(new int[] { row, column }, new int[] { r, c }));

                                r = row - 2; c = column + 1;
                                if (r >= 0 && c < 8 && (board[r, c].piece > 'Z' || board[r, c].piece == '\0'))
                                    newMoves.Add(new Move(new int[] { row, column }, new int[] { r, c }));

                                c = column - 1;
                                if (r >= 0 && c >= 0 && (board[r, c].piece > 'Z' || board[r, c].piece == '\0'))
                                    newMoves.Add(new Move(new int[] { row, column }, new int[] { r, c }));
                                #endregion
                                break;
                            case 'B':
                                #region whiteBishopMoves
                                int i = 1;
                                while (validSquare(row + i, column + i))
                                {
                                    char pjas = board[row + i, column + i].piece;
                                    if (pjas == '\0')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row + i, column + i }));
                                    }
                                    else if (pjas > 'Z')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row + i, column + i }));
                                        break;
                                    }
                                    else break;
                                    i++;
                                }
                                i = 1;
                                while (validSquare(row - i, column + i))
                                {
                                    char pjas = board[row - i, column + i].piece;
                                    if (pjas == '\0')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row - i, column + i }));
                                    }
                                    else if (pjas > 'Z')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row - i, column + i }));
                                        break;
                                    }
                                    else break;
                                    i++;
                                }
                                i = 1;
                                while (validSquare(row + i, column - i))
                                {
                                    char pjas = board[row + i, column - i].piece;
                                    if (pjas == '\0')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row + i, column - i }));
                                    }
                                    else if (pjas > 'Z')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row + i, column - i }));
                                        break;
                                    }
                                    else break;
                                    i++;
                                }
                                i = 1;
                                while (validSquare(row - i, column - i))
                                {
                                    char pjas = board[row - i, column - i].piece;
                                    if (pjas == '\0')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row - i, column - i }));
                                    }
                                    else if (pjas > 'Z')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row - i, column - i }));
                                        break;
                                    }
                                    else break;
                                    i++;
                                }
                                #endregion
                                break;
                            case 'R':
                                #region whiteRookMoves
                                i = 1;
                                while (row + i < 8)
                                {
                                    char pjas = board[row + i, column].piece;
                                    if (pjas == '\0')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row + i, column }));
                                        i++;
                                    }
                                    else if (pjas > 'Z')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row + i, column }));
                                        break;
                                    }
                                    else break;
                                }
                                i = 1;
                                while (row - i >= 0)
                                {
                                    char pjas = board[row - i, column].piece;
                                    if (pjas == '\0')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row - i, column }));
                                        i++;
                                    }
                                    else if (pjas > 'Z')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row - i, column }));
                                        break;
                                    }
                                    else break;
                                }
                                i = 1;
                                while (column + i < 8)
                                {
                                    char pjas = board[row, column + i].piece;
                                    if (pjas == '\0')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row, column + i }));
                                        i++;
                                    }
                                    else if (pjas > 'Z')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row, column + i }));
                                        break;
                                    }
                                    else break;
                                }
                                i = 1;
                                while (column - i >= 0)
                                {
                                    char pjas = board[row, column - i].piece;
                                    if (pjas == '\0')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row, column - i }));
                                        i++;
                                    }
                                    else if (pjas > 'Z')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row, column - i }));
                                        break;
                                    }
                                    else break;
                                }
                                #endregion
                                break;
                            case 'Q':
                                #region whiteBishopMoves
                                i = 1;
                                while (validSquare(row + i, column + i))
                                {
                                    char pjas = board[row + i, column + i].piece;
                                    if (pjas == '\0')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row + i, column + i }));
                                    }
                                    else if (pjas > 'Z')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row + i, column + i }));
                                        break;
                                    }
                                    else break;
                                    i++;
                                }
                                i = 1;
                                while (validSquare(row - i, column + i))
                                {
                                    char pjas = board[row - i, column + i].piece;
                                    if (pjas == '\0')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row - i, column + i }));
                                    }
                                    else if (pjas > 'Z')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row - i, column + i }));
                                        break;
                                    }
                                    else break;
                                    i++;
                                }
                                i = 1;
                                while (validSquare(row + i, column - i))
                                {
                                    char pjas = board[row + i, column - i].piece;
                                    if (pjas == '\0')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row + i, column - i }));
                                    }
                                    else if (pjas > 'Z')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row + i, column - i }));
                                        break;
                                    }
                                    else break;
                                    i++;
                                }
                                i = 1;
                                while (validSquare(row - i, column - i))
                                {
                                    char pjas = board[row - i, column - i].piece;
                                    if (pjas == '\0')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row - i, column - i }));
                                    }
                                    else if (pjas > 'Z')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row - i, column - i }));
                                        break;
                                    }
                                    else break;
                                    i++;
                                }
                                #endregion
                                #region whiteRookMoves
                                i = 1;
                                bool done = false;
                                while (row + i < 8 && !done)
                                {
                                    char pjas = board[row + i, column].piece;
                                    if (pjas == '\0')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row + i, column }));
                                        i++;
                                    }
                                    else if (pjas > 'Z')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row + i, column }));
                                        break;
                                    }
                                    else break;
                                }
                                i = 1;
                                done = false;
                                while (row - i >= 0 && !done)
                                {
                                    char pjas = board[row - i, column].piece;
                                    if (pjas == '\0')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row - i, column }));
                                        i++;
                                    }
                                    else if (pjas > 'Z')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row - i, column }));
                                        break;
                                    }
                                    else break;
                                }
                                i = 1;
                                done = false;
                                while (column + i < 8 && !done)
                                {
                                    char pjas = board[row, column + i].piece;
                                    if (pjas == '\0')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row, column + i }));
                                        i++;
                                    }
                                    else if (pjas > 'Z')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row, column + i }));
                                        break;
                                    }
                                    else break;
                                }
                                i = 1;
                                done = false;
                                while (column - i >= 0 && !done)
                                {
                                    char pjas = board[row, column - i].piece;
                                    if (pjas == '\0')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row, column - i }));
                                        i++;
                                    }
                                    else if (pjas > 'Z')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row, column - i }));
                                        break;
                                    }
                                    else break;
                                }
                                #endregion
                                break;
                            default: break;
                        }
                        removeIllegalPinMoves(legalSquares, newMoves);
                        moves.AddRange(newMoves);
                        newMoves.Clear();
                    }
                }
            }
            else
            {
                for (int row = 0; row < 8; row++)
                {
                    for (int column = 0; column < 8; column++)
                    {
                        List<int[]> legalSquares = squaresIfPinned(row, column, false);
                        
                        switch (board[row, column].piece)
                        {
                            case 'p':
                                #region blackPawnMoves
                                if (column > 0)
                                {
                                    char pjas = board[row + 1, column - 1].piece;
                                    if ((pjas != '\0' && pjas < 'Z')
                                        || (enPassantSquare[0] == row + 1 && enPassantSquare[1] == column - 1)) //Vit pjäs  eller passant
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row + 1, column - 1 }));
                                    }
                                }
                                if (column < 7)
                                {
                                    char pjas = board[row + 1, column + 1].piece;
                                    if ((pjas != '\0' && pjas < 'Z')
                                        || (enPassantSquare[0] == row + 1 && enPassantSquare[1] == column + 1)) //Vit pjäs eller passant
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row + 1, column + 1 }));
                                    }
                                }
                                if (board[row + 1, column].piece == '\0') //Tomt fält
                                {
                                    newMoves.Add(new Move(new int[] { row, column }, new int[] { row + 1, column }));
                                    if (row == 1 && board[row + 2, column].piece == '\0')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row + 2, column }));
                                    }
                                }
                                #endregion
                                break;
                            case 'k':
                                #region blackKingMoves
                                int r = row, c = column - 1;
                                if (c >= 0 && !board[r, c].wControl && accessableFor(false, board[r, c].piece))
                                {
                                    newMoves.Add(new Move(new int[] { row, column }, new int[] { r, c }));
                                }
                                c = column + 1;
                                if (c < 8 && !board[r, c].wControl && accessableFor(false, board[r, c].piece))
                                {
                                    newMoves.Add(new Move(new int[] { row, column }, new int[] { r, c }));
                                }

                                r = row + 1; c = column - 1;
                                if (r < 8 && c >= 0 && !board[r, c].wControl && accessableFor(false, board[r, c].piece))
                                {
                                    newMoves.Add(new Move(new int[] { row, column }, new int[] { r, c }));
                                }
                                c = column + 1;
                                if (r < 8 && c < 8 && !board[r, c].wControl && accessableFor(false, board[r, c].piece))
                                {
                                    newMoves.Add(new Move(new int[] { row, column }, new int[] { r, c }));
                                }
                                c = column;
                                if (r < 8 && !board[r, c].wControl && accessableFor(false, board[r, c].piece))
                                {
                                    newMoves.Add(new Move(new int[] { row, column }, new int[] { r, c }));
                                }

                                r = row - 1; c = column - 1;
                                if (r >= 0 && c >= 0 && !board[r, c].wControl && accessableFor(false, board[r, c].piece))
                                {
                                    newMoves.Add(new Move(new int[] { row, column }, new int[] { r, c }));
                                }
                                c = column + 1;
                                if (r >= 0 && c < 8 && !board[r, c].wControl && accessableFor(false, board[r, c].piece))
                                {
                                    newMoves.Add(new Move(new int[] { row, column }, new int[] { r, c }));
                                }
                                c = column;
                                if (r >= 0 && !board[r, c].wControl && accessableFor(false, board[r, c].piece))
                                {
                                    newMoves.Add(new Move(new int[] { row, column }, new int[] { r, c }));
                                }

                                if (column == 4 && row == 0)
                                {
                                    //Kort rockad
                                    if (castlingRights[2] && board[0, 5].piece == '\0' && board[0, 6].piece == '\0'
                                        && !board[0, 5].wControl && !board[0, 6].wControl)
                                    {
                                        newMoves.Add(new Castle(new int[] { column, row }, new int[] { 0, 6 },
                                            new int[] { 0, 7 }, new int[] { 0, 5 }));
                                    }

                                    //Lång rockad
                                    if (castlingRights[3] && board[0, 3].piece == '\0' && board[0, 2].piece == '\0'
                                        && board[0, 1].piece == '\0' && !board[0, 3].wControl && !board[0, 2].wControl)
                                    {
                                        newMoves.Add(new Castle(new int[] { column, row }, new int[] { 0, 2 },
                                            new int[] { 0, 0 }, new int[] { 0, 3 }));
                                    }
                                }

                                #endregion
                                break;
                            case 'n':
                                #region blackKnightMoves
                                r = row + 2; c = column + 1;
                                if (r < 8 && c < 8 && board[r, c].piece < 'Z')
                                    newMoves.Add(new Move(new int[] { row, column }, new int[] { r, c }));

                                c = column - 1;
                                if (r < 8 && c >= 0 && board[r, c].piece < 'Z')
                                    newMoves.Add(new Move(new int[] { row, column }, new int[] { r, c }));

                                r = row + 1; c = column + 2;
                                if (r < 8 && c < 8 && board[r, c].piece < 'Z')
                                    newMoves.Add(new Move(new int[] { row, column }, new int[] { r, c }));

                                c = column - 2;
                                if (r < 8 && c >= 0 && board[r, c].piece < 'Z')
                                    newMoves.Add(new Move(new int[] { row, column }, new int[] { r, c }));

                                r = row - 1; c = column + 2;
                                if (r >= 0 && c < 8 && board[r, c].piece < 'Z')
                                    newMoves.Add(new Move(new int[] { row, column }, new int[] { r, c }));

                                c = column - 2;
                                if (r >= 0 && c >= 0 && board[r, c].piece < 'Z')
                                    newMoves.Add(new Move(new int[] { row, column }, new int[] { r, c }));

                                r = row - 2; c = column + 1;
                                if (r >= 0 && c < 8 && board[r, c].piece < 'Z')
                                    newMoves.Add(new Move(new int[] { row, column }, new int[] { r, c }));

                                c = column - 1;
                                if (r >= 0 && c >= 0 && board[r, c].piece < 'Z')
                                    newMoves.Add(new Move(new int[] { row, column }, new int[] { r, c }));
                                #endregion
                                break;
                            case 'b':
                                #region blackBishopMoves
                                int i = 1;
                                bool done = false;
                                while (validSquare(row + i, column + i) && !done)
                                {
                                    char pjas = board[row + i, column + i].piece;
                                    if (pjas == '\0')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row + i, column + i }));
                                    }
                                    else if (pjas < 'Z')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row + i, column + i }));
                                        break;
                                    }
                                    else break;
                                    i++;
                                }
                                i = 1;
                                done = false;
                                while (validSquare(row - i, column + i) && !done)
                                {
                                    char pjas = board[row - i, column + i].piece;
                                    if (pjas == '\0')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row - i, column + i }));
                                    }
                                    else if (pjas < 'Z')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row - i, column + i }));
                                        break;
                                    }
                                    else break;
                                    i++;
                                }
                                i = 1;
                                done = false;
                                while (validSquare(row + i, column - i) && !done)
                                {
                                    char pjas = board[row + i, column - i].piece;
                                    if (pjas == '\0')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row + i, column - i }));
                                    }
                                    else if (pjas < 'Z')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row + i, column - i }));
                                        break;
                                    }
                                    else break;
                                    i++;
                                }
                                i = 1;
                                done = false;
                                while (validSquare(row - i, column - i) && !done)
                                {
                                    char pjas = board[row - i, column - i].piece;
                                    if (pjas == '\0')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row - i, column - i }));
                                    }
                                    else if (pjas < 'Z')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row - i, column - i }));
                                        break;
                                    }
                                    else break;
                                    i++;
                                }
                                #endregion
                                break;
                            case 'r':
                                #region blackRookMoves
                                i = 1;
                                while (row + i < 8)
                                {
                                    char pjas = board[row + i, column].piece;
                                    if (pjas == '\0')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row + i, column }));
                                        i++;
                                    }
                                    else if (pjas < 'Z')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row + i, column }));
                                        break;
                                    }
                                    else break;
                                }
                                i = 1;
                                while (row - i >= 0)
                                {
                                    char pjas = board[row - i, column].piece;
                                    if (pjas == '\0')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row - i, column }));
                                        i++;
                                    }
                                    else if (pjas < 'Z')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row - i, column }));
                                        break;
                                    }
                                    else break;
                                }
                                i = 1;
                                while (column + i < 8)
                                {
                                    char pjas = board[row, column + i].piece;
                                    if (pjas == '\0')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row, column + i }));
                                        i++;
                                    }
                                    else if (pjas < 'Z')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row, column + i }));
                                        break;
                                    }
                                    else break;
                                }
                                i = 1;
                                while (column - i >= 0)
                                {
                                    char pjas = board[row, column - i].piece;
                                    if (pjas == '\0')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row, column - i }));
                                        i++;
                                    }
                                    else if (pjas < 'Z')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row, column - i }));
                                        break;
                                    }
                                    else break;
                                }
                                #endregion
                                break;
                            case 'q':
                                #region blackBishopMoves
                                i = 1;
                                done = false;
                                while (validSquare(row + i, column + i) && !done)
                                {
                                    char pjas = board[row + i, column + i].piece;
                                    if (pjas == '\0')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row + i, column + i }));
                                    }
                                    else if (pjas < 'Z')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row + i, column + i }));
                                        break;
                                    }
                                    else break;
                                    i++;
                                }
                                i = 1;
                                done = false;
                                while (validSquare(row - i, column + i) && !done)
                                {
                                    char pjas = board[row - i, column + i].piece;
                                    if (pjas == '\0')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row - i, column + i }));
                                    }
                                    else if (pjas < 'Z')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row - i, column + i }));
                                        break;
                                    }
                                    else break;
                                    i++;
                                }
                                i = 1;
                                done = false;
                                while (validSquare(row + i, column - i) && !done)
                                {
                                    char pjas = board[row + i, column - i].piece;
                                    if (pjas == '\0')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row + i, column - i }));
                                    }
                                    else if (pjas < 'Z')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row + i, column - i }));
                                        break;
                                    }
                                    else break;
                                    i++;
                                }
                                i = 1;
                                done = false;
                                while (validSquare(row - i, column - i) && !done)
                                {
                                    char pjas = board[row - i, column - i].piece;
                                    if (pjas == '\0')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row - i, column - i }));
                                    }
                                    else if (pjas < 'Z')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row - i, column - i }));
                                        break;
                                    }
                                    else break;
                                    i++;
                                }
                                #endregion
                                #region blackRookMoves
                                i = 1;
                                done = false;
                                while (row + i < 8 && !done)
                                {
                                    char pjas = board[row + i, column].piece;
                                    if (pjas == '\0')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row + i, column }));
                                        i++;
                                    }
                                    else if (pjas < 'Z')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row + i, column }));
                                        break;
                                    }
                                    else break;
                                }
                                i = 1;
                                done = false;
                                while (row - i >= 0 && !done)
                                {
                                    char pjas = board[row - i, column].piece;
                                    if (pjas == '\0')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row - i, column }));
                                        i++;
                                    }
                                    else if (pjas < 'Z')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row - i, column }));
                                        break;
                                    }
                                    else break;
                                }
                                i = 1;
                                done = false;
                                while (column + i < 8 && !done)
                                {
                                    char pjas = board[row, column + i].piece;
                                    if (pjas == '\0')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row, column + i }));
                                        i++;
                                    }
                                    else if (pjas < 'Z')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row, column + i }));
                                        break;
                                    }
                                    else break;
                                }
                                i = 1;
                                done = false;
                                while (column - i >= 0 && !done)
                                {
                                    char pjas = board[row, column - i].piece;
                                    if (pjas == '\0')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row, column - i }));
                                        i++;
                                    }
                                    else if (pjas < 'Z')
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row, column - i }));
                                        break;
                                    }
                                    else break;
                                }
                                #endregion
                                break;
                            default: break;
                        }
                        removeIllegalPinMoves(legalSquares, newMoves);
                        moves.AddRange(newMoves);
                        newMoves.Clear();
                    }
                }
            }
            return moves;
        }
        private List<int[]> squaresIfPinned(int row, int column, bool whiteToMove)
        {
            //Funktion som räknar ut huruvida en pjäs på ett givet fält (row, column) är spikad.
            //Om så är fallet så returneras en lista med de fält pjäsen kan gå till, annars null.
            char thisPiece = board[row, column].piece;
            if (thisPiece == '\0') return null;
            if (thisPiece == 'k') return null;
            if (thisPiece == 'K') return null;
            //TODO: Pjäser av motsatt färg behöver inte kollas.

            List<int[]> legalSquares = new List<int[]>();
            int dRow, dCol; //Skillnaden i x/y-led
            if (whiteToMove)
            {
                dRow = row - this.kingPositions[1, 0];
                dCol = column - this.kingPositions[1, 1];
            }
            else
            {
                dRow = row - this.kingPositions[0, 0];
                dCol = column - this.kingPositions[0, 1];
            }
            if (dCol == 0) //Samma linje
            {
                string piecesToLookFor = "rq";
                if (!whiteToMove) piecesToLookFor = "RQ";
                int sign = Math.Sign(dRow);
                for (int j = 1; j < Math.Abs(dRow); j++)
                {
                    int newRow = row - (j * sign);
                    if (board[newRow, column].piece != '\0')
                    {
                        return null; //Om det står något emellan kungen och pjäsen ifråga.
                    }
                    legalSquares.Add(new int[] { newRow, column });
                }
                //Kommer endast hit om kungen står på ett sådant sätt att den kan vara spikad.
                int i = 1;
                while (validSquare(row + (i * sign), column))
                {
                    int newRow = row + (i * sign);
                    if (piecesToLookFor.Contains(board[newRow, column].piece))
                    {
                        legalSquares.Add(new int[] { newRow, column });
                        return legalSquares; //Det står ett torn eller dam på andra sidan.
                    }
                    else if (board[newRow, column].piece != '\0')
                        return null;
                    else
                    {
                        legalSquares.Add(new int[] { newRow, column });
                    }
                    i++;
                }
                return null; //Borde inte anropas.
            }
            else if (dRow == 0) //Samma rad
            {
                string piecesToLookFor = "rq";
                if (!whiteToMove) piecesToLookFor = "RQ";
                int sign = Math.Sign(dCol); //För att veta vilken riktning kungen är åt.
                for (int j = 1; j < Math.Abs(dCol); j++)
                {
                    int newCol = column - (j * sign);
                    if (board[row, newCol].piece != '\0')
                    {

                        return null; //Om det står något emellan kungen och pjäsen ifråga.
                    }
                    legalSquares.Add(new int[] { row, newCol });
                }
                //Kommer endast hit om kungen står på ett sådant sätt att den kan vara spikad.
                int i = 1;
                while (validSquare(row, column + (i * sign)))
                {
                    int newCol = column + (i * sign);
                    if (piecesToLookFor.Contains(board[row, newCol].piece))
                    {
                        legalSquares.Add(new int[] { row, newCol });
                        return legalSquares; //Det står ett torn eller dam på andra sidan.
                    }
                    else if (board[row, newCol].piece != '\0')
                        return null;
                    else
                    {
                        legalSquares.Add(new int[] { row, newCol });
                    }
                    i++;
                }
                return null; //Borde inte anropas.
            }
            else if (Math.Abs(dRow) == Math.Abs(dCol)) //Samma diagonal
            {
                string piecesToLookFor = "bq";
                if (!whiteToMove) piecesToLookFor = "BQ";
                int signRow = Math.Sign(dRow);
                int signCol = Math.Sign(dCol); //För att veta vilken riktning kungen är åt.
                for (int j = 1; j < Math.Abs(dCol); j++)
                {
                    int newCol = column - (j * signCol);
                    int newRow = row - (j * signRow);
                    if (board[newRow, newCol].piece != '\0')
                    {

                        return null; //Om det står något emellan kungen och pjäsen ifråga.
                    }
                    legalSquares.Add(new int[] { newRow, newCol });
                }
                //Kommer endast hit om kungen står på ett sådant sätt att den kan vara spikad.
                int i = 1;
                while (validSquare(row + (i * signRow), column + (i * signCol)))
                {
                    //TODO: Sätt dessa tidigare.
                    int newRow = row + (i * signRow);
                    int newCol = column + (i * signCol);
                    if (piecesToLookFor.Contains(board[newRow, newCol].piece))
                    {
                        legalSquares.Add(new int[] { newRow, newCol });
                        return legalSquares; //Det står ett torn eller dam på andra sidan.
                    }
                    else if (board[newRow, newCol].piece != '\0')
                        return null;
                    else
                    {
                        legalSquares.Add(new int[] { newRow, newCol });
                    }
                    i++;
                }
                return null; //Borde inte anropas.
            }
            else return null;
        }
        private List<Move> removeIllegalPinMoves(List<int[]> legalSquares, List<Move> moves)
        {
            //Tar bort alla drag ur moves som innebär förflyttning till ett icke tillåtet fält.
            //Om legalSqaures är null så kommer moves returneras oförändrad.
            if (legalSquares != null)
            {
                for (int i = 0; i < moves.Count; i++)
                {
                    bool shouldRemove = true;
                    foreach (int[] sq in legalSquares)
                    {
                        if (sq[0] == moves[i].to[0] && sq[1] == moves[i].to[1])
                        {
                            shouldRemove = false;
                            break;
                        }
                    }
                    if (shouldRemove)
                    {
                        moves.RemoveAt(i);
                        i--;
                    }
                }
            }
            return moves;
        }
        private double evalPawns(int[] numberOfPawns, double[] posFactor, int[,] pawns)
        {
            for (int c = 0; c < 2; c++)
            {
                int neighbours = 0;
                int lines = 0;
                if (pawns[c, 0] > 0) lines++; //specialfall för a-linjen.
                if (pawns[c, 1] > 0) neighbours += pawns[c, 0];
                for (int i = 1; i < 7; i++) //från b till g sista. 
                {
                    if (pawns[c, i] > 0) lines++;
                    if (pawns[c, i - 1] > 0) neighbours += pawns[c, 0];
                    if (pawns[c, i + 1] > 0) neighbours += pawns[c, 0];
                }
                if (pawns[c, 7] > 0) lines++; //specialfall för h-linjen.
                if (pawns[c, 6] > 0) neighbours += pawns[c, 0];
                pawnValues[c] = ((numberOfPawns[c] * (neighbours + lines + 41)) + 9.37f) / 64;
                //e(p,s)=(p(s+41)+9,37)/64. TODO: Förbättra formel
                pawnValues[c] *= posFactor[c];
            }
            return pawnValues[1] - pawnValues[0];
        }
        private bool accessableFor(bool forWhite, char piece)
        {
            if (forWhite)
            {
                if (piece == '\0' || piece > 'Z') return true;
                else return false;
            }
            else
            {
                if (piece < 'Z') return true;
                else return false;
            }
        }
        private bool validSquare(int row, int column)
        {
            return (row < 8) && (row >= 0) && (column < 8) && (column >= 0);
        }
    }
}