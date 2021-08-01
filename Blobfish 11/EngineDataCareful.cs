using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blobfish_11
{
    public sealed class EngineDataCareful : EngineData
    {
        public override float BishopPairValue => 0.8f;
        public override int EndgameLimit => 6;
        public override float KingSafteyCoefficient => 1.15f;
        public override float SafteySoftCap => 5f;
        public override float ToMoveValue => 0.15f;

        private readonly float[] pieceDefenceValues = { 1.2f, 2.2f, 1.4f, 0.4f, 0.1f };
        public override float[] PieceDefenceValues => this.pieceDefenceValues;
    }
}
