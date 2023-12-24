using Main.Tools;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Main.Solutions
{
    internal class Day19Solution : ISolution
    {
        public string RunPartA(string[] inputData)
        {
            (Dictionary<string, Workflow> workflowDict, List<Part> listOfParts) = ProcessLines(inputData);

            List<Part> approvedParts = listOfParts.Where(part => IsApproved(workflowDict, part, "in")).ToList();

            long totalSum = approvedParts.Sum(part => part.values.Values.Sum());

            return totalSum.ToString();
        }

        public string RunPartB(string[] inputData)
        {
            throw new NotImplementedException();
        }

        public static (Dictionary<string, Workflow>, List<Part>) ProcessLines(string[] inputData)
        {
            Dictionary<string, Workflow> workflowDict = [];
            List<Part> listOfParts = [];

            int indexOfEmptyString = Array.IndexOf(inputData, "");

            if (indexOfEmptyString != -1)
            {
                List<string> workflowLines = inputData.Take(indexOfEmptyString).ToList();
                List<string> partLines = inputData.Skip(indexOfEmptyString + 1).ToList();

                foreach (string line in workflowLines)
                {
                    string[] nameAndDesc = line[..(line.Length - 1)].Split('{');
                    string name = nameAndDesc[0];
                    string[] rules = nameAndDesc[1].Split(',');
                    string fallback = rules[^1];
                    Workflow newWorkflow = new()
                    {
                        fallback = fallback,
                        name = name
                    };

                    for (int i = 0; i < rules.Length - 1; i++)
                    {
                        Match match = Regex.Match(rules[i], @"^([xmas])([<>])(\d+):([a-zA-Z]+)$");

                        if (match.Success && Enum.TryParse(match.Groups[1].Value, out Category category))
                        {
                            newWorkflow.rules.Add(new Rule()
                            {
                                category = category,
                                cutLine = int.Parse(match.Groups[3].Value),
                                isGreater = match.Groups[2].Value == ">",
                                result = match.Groups[4].Value
                            });
                        }
                        else
                        {
                            throw new Exception("Error parsing.");
                        }
                    }

                    workflowDict.Add(newWorkflow.name, newWorkflow);
                }

                foreach (string line in partLines)
                {
                    string[] values = line.Trim('{', '}').Split(',');

                    Part newPart = new();

                    foreach (string v in values)
                    {
                        string[] parts = v.Split('=');

                        if (parts.Length == 2 && int.TryParse(parts[1], out int value) && Enum.TryParse(parts[0], out Category category))
                        {
                            newPart.values[category] = value;
                        }
                    }

                    listOfParts.Add(newPart);
                }

                return (workflowDict, listOfParts);
            }

            throw new Exception("Error parsing.");
        }

        public static bool IsApproved(Dictionary<string, Workflow> workflowDict, Part part, string workflowName)
        {
            if (workflowName == "R") return false;
            if (workflowName == "A") return true;

            if (workflowDict.TryGetValue(workflowName, out Workflow? workflow))
            {
                foreach (Rule rule in workflow.rules)
                {
                    if (IsRuleAproved(rule, part))
                    {
                        return IsApproved(workflowDict, part, rule.result);
                    }
                }

                return IsApproved(workflowDict, part, workflow.fallback);
            }
            else
            {
                throw new Exception($"Work flow \"{workflowName}\" not found.");
            }
        }

        public static bool IsRuleAproved(Rule rule, Part part)
        {
            return rule.isGreater ? part.values[rule.category] > rule.cutLine : part.values[rule.category] < rule.cutLine;
        }
    }

    internal enum Category
    {
        x,
        m,
        a,
        s
    }

    internal class Rule
    {
        public required Category category;
        public required bool isGreater;
        public required int cutLine;
        public required string result;
    }

    internal class Workflow
    {
        public required string name;
        public List<Rule> rules = [];
        public required string fallback;
    }

    internal class Part
    {
        public Dictionary<Category, int> values = [];
    }
}
