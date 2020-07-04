using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Blobfish_11
{
    public partial class Engine
    {
        public EvalResult eval(Position pos, int depth)
        {
            List<Move> moves =  allValidMoves(pos);

            EvalResult result = new EvalResult();

            //TODO: Överflödigt att beräkna detta här också.
            //Square relevantKingSquare = pos.whiteToMove ? new Square(pos.kingPositions[1, 0], pos.kingPositions[1, 1]) :
            //    new Square(pos.kingPositions[0, 0], pos.kingPositions[0, 1]);
            //bool isCheck = isControlledBy(pos, relevantKingSquare, !pos.whiteToMove);


            int gameResult = decisiveResult(pos, moves);
            if(gameResult != -2)
            {
                if (gameResult == 1)
                    result.evaluation = 1000;
                else if (gameResult == -1)
                    result.evaluation = -1000;
                else
                    result.evaluation = (double)gameResult;
                result.allMoves = new List<Move>();
                result.allEvals = null;
                return result; //Ställningen är avgjord.
            }
            else
            {
                List<double> allEvals = new List<double>();
                double bestValue = pos.whiteToMove ? double.NegativeInfinity : double.PositiveInfinity;
                foreach (Move currentMove in moves)
                {
                    double currentEval = alphaBeta(currentMove.execute(pos), depth-1, double.NegativeInfinity, double.PositiveInfinity, pos.whiteToMove);
                    allEvals.Add(currentEval);
                    if (pos.whiteToMove)
                    {
                        bestValue = Math.Max(bestValue, currentEval);
                    }
                    else
                    {
                        bestValue = Math.Min(bestValue, currentEval);
                    }

                    //TODO: Gör finare
                    if (bestValue == currentEval)
                        result.bestMove = currentMove;
                }
                result.evaluation = bestValue;
                result.allEvals = allEvals;
            }

            result.allMoves = moves;
            return result;
        }
        private double numericEval(Position pos)
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
                for (int column = 0; column < 8; column++)
                {
                    switch (pos.board[row, column])
                    {
                        case 'p':
                            numberOfPawns[0]++;
                            pawns[0, column]++;
                            posFactor[0] += pawn[0, row, column];
                            break;

                        case 'P':
                            numberOfPawns[1]++;
                            pawns[1, column]++;
                            posFactor[1] += pawn[1, row, column];
                            break;

                        case 'n':
                            pieceValue -= pieceValues[0] * knight[row, column];
                            break;
                        case 'N':
                            pieceValue += pieceValues[0] * knight[row, column];
                            break;

                        case 'b':
                            pieceValue -= pieceValues[1] * bishop[row, column];
                            break;
                        case 'B':
                            pieceValue += pieceValues[1] * bishop[row, column];
                            break;

                        case 'r':
                            pieceValue -= pieceValues[2] * rook[row, column];
                            break;
                        case 'R':
                            pieceValue += pieceValues[2] * rook[row, column];
                            break;

                        case 'k':
                            pos.kingPositions[0, 0] = row;
                            pos.kingPositions[0, 1] = column;
                            break;
                        case 'K':
                            pos.kingPositions[1, 0] = row;
                            pos.kingPositions[1, 1] = column;
                            break;

                        case 'q':
                            pieceValue -= pieceValues[3] * queen[row, column];
                            break;
                        case 'Q':
                            pieceValue += pieceValues[3] * queen[row, column];
                            break;
                        default:
                            break;
                    }
                }
            }

            for (int i = 0; i < 2; i++)
            {
                if (numberOfPawns[i] == 0)
                    posFactor[i] = 0f;
                else
                    posFactor[i] /= numberOfPawns[i];
            }
            double pawnValue = evalPawns(numberOfPawns, posFactor, pawns);
            return pieceValue + pawnValue;
        }
        private double evalPawns(int[] numberOfPawns, double[] posFactor, int[,] pawns)
        {
            double[] pawnValues = new double[2];
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
        private ControlAndCheckingPieces calculateControl(Position pos)
        {
            SquareControl[,] board = new SquareControl[8, 8];
            
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
                    switch (pos.board[row, column])
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
                                if (pos.board[row + i, column + i] != '\0')
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
                                if (pos.board[row - i, column + i] != '\0')
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
                                if (pos.board[row + i, column - i] != '\0')
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
                                if (pos.board[row - i, column - i] != '\0')
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
                                if (pos.board[row + i, column + i] != '\0')
                                {
                                    done = true;
                                }
                                i++;
                            }
                            i = 1;
                            done = false;
                            while (row - i >= 0 && column + i < 8 && !done)
                            {
                                setControl(row - i, column + i, true);
                                if (pos.board[row - i, column + i] != '\0')
                                {
                                    done = true;
                                }
                                i++;
                            }
                            i = 1;
                            done = false;
                            while (row + i < 8 && column - i >= 0 && !done)
                            {
                                setControl(row + i, column - i, true);
                                if (pos.board[row + i, column - i] != '\0')
                                {
                                    done = true;
                                }
                                i++;
                            }
                            i = 1;
                            done = false;
                            while (row - i >= 0 && column - i >= 0 && !done)
                            {
                                setControl(row - i, column - i, true);
                                if (pos.board[row - i, column - i] != '\0')
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
                                if (pos.board[row + i, column] != '\0')
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
                                if (pos.board[row - i, column] != '\0')
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
                                if (pos.board[row, column + i] != '\0')
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
                                if (pos.board[row, column - i] != '\0')
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
                                if (pos.board[row + i, column] != '\0')
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
                                if (pos.board[row - i, column] != '\0')
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
                                if (pos.board[row, column + i] != '\0')
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
                                if (pos.board[row, column - i] != '\0')
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
                                if (pos.board[row + i, column] != '\0')
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
                                if (pos.board[row - i, column] != '\0')
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
                                if (pos.board[row, column + i] != '\0')
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
                                if (pos.board[row, column - i] != '\0')
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
                                if (pos.board[row + i, column + i] != '\0')
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
                                if (pos.board[row - i, column + i] != '\0')
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
                                if (pos.board[row + i, column - i] != '\0')
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
                                if (pos.board[row - i, column - i] != '\0')
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
                                if (pos.board[row + i, column] != '\0')
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
                                if (pos.board[row - i, column] != '\0')
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
                                if (pos.board[row, column + i] != '\0')
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
                                if (pos.board[row, column - i] != '\0')
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
                                if (pos.board[row + i, column + i] != '\0')
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
                                if (pos.board[row - i, column + i] != '\0')
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
                                if (pos.board[row + i, column - i] != '\0')
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
                                if (pos.board[row - i, column - i] != '\0')
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
            ControlAndCheckingPieces result = new ControlAndCheckingPieces();
            result.board = board;
            result.checkingPieces = checkingPieces;
            return result;
        }
        private int decisiveResult(Position pos,  List<Move> moves)
        {
            if (moves.Count == 0)
            {
                Square relevantKingSquare = pos.whiteToMove ? new Square(pos.kingPositions[1, 0], pos.kingPositions[1, 1]) :
                    new Square(pos.kingPositions[0, 0], pos.kingPositions[0, 1]);
                bool isCheck = isControlledBy(pos, relevantKingSquare, !pos.whiteToMove);
                if (isCheck)
                {
                    if (pos.whiteToMove) return -1000;
                    else return 1000;
                }
                else return 0; //Patt
            }

            if (pos.halfMoveClock >= 100)
            {
                return 0; //Femtiodragsregeln.
            }

            return -2;
        }
        private double alphaBeta(Position pos, int depth, double alpha, double beta, bool whiteToMove)
        {
            List<Move> moves = allValidMoves(pos);
            if (moves.Count == 0)
                return decisiveResult(pos, moves);

            //TODO: Fixa horisonten.
            if (depth == 0)
                return numericEval(pos);
            if (whiteToMove)
            {
                double value = double.NegativeInfinity;
                foreach (Move currentMove in moves)
                {
                    value = Math.Max(value, alphaBeta(currentMove.execute(pos), depth - 1, alpha, beta, false));
                    alpha = Math.Max(alpha, value);
                    if (alpha >= beta)
                        break; //Pruning
                }
                return value;
            }
            else
            {
                double value = double.PositiveInfinity;
                foreach (Move currentMove in moves)
                {
                    value = Math.Min(value, alphaBeta(currentMove.execute(pos), depth - 1, alpha, beta, true));
                    beta = Math.Min(beta, value);
                    if (beta <= alpha)
                        break; //Pruning
                }
                return value;
            }
        }
    }
}
