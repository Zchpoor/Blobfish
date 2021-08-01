using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Blobfish_11
{
    public class EngineData
    {
        //Pjäsernas grundvärde.
        private readonly float[] pieceValues = { 1, 3, 3, 5, 9 };
        private readonly float kingValue = 4f;

        //Löparparets egenvärde.
        private readonly float bishopPairValue = 0.4f;

        private const float rookOnSemiOpenLineCoefficient = 1.08f;
        private const float rookOnOpenLineCoefficient = 1.11f;

        //Värdet av att vara vid draget när en variant slutar.
        private readonly float toMoveValue = 0.25f;

        //Förberäknade värden på bönderna utifrån formeln för bondeevaluering.
        //Gör denna till readonly när detta gjorts till egen klass.
        public virtual float[,] PrecomputedPawnValues { get; private set; }

        //Pjäsernas försvarsvärde.
        private readonly float[] pieceDefenceValues = { 1, 1.5f, 1.2f, 0.4f, 0.1f };

        //Gränsen efter vilken försvarsnyttan halveras.
        private const float safteySoftCap = 4f;
        private const float kingSafteyCoefficient = 1;

        //Partiet anses ha gått in i slutspel omm värdet av motståndarens 
        //tunga pjäser uppgår till mindre än eller lika med endgameLimit.
        private const int endgameLimit = 8;
        private const int sleepTime = 100;

        //Uppskattning av hur mycket pjäserna bidrar till beräkningstid.
        private readonly double[] calculationWeights = { 1, 4, 6, 7, 20 }; //PNBRQ

        //För vart och ett av talen som är större än antalet drag i ställningen så
        //ökas djupet med ett. Talen bör vara i minskande ordning.
        //Till exempel: {20, 8, 2}
        private readonly int[] moveIncreaseLimits = { };

        //Maximalt djup en variant kan beräknas efter minDepth uppnåtts.
        private readonly int maximumDepth = 8;

        public EngineData()
        {
            fillPrecomputedPawnValues();
        }

        private void fillPrecomputedPawnValues()
        {
            int maxNumberOfPawns = 8;
            //Summan av antal grannar och antalet linjer som bönderna står på bör inte kunna överskridas i ett vanligt parti.
            int maxNeighbourLineSum = 22;

            this.PrecomputedPawnValues = new float[maxNumberOfPawns+1, maxNeighbourLineSum+1];

            for (int numberOfPawns = 0; numberOfPawns <= maxNumberOfPawns; numberOfPawns++)
            {
                for (int neighbourLineSum = 0; neighbourLineSum <= maxNeighbourLineSum; neighbourLineSum++)
                {
                    float pawnValue = ((numberOfPawns * (neighbourLineSum + 41)) + 9.37f) / 64;
                    //e(p,s)=(p(s+41)+9,37)/64. TODO: Förbättra formel
                    this.PrecomputedPawnValues[numberOfPawns, neighbourLineSum] = pawnValue;
                }
            }
        }

        private readonly float[,,] pawn =
        {
            { //Svarta bönder
                { 0f,    0f,    0f,    0f,   0f,   0f,    0f,    0f    }, //8
                { 0.5f,  0.6f,  0.7f,  0.8f, 0.8f, 0.7f,  0.6f,  0.5f  }, //7
                { 0.55f, 0.7f,  0.9f,  1.1f, 1.1f, 0.9f,  0.7f,  0.55f }, //6
                { 0.6f,  0.75f, 1.1f,  1.3f, 1.3f, 1.1f,  0.75f, 0.6f  }, //5
                { 0.7f,  0.9f,  1.3f,  1.5f, 1.5f, 1.3f,  0.9f,  0.7f  }, //4
                { 1f,    1.1f,  1.5f,  1.7f, 1.7f, 1.5f,  1.1f,  1f    }, //3
                { 1.9f,  2f,    2.1f,  2.2f, 2.2f, 2.1f,  2f,    1.9f  }, //2
                { 0f,    0f,    0f,    0f,   0f,   0f,    0f,    0f    }  //1
            },
            { //Vita bönder
                { 0f,    0f,    0f,    0f,   0f,   0f,    0f,    0f    }, //8
                { 1.9f,  2f,    2.1f,  2.2f, 2.2f, 2.1f,  2f,    1.9f  }, //7
                { 1f,    1.1f,  1.5f,  1.7f, 1.7f, 1.5f,  1.1f,  1f    }, //6
                { 0.7f,  0.9f,  1.3f,  1.5f, 1.5f, 1.3f,  0.9f,  0.7f  }, //5
                { 0.6f,  0.75f, 1.1f,  1.3f, 1.3f, 1.1f,  0.75f, 0.6f  }, //4
                { 0.55f, 0.7f,  0.9f,  1.1f, 1.1f, 0.9f,  0.7f,  0.55f }, //3
                { 0.5f,  0.6f,  0.7f,  0.8f, 0.8f, 0.7f,  0.6f,  0.5f  }, //2
                { 0f,    0f,    0f,    0f,   0f,   0f,    0f,    0f    }  //1
            }
        };
        private readonly float[,] knight =
        {
            {0.89f,    0.95f,    0.97f,    0.98f,    0.98f,    0.97f,    0.95f,    0.89f },
            {0.95f,    0.98f,    1f,       1.02f,    1.02f,    1f,       0.98f,    0.95f },
            {0.97f,    1f,       1.05f,    1.07f,    1.07f,    1.05f,    1f,       0.97f },
            {0.98f,    1.02f,    1.07f,    1.1f,     1.1f,     1.07f,    1.02f,    0.98f },
            {0.98f,    1.02f,    1.07f,    1.1f,     1.1f,     1.07f,    1.02f,    0.98f },
            {0.97f,    1f,       1.05f,    1.07f,    1.07f,    1.05f,    1f,       0.97f },
            {0.95f,    0.98f,    1f,       1.02f,    1.02f,    1f,       0.98f,    0.95f },
            {0.89f,    0.95f,    0.97f,    0.98f,    0.98f,    0.97f,     0.95f,    0.89f }
        };
        private readonly float[,] bishop =
        {
            {1f,    0.97f, 0.94f, 0.92f, 0.92f, 0.94f, 0.97f, 1f    },
            {0.97f, 1.02f, 0.98f, 0.96f, 0.96f, 0.98f, 1.02f, 0.97f },
            {0.94f, 0.98f, 1.03f, 1.01f, 1.01f, 1.03f, 0.98f, 0.94f },
            {0.92f, 0.96f, 1.01f, 1.04f, 1.04f, 1.01f, 0.96f, 0.92f },
            {0.92f, 0.96f, 1.01f, 1.04f, 1.04f, 1.01f, 0.96f, 0.92f },
            {0.94f, 0.98f, 1.03f, 1.01f, 1.01f, 1.03f, 0.98f, 0.94f },
            {0.97f, 1.02f, 0.98f, 0.96f, 0.96f, 0.98f, 1.02f, 0.97f },
            {1f,    0.97f, 0.94f, 0.92f, 0.92f, 0.94f, 0.97f, 1f    }
        };
        private readonly float[,] rook =
        {
            {1.01f, 1.02f, 1.04f, 1.06f, 1.06f, 1.04f, 1.02f, 1.01f },
            {1.02f, 0.99f, 0.98f, 1f,    1f,    0.98f, 0.99f, 1.02f },
            {1.04f, 0.98f, 0.96f, 0.95f, 0.95f, 0.96f, 0.98f, 1.04f },
            {1.06f, 1f,    0.95f, 0.94f, 0.94f, 0.95f, 1f,    1.06f },
            {1.06f, 1f,    0.95f, 0.94f, 0.94f, 0.95f, 1f,    1.06f },
            {1.04f, 0.98f, 0.96f, 0.95f, 0.95f, 0.96f, 0.98f, 1.04f },
            {1.02f, 0.99f, 0.98f, 1f,    1f,    0.98f, 0.99f, 1.02f },
            {1.01f, 1.02f, 1.04f, 1.06f, 1.06f, 1.04f, 1.02f, 1.01f }
        };
        private readonly float[,] queen =
        {
            {0.990f, 0.993f, 0.997f, 1.000f, 1.000f, 0.997f, 0.993f, 0.990f},
            {0.993f, 0.997f, 1.000f, 1.003f, 1.003f, 1.000f, 0.997f, 0.993f},
            {0.997f, 1.000f, 1.003f, 1.007f, 1.007f, 1.003f, 1.000f, 0.997f},
            {1.000f, 1.003f, 1.007f, 1.010f, 1.010f, 1.007f, 1.003f, 1.000f},
            {1.000f, 1.003f, 1.007f, 1.010f, 1.010f, 1.007f, 1.003f, 1.000f},
            {0.997f, 1.000f, 1.003f, 1.007f, 1.007f, 1.003f, 1.000f, 0.997f},
            {0.993f, 0.997f, 1.000f, 1.003f, 1.003f, 1.000f, 0.997f, 0.993f},
            {0.990f, 0.993f, 0.997f, 1.000f, 1.000f, 0.997f, 0.993f, 0.990f}
        };
        private readonly float[,,] king =
        {
           { //Ej slutspel
                {2f,   1.9f,  1.4f,  1f,    1f,    1.4f,  1.9f,  2f   },
                {1.7f, 1.5f,  0.95f, 0.85f, 0.85f, 0.95f, 1.5f,  1.7f },
                {0.8f, 0.6f,  0.45f, 0.3f,  0.3f,  0.45f, 0.6f,  0.8f },
                {0.4f, 0.25f, 0.15f, 0.1f,  0.1f,  0.15f, 0.25f, 0.4f },
                {0.4f, 0.25f, 0.15f, 0.1f,  0.1f,  0.15f, 0.25f, 0.4f },
                {0.8f, 0.6f,  0.45f, 0.3f,  0.3f,  0.45f, 0.6f,  0.8f },
                {1.7f, 1.5f,  0.95f, 0.85f, 0.85f, 0.95f, 1.5f,  1.7f },
                {2f,   1.9f,  1.4f,  1f,    1f,    1.4f,  1.9f,  2f   }
            },

            { //Slutspel
                {0.91f,   0.93f,    0.95f,    0.96f,    0.96f,    0.95f,    0.93f,    0.91f, },
                {0.93f,   0.98f,    1f,       1.03f,    1.03f,    1f,       0.98f,    0.93f, },
                {0.95f,   1f,       1.06f,    1.09f,    1.09f,    1.06f,    1f,       0.95f, },
                {0.96f,   1.03f,    1.09f,    1.13f,    1.13f,    1.09f,    1.03f,    0.96f, },
                {0.96f,   1.03f,    1.09f,    1.13f,    1.13f,    1.09f,    1.03f,    0.96f, },
                {0.95f,   1f,       1.06f,    1.09f,    1.09f,    1.06f,    1f,       0.95f, },
                {0.93f,   0.98f,    1f,       1.03f,    1.03f,    1f,       0.98f,    0.93f, },
                {0.91f,   0.93f,    0.95f,    0.96f,    0.96f,    0.95f,    0.93f,    0.91f, }
            }
        };
        private readonly float[,] defence =
        {
            {0.25f,  0.6f, 0.8f,   0.6f, 0.25f,},
            {0.2f,   1.3f, 1.4f, 1.3f, 0.2f, },
            {0.1f,   1f,   0f,   1f,   0.1f, }
        };

        public virtual float[] PieceValues => pieceValues;
        public virtual float KingValue => kingValue;
        public virtual float BishopPairValue => bishopPairValue;
        public virtual float RookOnSemiOpenLineCoefficient => rookOnSemiOpenLineCoefficient;
        public virtual float RookOnOpenLineCoefficient => rookOnOpenLineCoefficient;
        public virtual float ToMoveValue => toMoveValue;
        public virtual float[] PieceDefenceValues => pieceDefenceValues;
        public virtual float SafteySoftCap => safteySoftCap;
        public virtual float KingSafteyCoefficient => kingSafteyCoefficient;
        public virtual int EndgameLimit => endgameLimit;
        public virtual int SleepTime => sleepTime;
        public virtual double[] CalculationWeights => calculationWeights;
        public virtual int[] MoveIncreaseLimits => moveIncreaseLimits;
        public virtual int MaximumDepth => maximumDepth;

        public virtual float[,,] Pawn => pawn;
        public virtual float[,] Knight => knight;
        public virtual float[,] Bishop => bishop;
        public virtual float[,] Rook => rook;
        public virtual float[,] Queen => queen;
        public virtual float[,,] King => king;
        public virtual float[,] Defence => defence;

    }
}