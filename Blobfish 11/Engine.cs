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
        public SecureDouble cancelFlag = new SecureDouble(0);
        public EvalResult eval(Position pos, int minDepth)
        {
            List<Move> moves = allValidMoves(pos);
            EvalResult result = new EvalResult();

            int gameResult = decisiveResult(pos, moves);
            if (gameResult != -2)
            {
                result.evaluation.eval = (double)gameResult;
                result.allMoves = new List<Move>();
                result.allEvals = null;
                return result; //Ställningen är avgjord.
            }
            else
            {
                List<col> allEvals = new List<col>();
                col bestValue = pos.whiteToMove ? new col(double.NegativeInfinity) : new col(double.PositiveInfinity);

                if (moves.Count == 1)
                {
                    EvalResult res = eval(moves[0].execute(pos), minDepth);
                    result.evaluation.eval = evaluationStep(res.evaluation.eval);
                    result.allMoves = moves;
                    result.bestMove = moves[0];
                    return result;
                    //Forcerande drag beräknas alltid ytterligare ett drag.
                    //Bör testas. Skulle eventuellt kunna orsaka buggar i extremfall.
                }

                // Ökar minimum-djupet om antalet tillgängliga drag är färre än de som anges
                // i moveIncreaseLimits, vilka återfinns i EngineData.
                foreach (int item in moveIncreaseLimits)
                {
                    if (moves.Count <= item) minDepth++;
                    else break;
                }

                //TODO: Öka längden om antalet tunga pjäser är få.

                SecureDouble globalAlpha = new SecureDouble();
                globalAlpha.setValue(double.NegativeInfinity);
                SecureDouble globalBeta = new SecureDouble();
                globalBeta.setValue(double.PositiveInfinity);
                List<Thread> threadList = new List<Thread>();
                foreach (Move currentMove in moves)
                {
                    col newDouble = new col(double.NaN);
                    allEvals.Add(newDouble);
                    Thread thread = new Thread(delegate ()
                    {
                        threadStart(currentMove.execute(pos), (sbyte)(minDepth - 1), newDouble, globalAlpha, globalBeta);
                    });
                    thread.Name = currentMove.toString(pos.board);
                    thread.Start();
                    threadList.Add(thread);
                }
                Thread.Sleep(sleepTime);
                for (int i = 0; i < allEvals.Count; i++)
                {
                    col threadResult = allEvals[i];
#pragma warning disable CS1718 // Comparison made to same variable
                    if (threadResult.eval != threadResult.eval) //Kollar om talet är odefinierat.
#pragma warning restore CS1718 // Comparison made to same variable
                    {
                        //Om resultatet inte hunnit beräknas.
                        if(cancelFlag.getValue() != 0)
                        {
                            abortAll(threadList);
                            result.bestMove = null;
                            result.evaluation.eval = double.NaN;
                            result.allEvals = null;
                            return result;
                        }
                        Thread.Sleep(sleepTime);
                        i--;
                    }
                    else
                    { //Om resultatet är klart.
                        if (pos.whiteToMove)
                        {
                            if(bestValue.eval < threadResult.eval)
                            {
                                bestValue = threadResult;
                            }
                        }
                        else
                        {
                            if (bestValue.eval > threadResult.eval)
                            {
                                bestValue = threadResult;
                            }
                        }

                        //TODO: Gör finare
                        if (bestValue.eval == threadResult.eval)
                        {
                            result.bestMove = moves[i];
                        }
                    }
                }
                result.evaluation = bestValue;
                result.allEvals = allEvals;
            }

            result.allMoves = moves;
            //result.evaluation.name = result.bestMove.toString(pos.board) + " " + result.evaluation.name;
            return result;
        }
        public void threadStart(Position pos, sbyte depth, col ansPlace, SecureDouble globalAlpha, SecureDouble globalBeta)
        {
            col value = alphaBeta(pos, depth, globalAlpha, globalBeta, false);
            ansPlace.eval = (value.eval);
            ansPlace.name = value.name;
            if (!pos.whiteToMove)
            {
                globalAlpha.setValue(Math.Max(globalAlpha.getValue(), value.eval));
            }
            else
            {
                globalBeta.setValue(Math.Min(globalBeta.getValue(), value.eval));
            }
        }
        private col alphaBeta(Position pos, sbyte depth, DoubleContainer alphaContainer, DoubleContainer betaContainer, bool forceBranching)
        {
            if (depth <= 0 && !forceBranching)
                return new col(numericEval(pos));

            if (depth <= -8) //Maximalt antal forcerande drag som får ta plats i slutet av en variant.
            {
                return new col(numericEval(pos));
            }
            List<Move> moves = allValidMoves(pos);
            if (moves.Count == 0)
                return new col(decisiveResult(pos, moves));

            OrdinaryDouble alpha = new OrdinaryDouble(alphaContainer.getValue());
            OrdinaryDouble beta = new OrdinaryDouble(betaContainer.getValue());


            if (pos.whiteToMove)
            {
                col value = new col(double.NegativeInfinity);
                foreach (Move currentMove in moves)
                {
                    Position newPos = currentMove.execute(pos);
                    if (extendedDepth(currentMove, pos, depth, moves.Count) || isCheck(newPos))
                    {
                        col ret = alphaBeta(newPos, (sbyte)(depth - 1), alpha, beta, true);
                        if(ret.eval >= value.eval)
                        {
                            value = ret;
                            value.name = currentMove.toString(pos.board) + " " + value.name;
                        }
                    }
                    else
                    {
                        col ret = alphaBeta(newPos, (sbyte)(depth - 1), alpha, beta, false);
                        if (ret.eval >= value.eval)
                        {
                            value = ret;
                            value.name = currentMove.toString(pos.board) + " " + value.name;
                        }
                    }
                    alpha.setValue(Math.Max(alpha.getValue(), value.eval));
                    if (alpha.getValue() >= beta.getValue())
                    {
                        if (alphaContainer is SecureDouble)
                            return new col(double.PositiveInfinity);
                        else
                            break; //Pruning

                    }
                }
                value.eval = evaluationStep(value.eval);
                return value;
            }
            else
            {
                col value = new col(double.PositiveInfinity);
                foreach (Move currentMove in moves)
                {
                    Position newPos = currentMove.execute(pos);
                    if (extendedDepth(currentMove, pos, depth, moves.Count) || isCheck(newPos))
                    {
                        col ret = alphaBeta(newPos, (sbyte)(depth - 1), alpha, beta, true);
                        if (ret.eval < value.eval)
                        {
                            value = ret;
                            value.name = currentMove.toString(pos.board) + " " + value.name;
                        }
                    }
                    else
                    {
                        col ret = alphaBeta(newPos, (sbyte)(depth - 1), alpha, beta, false);
                        if (ret.eval < value.eval)
                        {
                            value = ret;
                            value.name = currentMove.toString(pos.board) + " " + value.name;
                        }
                    }
                    beta.setValue(Math.Min(beta.getValue(), value.eval));
                    if (beta.getValue() <= alpha.getValue())
                    {
                        if (betaContainer is SecureDouble)
                            return new col(double.NegativeInfinity);
                        else
                            break; //Pruning
                    }
                }
                value.eval = evaluationStep(value.eval);
                return value;
            }
        }
        public double numericEval(Position pos)
        {
            /* 
             * [rank, line]
             * rank==0       -> rad 8.
             * rank==7       -> rad 1.
             * line==0    -> a-linjen.
             * line==7    -> h-linjen.
             */
            int[] numberOfPawns = new int[2];
            double[] pawnPosFactor = { 1f, 1f };
            int[] heavyMaterial = new int[] { 0, 0 }; //Grov uppskattning av moståndarens tunga pjäser.
            sbyte[,] pawns = new sbyte[2, 8]; //0=svart, 1=vit.
            //bool whiteSquare = true; //TODO: ordna med färgkomplex.
            bool[] bishopColors = new bool[4] { false, false, false, false }; //WS, DS, ws, ds
            double pieceValue = 0;
            for (sbyte rank = 0; rank < 8; rank++)
            {
                for (sbyte line = 0; line < 8; line++)
                {
                    switch (pos.board[rank, line])
                    {
                        case 'p':
                            numberOfPawns[0]++;
                            pawns[0, line]++;
                            pawnPosFactor[0] += pawn[0, rank, line];
                            break;

                        case 'P':
                            numberOfPawns[1]++;
                            pawns[1, line]++;
                            pawnPosFactor[1] += pawn[1, rank, line];
                            break;

                        case 'n':
                            pieceValue -= pieceValues[0] * knight[rank, line];
                            heavyMaterial[1] += 3;
                            break;

                        case 'N':
                            pieceValue += pieceValues[0] * knight[rank, line];
                            heavyMaterial[0] += 3;
                            break;

                        case 'b':
                            pieceValue -= pieceValues[1] * bishop[rank, line];
                            heavyMaterial[1] += 3;
                            if ((rank + line) % 2 == 0)
                                bishopColors[2] = true; //Svart löpare på vitt fält
                            else
                                bishopColors[3] = true; //Svart löpare på svart fält
                            break;

                        case 'B':
                            pieceValue += pieceValues[1] * bishop[rank, line];
                            heavyMaterial[0] += 3;
                            if ((rank + line) % 2 == 0)
                                bishopColors[0] = true; //Vit löpare på vitt fält
                            else
                                bishopColors[1] = true; //Vit löpare på svart fält
                            break;

                        case 'r':
                            pieceValue -= pieceValues[2] * rook[rank, line];
                            heavyMaterial[1] += 5;
                            break;
                        case 'R':
                            pieceValue += pieceValues[2] * rook[rank, line];
                            heavyMaterial[0] += 5;
                            break;

                        case 'k':
                            pos.kingPositions[0] = new Square(rank, line);
                            break;

                        case 'K':
                            pos.kingPositions[1] = new Square(rank, line);
                            break;

                        case 'q':
                            pieceValue -= pieceValues[3] * queen[rank, line];
                            heavyMaterial[1] += 9;
                            break;

                        case 'Q':
                            pieceValue += pieceValues[3] * queen[rank, line];
                            heavyMaterial[0] += 9;
                            break;
                        default:
                            break;
                    }
                }
            }

            double kingSafteyDifference = kingSaftey(pos, true, heavyMaterial[0])
                - kingSaftey(pos, false, heavyMaterial[1]);

            if (bishopColors[0] && bishopColors[1])
            {
                pieceValue += bishopPairValue;
            }
            if (bishopColors[2] && bishopColors[3])
            {
                pieceValue -= bishopPairValue;
            }

            for (sbyte i = 0; i < 2; i++)
            {
                if (numberOfPawns[i] == 0)
                    pawnPosFactor[i] = 0f;
                else
                    pawnPosFactor[i] /= numberOfPawns[i];
            }
            double pawnValue = evalPawns(numberOfPawns, pawnPosFactor, pawns);
            return pieceValue + pawnValue + kingSafteyDifference;
        }
        private double kingSaftey(Position pos, bool forWhite, int oppHeavyMaterial)
        {
            double defenceAccumulator = 0;
            sbyte direction = forWhite ? (sbyte) -1 : (sbyte) 1;
            Square kingSquare = forWhite ? pos.kingPositions[1] : pos.kingPositions[0];
            for (int i = 0; i < 3; i++)
            {
                for (int j = -2; j < 3; j++)
                {
                    Square currentSquare = new Square(kingSquare.rank + (direction * i), kingSquare.line + j);
                    if (!validSquare(currentSquare)){
                        continue;
                    }
                    char currentPiece = pos.board[currentSquare.rank, currentSquare.line];
                    double defValue = defenceValueOf(currentPiece);
                    if(defValue == 0)
                    {
                        continue;
                    }
                    double defCoeff = defence[2-i, j + 2];
                    double finDefValue = defValue * defCoeff;

                    defenceAccumulator += finDefValue;
                }
            }
            double safteyValue = (defenceAccumulator * (oppHeavyMaterial - endgameLimit)) / kingSafteyDivisor;

            double kingCoefficient;
            if (oppHeavyMaterial > endgameLimit)
            {
                kingCoefficient =   king[0, kingSquare.rank, kingSquare.line];
            }
            else
            {
                kingCoefficient =  king[1, kingSquare.rank, kingSquare.line];
                return kingCoefficient * kingValue;
            }
            return kingCoefficient * safteyValue;
        }
        private double defenceValueOf(char piece)
        {
            switch (piece.ToString().ToUpper())
            {
                case "": return 0; //Kolla om denna fungerar.
                case "P": return defenceValues[0];
                case "N": return defenceValues[1];
                case "B": return defenceValues[2];
                case "R": return defenceValues[3];
                case "Q": return defenceValues[4];
                default: return 0;
            }
        }
        private double evalPawns(int[] numberOfPawns, double[] posFactor, sbyte[,] pawns)
        {
            double[] pawnValues = new double[2];
            for (sbyte c = 0; c < 2; c++)
            {
                sbyte neighbours = 0;
                sbyte lines = 0;
                if (pawns[c, 0] > 0) lines++; //specialfall för a-linjen.
                if (pawns[c, 1] > 0) neighbours += pawns[c, 0];
                for (sbyte i = 1; i < 7; i++) //från b till g sista. 
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
                Square relevantKingSquare = pos.whiteToMove ? pos.kingPositions[1] : pos.kingPositions[0];
                bool isCheck = isControlledBy(pos, relevantKingSquare, !pos.whiteToMove);
                if (isCheck)
                {
                    if (pos.whiteToMove) return -2000;
                    else return 2000;
                }
                else return 0; //Patt
            }

            if (pos.halfMoveClock >= 100)
            {
                return 0; //Femtiodragsregeln.
            }

            return -2;
        }
        private bool extendedDepth(Move move, Position pos, int currentDepth, int numberOfAvailableMoves)
        {
            if (currentDepth >= 2)
                return false;
            else if (numberOfAvailableMoves == 1)
            {
                return true;
            }
            else if (move.isCapture(pos.board))
            {
                return true;
            }
            else
            {
                Square relevantKingSquare = pos.whiteToMove ? pos.kingPositions[0] : pos.kingPositions[1];
                bool isCheck = isControlledBy(pos, relevantKingSquare, pos.whiteToMove);
                return isCheck;
            }
        }
        private bool isCheck(Position pos)
        {
            Square relevantKingSquare = pos.whiteToMove ? pos.kingPositions[1] : pos.kingPositions[0];
            bool isCheck = isControlledBy(pos, relevantKingSquare, !pos.whiteToMove);
            return isCheck;
        }
        private double evaluationStep(double value)
        {
            //Ökar antal drag till matt, ifall evalueringen är forcerad matt.
            if (value > 1000)
                return value - 1;
            else if (value < -1000)
                return value + 1;
            else return value;
        }
        private void abortAll(List<Thread> threadList)
        {
            foreach (Thread item in threadList)
            {
                if (item.IsAlive)
                    item.Abort();
            }
        }
    }
}
