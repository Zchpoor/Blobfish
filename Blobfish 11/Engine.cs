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
        public SecureFloat cancelFlag = new SecureFloat(0f);
        public SecureFloat moveNowFlag = new SecureFloat(0f);

        public EngineData Data { get; set; }

        public Engine()
        {
            this.Data = new EngineData();
        }
        public Engine(EngineData data)
        {
            this.Data = data;
        }

        public EvalResult eval(Position pos, int minDepth)
        {

            //Om minDepth är -1, skall datorn själv bestämma djup.
            if (minDepth == -1)
            {
                minDepth = automaticDepth(pos);
            }
            List<Move> moves = PieceMovementHandler.AllValidMoves(pos, true);
            EvalResult result = new EvalResult();

            GameResult gameResult = decisiveResult(pos, moves);
            if (gameResult != GameResult.Undecided)
            {
                result.evaluation = numericEval(gameResult);
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
                    result.evaluation = evaluationStep(res.evaluation);
                    result.allMoves = moves;
                    result.bestMove = moves[0];
                    return result;
                }

                // Ökar minimum-djupet om antalet tillgängliga drag är färre än de som anges
                // i moveIncreaseLimits, vilka återfinns i EngineData.
                foreach (int item in this.Data.MoveIncreaseLimits)
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
                //foreach (Move currentMove in moves)
                {
                    Move currentMove = moves[i];
                    SecureFloat newFloat = new SecureFloat();
                    int iCopy = i;
                    allEvals.Add(newFloat);

                    Thread thread = new Thread(delegate ()
                    {
                        threadStart(currentMove.execute(pos), (sbyte)(minDepth - 1), iCopy, bestMove, newFloat, globalAlpha, globalBeta);
                    });
                    thread.Name = currentMove.toString(pos);
                    thread.Start();
                    threadList.Add(thread);

                }
                //result.bestMove = moves[0];
                Thread.Sleep(this.Data.SleepTime);
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
                            abortAll(threadList);
                            result.bestMove = null;
                            result.evaluation = float.NaN;
                            result.allEvals = null;
                            Thread.Sleep(10); //För att ge övriga trådar chans att stanna.
                            return result;
                        }
                        else if(this.moveNowFlag.getValue() != 0)
                        {
                            abortAll(threadList);
                            Thread.Sleep(10); //För att ge övriga trådar chans att stanna.
                            result.bestMove = moves[(int)bestMove.getValue()];
                            return result;
                        }
                        Thread.Sleep(this.Data.SleepTime);
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
        public void threadStart(Position pos, sbyte depth, int moveIndex, SecureFloat bestMove,
            SecureFloat ansPlace, SecureFloat globalAlpha, SecureFloat globalBeta)
        {
            float value = alphaBeta(pos, depth, globalAlpha, globalBeta, false);
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
            if (depth <= 0 && !forceBranching)
                return numericEval(pos);

            if (depth <= -this.Data.MaximumDepth) //Maximalt antal forcerande drag som får ta plats i slutet av en variant.
            {
                return numericEval(pos);
            }
            List<Move> moves = PieceMovementHandler.AllValidMoves(pos, true);
            if (moves.Count == 0)
                return numericEval(decisiveResult(pos, moves));

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
                        value = Math.Max(value, alphaBeta(currentMove.execute(pos), (sbyte)(depth - 1), alpha, beta, true));
                    }
                    else
                    {
                        value = Math.Max(value, alphaBeta(currentMove.execute(pos), (sbyte)(depth - 1), alpha, beta, false));
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
                value = evaluationStep(value);
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
                        value = Math.Min(value, alphaBeta(currentMove.execute(pos), (sbyte)(depth - 1), alpha, beta, true));
                    }
                    else
                    {
                        value = Math.Min(value, alphaBeta(currentMove.execute(pos), (sbyte)(depth - 1), alpha, beta, false));
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
                value = evaluationStep(value);
                return value;
            }
        }
        private float EvaluateFinalMove(Position pos, float currentEvaluation, Move move)
        {
            if (move.isCapture(pos.board)
                || move is Castle
                || move is Promotion
                || move.from.Equals(pos.kingPositions[0]) //TODO: Effetivisera för fler typer av drag?
                || move.from.Equals(pos.kingPositions[1])
            )
            {
                return numericEval(pos); //Det långsamma sättet, men som alltid fungerar.
            }
            else
            {
                Piece movedPiece = pos[move.from];
                //Beräkna skillnaden som draget gör på ställningen.
                return numericEval(pos);
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
            Piece[,] board = pos.board;

            for (sbyte rank = 0; rank < 8; rank++)
            {
                for (sbyte line = 0; line < 8; line++)
                {
                    switch (board[rank, line])
                    {
                        case Piece.Pawn:
                            numberOfPawns[0]++;
                            pawns[0, line]++;
                            pawnPosFactor[0] += this.Data.Pawn[0, rank, line];
                            break;

                        case (Piece.Pawn | Piece.White):
                            numberOfPawns[1]++;
                            pawns[1, line]++;
                            pawnPosFactor[1] += this.Data.Pawn[1, rank, line];
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
                        case Piece.Knight:
                            pieceValue -= this.Data.PieceValues[1] * this.Data.Knight[rank, line];
                            heavyMaterial[0] += 3;
                            break;
                        case (Piece.Knight | Piece.White):
                            pieceValue += this.Data.PieceValues[1] * this.Data.Knight[rank, line];
                            heavyMaterial[1] += 3;
                            break;
                        case Piece.Bishop:
                            pieceValue -= this.Data.PieceValues[2] * this.Data.Bishop[rank, line];
                            heavyMaterial[0] += 3;
                            if ((rank + line) % 2 == 0)
                                bishopColors[2] = true; //Svart löpare på vitt fält
                            else
                                bishopColors[3] = true; //Svart löpare på svart fält
                            break;
                        case (Piece.Bishop | Piece.White):
                            pieceValue += this.Data.PieceValues[2] * this.Data.Bishop[rank, line];
                            heavyMaterial[1] += 3;
                            if ((rank + line) % 2 == 0)
                                bishopColors[0] = true; //Vit löpare på vitt fält
                            else
                                bishopColors[1] = true; //Vit löpare på svart fält
                            break;

                        case Piece.Rook:
                            float val = this.Data.PieceValues[3] * this.Data.Rook[rank, line];
                            if(pawns[0, line] == 0) //Om tornet står på en öppen eller halvöppen linje.
                            {
                                if(pawns[1, line] == 0)
                                {
                                    val *= this.Data.RookOnOpenLineCoefficient;
                                }
                                else
                                {
                                    val *= this.Data.RookOnSemiOpenLineCoefficient;
                                }
                            }
                            pieceValue -= val;
                            heavyMaterial[0] += 5;
                            break;
                        case (Piece.Rook | Piece.White):
                            val = this.Data.PieceValues[3] * this.Data.Rook[rank, line];
                            if (pawns[1, line] == 0) //Om tornet står på en öppen eller halvöppen linje.
                            {
                                if (pawns[0, line] == 0)
                                {
                                    val *= this.Data.RookOnOpenLineCoefficient;
                                }
                                else
                                {
                                    val *= this.Data.RookOnSemiOpenLineCoefficient;
                                }
                            }
                            pieceValue += val;
                            heavyMaterial[1] += 5;
                            break;
                        
                        case Piece.Queen:
                            pieceValue -= this.Data.PieceValues[4] * this.Data.Queen[rank, line];
                            heavyMaterial[0] += 9;
                            break;
                        case (Piece.Queen | Piece.White):
                            pieceValue += this.Data.PieceValues[4] * this.Data.Queen[rank, line];
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
                pieceValue += this.Data.BishopPairValue;
            }
            if (bishopColors[2] && bishopColors[3])
            {
                pieceValue -= this.Data.BishopPairValue;
            }

            for (sbyte i = 0; i < 2; i++)
            {
                if (numberOfPawns[i] == 0)
                    pawnPosFactor[i] = 0f;
                else
                    pawnPosFactor[i] /= numberOfPawns[i];
            }
            float pawnValue = this.Data.PieceValues[0] * evalPawns(numberOfPawns, pawnPosFactor, pawns);
            float toMoveAdvantage = this.Data.ToMoveValue * (pos.whiteToMove ? 1 : -1);
            return pieceValue + pawnValue + kingSafteyDifference + toMoveAdvantage;
        }
        private float numericEval(GameResult gr)
        {
            if (gr == GameResult.WhiteWin) return 2000;
            else if (gr == GameResult.BlackWin) return -2000;
            else return 0;
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
                    Piece currentPiece = pos[currentSquare];
                    float defValue = defenceValueOf(currentPiece);
                    if(defValue == 0f)
                    {
                        continue;
                    }
                    float defCoeff = this.Data.Defence[2-i, j + 2];
                    float finDefValue = defValue * defCoeff;

                    defenceAccumulator += finDefValue;
                }
            }
            if(defenceAccumulator > this.Data.SafteySoftCap)
            {
                //Halverar nyttan av kungsförsvar efter en viss gräns.
                defenceAccumulator -= (defenceAccumulator - this.Data.SafteySoftCap) / 2;
            }
            float safteyValue = (defenceAccumulator * (oppHeavyMaterial - this.Data.EndgameLimit) * this.Data.KingSafteyCoefficient) /200f;

            float kingCoefficient;
            if (oppHeavyMaterial > this.Data.EndgameLimit)
            {
                kingCoefficient = this.Data.King[0, kingSquare.rank, kingSquare.line];
            }
            else
            {
                kingCoefficient = this.Data.King[1, kingSquare.rank, kingSquare.line];
                return kingCoefficient * this.Data.KingValue;
            }
            return kingCoefficient * safteyValue;
        }
        private float defenceValueOf(Piece piece)
        {
            if (piece == Piece.None) return 0;
            piece = piece.AsBlack();
            switch (piece)
            {
                case Piece.Pawn: return this.Data.PieceDefenceValues[0];
                case Piece.Knight: return this.Data.PieceDefenceValues[1];
                case Piece.Bishop: return this.Data.PieceDefenceValues[2];
                case Piece.Rook: return this.Data.PieceDefenceValues[3];
                case Piece.Queen: return this.Data.PieceDefenceValues[4];
                case Piece.King: return 0;
                default: 
                    throw new Exception("Okänd pjäs!");
            }
        }
        private float evalPawns(int[] numberOfPawns, float[] posFactor, sbyte[,] pawns)
        {
            float[] pawnValues = new float[2];
            for (sbyte c = 0; c < 2; c++)
            {
                sbyte neighbours = 0; //Summan av antalet "grannar" som linjer där det står bönder har.
                sbyte lines = 0; //Antal rader på vilka det finns bönder.

                if (pawns[c, 0] > 0) //Specialfall för a-linjen.
                {
                    lines++; 
                    if (pawns[c, 1] > 0) neighbours += pawns[c, 0];
                }

                for (sbyte i = 1; i < 7; i++) //från b-linjen till g-linjen. 
                {
                    if (pawns[c, i] > 0)
                    {
                        lines++;
                        if (pawns[c, i - 1] > 0) neighbours += pawns[c, i];
                        if (pawns[c, i + 1] > 0) neighbours += pawns[c, i];
                    }
                }

                if (pawns[c, 7] > 0) //Specialfall för h-linjen.
                {
                    lines++;
                    if (pawns[c, 6] > 0) neighbours += pawns[c, 7];
                }
                
                pawnValues[c] = this.Data.PrecomputedPawnValues[numberOfPawns[c], neighbours + lines];
                pawnValues[c] *= posFactor[c];
            }
            return pawnValues[1] - pawnValues[0];
        }
        public GameResult decisiveResult(Position pos, List<Move> moves)
        {
            if (moves.Count == 0)
            {
                Square relevantKingSquare = pos.whiteToMove ? pos.kingPositions[1] : pos.kingPositions[0];
                bool isCheck = isControlledBy(pos, relevantKingSquare, !pos.whiteToMove);
                if (isCheck)
                {
                    if (pos.whiteToMove) return GameResult.BlackWin;
                    else return GameResult.WhiteWin;
                }
                else return GameResult.DrawByStaleMate;
            }
            else if (pos.halfMoveClock >= 100)
            {
                return GameResult.DrawBy50MoveRule;
            }
            else if (!mateableMaterial(pos.board))
            {
                return GameResult.DrawByInsufficientMaterial;
            }
            else return GameResult.Undecided;
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
        private float evaluationStep(float value)
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
        private bool mateableMaterial(Piece[,] board)
        {
            //TODO: Fixa för mattbart material för de respektive spelarna.
            bool anyKnight = false;
            bool anyLightSquaredBishop = false;
            bool anyDarkSquaredBishop = false;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {

                switch (board[i,j].AsBlack())
                {
                    case Piece.Pawn: return true;
                    case Piece.Queen: return true;
                    case Piece.Rook: return true;
                    case Piece.Knight: if (anyKnight) return true; else anyKnight = true; break;
                    case Piece.Bishop: 
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
        //private char myToUpper(Piece piece)
        //{
        //    if (!piece.IsWhite)
        //    {
        //        return (char)(piece - ('a' - 'A')); //Gör om tecknet till stor bokstav.
        //    }
        //    return piece;
        //}
        private int automaticDepth(Position pos)
        {
            double materialSum = 0;
            double weightForPawnOnLastRank = this.Data.CalculationWeights[4] * 0.75f;

            for (int rank = 0; rank < 8; rank++)
            {
                for (int line = 0; line < 8; line++)
                {
                    Piece piece = pos.board[rank, line];
                    if (piece.Is(Piece.Pawn))
                    {
                        if ((piece == Piece.Pawn.AsWhite() && rank == 1) || (piece == Piece.Pawn && rank == 6))
                        {
                            //Bönder som är ett steg ifrån att promotera.
                            materialSum += weightForPawnOnLastRank;
                        }
                        else
                        {
                            materialSum += this.Data.CalculationWeights[0];
                        }
                    }
                    else if (piece.Is(Piece.Knight))
                        materialSum += this.Data.CalculationWeights[1];
                    else if (piece.Is(Piece.Bishop))
                        materialSum += this.Data.CalculationWeights[2];
                    else if (piece.Is(Piece.Rook))
                        materialSum += this.Data.CalculationWeights[3];
                    if (piece.Is(Piece.Queen))
                        materialSum += this.Data.CalculationWeights[4];
                }
            }
            if (materialSum < 11)
                return 7;
            else if (materialSum <= this.Data.CalculationWeights[4])
                return 6;
            else if (materialSum < 25)
                return 5;
            else return 4;
        }
    }
}
