namespace Gfx2d
{
    /// <summary>
    /// Represents a point in 2d space.
    /// </summary>
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

        /// <summary>
        /// Returns the square bounds around the point from a given width.
        /// </summary>
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

        /// <summary>
        /// Uses Pythagorean's theorem to return the length between two <see cref="Point2d"/> objects.
        /// </summary>
        public static double GetLength(Point2d p1, Point2d p2) => Math.Round(Convert.ToSingle(Math.Sqrt((p2.X -  p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y))), 4);
        
        public override string ToString() => $"({Math.Round(this.X, 4)}, {Math.Round(this.Y, 4)})";
    }
}
