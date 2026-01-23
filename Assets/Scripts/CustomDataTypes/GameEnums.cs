namespace GameEnums
{
    public enum ReturnState
    {
        CreationState,
        EditState,
        BuildState
    }

    public enum TileLevel
    {
        Base,
        Level

    }
    public enum InteractType
    {
        Actor,
        Input,
        Output,
        Event,
        None
    }

    public enum PropLevel
    {
        Base,
        Level
    }

    public enum RotationLevel
    {
        Landscape,
        Portrait
    }

    public enum Nodes
    {
        PlusNode = 0,
        MinusNode = 1,
        MultiplyNode = 2,
        DivideNode = 3,
        DoubleValueNode = 4,
        FloatValueNode = 5,
        IntValueNode = 6,
    }


    public enum MathOperations
    {
        Add, Subtract, Multiply, Divide
    }

    public enum FieldInput
    {
        None,
        Text,
        Number,
        Dropdown,
        Toggle
    }

    public enum NodeValueType
    {
        Double,
        Float,
        Int,
        Boolean,
        String
    }

    public enum NumberTypes
    {
        Int,
        Long,
        Double,
        Float
    }

    public enum NodeFieldFlag
    {
        InputRef,
        OutputRef,
    }


}