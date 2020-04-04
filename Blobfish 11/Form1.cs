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
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            position pos = new position(fenBox.Text);
            evalBox.Text = "";
            double value = pos.eval();
            evalBox.Text += "Evaluering:\n";
            evalBox.Text += "Pjäser: " + Math.Round(pos.material, 2) + "\n";
            evalBox.Text += "Bönder: " + Math.Round(pos.pawnValues[1] - pos.pawnValues[0], 2) + "\n";
            evalBox.Text += "    Vita: " + Math.Round(pos.pawnValues[1], 2) + "\n";
            evalBox.Text += "    Svarta: " + Math.Round(pos.pawnValues[0], 2) + "\n";
            evalBox.Text += "Totalt: " + Math.Round(value, 2) + "\n";
        }
    }
    public class position
    {
        private readonly string FEN;
        public bool whiteToMove;
        private int halfMoveClock = 0;
        private int moveCounter = 0;
        private int[] enPassantSquare = new int[2];
        private bool[] castlingRights = new bool[4];
        public double material = 0; //TODO: Städa upp
        public double[] pawnValues = new double[2];
        square[,] board = new square[8, 8];
        int[,] kingPositions = new int[2, 2]; //Bra att kunna komma åt snabbt.
        public position(string FEN)
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
            int tep = infoString.IndexOf(' ');
            string castlingString = infoString.Substring(0, infoString.IndexOf(' ')); //Till exempel: KQkq
            #region castlingRights
            if (castlingString == "-")
            {
                 castlingRights =  new bool[] { false, false, false, false };
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
            string EPString = infoString.Substring(infoString.IndexOf(' ')+1, infoString.Length - infoString.IndexOf(' ')-1);
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
            string lastString = EPString.Substring(EPString.IndexOf(' ')+1, EPString.Length - EPString.IndexOf(' ')-1);
            //Till exempel: "1 2".
            string clockString = lastString.Substring(0, lastString.IndexOf(' '));
            //Till exempel: "1".
            this.halfMoveClock = int.Parse(clockString);
            string moveString = lastString.Substring(lastString.IndexOf(' '), lastString.Length - lastString.IndexOf(' '));
            this.moveCounter = int.Parse(moveString);
            //Till exempel: "2".

        }
        public double eval()
        {
            List<Move> moves = controlAndMoves();

            return extract();
        }
        private int[] setControl(int row, int column, bool byWhite)
        {
            int[] checkingPieces = { -1, -1, -1, -1 }; //Coordinaterna för första, respektive andra shackande pjäs.
            if (byWhite)
            {
                board[row, column].wControl = true;
                if (row == kingPositions[1, 0] && row == kingPositions[1, 1])
                {
                    if (checkingPieces[0] == -1) //Om detta är den första pjäsen som schackar
                    {
                        checkingPieces[0] = row;
                        checkingPieces[1] = column;
                    }
                    else if(checkingPieces[2] == -1) //Om detta är den andra pjäsen som schackar
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
                if (row == kingPositions[0, 0] && row == kingPositions[0, 1])
                {
                    if (checkingPieces[0] == -1) //Om detta är den första pjäsen som schackar
                    {
                        checkingPieces[0] = row;
                        checkingPieces[1] = column;
                    }
                    else if (checkingPieces[3] == -1) //Om detta är den andra pjäsen som schackar
                    {
                        checkingPieces[3] = row;
                        checkingPieces[4] = column;
                    }
                    else
                    {
                        throw new Exception("Fler än 2 schackande pjäser!");
                    }
                }
            }
            return checkingPieces;
        }
        public double extract() //Förlegad
        {
            /* 
             * [row, column]
             * row==0       -> rad 8.
             * row==7       -> rad 1.
             * column==0    -> a-linjen.
             * column==7    -> h-linjen.
             */
            int[] numberOfPawns = new int[2];
            square[,] board = new square[8, 8]; //TODO: Make higher
            double[] posFactor = { 1f, 1f };
            int[,] pawns = new int[2, 8]; //0=black, 1=white.
            bool done = false; //TODO: substring?
            bool whiteSquare = true; //TODO: ordna med färgkomplex.
            int column = 0, row = 0;
            double[] pieceValues = { 3, 3, 5, 9 };
            double pieceValue = 0;
            foreach (char tkn in FEN)
            {
                if (done) break;
                switch (tkn)
                {
                    case 'p':
                        board[row, column].piece = tkn;
                        numberOfPawns[0]++;
                        pawns[0, column]++;
                        posFactor[0] += placement.pawn[0, row, column];
                        column++;
                       
                        break;
                    case 'P':
                        board[row, column].piece = tkn;
                        numberOfPawns[1]++;
                        pawns[1, column]++;
                        posFactor[1] += placement.pawn[1, row, column];
                        column++;
                        whiteSquare = !whiteSquare;
                        break;

                    case 'n':
                        board[row, column].piece = tkn;
                        pieceValue -= pieceValues[0] * placement.knight[row, column];
                        column++; whiteSquare = !whiteSquare; break;
                    case 'N':
                        board[row, column].piece = tkn;
                        pieceValue += pieceValues[0] * placement.knight[row, column];
                        column++; whiteSquare = !whiteSquare; break;

                    case 'b':
                        board[row, column].piece = tkn;
                        pieceValue -= pieceValues[1] * placement.bishop[row, column];
                        column++; whiteSquare = !whiteSquare; break;
                    case 'B':
                        board[row, column].piece = tkn;
                        pieceValue += pieceValues[1] * placement.bishop[row, column];
                        column++; whiteSquare = !whiteSquare; break;

                    case 'r':
                        board[row, column].piece = tkn;
                        pieceValue -= pieceValues[2] * placement.rook[row, column];
                        column++; whiteSquare = !whiteSquare; break;
                    case 'R':
                        board[row, column].piece = tkn;
                        pieceValue += pieceValues[2] * placement.rook[row, column];
                        column++; whiteSquare = !whiteSquare; break;

                    case 'k':
                        board[row, column].piece = tkn;
                        kingPositions[0, 0] = row;
                        kingPositions[0, 1] = column;
                        column++; whiteSquare = !whiteSquare; break;
                    case 'K':
                        board[row, column].piece = tkn;
                        kingPositions[1, 0] = row;
                        kingPositions[1, 1] = column;
                        column++; whiteSquare = !whiteSquare; break;

                    case 'q':
                        board[row, column].piece = tkn;
                        pieceValue -= pieceValues[3] * placement.queen[row, column];
                        column++; whiteSquare = !whiteSquare; break;
                    case 'Q':
                        board[row, column].piece = tkn;
                        pieceValue += pieceValues[3] * placement.queen[row, column];
                        column++; whiteSquare = !whiteSquare; break;

                    case '/': column = 0; row++; break;
                    case ' ':
                        done = true;
                        break;
                    case '1': column += 1; whiteSquare = !whiteSquare; break;
                    case '2': column += 2; break;
                    case '3': column += 3; whiteSquare = !whiteSquare; break;
                    case '4': column += 4; break;
                    case '5': column += 5; whiteSquare = !whiteSquare; break;
                    case '6': column += 6; break;
                    case '7': column += 7; whiteSquare = !whiteSquare; break;
                    case '8': whiteSquare = !whiteSquare; break;
                    default:
                        column++;
                        break;
                }
            }
            List<Move> moves = controlAndMoves();
            int result = decisiveResult(moves ); //1=vit vinst, -1=svart vinst, 0=remi, -2=oklart.

            posFactor[0] /= numberOfPawns[0];
            posFactor[1] /= numberOfPawns[1];
            double pawnValue = evalPawns(numberOfPawns, posFactor, pawns);
            this.material = pieceValue;
            return pieceValue + pawnValue;
        }
        private int decisiveResult(List<Move> moves, int[] checkingPieces)
        {
            int i = 0;
            if (whiteToMove) i = 1;
            if (checkingPieces[0] != -1) //Schack
            {
                if (checkingPieces[2] == -1) //Enkelschack
                {

                }
                else //Dubbelschack
                {

                }
            }
            else
            {

            }

            return -2;
        }
        private List<Move> controlAndMoves()
        {
            List<Move> moves = new List<Move>();
            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    switch (board[row, column].piece)
                    {
                        case 'p':
                            #region whitePawnControl
                            if (column > 0) {
                                setControl(row + 1, column - 1, false);
                                if(board[row + 1, column - 1].piece < 'Z') //Vit pjäs
                                {
                                    moves.Add(new Move(new int[] {row, column}, new int[] { row+1, column-1 }));
                                }
                            }
                            if (column < 7)
                            {
                                setControl(row + 1, column + 1, false);
                                if (board[row + 1, column + 1].piece < 'Z') //Vit pjäs
                                {
                                    moves.Add(new Move(new int[] { row, column }, new int[] { row + 1, column + 1 }));

                                }
                            }
                            if (board[row + 1, column].piece == '\0') //Tomt fält
                            {
                                moves.Add(new Move(new int[] { row, column }, new int[] { row + 1, column }));
                                if(row == 1 && board[row + 2, column].piece == '\0')
                                {
                                    moves.Add(new Move(new int[] { row, column }, new int[] { row + 2, column }));
                                }
                            }
                            #endregion
                            break;
                        case 'P':
                            #region blackPawnControl
                            if (column > 0)
                            {
                                setControl(row + 1, column - 1, true);
                                if (board[row + 1, column - 1].piece > 'Z') //Svart pjäs
                                {
                                    moves.Add(new Move(new int[] { row, column }, new int[] { row - 1, column - 1 }));
                                }
                            }
                            if (column < 7)
                            {
                                setControl(row + 1, column + 1, true);
                                if (board[row + 1, column + 1].piece > 'Z') //Svart pjäs
                                {
                                    moves.Add(new Move(new int[] { row, column }, new int[] { row - 1, column + 1 }));
                                }
                            }
                            if (board[row + 1, column].piece == '\0') //Tomt fält
                            {
                                moves.Add(new Move(new int[] { row, column }, new int[] { row - 1, column }));
                                if (row == 6 && board[row - 2, column].piece == '\0')
                                {
                                    moves.Add(new Move(new int[] { row, column }, new int[] { row - 2, column }));
                                }
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
                            if (row + 2 < 8 && column - 1 > 0)
                                setControl(row + 2, column - 1, false);
                            if (row + 1 < 8 && column + 2 < 8)
                                setControl(row + 1, column + 2, false);
                            if (row + 1 < 8 && column - 2 > 0)
                                setControl(row + 1, column - 2, false);
                            if (row - 1 > 0 && column + 2 < 8)
                                setControl(row - 1, column + 2, false);
                            if (row - 1 > 0 && column - 2 > 0)
                                setControl(row - 1, column - 2, false);
                            if (row - 2 > 0 && column + 1 < 8)
                                setControl(row - 2, column + 1, false);
                            if (row - 2 > 0 && column - 1 > 0)
                                setControl(row - 2, column - 1, false);
                            #endregion
                            break;
                        case 'N': //TODO: Förbättra!
                            #region whiteKnightControl
                            if (row + 2 < 8 && column + 1 < 8)
                                setControl(row + 2, column + 1, true);
                            if (row + 2 < 8 && column - 1 > 0)
                                setControl(row + 2, column - 1, true);
                            if (row + 1 < 8 && column + 2 < 8)
                                setControl(row + 1, column + 2, true);
                            if (row + 1 < 8 && column - 2 > 0)
                                setControl(row + 1, column - 2, true);
                            if (row - 1 > 0 && column + 2 < 8)
                                setControl(row - 1, column + 2, true);
                            if (row - 1 > 0 && column - 2 > 0)
                                setControl(row - 1, column - 2, true);
                            if (row - 2 > 0 && column + 1 < 8)
                                setControl(row - 2, column + 1, true);
                            if (row - 2 > 0 && column - 1 > 0)
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
                            while (row + i < 8 && column + i < 8 && !done)
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
                            while (row + i < 8 && column + i < 8 && !done)
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
                            while (row + i < 8 && column + i < 8 && !done)
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
                            while (row + i < 8 && column + i < 8 && !done)
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
        private square[,] getInfo()
        {
            foreach (char tkn in FEN)
            {
                square[,] board = new square[8, 8];
                bool done = false; //TODO: substring?
                bool whiteSquare = true;
                int column = 0, row = 0;
                if (done) break;
                switch (tkn)
                {
                    case 'p':
                        board[row, column].piece = 'p';
                        if (column > 0) board[row + 1, column - 1].bControl = true;
                        if (column < 7) board[row + 1, column + 1].bControl = true;
                        column++;
                        whiteSquare = !whiteSquare;
                        break;
                    case 'P':
                        board[row, column].piece = 'P';
                        if (column > 0) board[row - 1, column - 1].wControl = true;
                        if (column < 7) board[row - 1, column + 1].bControl = true;
                        column++;
                        whiteSquare = !whiteSquare;
                        break;
                    case '/': column = 0; row++; break;
                    case ' ':
                        done = true;
                        break;
                    case '1': column += 1; whiteSquare = !whiteSquare; break;
                    case '2': column += 2; break;
                    case '3': column += 3; whiteSquare = !whiteSquare; break;
                    case '4': column += 4; break;
                    case '5': column += 5; whiteSquare = !whiteSquare; break;
                    case '6': column += 6; break;
                    case '7': column += 7; whiteSquare = !whiteSquare; break;
                    case '8': whiteSquare = !whiteSquare; break;
                    default:
                        column++;
                        break;
                }
            }

            return null;
        }
    }
    static class placement
    { //TODO: För hårda värden.
        public static readonly double[,,] pawn =
        {
            { //Svarta bönder
                { 0f,    0f,    0f,    0f,   0f,   0f,    0f,    0f    }, //8
                { 0.5f,  0.6f,  0.7f,  0.8f, 0.8f, 0.7f,  0.6f,  0.5f  }, //7
                { 0.55f, 0.7f,  0.85f, 1f,   1f,   0.85f, 0.7f,  0.55f }, //6
                { 0.6f,  0.75f, 1f,    1.2f, 1.2f, 1f,    0.75f, 0.6f  }, //5
                { 0.7f,  0.9f,  1.2f,  1.4f, 1.4f, 1.2f,  0.9f,  0.7f  }, //4
                { 1f,    1.1f,  1.5f,  1.7f, 1.7f, 1.5f,  1.1f,  1f    }, //3
                { 1.9f,  2f,    2.1f,  2.2f, 2.2f, 2.1f,  2f,    1.9f  }, //2
                { 0f,    0f,    0f,    0f,   0f,   0f,    0f,    0f    }  //1
            },
            { //Vita bönder
                { 0f,    0f,    0f,    0f,   0f,   0f,    0f,    0f    }, //8
                { 1.9f,  2f,    2.1f,  2.2f, 2.2f, 2.1f,  2f,    1.9f  }, //7
                { 1f,    1.1f,  1.5f,  1.7f, 1.7f, 1.5f,  1.1f,  1f    }, //6
                { 0.7f,  0.9f,  1.2f,  1.4f, 1.4f, 1.2f,  0.9f,  0.7f  }, //5
                { 0.6f,  0.75f, 1f,    1.2f, 1.2f, 1f,    0.75f, 0.6f  }, //4
                { 0.55f, 0.7f,  0.85f, 1f,   1f,   0.85f, 0.7f,  0.55f }, //3
                { 0.5f,  0.6f,  0.7f,  0.8f, 0.8f, 0.7f,  0.6f,  0.5f  }, //2
                { 0f,    0f,    0f,    0f,   0f,   0f,    0f,    0f    }  //1
            }
        };
        public static readonly double[,] knight =
        {
            { 0.55f, 0.65f, 0.75f, 0.9f,  0.9f,  0.75f, 0.65f, 0.55f },
            { 0.65f, 0.8f,  1f,    1.05f, 1.05f, 1f,    0.8f,  0.65f },
            { 0.75f, 1f,    1.35f, 1.5f,  1.5f,  1.35f, 1f,    0.75f },
            { 0.9f,  1.05f, 1.5f,  1.6f,  1.6f,  1.5f,  1.05f, 0.9f  },
            { 0.9f,  1.05f, 1.5f,  1.6f,  1.6f,  1.5f,  1.05f, 0.9f  },
            { 0.75f, 1f,    1.35f, 1.5f,  1.5f,  1.35f, 1f,    0.75f },
            { 0.65f, 0.8f,  1f,    1.05f, 1.05f, 1f,    0.8f,  0.65f },
            { 0.55f, 0.65f, 0.75f, 0.9f,  0.9f,  0.75f, 0.65f, 0.55f }
        };
        public static readonly double[,] bishop =
        {
            { 1f,    0.95f, 0.85f, 0.8f,  0.8f,  0.85f, 0.95f, 1f    },
            { 0.95f, 1.1f,  1f,    0.85f, 0.85f, 1f,    1.1f,  0.95f },
            { 0.85f, 1f,    1.3f,  1.1f,  1.1f,  1.3f,  1f,    0.85f },
            { 0.8f,  0.85f, 1.1f,  1.5f,  1.5f,  1.1f,  0.85f, 0.8f  },
            { 0.8f,  0.85f, 1.1f,  1.5f,  1.5f,  1.1f,  0.85f, 0.8f  },
            { 0.85f, 1f,    1.3f,  1.1f,  1.1f,  1.3f,  1f,    0.85f },
            { 0.95f, 1.1f,  1f,    0.85f, 0.85f, 1f,    1.1f,  0.95f },
            { 1f,    0.95f, 0.85f, 0.8f,  0.8f,  0.85f, 0.95f, 1f    }
        };
        public static readonly double[,] rook =
        {
            { 0.85f, 0.9f,  0.95f, 1f,     1f,    0.95f, 0.9f,  0.85f, },
            { 0.9f,  0.95f, 1f,    1.05f,  1.05f, 1f,    0.95f, 0.9f,  },
            { 0.95f, 1f,    1.05f, 1.1f,   1.1f,  1.05f, 1f,    0.95f, },
            { 1f,    1.05f, 1.1f,  1.15f,  1.15f, 1.1f,  1.05f, 1f,    },
            { 1f,    1.05f, 1.1f,  1.15f,  1.15f, 1.1f,  1.05f, 1f,    },
            { 0.95f, 1f,    1.05f, 1.1f,   1.1f,  1.05f, 1f,    0.95f, },
            { 0.9f,  0.95f, 1f,    1.05f,  1.05f, 1f,    0.95f, 0.9f,  },
            { 0.85f, 0.9f,  0.95f, 1f,     1f,    0.95f, 0.9f,  0.85f, }
        };
        public static readonly double[,] queen =
        {
            { 0.93f, 0.93f, 0.90f, 0.90f, 0.90f, 0.90f, 0.93f, 0.93f},
            { 0.93f, 1.03f, 1.00f, 0.95f, 0.95f, 1.00f, 1.03f, 0.93f},
            { 0.90f, 1.00f, 1.18f, 1.10f, 1.10f, 1.18f, 1.00f, 0.90f},
            { 0.90f, 0.95f, 1.10f, 1.33f, 1.33f, 1.10f, 0.95f, 0.90f},
            { 0.90f, 0.95f, 1.10f, 1.33f, 1.33f, 1.10f, 0.95f, 0.90f},
            { 0.90f, 1.00f, 1.18f, 1.10f, 1.10f, 1.18f, 1.00f, 0.90f},
            { 0.93f, 1.03f, 1.00f, 0.95f, 0.95f, 1.00f, 1.03f, 0.93f},
            { 0.93f, 0.93f, 0.90f, 0.90f, 0.90f, 0.90f, 0.93f, 0.93f}
        };
    }
    struct square
    {
        public bool wControl;
        public bool bControl;
        public char piece;
    }
    class Move
    {
        int[] from = new int[2];
        int[] to = new int[2];
        public Move(int[] to, int[] from){
            this.from = from;
            this.to = to;
        }
    }   
}
