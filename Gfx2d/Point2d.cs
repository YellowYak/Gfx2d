namespace Gfx2d
{
    internal class Point2d
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point2d(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Point2d(Point2d clone)
        {
            X = clone.X;
            Y = clone.Y;
        }

        public IEnumerable<Point2d> GetRectBounds(double width)
        {
            double halfWidth = width / 2;

            return new Point2d[]
            {
                new Point2d(this.X - halfWidth, this.Y - halfWidth),
                new Point2d(this.X - halfWidth, this.Y + halfWidth),
                new Point2d(this.X + halfWidth, this.Y - halfWidth),
                new Point2d(this.X + halfWidth, this.Y + halfWidth),
            };
        }

        public static double GetLength(Point2d p1, Point2d p2) => Convert.ToSingle(Math.Sqrt((p2.X -  p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y)));
        
        public override string ToString() => $"({this.X}, {this.Y})";
    }
}
