using GameEnums;
using System;
using UnityEngine;

namespace GameNodes
{
    public class BaseNode : Node
    {
        public BaseNode() { }
        public BaseNode(string name, int inputFieldNumber, int outputFieldNumber) : base(name, inputFieldNumber, outputFieldNumber) { }

        public override void ValueAssignment()
        {
            foreach (var inputRef in inputfields)
            {
                if (!inputRef.isAssigned)
                {
                    return;
                }
            }

            executeReady = true;
        }

        public override void Operation()
        {
            foreach (var outputNode in outputfields[0].nodeRefs)
            {
                outputNode.baseValue = baseValue;
                outputNode.ValueAssignment();
                if (outputNode.executeReady)
                {
                    outputNode.Operation();
                }
            }
        }

        public override string ToString()
        {
            string baseValueString = "In " + name + " Value: " + baseValue.ToString();

            foreach (var outputRef in outputfields)
            {
                if (outputRef.nodeRefs.Count != 0)
                {
                    foreach (var nodeRef in outputRef.nodeRefs)
                    {
                        baseValueString += "\n To Ref: " + nodeRef.name + ": " + nodeRef.ToString() + "\n ";
                    }
                }
            }

            return baseValueString + base.ToString();
        }
    }


    public class BasicMathNode : BaseNode
    {
        public MathOperations operation;
        //public NumberTypes numberType;

        public NodeValue numberA;
        public NodeValue numberB;

        public BasicMathNode(string name) : base(name, 2, 1)
        {
            inputfields[0] = new()
            {
                name = "A",
                fieldInputType = FieldInput.Number,
            };
            inputfields[0].nodeRefs = new();


            inputfields[1] = new()
            {
                name = "B",
                fieldInputType = FieldInput.Number,

            };
            inputfields[1].nodeRefs = new();


            outputfields[0] = new()
            {
                name = "Output",
                fieldInputType = FieldInput.None,
            };
            outputfields[0].nodeRefs = new();
        }

        public override void ValueAssignment()
        {
            if (inputfields[0].nodeRefs[0] != null)
            {
                numberA = inputfields[0].nodeRefs[0].baseValue;
            }

            if (inputfields[1].nodeRefs[0] != null)
            {
                numberB = inputfields[1].nodeRefs[0].baseValue;
            }

            base.ValueAssignment();

        }

        public override void Operation()
        {
            MathOperation(numberA.AsDouble(), numberB.AsDouble());

            Debug.Log("Base Value in Math Node: " + baseValue.ToString());
            base.Operation();

        }

        public void MathOperation(double numA, double numB)
        {
            switch (operation)
            {
                case MathOperations.Add:
                    baseValue.CastDouble((numA + numB));
                    break;
                case MathOperations.Subtract:
                    baseValue.CastDouble((numA - numB));
                    break;
                case MathOperations.Multiply:
                    baseValue.CastDouble((numA * numB));
                    break;
                case MathOperations.Divide:
                    baseValue.CastDouble((numA / numB));
                    break;
            }
        }


        public override string ToString()
        {
            return base.ToString();
        }

    }

    public class ValueNode : BaseNode
    {
        public ValueNode(string name, NodeValueType valueType) : base(name, 1, 1)
        {
            inputfields[0] = new();
            inputfields[0].name = name;

            if (valueType == NodeValueType.Double)
            {
                inputfields[0].fieldInputType = FieldInput.Number;
            }
            else if (valueType == NodeValueType.String)
            {
                inputfields[0].fieldInputType = FieldInput.Text;
            }
            else if (valueType == NodeValueType.Boolean)
            {
                inputfields[0].fieldInputType = FieldInput.Toggle;
            }

            inputfields[0].nodeRefs = new();

            outputfields[0] = new()
            {
                name = "Output",
                fieldInputType = FieldInput.None,
            };
            outputfields[0].nodeRefs = new();

            baseValue.type = valueType;
            baseNode = true;
        }

        public override void Operation()
        {
            if (!executeReady)
            {
                throw new InvalidOperationException("Execute called while not ready");
            }

            Debug.Log(ToString());

            base.Operation();

        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override void ValueAssignment()
        {
            throw new System.NotImplementedException();
        }
    }

    public class StatementNode : BaseNode
    {
        public StatementNode(string name, NodeValueType valueType) : base(name, 1, 1)
        {

        }

        public override void Operation()
        {

        }

        public override void ValueAssignment()
        {
            baseValue = inputfields[0].nodeRefs[0].baseValue;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }


    public class ConditionNode : BaseNode
    {
        public ConditionNode(string name, NodeValueType valueType) : base(name, 1, 1)
        {

        }

