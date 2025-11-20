namespace csh2tscc;

public class BooleanContainer(bool[] array, bool defaultValue)
{
    private int _pointer = 1;

    public bool GetValueAndMoveNext()
    {
        if (array.Length == 0)
        {
            return defaultValue;
        }

        return array.Length <= _pointer ? array[0] : array[_pointer++];
    }

    public static BooleanContainer CreateTrue() => new([true], true);
    public static BooleanContainer CreateFalse() => new([false], false);
}