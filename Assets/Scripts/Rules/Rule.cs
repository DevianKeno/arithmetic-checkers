namespace Aritche.Rules
{
    /// <summary>
    /// Represents an individual rule, along with its value type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct Rule<T>
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public T Value { get; private set; }

        public Rule(int id, string name, T value)
        {
            Id = id;
            Name = name;
            Value = value;
        }

        public void SetValue(T value)
        {
            Value = value;
        }
    }
}