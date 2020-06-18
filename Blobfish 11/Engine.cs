using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Blobfish_11
{
    //TODO: Skapa eget namespace?
    class Engine
    {
        public double eval(Position pos, int depth)
        {
            BoardAndCheckingPieces controlResult = calculateControl(pos);
            List<Move> moves = calculateMoves(pos);
            int gameResult = decisiveResult(pos, controlResult.checkingPieces, controlResult);
            if(gameResult != -2)
            {
                return (double)gameResult; //Ställningen är avgjord.
            }

            return 0;
        }
        private BoardAndCheckingPieces calculateControl(Position pos)
        {
            //TODO: Ändra från Square.
            Square[,] board = new Square[8, 8];
            
            //TODO: Returnera denna på något vis.
            int[] checkingPieces = { -1, -1, -1, -1 };
            

            //Lokal funktion för att göra ett fält kontrollerat av en färg.
            int[] setControl(int row, int column, bool byWhite)
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
                        if (row == pos.kingPositions[0, 0] && column == pos.kingPositions[0, 1])
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
                        if (row == pos.kingPositions[1, 0] && column == pos.kingPositions[1, 1])
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
            
            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    switch (pos.board[row, column].piece)
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
                                if (pos.board[row + i, column + i].piece != '\0')
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
                                if (pos.board[row - i, column + i].piece != '\0')
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
                                if (pos.board[row + i, column - i].piece != '\0')
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
                                if (pos.board[row - i, column - i].piece != '\0')
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
                                if (pos.board[row + i, column + i].piece != '\0')
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
                                if (pos.board[row - i, column + i].piece != '\0')
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
                                if (pos.board[row - i, column - i].piece != '\0')
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
                                if (pos.board[row + i, column].piece != '\0')
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
                                if (pos.board[row - i, column].piece != '\0')
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
                                if (pos.board[row, column + i].piece != '\0')
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
                                if (pos.board[row, column - i].piece != '\0')
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
                                if (pos.board[row + i, column].piece != '\0')
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
                                if (pos.board[row - i, column].piece != '\0')
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
                                if (pos.board[row, column + i].piece != '\0')
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
                                if (pos.board[row, column - i].piece != '\0')
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
                                if (pos.board[row + i, column].piece != '\0')
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
                                if (pos.board[row - i, column].piece != '\0')
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
                                if (pos.board[row, column + i].piece != '\0')
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
                                if (pos.board[row, column - i].piece != '\0')
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
                                if (pos.board[row + i, column + i].piece != '\0')
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
                                if (pos.board[row - i, column + i].piece != '\0')
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
                                if (pos.board[row + i, column - i].piece != '\0')
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
                                if (pos.board[row - i, column - i].piece != '\0')
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
                                if (pos.board[row + i, column].piece != '\0')
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
                                if (pos.board[row - i, column].piece != '\0')
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
                                if (pos.board[row, column + i].piece != '\0')
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
                                if (pos.board[row, column - i].piece != '\0')
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
                                if (pos.board[row + i, column + i].piece != '\0')
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
                                if (pos.board[row - i, column + i].piece != '\0')
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
                                if (pos.board[row + i, column - i].piece != '\0')
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
                                if (pos.board[row - i, column - i].piece != '\0')
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
            BoardAndCheckingPieces result = new BoardAndCheckingPieces();
            result.board = board;
            result.checkingPieces = checkingPieces;
            return result;
        }
        public int decisiveResult(Position pos, int[] checkingPieces, BoardAndCheckingPieces controlResult)
        {
            // 1=vit vinst, -1=svart vinst, 0=remi, -2=oklart.
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

            if (pos.moveCounter > 100)
            {
                return 0; //Femtiodragsregeln.
            }

            return -2;
        }

        //TODO: Gör denna funktion mindre
        public List<Move> calculateMoves(Position pos)
        {
            Square[,] board = pos.board;

            //TODO: Få bort dessa?
            List<int[]> squaresIfPinned(int row, int column, bool whiteToMove)
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
                    dRow = row - pos.kingPositions[1, 0];
                    dCol = column - pos.kingPositions[1, 1];
                }
                else
                {
                    dRow = row - pos.kingPositions[0, 0];
                    dCol = column - pos.kingPositions[0, 1];
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
            List<Move> removeIllegalPinMoves(List<int[]> legalSquares, List<Move> allMoves)
            {
                //Tar bort alla drag ur moves som innebär förflyttning till ett icke tillåtet fält.
                //Om legalSqaures är null så kommer moves returneras oförändrad.
                if (legalSquares != null)
                {
                    for (int i = 0; i < allMoves.Count; i++)
                    {
                        bool shouldRemove = true;
                        foreach (int[] sq in legalSquares)
                        {
                            if (sq[0] == allMoves[i].to[0] && sq[1] == allMoves[i].to[1])
                            {
                                shouldRemove = false;
                                break;
                            }
                        }
                        if (shouldRemove)
                        {
                            allMoves.RemoveAt(i);
                            i--;
                        }
                    }
                }
                return allMoves;
            }

            //TODO: Fixa olagliga kungsdrag när fältet bakom kungen inte räknas som kontrollerat.
            List<Move> moves = new List<Move>();
            List<Move> newMoves = new List<Move>();
            if (pos.whiteToMove)
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
                                        || (pos.enPassantSquare[0] == row + 1 && pos.enPassantSquare[1] == column - 1)) //Svart pjäs eller passant
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row - 1, column - 1 }));
                                    }
                                }
                                if (column < 7)
                                {
                                    if (board[row - 1, column + 1].piece > 'Z'
                                        || (pos.enPassantSquare[0] == row + 1 && pos.enPassantSquare[1] == column + 1)) //Svart pjäs eller passant
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
                                    if (pos.castlingRights[0] && board[7, 5].piece == '\0' && board[7, 6].piece == '\0'
                                        && !board[7, 5].bControl && !board[7, 6].bControl)
                                    {
                                        newMoves.Add(new Castle(new int[] { column, row }, new int[] { 7, 6 },
                                            new int[] { 7, 7 }, new int[] { 7, 5 }));
                                    }

                                    //Lång rockad
                                    if (pos.castlingRights[1] && board[7, 3].piece == '\0' && board[7, 2].piece == '\0'
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
                                        || (pos.enPassantSquare[0] == row + 1 && pos.enPassantSquare[1] == column - 1)) //Vit pjäs  eller passant
                                    {
                                        newMoves.Add(new Move(new int[] { row, column }, new int[] { row + 1, column - 1 }));
                                    }
                                }
                                if (column < 7)
                                {
                                    char pjas = board[row + 1, column + 1].piece;
                                    if ((pjas != '\0' && pjas < 'Z')
                                        || (pos.enPassantSquare[0] == row + 1 && pos.enPassantSquare[1] == column + 1)) //Vit pjäs eller passant
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
                                    if (pos.castlingRights[2] && board[0, 5].piece == '\0' && board[0, 6].piece == '\0'
                                        && !board[0, 5].wControl && !board[0, 6].wControl)
                                    {
                                        newMoves.Add(new Castle(new int[] { column, row }, new int[] { 0, 6 },
                                            new int[] { 0, 7 }, new int[] { 0, 5 }));
                                    }

                                    //Lång rockad
                                    if (pos.castlingRights[3] && board[0, 3].piece == '\0' && board[0, 2].piece == '\0'
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
    public struct BoardAndCheckingPieces
    {
        public Square[,] board;
        public int[] checkingPieces;
    }
}
