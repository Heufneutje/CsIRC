namespace CsIRC.Core
{
    /// <summary>
    /// A representation of a single mode change.
    /// </summary>
    public class ModeChange
    {
        /// <summary>
        /// The mode that is being changed.
        /// </summary>
        public char Mode { get; private set; }

        /// <summary>
        /// The parameter for modes that take one.
        /// </summary>
        public string Parameter { get; private set; }

        /// <summary>
        /// Whether the mode is being added or removed.
        /// </summary>
        public bool IsAdding { get; private set; }

        /// <summary>
        /// The type of mode that's being changed.
        /// </summary>
        public ModeType ModeType { get; private set; }

        /// <summary>
        /// Creates a representation of a single mode change.
        /// </summary>
        /// <param name="mode">The mode that is being changed.</param>
        /// <param name="parameter">The parameter for modes that take one.</param>
        /// <param name="isAdding">Whether the mode is being added or removed.</param>
        /// <param name="modeType">The type of mode that's being changed.</param>
        public ModeChange(char mode, string parameter, bool isAdding, ModeType modeType)
        {
            Mode = mode;
            Parameter = parameter;
            IsAdding = isAdding;
            ModeType = modeType;
        }

        /// <summary>
        /// Override for string representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"{(IsAdding ? "+" : "-")}{Mode} {Parameter}".TrimEnd();
        }
    }
}
