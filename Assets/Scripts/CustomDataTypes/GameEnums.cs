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
        StringValueNode = 7,
        BooleanValueNode = 8,
        StatementNode = 9,
        ConditionNode = 10,
        ComparisonOpNode = 11,
        LogicalOpNode = 12,
        ItemRefInputNode = 13,
        ItemRefActorNode = 14,
        ItemRefOutputNode = 15,
        ExecuteNode = 100,
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


    public enum NodeUISides
    {
        left,
        right,
        top,
        bottom
    }

    public enum ItemType
    {
        None = -1,
        Actor = 0,
        Input = 1,
        Output = 2,
    }

    public enum ComparisonOperators
    {
        LessThan = 0, //<
        LessThanOrEqual = 1,//<=
        GreaterThan = 2, //>
        GreaterThanOrEqual = 3,//>=
        Equal = 4, //==
        NotEqual = 5, //!=
    }

    public enum LogicalOperators
    {
        And = 6,
        Or = 7,
    }


}
/*
 <
<=
>
>=
==
!=



 
 
 */