using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// this is a static class that implements a reverse polish notation calculator.
/// use this with "using static RPNEvaluator" in the header of your file.
/// Then use any of the methods, or just use RPNEvaluator.Evaluate(...)
/// </summary>
public static class RPNEvaluator
{
    /// <summary>
    /// This evaluates a reverse polish notation.
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="variables"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static int Evaluate(string expression, Dictionary<string, int> variables = null)
    {
        if (string.IsNullOrEmpty(expression))
            return 0;

        if (expression == "base" && variables != null && variables.ContainsKey("base"))
            return variables["base"];

        Stack<int> stack = new Stack<int>();
        string[] tokens = expression.Split(' ');

        foreach (string token in tokens)
        {
            if (IsOperator(token))
            {
                if (stack.Count < 2)
                    throw new ArgumentException($"Invalid RPN expression: {expression}. Not enough operands for operator {token}.");

                int b = stack.Pop();
                int a = stack.Pop();
                stack.Push(ApplyOperator(a, b, token));
            }
            else if (int.TryParse(token, out int value))
            {
                stack.Push(value);
            }
            else if (variables != null && variables.ContainsKey(token))
            {
                stack.Push(variables[token]);
            }
            else
            {
                throw new ArgumentException($"Invalid token in RPN expression: {token}");
            }

            var stackStr = "";
            foreach (var i in stack.ToList()) stackStr += i.ToString() + " ";
            //Debug.Log(stackStr);
        }

        if (stack.Count != 1)
        {

            var stackStr = "";
            foreach (var i in stack.ToList()) stackStr += i.ToString() + " ";

            throw new ArgumentException($"Invalid RPN expression: {expression}. Stack contains {stackStr} elements after evaluation.");
        }

        return stack.Pop();
    }

    private static bool IsOperator(string token)
    {
        return token == "+" || token == "-" || token == "*" || token == "/" || token == "%";
    }

    private static int ApplyOperator(int a, int b, string op)
    {
        return op switch
        {
            "+" => a + b,
            "-" => a - b,
            "*" => a * b,
            "/" => a / b,
            "%" => a % b,
            _ => throw new ArgumentException($"Unsupported operator: {op}")
        };
    }
}