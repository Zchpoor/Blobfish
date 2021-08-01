using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blobfish_11
{
    class EngineDataAggressive : EngineData
    {
        public override float BishopPairValue => 0.4f;
        public override int EndgameLimit => 8;
        public override float KingSafteyCoefficient => 0.5f;
        public override float SafteySoftCap => 2.5f;
        public override float ToMoveValue => 0.4f;

        private readonly float[] pieceValues = { 1.2f, 4f, 4f, 6.5f, 12f };
        public override float[] PieceValues => this.pieceValues;

        private readonly float[] pieceDefenceValues = { 1, 2, 1.4f, 0.4f, 0.1f };
        public override float[] PieceDefenceValues => this.pieceDefenceValues;
    }
}
