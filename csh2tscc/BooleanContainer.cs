namespace csh2tscc;

/// <summary>
/// Tracks nullable state for generic type arguments based on C# NullableAttribute metadata.
/// The nullable attribute contains a byte array where:
/// - Index 0: nullability of the property/field type itself
/// - Index 1+: nullability of each generic type argument (in depth-first order)
/// This class iterates through the generic argument positions (starting at index 1).
/// </summary>
public class BooleanContainer(bool[] nullableFlags, bool defaultNullable)
{
    private int _position = NullabilityConstants.FirstGenericArgIndex;

    /// <summary>
    /// Gets the nullable flag for the next generic type argument and advances the position.
    /// </summary>
    /// <returns>
    /// True if the generic argument is nullable, false otherwise.
    /// Returns defaultNullable if no flags are available,
    /// or falls back to the base flag (index 0) when all positions are consumed.
    /// </returns>
    public bool GetValueAndMoveNext()
    {
        // No flags available - use class-level nullable context
        if (nullableFlags.Length == 0)
            return defaultNullable;

        // Single flag means uniform nullability for all positions,
        // or we've consumed all specific flags - use the base flag
        if (_position >= nullableFlags.Length)
            return nullableFlags[NullabilityConstants.PropertyTypeIndex];

        return nullableFlags[_position++];
    }

    public static BooleanContainer CreateTrue() => new([], true);
    public static BooleanContainer CreateFalse() => new([], false);
}