﻿using System;
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
        public SecureFloat cancelFlag = new SecureFloat(0f);
        public SecureFloat moveNowFlag = new SecureFloat(0f);
        public EvalResult eval(Position pos, int minDepth)
        {

            //Om minDepth är -1, skall datorn själv bestämma djup.
            if (minDepth == -1)
            {
                minDepth = automaticDepth(pos);
            }
            minimumDepth = (sbyte)minDepth;
            maximumDepth = (sbyte)(minDepth + depthExtend);

            List<Move> moves = allValidMoves(pos, true);
            EvalResult result = new EvalResult();

            int gameResult = decisiveResult(pos, moves);
            if (gameResult != -2)
            {
                result.evaluation = gameResult;
                result.allMoves = new List<Move>();
                result.allEvals = null;
                return result; //Ställningen är avgjord.
            }
            else
            {
                List<SecureFloat> allEvals = new List<SecureFloat>();
                float bestValue = pos.whiteToMove ? float.NegativeInfinity : float.PositiveInfinity;

                if (moves.Count == 1)
                {
                    //Spela forcerande drag omedelbart?
                    EvalResult res = eval(moves[0].execute(pos), minDepth-1);
                    result.evaluation = adjustCheckmateEval(res.evaluation, 1);
                    result.allMoves = moves;
                    result.bestMove = moves[0];
                    return result;
                }

                // Ökar minimum-djupet om antalet tillgängliga drag är färre än de som anges
                // i moveIncreaseLimits, vilka återfinns i EngineData.
                foreach (int item in moveIncreaseLimits)
                {
                    if (moves.Count <= item) minDepth++;
                    else break;
                }


                SecureFloat globalAlpha = new SecureFloat();
                globalAlpha.setValue(float.NegativeInfinity);
                SecureFloat globalBeta = new SecureFloat();
                globalBeta.setValue(float.PositiveInfinity);
                List<Thread> threadList = new List<Thread>();

                SecureFloat bestMove = new SecureFloat(0);
                for(int i = 0;i<moves.Count;i++)
                {
                    Move currentMove = moves[i];
                    SecureFloat newFloat = new SecureFloat();
                    int iCopy = i;
                    allEvals.Add(newFloat);

                    Thread thread = new Thread(delegate ()
                    {
                        threadStart(currentMove.execute(pos), iCopy, bestMove, newFloat, globalAlpha, globalBeta);
                    });
                    thread.Name = currentMove.toString(pos);
                    thread.Start();
                    threadList.Add(thread);

                }
                Thread.Sleep(sleepTime);
                for (int i = 0; i < allEvals.Count; i++)
                {
                    SecureFloat threadResult = allEvals[i];
                    float value = threadResult.getValue();
#pragma warning disable CS1718 // Comparison made to same variable
                    if (value != value) //Kollar om talet är odefinierat.
#pragma warning restore CS1718 // Comparison made to same variable
                    {
                        //Om resultatet inte hunnit beräknas.
                        if(cancelFlag.getValue() != 0)
                        {
                            result.bestMove = null;
                            result.evaluation = float.NaN;
                            result.allEvals = null;
                            return result;
                        }
                        Thread.Sleep(sleepTime);
                        i--;
                    }
                    else
                    {
                        //Om resultatet är klart.
                        result.bestMove = moves[(int)bestMove.getValue()];
                    }
                }
                if (pos.whiteToMove)
                    result.evaluation = globalAlpha.getValue();
                else
                    result.evaluation = globalBeta.getValue();
                result.allEvals = allEvals;
            }

            result.allMoves = moves;
            return result;
        }
        public void threadStart(Position pos, int moveIndex, SecureFloat bestMove,
            SecureFloat ansPlace, SecureFloat globalAlpha, SecureFloat globalBeta)
        {
            float value = alphaBeta(pos, 1, globalAlpha, globalBeta, false);
            ansPlace.setValue(value);
                if (!pos.whiteToMove)
            {
                if(value > globalAlpha.getValue())
                {
                    globalAlpha.setValue(value);
                    bestMove.setValue(moveIndex);
                }
            }
            else
            {
                if (value < globalBeta.getValue())
                {
                    globalBeta.setValue(value);
                    bestMove.setValue(moveIndex);
                }
            }
        }
        private float alphaBeta(Position pos, sbyte depth, FloatContainer alphaContainer, FloatContainer betaContainer, bool forceBranching)
        {
            if (cancelFlag.getValue() != 0)
                return 0f;

            if (depth >= minimumDepth && !forceBranching)
                return numericEval(pos);

            if (depth >= maximumDepth || moveNowFlag.getValue() != 0)
            {
                return numericEval(pos);
            }
            List<Move> moves = allValidMoves(pos, true);
            if (moves.Count == 0)
                return decisiveResult(pos, moves);

            OrdinaryFloat alpha = new OrdinaryFloat(alphaContainer.getValue());
            OrdinaryFloat beta = new OrdinaryFloat(betaContainer.getValue());


            if (pos.whiteToMove)
            {
                float value = float.NegativeInfinity;
                foreach (Move currentMove in moves)
                {
                    if (betaContainer is SecureFloat && betaContainer.getValue() < beta.getValue())
                        beta.setValue(betaContainer.getValue());

                    Position newPos = currentMove.execute(pos);
                    if (extendedDepth(currentMove, pos, depth, moves.Count) || isCheck(newPos))
                    {
                        value = Math.Max(value, alphaBeta(currentMove.execute(pos), (sbyte)(depth + 1), alpha, beta, true));
                    }
                    else
                    {
                        value = Math.Max(value, alphaBeta(currentMove.execute(pos), (sbyte)(depth + 1), alpha, beta, false));
                    }
                    alpha.setValue(Math.Max(alpha.getValue(), value));
                    if (alpha.getValue() >= beta.getValue())
                    {
                        if (alphaContainer is SecureFloat)
                            return float.PositiveInfinity;
                        else
                            break; //Pruning
                    }
                }
                value = adjustCheckmateEval(value, depth);
                return value;
            }
            else
            {
                float value = float.PositiveInfinity;
                foreach (Move currentMove in moves)
                {
                    if (alphaContainer is SecureFloat && alphaContainer.getValue() > alpha.getValue())
                        alpha.setValue(alphaContainer.getValue());

                    Position newPos = currentMove.execute(pos);
                    if (extendedDepth(currentMove, pos, depth, moves.Count) || isCheck(newPos))
                    {
                        value = Math.Min(value, alphaBeta(currentMove.execute(pos), (sbyte)(depth + 1), alpha, beta, true));
                    }
                    else
                    {
                        value = Math.Min(value, alphaBeta(currentMove.execute(pos), (sbyte)(depth + 1), alpha, beta, false));
                    }
                    beta.setValue(Math.Min(beta.getValue(), value));
                    if (beta.getValue() <= alpha.getValue())
                    {
                        if (betaContainer is SecureFloat)
                            return float.NegativeInfinity;
                        else
                            break; //Pruning
                    }
                }
                value = adjustCheckmateEval(value, depth);
                return value;
            }
        }
        public float numericEval(Position pos)
        {
            /* 
             * [rank, line]
             * rank==0    -> rad 8.
             * rank==7    -> rad 1.
             * line==0    -> a-linjen.
             * line==7    -> h-linjen.
             */

            int[] numberOfPawns = new int[2];
            float[] pawnPosFactor = { 1f, 1f };
            sbyte[,] pawns = new sbyte[2, 8]; //0=svart, 1=vit.
            //TODO: ordna med färgkomplex.
            char[,] board = pos.board;

            for (sbyte rank = 0; rank < 8; rank++)
            {
                for (sbyte line = 0; line < 8; line++)
                {
                    switch (board[rank, line])
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
                    }
                }
            }

            //Grov uppskattning av moståndarens tunga pjäser.
            //0 = svart, 1 = vit.
            int[] heavyMaterial = new int[] { 0, 0 };

            bool[] bishopColors = new bool[4] { false, false, false, false }; //WS, DS, ws, ds
            float pieceValue = 0;
            for (sbyte rank = 0; rank < 8; rank++)
            {
                for (sbyte line = 0; line < 8; line++)
                {
                    switch (board[rank, line])
                    {
                        case 'n':
                            pieceValue -= pieceValues[1] * knight[rank, line];
                            heavyMaterial[0] += 3;
                            break;
                        case 'N':
                            pieceValue += pieceValues[1] * knight[rank, line];
                            heavyMaterial[1] += 3;
                            break;

                        case 'b':
                            pieceValue -= pieceValues[2] * bishop[rank, line];
                            heavyMaterial[0] += 3;
                            if ((rank + line) % 2 == 0)
                                bishopColors[2] = true; //Svart löpare på vitt fält
                            else
                                bishopColors[3] = true; //Svart löpare på svart fält
                            break;
                        case 'B':
                            pieceValue += pieceValues[2] * bishop[rank, line];
                            heavyMaterial[1] += 3;
                            if ((rank + line) % 2 == 0)
                                bishopColors[0] = true; //Vit löpare på vitt fält
                            else
                                bishopColors[1] = true; //Vit löpare på svart fält
                            break;

                        case 'r':
                            float val = pieceValues[3] * rook[rank, line];
                            if(pawns[0, line] == 0) //Om tornet står på en öppen eller halvöppen linje.
                            {
                                if(pawns[1, line] == 0)
                                {
                                    val *= rookOnOpenLineCoefficient;
                                }
                                else
                                {
                                    val *= rookOnSemiOpenLineCoefficient;
                                }
                            }
                            pieceValue -= val;
                            heavyMaterial[0] += 5;
                            break;
                        case 'R':
                            val = pieceValues[3] * rook[rank, line];
                            if (pawns[1, line] == 0) //Om tornet står på en öppen eller halvöppen linje.
                            {
                                if (pawns[0, line] == 0)
                                {
                                    val *= rookOnOpenLineCoefficient;
                                }
                                else
                                {
                                    val *= rookOnSemiOpenLineCoefficient;
                                }
                            }
                            pieceValue += val;
                            heavyMaterial[1] += 5;
                            break;
                        
                        case 'q':
                            pieceValue -= pieceValues[4] * queen[rank, line];
                            heavyMaterial[0] += 9;
                            break;
                        case 'Q':
                            pieceValue += pieceValues[4] * queen[rank, line];
                            heavyMaterial[1] += 9;
                            break;
                        default:
                            break;
                    }
                }
            }

            float kingSafteyDifference = kingSaftey(pos, true, heavyMaterial[0])
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
            float pawnValue = pieceValues[0] * evalPawns(numberOfPawns, pawnPosFactor, pawns);
            float toMoveAdvantage = pos.whiteToMove ? toMoveValue : -toMoveValue;
            // TODO: Variera värdet av att vara vid draget?
            return pieceValue + pawnValue + kingSafteyDifference + toMoveAdvantage;
        }
        private float kingSaftey(Position pos, bool forWhite, int oppHeavyMaterial)
        {
            float defenceAccumulator = 0;
            sbyte direction = forWhite ? (sbyte) -1 : (sbyte) 1;
            Square kingSquare = forWhite ? pos.kingPositions[1] : pos.kingPositions[0];
            Square currentSquare = new Square();
            for (int i = 0; i < 3; i++)
            {
                for (int j = -2; j < 3; j++)
                {
                    currentSquare.rank = (sbyte)(kingSquare.rank + (direction * i));
                    currentSquare.line = (sbyte)(kingSquare.line + j);
                    if (!validSquare(currentSquare)){
                        continue;
                    }
                    char currentPiece = pos.board[currentSquare.rank, currentSquare.line];
                    float defValue = defenceValueOf(currentPiece);
                    if(defValue == 0f)
                    {
                        continue;
                    }
                    float defCoeff = defence[2-i, j + 2];
                    float finDefValue = defValue * defCoeff;

                    //float DefContribution = (finDefValue * (oppHeavyMaterial - endgameLimit)) / kingSafteyDivisor;

                    defenceAccumulator += finDefValue;
                }
            }
            if(defenceAccumulator > safteySoftCap)
            {
                //Halverar nyttan av kungsförsvar efter en viss gräns.
                defenceAccumulator -= (defenceAccumulator - safteySoftCap) / 2;
            }
            float safteyValue = (defenceAccumulator * (oppHeavyMaterial - endgameLimit)* kingSafteyCoefficient) /200f;

            float kingCoefficient;
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
        private float defenceValueOf(char piece)
        {
            if (piece == '\0') return 0;
            if (piece > 'a') 
                piece = (char)(piece - ('a' - 'A')); //Gör om tecknet till stor bokstav.
            switch (piece)
            {
                case 'P': return pieceDefenceValues[0];
                case 'N': return pieceDefenceValues[1];
                case 'B': return pieceDefenceValues[2];
                case 'R': return pieceDefenceValues[3];
                case 'Q': return pieceDefenceValues[4];
                case 'K': return 0;
                default: 
                    throw new Exception("Okänt tecken!");
            }
        }
        private float evalPawns(int[] numberOfPawns, float[] posFactor, sbyte[,] pawns)
        {
            float[] pawnValues = new float[2];
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
            if (currentDepth <= minimumDepth - 2)
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
        private float adjustCheckmateEval(float evaluation, sbyte movesToMate)
        {
            if (evaluation >= 2000)
                return evaluation - movesToMate;
            else if (evaluation <= -2000)
                return evaluation + movesToMate;
            else return evaluation;
        }
        public bool mateableMaterial(char[,] board)
        {
            //TODO: Fixa för mattbart material för de respektive spelarna.
            bool anyKnight = false;
            bool anyLightSquaredBishop = false;
            bool anyDarkSquaredBishop = false;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {

                switch (myToUpper(board[i,j]))
                {
                    case 'P': return true;
                    case 'Q': return true;
                    case 'R': return true;
                    case 'N': if (anyKnight) return true; else anyKnight = true; break;
                    case 'B': 
                            if ((i + j) % 2 == 0) anyLightSquaredBishop = true;
                            else anyDarkSquaredBishop = true;
                            if (anyDarkSquaredBishop && anyLightSquaredBishop) return true;
                            break;
                    default: break;
                    }
                }
            }
            if (anyKnight && (anyDarkSquaredBishop || anyLightSquaredBishop)) return true;
                return false;
        }
        private char myToUpper(char piece)
        {
            if (piece > 'a')
            {
                return (char)(piece - ('a' - 'A')); //Gör om tecknet till stor bokstav.
            }
            return piece;
        }
        private int automaticDepth(Position pos)
        {
            double materialSum = 0;
            double weightForPawnOnLastRank = calculationWeights[4] * 0.75f;

            for (int rank = 0; rank < 8; rank++)
            {
                for (int line = 0; line < 8; line++)
                {
                    char piece = pos.board[rank, line];
                    if (piece == 'P' || piece == 'p')
                    {
                        if ((piece == 'P' && rank == 1) || (piece == 'p' && rank == 6))
                        {
                            //Bönder som är ett steg ifrån att promotera.
                            materialSum += weightForPawnOnLastRank;
                        }
                        else
                        {
                            materialSum += calculationWeights[0];
                        }
                    }
                    else if (piece == 'N' || piece == 'n')
                        materialSum += calculationWeights[1];
                    else if (piece == 'B' || piece == 'b')
                        materialSum += calculationWeights[2];
                    else if (piece == 'R' || piece == 'r')
                        materialSum += calculationWeights[3];
                    if (piece == 'Q' || piece == 'q')
                        materialSum += calculationWeights[4];
                }
            }
            if (materialSum < 11)
                return 7;
            else if (materialSum <= calculationWeights[4])
                return 6;
            else if (materialSum < 25)
                return 5;
            else return 4;
        }
    }
}
