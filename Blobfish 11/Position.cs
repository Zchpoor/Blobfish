using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blobfish_11
{
    public class Position
    {
        //TODO: Gör om till struct?
        //TODO: Ändra allt till Square.
        private readonly string FEN;
        public bool whiteToMove;
        public int halfMoveClock = 0;
        public int moveCounter = 0;
        public int[] enPassantSquare = new int[2]; //{-1, -1} om en passant ej kan spelas.
        public bool[] castlingRights = new bool[4]; //KQkq
        public double material = 0; //TODO: Städa upp
        public double[] pawnValues = new double[2];
        public char[,] board = new char[8, 8]; //[0] är rader (siffror), [1] är kolumner (bokstäver)
        public int[,] kingPositions = new int[2, 2]; //Bra att kunna komma åt snabbt. 0=svart, 1=vit
        int[] checkingPieces = { -1, -1, -1, -1 };

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
                        board[row, column] = tkn;
                        column++;
                        break;
                    case 'P':
                        board[row, column] = tkn;
                        column++;
                        break;

                    case 'n':
                        board[row, column] = tkn;
                        column++; break;
                    case 'N':
                        board[row, column] = tkn;
                        column++; break;

                    case 'b':
                        board[row, column] = tkn;
                        column++; break;
                    case 'B':
                        board[row, column] = tkn;
                        column++; break;

                    case 'r':
                        board[row, column] = tkn;
                        column++; break;
                    case 'R':
                        board[row, column] = tkn;
                        column++; break;

                    case 'k':
                        board[row, column] = tkn;
                        kingPositions[0, 0] = row;
                        kingPositions[0, 1] = column;
                        column++; break;
                    case 'K':
                        board[row, column] = tkn;
                        kingPositions[1, 0] = row;
                        kingPositions[1, 1] = column;
                        column++; break;

                    case 'q':
                        board[row, column] = tkn;
                        column++; break;
                    case 'Q':
                        board[row, column] = tkn;
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
        public Position(char[,] board, bool whiteToMove, bool[] castlingRights, int[] enPassantSquare,
            int halfMoveClock, int moveCounter, int[,] kingPositions)
        {
            this.board = board;
            this.FEN = "";
            this.whiteToMove = whiteToMove;
            this.castlingRights = castlingRights;
            this.halfMoveClock = halfMoveClock;
            this.moveCounter = moveCounter;
            this.enPassantSquare = enPassantSquare;
            this.kingPositions = kingPositions;
        }
    }
}
