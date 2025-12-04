using Gfx2d.Resources;

namespace Gfx2d.Engine
{
    internal class RaycastResult
    {
        public int ColumnX { get; set; }

        public double RaycastLength { get; set; }
        public double PerpendicularLength { get; set; }

        public double Shading { get; set; }

        public Point2d? PlayerPos { get; set; }
        public Point2d? RaycastPoint { get; set; }

        public int FloorHeight { get; set; }
        public int WallHeight { get; set; }
        public int CeilingHeight { get; set; }

        public override string ToString()
        {
            string fromDesc = "UNKNOWN";
            string toDesc = "UNKNOWN";

            if (PlayerPos != null)
                fromDesc = PlayerPos.ToString();

            if (RaycastPoint != null)
                toDesc = RaycastPoint.ToString();

            return $"{ColumnX}: C={CeilingHeight}, W={WallHeight}, F={FloorHeight} (From {fromDesc} to {toDesc} - L={Math.Round(RaycastLength, 4)}, P={Math.Round(PerpendicularLength, 4)}) (Shading {Shading})";
        }
    }
}
