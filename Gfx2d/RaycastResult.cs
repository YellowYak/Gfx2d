namespace Gfx2d
{
    internal class RaycastResult
    {
        public int ColumnX { get; set; }

        public double RaycastLength { get; set; }

        public int FloorHeight { get; set; }
        public int WallHeight { get; set; }
        public int CeilingHeight { get; set; }
    }
}
