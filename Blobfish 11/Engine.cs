using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Blobfish_11
{
    public partial class Engine
    {
        public EvalResult eval(Position pos, int baseDepth)
        {
            List<Move> moves =  allValidMoves(pos);
            EvalResult result = new EvalResult();

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
                List<SecureDouble> allEvals = new List<SecureDouble>();
                double bestValue = pos.whiteToMove ? double.NegativeInfinity : double.PositiveInfinity;
                foreach (Move currentMove in moves)
                {
                    SecureDouble newDouble = new SecureDouble();
                    allEvals.Add(newDouble);
                    Thread thread = new Thread(delegate ()
                    {
                        threadStart(currentMove.execute(pos), baseDepth - 1, !pos.whiteToMove, newDouble) ;
                    });
                    thread.Start();
                }
                Thread.Sleep(100);
                for (int i = 0; i < allEvals.Count; i++)
                {
                    SecureDouble threadResult = allEvals[i];
                    try
                    {
                        threadResult.mutex.WaitOne();
                        double value = threadResult.value;
#pragma warning disable CS1718 // Comparison made to same variable
                        if (value != value) //Kollar om talet är odefinierat.
#pragma warning restore CS1718 // Comparison made to same variable
                        {//Om resultatet inte hunnit beräknas.
                            Thread.Sleep(100);
                            i--;
                        }
                        else
                        { //Om resultatet är klart.
                            if (pos.whiteToMove)
                            {
                                bestValue = Math.Max(bestValue, value);
                            }
                            else
                            {
                                bestValue = Math.Min(bestValue, value);
                            }
                            
                            //TODO: Gör finare
                            if(bestValue == value)
                            {
                                result.bestMove = moves[i];
                            }
                        }
                    }
                    finally
                    {
                        threadResult.mutex.ReleaseMutex();
                    }
                }
                result.evaluation = bestValue;
                result.allEvals = allEvals;
            }

            result.allMoves = moves;
            return result;
        }
        public void threadStart(Position pos, int depth, bool whiteToMove, SecureDouble ansPlace)
        {
            double value = alphaBeta(pos, depth, double.NegativeInfinity, double.PositiveInfinity, whiteToMove);
            try
            {
                ansPlace.mutex.WaitOne();
                ansPlace.value = value;
            }
            finally
            {
                ansPlace.mutex.ReleaseMutex();
            }
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
        public int decisiveResult(Position pos, List<Move> moves)
        {
            //TODO: Gör om funktion. Märkliga argument.
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
            if (depth <= 0)
                return numericEval(pos);

            List<Move> moves = allValidMoves(pos);
            if (moves.Count == 0)
                return decisiveResult(pos, moves);

            if (whiteToMove)
            {
                double value = double.NegativeInfinity;
                foreach (Move currentMove in moves)
                {
                    Position newPos = currentMove.execute(pos);
                    if (extraDepth(currentMove, pos.board, depth))
                    {
                        value = Math.Max(value, alphaBeta(currentMove.execute(pos), depth, alpha, beta, false));
                    }
                    else
                    {
                        value = Math.Max(value, alphaBeta(currentMove.execute(pos), depth - 1, alpha, beta, false));
                    }
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
                    if (extraDepth(currentMove, pos.board, depth))
                    {
                        value = Math.Min(value, alphaBeta(currentMove.execute(pos), depth, alpha, beta, true));
                    }
                    else
                    {
                        value = Math.Min(value, alphaBeta(currentMove.execute(pos), depth - 1, alpha, beta, true));
                    }
                    beta = Math.Min(beta, value);
                    if (beta <= alpha)
                        break; //Pruning
                }
                return value;
            }
        }
        private bool extraDepth(Move move, char[,] board, int currentDepth)
        {
            return currentDepth < 2 && move.isCapture(board);
        }
    }
}
