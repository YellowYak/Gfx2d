namespace Gfx2d
{
    internal class MathHelpers
    {
        private double[]? possibleRotationRadians;
        private double[]? cosTable;
        private double[]? sinTable;

        private int possibleRotationRadiansLength;
        public int PossibleRotationRadiansLength => this.possibleRotationRadiansLength;

        private int piOverTwoIndex;
        public int PiOverTwoIndex => this.piOverTwoIndex;

        private int piIndex;
        public int PiIndex => this.piIndex;

        private int threePiOverTwoIndex;
        public int ThreePiOverTwoIndex => this.threePiOverTwoIndex;


        public double RotationRadiansDelta { get; private set; }

        private int rotationIndexDistanceWhenPlayerRotating;
        public int RotationIndexDistanceWhenPlayerRotating => this.rotationIndexDistanceWhenPlayerRotating;


        public MathHelpers(double rotationRadiansDelta)
        {
            if (rotationRadiansDelta < 0.001) throw new ArgumentOutOfRangeException(nameof(rotationRadiansDelta));
            if (rotationRadiansDelta >= 0.25) throw new ArgumentOutOfRangeException(nameof(rotationRadiansDelta));

            // Populate rotation radians table, sin & cos table, and associated properties.
            // First, determine how many entries to add. rotationRadiansDelta specifies the desired granularity and 6.283319 is ~2PI.
            int prEntries = (int)(6.283319 / rotationRadiansDelta);
            
            possibleRotationRadians = new double[prEntries];            
            cosTable = new double[prEntries];
            sinTable = new double[prEntries];

            // Assign the starting values in each table
            possibleRotationRadians[0] = 0;
            cosTable[0] = Math.Cos(possibleRotationRadians[0]);
            sinTable[0] = Math.Sin(possibleRotationRadians[0]);

            // Loop through as many entries as needed
            for (int i = 1; i < prEntries; i++)
            {
                // Set the current rotation radians entry to be rotationRadiansDelta rads ahead of the last one.
                possibleRotationRadians[i] = possibleRotationRadians[i - 1] + rotationRadiansDelta;

                // Add entries to the sin and cos table for the latest radian value.
                cosTable[i] = Math.Cos(possibleRotationRadians[i]);
                sinTable[i] = Math.Sin(possibleRotationRadians[i]);
            }

            // Pre-compute these values, as they are used frequently in tight loops
            this.possibleRotationRadiansLength = possibleRotationRadians.Length;
            this.piOverTwoIndex = (int)(possibleRotationRadiansLength * 0.25);
            this.piIndex = (int)(possibleRotationRadiansLength * 0.5);
            this.threePiOverTwoIndex = (int)(possibleRotationRadiansLength * 0.75);

            this.RotationRadiansDelta = rotationRadiansDelta;

            // When the user rotates left or right we want to move DesiredRotationRadiansWhenPlayerRotating radians per screen refresh.
            // Here we calculate just how many elements in the array we should "jump" with each step of the rotation to main close to that many radians.
            const double DesiredRotationRadiansWhenPlayerRotating = 0.02;
            this.rotationIndexDistanceWhenPlayerRotating = (int)(DesiredRotationRadiansWhenPlayerRotating / RotationRadiansDelta);
        }

        /// <summary>
        /// Returns the radians at the specified index. A little modulo math is in place to account for index values that may be outside the bounds of the array.
        /// </summary>
        public double GetPossibleRotationRadians(int index) => this.possibleRotationRadians![index % possibleRotationRadiansLength];

        /// <summary>
        /// Returns the cos value for a radian at the specified index. A little modulo math is in place to account for index values that may be outside the bounds of the array.
        /// </summary>
        public double Cos(int rotationRadianIndex) => this.cosTable![rotationRadianIndex % possibleRotationRadiansLength];

        /// <summary>
        /// Returns the sin value for a radian at the specified index. A little modulo math is in place to account for index values that may be outside the bounds of the array.
        /// </summary>
        public double Sin(int rotationRadianIndex) => this.sinTable![rotationRadianIndex % possibleRotationRadiansLength];


        /// <summary>
        /// Returns the X direction based on the rotation index.
        /// </summary>
        /// <returns>1 if the angle is easterly. -1 if it is westerly. 0 if due north or due south.</returns>
        public int GetDirectionXFromRotationIndex(int prrIndex)
        {
            int direction_x = 0;
            if (prrIndex < PiOverTwoIndex || prrIndex > ThreePiOverTwoIndex)
                direction_x = 1;
            else if (prrIndex > PiOverTwoIndex && prrIndex < ThreePiOverTwoIndex)
                direction_x = -1;

            return direction_x;
        }

        /// <summary>
        /// Returns the Y direction based on the rotation index.
        /// </summary>
        /// <returns>1 if the angle is southerly pointing. -1 if it is notherly pointing. 0 if due east or due west.</returns>
        public int GetDirectionYFromRotationIndex(int prrIndex)
        {
            int direction_y = 0;
            if (prrIndex > PiIndex)
                direction_y = -1;
            else if (prrIndex > 0 && prrIndex < PiIndex)
                direction_y = 1;

            return direction_y;
        }

        public static int Floor(double a) => (int)a;

        public static int Ceiling(double a) => (int)Math.Ceiling(a);


        public static double RadiansToDegrees(double rad) => rad * 180 / Math.PI;
    }
}
