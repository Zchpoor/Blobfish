using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Blobfish_11
{
    public class Position
    {
        //TODO: Gör om till struct?
        public bool whiteToMove;
        public sbyte halfMoveClock;
        public short moveCounter;
        public Square enPassantSquare; //{-1, -1} om en passant ej kan spelas.
        public bool[] castlingRights; //KQkq
        public char[,] board; //[0] är rader (siffror), [1] är kolumner (bokstäver)
        public Square[] kingPositions; //Bra att kunna komma åt snabbt. 0=svart, 1=vit

        public Position(string FEN)
        {
            this.board = new char[8, 8];
            this.kingPositions = new Square[2];
            this.castlingRights = new bool[4];
            sbyte line = 0, rank = 0;
            string boardString = FEN.Substring(0, FEN.IndexOf(' '));
            foreach (char tkn in boardString)
            {
                switch (tkn)
                {
                    case 'p':
                        board[rank, line] = tkn;
                        line++;
                        break;
                    case 'P':
                        board[rank, line] = tkn;
                        line++;
                        break;

                    case 'n':
                        board[rank, line] = tkn;
                        line++; break;
                    case 'N':
                        board[rank, line] = tkn;
                        line++; break;

                    case 'b':
                        board[rank, line] = tkn;
                        line++; break;
                    case 'B':
                        board[rank, line] = tkn;
                        line++; break;

                    case 'r':
                        board[rank, line] = tkn;
                        line++; break;
                    case 'R':
                        board[rank, line] = tkn;
                        line++; break;

                    case 'k':
                        board[rank, line] = tkn;
                        kingPositions[0].rank = rank;
                        kingPositions[0].line = line;
                        line++; break;
                    case 'K':
                        board[rank, line] = tkn;
                        kingPositions[1].rank = rank;
                        kingPositions[1].line = line;
                        line++; break;

                    case 'q':
                        board[rank, line] = tkn;
                        line++; break;
                    case 'Q':
                        board[rank, line] = tkn;
                        line++; break;

                    case '/': line = 0; rank++; break;
                    case '1': line += 1; break;
                    case '2': line += 2; break;
                    case '3': line += 3; break;
                    case '4': line += 4; break;
                    case '5': line += 5; break;
                    case '6': line += 6; break;
                    case '7': line += 7; break;
                    case '8': break;
                    default:
                        line++;
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
                enPassantSquare = new Square(-1, -1);
            }
            else
            {
                int EPline = EPString[0] - 'a';
                int EPrank = EPString[1] - '1';
                if (EPline < 0 || EPline > 7 || EPrank < 0 || EPrank > 7)
                {
                    throw new Exception("Felaktig FEN."); //TODO: Hantera
                }
                enPassantSquare.rank = (sbyte) (7 - EPrank);
                enPassantSquare.line = (sbyte) EPline; //Eftersom ordingen är omvänd i FEN.
            }
            string lastString = EPString.Substring(EPString.IndexOf(' ') + 1, EPString.Length - EPString.IndexOf(' ') - 1);
            //Till exempel: "1 2".
            string clockString = lastString.Substring(0, lastString.IndexOf(' '));
            //Till exempel: "1".
            this.halfMoveClock = sbyte.Parse(clockString);
            string moveString = lastString.Substring(lastString.IndexOf(' '), lastString.Length - lastString.IndexOf(' '));
            this.moveCounter = sbyte.Parse(moveString);
            if(this.halfMoveClock < 0 || this.moveCounter < 0)
                throw new Exception("Felaktig FEN.");
            //Till exempel: "2".

        }
        public Position(char[,] board, bool whiteToMove, bool[] castlingRights, Square enPassantSquare,
            sbyte halfMoveClock, short moveCounter, Square[] kingPositions)
        {
            this.board = board;
            this.whiteToMove = whiteToMove;
            this.castlingRights = castlingRights;
            this.halfMoveClock = halfMoveClock;
            this.moveCounter = moveCounter;
            this.enPassantSquare = enPassantSquare;
            this.kingPositions = kingPositions;
        }
        public Position deepCopy()
        {
            char[,] newBoard = new char[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    newBoard[i, j] = this.board[i, j];
                }
            }
            return new Position(newBoard, whiteToMove, castlingRights,
                new Square(-1, -1), halfMoveClock, moveCounter, (Square[])kingPositions.Clone());
        }
        public string getFEN()
        {
            string FEN = "";
            for (int i = 0; i < 8; i++) //Eftersom ordningen är omvänd i FEN.
            {
                int emptySquaresCounter = 0;
                for (int j = 0; j < 8; j++)
                {
                    char currentPiece = board[i, j];
                    if ( currentPiece == '\0')
                        emptySquaresCounter++;
                    else
                    {
                        if(emptySquaresCounter > 0)
                        {
                            FEN += emptySquaresCounter.ToString();
                            emptySquaresCounter = 0;
                        }
                        FEN += currentPiece;
                    }
                }
                if (emptySquaresCounter > 0)
                {
                    FEN += emptySquaresCounter.ToString();
                }
                if (i < 7)
                {
                    FEN += '/';
                }
            }

            if (whiteToMove)
                FEN += " w ";
            else
                FEN += " b ";

            if(castlingRights[0] == false && castlingRights[1] == false && castlingRights[2] == false && castlingRights[3] == false)
            {
                FEN += "- ";
            }
            else
            {
                if (castlingRights[0])
                    FEN += "K";
                if (castlingRights[1])
                    FEN += "Q";
                if (castlingRights[2])
                    FEN += "k";
                if (castlingRights[3])
                    FEN += "q";
                FEN += " ";
            }

            if (enPassantSquare.rank == -1 || enPassantSquare.line == -1)
            {
                FEN += "- ";
            }
            else
            {
                FEN += (char) (enPassantSquare.line + 'a');
                FEN += (char) ('8' - enPassantSquare.rank);
                FEN += " ";
            }

            FEN += halfMoveClock.ToString() + " " + moveCounter.ToString();

            return FEN;
        }
    }
}
