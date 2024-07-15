namespace Mudra.Unity
{
    public enum HandType
    {
        Left,
        Right
    };

    public enum GestureType
    {
        None,
        Mid_Tap,
        Index_Tap,
        Thumb,
        Twist,
        DoubleIndexTap,
        DoubleMiddleTap,
        SwipeLeft,
        SwipeRight,
        LongPres,
        Fitting,
        Pinching,
        Tap,
        DoubleTwist
    };

    public enum NavigationButtons
    {
        
        Release,
        Press,
    }
    public enum Feature
    {
        RawData,
        TensorFlowData,
        DoubleTap
    };

    public enum LoggingSeverity
    {
        Debug,
        Info,
        Warning,
        Error
    };
    public enum MudraScale
    {
        LOW = 0,
        MID = 1,
        HIGH = 2
    }

    public enum MudraSensitivity
    {
        LOW = 0,
        MID_LOW = 1,
        MID = 2,
        MID_HIGH = 3,
        HIGH = 4
    }
}