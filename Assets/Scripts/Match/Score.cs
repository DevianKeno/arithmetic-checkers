namespace Damath
{
    /// <summary>
    /// This class imlpements methods for score evalution.
    /// </summary>
    public class Score
    {
        public float Value { get; private set; }

        public float Get()
        {
            return Value;
        }

        public void Set(float value)
        {
            Value = value;
        }

        public void Add(float value)
        {
            Value += value;
        }

        public void Remove(float value)
        {
            Value -= value;
        }

        public Score CompareHigher(Score score)
        {
            if (score.Get() > Value)
            {
                return score;
            }

            return this;
        }
    }
}