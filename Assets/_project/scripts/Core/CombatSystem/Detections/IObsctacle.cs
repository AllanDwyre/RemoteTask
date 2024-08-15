namespace _project.scripts.Core.CombatSystem.Detections
{
    public interface IObstacle
    {
        public Height GetHeight();
    }

    public struct Height
    {
        public float Min { get; private set; }
        public float Max { get; private set; }

        public Height(float max, float min = 0f)
        {
            Min = min;
            Max = max;
        }
    }
}