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

            // Populate rotation radians table, sin & cos table, and associated properties
            int prEntries = (int)(6.283319 / rotationRadiansDelta);
            
            possibleRotationRadians = new double[prEntries];            
            cosTable = new double[prEntries];
            sinTable = new double[prEntries];

            possibleRotationRadians[0] = 0;
            cosTable[0] = Math.Cos(possibleRotationRadians[0]);
            sinTable[0] = Math.Sin(possibleRotationRadians[0]);

            for (int i = 1; i < prEntries; i++)
            {
                possibleRotationRadians[i] = possibleRotationRadians[i - 1] + rotationRadiansDelta;
                cosTable[i] = Math.Cos(possibleRotationRadians[i]);
                sinTable[i] = Math.Sin(possibleRotationRadians[i]);
            }

            this.possibleRotationRadiansLength = possibleRotationRadians.Length;
            this.piOverTwoIndex = (int)(possibleRotationRadiansLength * 0.25);
            this.piIndex = (int)(possibleRotationRadiansLength * 0.5);
            this.threePiOverTwoIndex = (int)(possibleRotationRadiansLength * 0.75);

            this.RotationRadiansDelta = rotationRadiansDelta;

            const double DesiredRotationRadiansWhenPlayerRotating = 0.02;
            this.rotationIndexDistanceWhenPlayerRotating = (int)(DesiredRotationRadiansWhenPlayerRotating / RotationRadiansDelta);
        }

        public double GetPossibleRotationRadians(int index) => this.possibleRotationRadians![index % possibleRotationRadiansLength];
        public double Cos(int rotationRadianIndex) => this.cosTable![rotationRadianIndex % possibleRotationRadiansLength];
        public double Sin(int rotationRadianIndex) => this.sinTable![rotationRadianIndex % possibleRotationRadiansLength];

        public int GetDirectionXFromRotationIndex(int prrIndex)
        {
            int direction_x = 0;
            if (prrIndex < PiOverTwoIndex || prrIndex > ThreePiOverTwoIndex)
                direction_x = 1;
            else if (prrIndex > PiOverTwoIndex && prrIndex < ThreePiOverTwoIndex)
                direction_x = -1;

            return direction_x;
        }
        public int GetDirectionYFromRotationIndex(int prrIndex)
        {
            int direction_y = 0;
            if (prrIndex > PiIndex)
                direction_y = -1;
            else if (prrIndex > 0 && prrIndex < PiIndex)
                direction_y = 1;

            return direction_y;
        }

        public int Floor(double a) => (int)a;

        public int Ceiling(double a) => (int)Math.Ceiling(a);
    }
}