        public override void Operation()
        {

        }

        public override void ValueAssignment()
        {
            baseValue = inputfields[0].nodeRefs[0].baseValue;
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }

    public class LoopNode : BaseNode
    {


        public override void Operation()
        {

        }

        public override void ValueAssignment()
        {

        }

        public override string ToString()
        {
            return base.ToString();
        }
    }


    public class ExecuteNode : BaseNode
    {
        public ExecuteNode(string name) : base(name, 1, 0)
        {
            inputfields[0] = new()
            {
                name = "Execute",
                fieldInputType = FieldInput.Number,
            };
            inputfields[0].nodeRefs = new();

            executeReady = true;
        }

        public override void Operation()
        {
            Debug.Log("Base Value in Value Node: " + baseValue.ToString());
        }

        public override void ValueAssignment()
        {
            baseValue = inputfields[0].nodeRefs[0].baseValue;
        }
        public override string ToString()
        {
            return base.ToString();
        }

    }

}

/*
 * 
            if (inputfields[1].nodeRefs[0] != null && inputfields[1].nodeRefs[0] != null)
            {
            }
 * 
 * 
 *         public TypeNumberNode numberA;
        public TypeNumberNode numberB;

        public MathNode(MathNodes opp, NumberTypes type, TypeNumberNode nA, TypeNumberNode nB)
        {
            operation = opp;
            switch (type)
            {
                case NumberTypes.Int:
                    typeNumber = new TypeInt();
                    break;
                case NumberTypes.Long:
                    typeNumber = new TypeLong();
                    break;
                case NumberTypes.Double:
                    typeNumber = new TypeDouble();
                    break;
                case NumberTypes.Float:
                    typeNumber = new TypeFloat();
                    break;
            }

        }

 * 
 *     public class TypeNumberNode : Node
    {
        //public NumberTypes numberType;
        public BaseType typeNumber;
        public TypeNumberNode(NumberTypes numberType)
        {
            switch (numberType)
            {
                case NumberTypes.Int:
                    typeNumber = new TypeInt();
                    break;
                case NumberTypes.Long:
                    typeNumber = new TypeLong();
                    break;
                case NumberTypes.Double:
                    typeNumber = new TypeDouble();
                    break;
                case NumberTypes.Float:
                    typeNumber = new TypeFloat();
                    break;
            }
        }

        public int GetIntValue(NumberTypes numberType)
        {
            return (int)typeNumber.value;
        }

        public double GetDoubleValue(NumberTypes numberType)
        {

        }

        public long GetLongValue(NumberTypes numberType)
        {

        }

        public float GetFloatValue(NumberTypes numberType)
        {

        }



    }
 * 
 * 
    public class TypeNumberNode : Node
    {
        public NumberTypes numberType;
        private double number; // store numerics as double

        public int AsInt() => (int)number;

        public long AsLong() => (long)number;

        public float AsFloat() => (float)number;

        public double AsDouble() => number;

        public static TypeNumberNode From(int v) =>
            new() { numberType = NumberTypes.Int, number = v };

        public static TypeNumberNode From(float v) =>
            new() { numberType = NumberTypes.Float, number = v };

        public static TypeNumberNode From(double v) =>
            new() { numberType = NumberTypes.Double, number = v };

        public static TypeNumberNode From(long v) =>
            new() { numberType = NumberTypes.Long, number = v };
    }
 * 
 * 
         public TypeNumberNode() { }

        public TypeNumberNode(NumberTypes numberType, double number)
        {
            switch (numberType) {
                case NumberTypes.Int:
                    break;
                case NumberTypes.Long:
                    break;
                case NumberTypes.Double:
                    break;
                case NumberTypes.Float:
                    break;
            }
        }

         public void MathOperation(float numA, float numB)
        {
            switch (operation)
            {
                case MathOperations.Add:
                    baseValue.CastFloat((numA + numB));
                    break;
                case MathOperations.Subtract:
                    baseValue.CastFloat((numA - numB));
                    break;
                case MathOperations.Multiply:
                    baseValue.CastFloat((numA * numB));
                    break;
                case MathOperations.Divide:
                    baseValue.CastFloat((numA / numB));
                    break;
            }

        }

        public void MathOperation(int numA, int numB)
        {
            switch (operation)
            {
                case MathOperations.Add:
                    baseValue.CastInt((numA + numB));
                    break;
                case MathOperations.Subtract:
                    baseValue.CastInt((numA - numB));
                    break;
                case MathOperations.Multiply:
                    baseValue.CastInt((numA * numB));
                    break;
                case MathOperations.Divide:
                    baseValue.CastInt((numA / numB));
                    break;
            }
        }

 */