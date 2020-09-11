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
            List<Move> moves = allValidMoves(pos, true);
            EvalResult result = new EvalResult();

            int gameResult = decisiveResult(pos, moves);
            if (gameResult != -2)
            {
                result.evaluation = (double)gameResult;
                result.allMoves = new List<Move>();
                result.allEvals = null;
                return result; //Ställningen är avgjord.
            }
            else
            {
                List<SecureDouble> allEvals = new List<SecureDouble>();
                double bestValue = pos.whiteToMove ? double.NegativeInfinity : double.PositiveInfinity;

                if (moves.Count == 1)
                {
                    EvalResult res = eval(moves[0].execute(pos), minDepth);
                    result.evaluation = evaluationStep(res.evaluation);
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
                    SecureDouble newDouble = new SecureDouble();
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
                    SecureDouble threadResult = allEvals[i];
                    double value = threadResult.getValue();
#pragma warning disable CS1718 // Comparison made to same variable
                    if (value != value) //Kollar om talet är odefinierat.
#pragma warning restore CS1718 // Comparison made to same variable
                    {
                        //Om resultatet inte hunnit beräknas.
                        if(cancelFlag.getValue() != 0)
                        {
                            abortAll(threadList);
                            result.bestMove = null;
                            result.evaluation = double.NaN;
                            result.allEvals = null;
                            Thread.Sleep(10); //För att ge övriga trådar chans att stanna.
                            return result;
                        }
                        Thread.Sleep(sleepTime);
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
                        if (bestValue == value)
                        {
                            result.bestMove = moves[i];
                        }
                    }
                }
                result.evaluation = bestValue;
                result.allEvals = allEvals;
            }

            result.allMoves = moves;
            return result;
        }
        public void threadStart(Position pos, sbyte depth, SecureDouble ansPlace, SecureDouble globalAlpha, SecureDouble globalBeta)
        {
            double value = alphaBeta(pos, depth, globalAlpha, globalBeta, false);
            ansPlace.setValue(value);
            if (!pos.whiteToMove)
            {
                globalAlpha.setValue(Math.Max(globalAlpha.getValue(), value));
            }
            else
            {
                globalBeta.setValue(Math.Min(globalBeta.getValue(), value));
            }
        }
        private double alphaBeta(Position pos, sbyte depth, DoubleContainer alphaContainer, DoubleContainer betaContainer, bool forceBranching)
        {
            string moveName = ""; //Endast i debug-syfte
            if (depth <= 0 && !forceBranching)
                return numericEval(pos);

            if (depth <= -8) //Maximalt antal forcerande drag som får ta plats i slutet av en variant.
            {
                return numericEval(pos);
            }
            List<Move> moves = allValidMoves(pos, true);
            if (moves.Count == 0)
                return decisiveResult(pos, moves);

            OrdinaryDouble alpha = new OrdinaryDouble(alphaContainer.getValue());
            OrdinaryDouble beta = new OrdinaryDouble(betaContainer.getValue());


            if (pos.whiteToMove)
            {
                double value = double.NegativeInfinity;
                foreach (Move currentMove in moves)
                {
                    if (betaContainer is SecureDouble && betaContainer.getValue() < beta.getValue())
                        beta.setValue(betaContainer.getValue());

                    //Endast i debug-syfte
                    moveName = currentMove.toString(pos.board);

                    Position newPos = currentMove.execute(pos);
                    if (extendedDepth(currentMove, pos, depth, moves.Count) || isCheck(newPos))
                    {
                        value = Math.Max(value, alphaBeta(currentMove.execute(pos), (sbyte)(depth - 1), alpha, beta, true));
                    }
                    else
                    {
                        value = Math.Max(value, alphaBeta(currentMove.execute(pos), (sbyte)(depth - 1), alpha, beta, false));
                    }
                    alpha.setValue(Math.Max(alpha.getValue(), value));
                    if (alpha.getValue() >= beta.getValue())
                    {
                        if (alphaContainer is SecureDouble)
                            return double.PositiveInfinity;
                        else
                            break; //Pruning

                    }
                }
                value = evaluationStep(value);
                return value;
            }
            else
            {
                double value = double.PositiveInfinity;
                foreach (Move currentMove in moves)
                {
                    if (alphaContainer is SecureDouble && alphaContainer.getValue() > alpha.getValue())
                        alpha.setValue(alphaContainer.getValue());
                    //Endast i debug-syfte
                    moveName = currentMove.toString(pos.board);

                    Position newPos = currentMove.execute(pos);
                    if (extendedDepth(currentMove, pos, depth, moves.Count) || isCheck(newPos))
                    {
                        value = Math.Min(value, alphaBeta(currentMove.execute(pos), (sbyte)(depth - 1), alpha, beta, true));
                    }
                    else
                    {
                        value = Math.Min(value, alphaBeta(currentMove.execute(pos), (sbyte)(depth - 1), alpha, beta, false));
                    }
                    beta.setValue(Math.Min(beta.getValue(), value));
                    if (beta.getValue() <= alpha.getValue())
                    {
                        if (betaContainer is SecureDouble)
                            return double.NegativeInfinity;
                        else
                            break; //Pruning
                    }
                }
                value = evaluationStep(value);
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

                    //Tag bort denna
                    double DefContribution = (finDefValue * (oppHeavyMaterial - endgameLimit)) / kingSafteyDivisor;

                    defenceAccumulator += finDefValue;
                }
            }
            double safteyValue = (defenceAccumulator * (oppHeavyMaterial - endgameLimit)) /kingSafteyDivisor;

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
