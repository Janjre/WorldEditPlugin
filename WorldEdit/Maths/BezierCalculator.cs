using OnixRuntime.Api.Maths;

namespace WorldEdit.Maths
{
    public static class BezierCalculator
    {
        private static Vec3 Interpolate(Vec3 point1, Vec3 point2, float t)
        {
            return ((point2 - point1) * t) + point1;
        }

        public static Vec3 BezierPoint(List<Vec3> controlPoints, float t)
        {
            List<Vec3> points = new List<Vec3>(controlPoints);

            while (points.Count > 1)
            {
                List<Vec3> nextLevel = new List<Vec3>();

                for (int i = 0; i < points.Count - 1; i++)
                {
                    nextLevel.Add(Interpolate(points[i], points[i + 1], t));
                }

                points = nextLevel;
            }

            return points[0];
        }
    }
}
    
