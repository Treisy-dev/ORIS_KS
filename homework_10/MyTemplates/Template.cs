using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MyTemplates
{
    public class TemplateEngine
    {
        public string RenderTemplate(string template, object model)
        {
            StringBuilder result = new StringBuilder();
            int currentIndex = 0;

            while (currentIndex < template.Length)
            {
                int openTagIndex = template.IndexOf("@{", currentIndex);

                if (openTagIndex == -1)
                {
                    result.Append(template.Substring(currentIndex));
                    break;
                }

                int closeTagIndex = template.IndexOf("}", openTagIndex);

                if (closeTagIndex == -1)
                {
                    throw new ArgumentException("Invalid template format: Missing closing '}' for '{'");
                }

                result.Append(template.Substring(currentIndex, openTagIndex - currentIndex));
                string expression = template.Substring(openTagIndex + 2, closeTagIndex - openTagIndex - 2).Trim();

                if (expression.StartsWith("@if"))
                {
                    int thenIndex = expression.IndexOf("@then");

                    if (thenIndex != -1)
                    {
                        string condition = expression.Substring(3, thenIndex - 3).Trim();
                        string thenBlock = expression.Substring(thenIndex + 5).Trim();

                        if (EvaluateCondition(condition, model))
                        {
                            result.Append(thenBlock);
                        }
                    }
                }
                else if (expression.StartsWith("@for"))
                {
                    int inIndex = expression.IndexOf("in");

                    if (inIndex != -1)
                    {
                        string variable = expression.Substring(4, inIndex - 4).Trim();
                        string collectionName = expression.Substring(inIndex + 2).Trim();

                        IEnumerable<object> collection = GetCollection(collectionName, model);

                        foreach (var item in collection)
                        {
                            string itemTemplate = template.Substring(closeTagIndex + 1);

                            int endForIndex = itemTemplate.IndexOf("@endfor");

                            if (endForIndex != -1)
                            {
                                string itemRendered = RenderTemplate(itemTemplate.Substring(0, endForIndex), item);
                                result.Append(itemRendered);
                                itemTemplate = itemTemplate.Substring(endForIndex + 7);
                            }
                        }
                    }
                }
                else
                {
                    string variableName = expression;
                    string value = GetValue(variableName, model);
                    result.Append(value);
                }

                currentIndex = closeTagIndex + 1;
            }

            return result.ToString();
        }

        private bool EvaluateCondition(string condition, object model)
        {
            string[] conditionComponents = condition.Split(' ');

            if (conditionComponents.Length != 3)
            {
                return false;
            }

            string leftOperand = conditionComponents[0];
            string comparisonOperator = conditionComponents[1];
            string rightOperand = conditionComponents[2];

            PropertyInfo leftProperty = model.GetType().GetProperty(leftOperand);
            if (leftProperty == null)
            {
                return false;
            }
            object leftValue = leftProperty.GetValue(model);

            object rightValue = null;
            if (leftProperty.PropertyType == typeof(int))
            {
                int parsedValue;
                if (int.TryParse(rightOperand, out parsedValue))
                {
                    rightValue = parsedValue;
                }
            }

            if (rightValue == null)
            {
                return false;
            }

            switch (comparisonOperator)
            {
                case "==":
                    return leftValue.Equals(rightValue);
                case "!=":
                    return !leftValue.Equals(rightValue);
                default:
                    return false;
            }
        }

        private IEnumerable<object> GetCollection(string collectionName, object model)
        {
            PropertyInfo collectionProperty = model.GetType().GetProperty(collectionName);
            if (collectionProperty == null)
            {
                return new List<object>();
            }

            object collectionValue = collectionProperty.GetValue(model);

            if (collectionValue is IEnumerable<object>)
            {
                return (IEnumerable<object>)collectionValue;
            }
            else
            {
                return new List<object>();
            }
        }

        private string GetValue(string variableName, object model)
        {
            PropertyInfo property = model.GetType().GetProperty(variableName);
            if (property == null)
            {
                return "";
            }

            object value = property.GetValue(model);

            if (value != null)
            {
                return value.ToString();
            }
            else
            {
                return "";
            }
        }
    }
}