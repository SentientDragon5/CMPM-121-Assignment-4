using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This is a static class that implements a reverse polish notation calculator for float values.
/// </summary>
public static class FloatRPNEvaluator
{
    /// <summary>
    /// This evaluates a reverse polish notation expression with floating point values.
    /// </summary>
    /// <param name="expression">The RPN expression to evaluate</param>
    /// <param name="variables">Dictionary of variable names and their float values</param>
    /// <returns>The float result of the evaluated expression</returns>
    public static float Evaluate(string expression, Dictionary<string, float> variables = null)
    {
        if (string.IsNullOrEmpty(expression))
            return 0f;

        if (expression == "base" && variables != null && variables.ContainsKey("base"))
            return variables["base"];

        Stack<float> stack = new Stack<float>();
        string[] tokens = expression.Split(' ');

        foreach (string token in tokens)
        {
            if (IsOperator(token))
            {
                if (stack.Count < 2)
                    throw new ArgumentException($"Invalid RPN expression: {expression}. Not enough operands for operator {token}.");

                float b = stack.Pop();
                float a = stack.Pop();
                stack.Push(ApplyOperator(a, b, token));
            }
            else if (float.TryParse(token, out float value))
            {
                stack.Push(value);
            }
            else if (int.TryParse(token, out int intValue))
            {
                stack.Push((float)intValue);
            }
            else if (variables != null && variables.ContainsKey(token))
            {
                stack.Push(variables[token]);
            }
            else
            {
                throw new ArgumentException($"Invalid token in RPN expression: {token}");
            }
        }

        if (stack.Count != 1)
        {
            var stackStr = "";
            foreach(var i in stack.ToList()) stackStr += i.ToString() + " ";

            throw new ArgumentException($"Invalid RPN expression: {expression}. Stack contains {stackStr} elements after evaluation.");
        }

        return stack.Pop();
    }

    private static bool IsOperator(string token)
    {
        return token == "+" || token == "-" || token == "*" || token == "/" || token == "%";
    }

    private static float ApplyOperator(float a, float b, string op)
    {
        return op switch
        {
            "+" => a + b,
            "-" => a - b,
            "*" => a * b,
            "/" => b != 0 ? a / b : float.PositiveInfinity,
            "%" => b != 0 ? a % b : float.NaN,
            _ => throw new ArgumentException($"Unsupported operator: {op}")
        };
    }
}