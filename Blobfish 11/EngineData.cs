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
        readonly double[] pieceValues = { 3, 3, 5, 9 };
        readonly double kingValue = 4f;
        readonly double bishopPairValue = 0.4f;

        //Partiet anses ha gått in i slutspel omm värder av motståndarens 
        //tunga pjäser uppgår till mindre än denna variabel.
        readonly int endgameLimit = 6;

        readonly int sleepTime = 100;
        readonly int[] moveIncreaseLimits = {20, 8 }; // Talen bör vara i minskande ordning.

        private static readonly double[,,] pawn =
        {
            { //Svarta bönder
                { 0f,    0f,    0f,    0f,   0f,   0f,    0f,    0f    }, //8
                { 0.5f,  0.6f,  0.7f,  0.8f, 0.8f, 0.7f,  0.6f,  0.5f  }, //7
                { 0.55f, 0.7f,  0.85f, 1f,   1f,   0.85f, 0.7f,  0.55f }, //6
                { 0.6f,  0.75f, 1f,    1.2f, 1.2f, 1f,    0.75f, 0.6f  }, //5
                { 0.7f,  0.9f,  1.2f,  1.4f, 1.4f, 1.2f,  0.9f,  0.7f  }, //4
                { 1f,    1.1f,  1.5f,  1.7f, 1.7f, 1.5f,  1.1f,  1f    }, //3
                { 1.9f,  2f,    2.1f,  2.2f, 2.2f, 2.1f,  2f,    1.9f  }, //2
                { 0f,    0f,    0f,    0f,   0f,   0f,    0f,    0f    }  //1
            },
            { //Vita bönder
                { 0f,    0f,    0f,    0f,   0f,   0f,    0f,    0f    }, //8
                { 1.9f,  2f,    2.1f,  2.2f, 2.2f, 2.1f,  2f,    1.9f  }, //7
                { 1f,    1.1f,  1.5f,  1.7f, 1.7f, 1.5f,  1.1f,  1f    }, //6
                { 0.7f,  0.9f,  1.2f,  1.4f, 1.4f, 1.2f,  0.9f,  0.7f  }, //5
                { 0.6f,  0.75f, 1f,    1.2f, 1.2f, 1f,    0.75f, 0.6f  }, //4
                { 0.55f, 0.7f,  0.85f, 1f,   1f,   0.85f, 0.7f,  0.55f }, //3
                { 0.5f,  0.6f,  0.7f,  0.8f, 0.8f, 0.7f,  0.6f,  0.5f  }, //2
                { 0f,    0f,    0f,    0f,   0f,   0f,    0f,    0f    }  //1
            }
        };
        private static readonly double[,] knight =
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
        private static readonly double[,] bishop =
        {
            {1f,      0.97f,    0.93f,    0.9f,     0.9f,     0.93f,    0.97f,    1f   },
            {0.97f,   1.05f,    1f,       0.95f,    0.95f,    1f,       1.05f,    0.97f},
            {0.93f,   1f,       1.12f,    1.09f,    1.09f,    1.12f,    1f,       0.93f},
            {0.9f,    0.95f,    1.09f,    1.15f,    1.15f,    1.09f,    0.95f,    0.9f},
            {0.9f,    0.95f,    1.09f,    1.15f,    1.15f,    1.09f,    0.95f,    0.9f},
            {0.93f,   1f,       1.12f,    1.09f,    1.09f,    1.12f,    1f,       0.93f},
            {0.97f,   1.05f,    1f,       0.95f,    0.95f,    1f,       1.05f,    0.97f},
            {1f,      0.97f,    0.93f,    0.9f,     0.9f,     0.93f,    0.97f,    1f   }
        };
        private static readonly double[,] rook =
        {
            {1f,      1.03f,    1.07f,    1.1f,     1.1f,     1.07f,    1.03f,    1f,    },
            {1.03f,   1f,       0.96f,    1.02f,    1.02f,    0.96f,    1f,       1.03f, },
            {1.07f,   0.96f,    0.93f,    0.91f,    0.91f,    0.93f,    0.96f,    1.07f, },
            {1.1f,    1.02f,    0.91f,    0.89f,    0.89f,    0.91f,    1.02f,    1.1f,  },
            {1.1f,    1.02f,    0.91f,    0.89f,    0.89f,    0.91f,    1.02f,    1.1f,  },
            {1.07f,   0.96f,    0.93f,    0.91f,    0.91f,    0.93f,    0.96f,    1.07f, },
            {1.03f,   1f,       0.96f,    1.02f,    1.02f,    0.96f,    1f,       1.03f, },
            {1f,      1.03f,    1.07f,    1.1f,     1.1f,     1.07f,    1.03f,    1f,    }
        };
        private static readonly double[,] queen =
        {
            {0.97f,    0.98f,    0.99f,    1f,       1f,       0.99f,    0.98f,    0.97f},
            {0.98f,    0.99f,    1f,       1.01f,    1.01f,    1f,       0.99f,    0.98f},
            {0.99f,    1f,       1.01f,    1.02f,    1.02f,    1.01f,    1f,       0.99f},
            {1f,       1.01f,    1.02f,    1.03f,    1.03f,    1.02f,    1.01f,    1f   },
            {1f,       1.01f,    1.02f,    1.03f,    1.03f,    1.02f,    1.01f,    1f   },
            {0.99f,    1f,       1.01f,    1.02f,    1.02f,    1.01f,    1f,       0.99f},
            {0.98f,    0.99f,    1f,       1.01f,    1.01f,    1f,       0.99f,    0.98f},
            {0.97f,    0.98f,    0.99f,    1f,       1f,       0.99f,    0.98f,    0.97f}
        };
        private static readonly double[,,] king =
        {
            {
                { 1.18f,  1.14f,    1.05f,    1f,       1f,       1.05f,    1.14f,    1.18f, },
                { 1.1f,   1.04f,    0.97f,    0.9f,     0.9f,     0.97f,    1.04f,    1.1f,  },
                { 0.85f,  0.75f,    0.65f,    0.6f,     0.6f,     0.65f,    0.75f,    0.85f, },
                { 0.6f,   0.5f,     0.4f,     0.3f,     0.3f,     0.4f,     0.5f,     0.6f,  },
                { 0.6f,   0.5f,     0.4f,     0.3f,     0.3f,     0.4f,     0.5f,     0.6f,  },
                { 0.85f,  0.75f,    0.65f,    0.6f,     0.6f,     0.65f,    0.75f,    0.85f, },
                { 1.1f,   1.04f,    0.97f,    0.9f,     0.9f,     0.97f,    1.04f,    1.1f,  },
                { 1.18f,  1.14f,    1.05f,    1f,       1f,       1.05f,    1.14f,    1.18f, }
            },




            {
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
    }
}