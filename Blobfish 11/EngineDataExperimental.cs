using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blobfish_11
{
    class EngineDataExperimental : EngineData
    {
        public override float BishopPairValue => 0.4f;
        public override int EndgameLimit => 8;
        public override float KingSafteyCoefficient => 1f;
        public override float SafteySoftCap => 1f;
        public override float ToMoveValue => 0.25f;

        private readonly float[] pieceValues = { 1f, 3f, 3f, 4.5f, 9f };
        public override float[] PieceValues => this.pieceValues;

        private readonly float[] pieceDefenceValues = { 1, 1f, 0.8f, 0.1f, 0.05f };
        public override float[] PieceDefenceValues => this.pieceDefenceValues;
    }
}
