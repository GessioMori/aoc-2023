using Main.Tools;
using System;
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
            (Dictionary<string, Workflow> workflowDict, List<Part> _) = ProcessLines(inputData);

            Dictionary<Category, (int, int)> originalRanges = new() {
                {Category.x, (1, 4000) },
                {Category.m, (1, 4000) },
                {Category.a, (1, 4000) },
                {Category.s, (1, 4000) }
            };

            Range firstRange = new Range(workflowDict["in"], originalRanges, 0);

            List<Range> rangeQueue = [firstRange];

            List<Range> approvedRanges = [];

            while (rangeQueue.Count > 0)
            {
                Range currentRange = rangeQueue[0];

                rangeQueue.RemoveAt(0);

                ProcessRangeForWorkflowRule(currentRange, currentRange.currentWorkflow, currentRange.currentWorkflowStage, approvedRanges, workflowDict, rangeQueue);
            }

            double total = approvedRanges.Sum(range =>
            {
                double totalX = CalculateTotalForCategory(range, Category.x);
                double totalM = CalculateTotalForCategory(range, Category.m);
                double totalA = CalculateTotalForCategory(range, Category.a);
                double totalS = CalculateTotalForCategory(range, Category.s);

                return totalX * totalM * totalA * totalS;
            });

            return total.ToString();
        }

        static double CalculateTotalForCategory(Range range, Category category)
        {
            (int start, int end) = range.ranges[category];
            return end - start + 1;
        }

        public static void ProcessRangeForWorkflowRule(Range range, Workflow currentWorkflow, int currentWorkflowRule, List<Range> approvedRanges, Dictionary<string, Workflow> workflowDict, List<Range> rangeQueue)
        {
            if (currentWorkflow.rules.Count == currentWorkflowRule)
            {
                if (currentWorkflow.fallback == "A")
                {
                    Range copy = new(range.currentWorkflow, range.ranges, range.currentWorkflowStage);
                    approvedRanges.Add(copy);
                }
                else if (currentWorkflow.fallback != "R")
                {
                    range.currentWorkflow = workflowDict[currentWorkflow.fallback];
                    range.currentWorkflowStage = 0;
                    rangeQueue.Add(range);
                }
            }
            else
            {
                Rule currentRule = currentWorkflow.rules[currentWorkflowRule];

                Category categoryToSplit = currentRule.category;

                int numToSplit = currentRule.isGreater ? currentRule.cutLine + 1 : currentRule.cutLine - 1;

                bool shouldSplit = range.ranges[categoryToSplit].Item1 < numToSplit &&
                    range.ranges[categoryToSplit].Item2 > numToSplit;

                if (shouldSplit)
                {
                    Dictionary<Category, (int, int)> DictPartA = new(range.ranges);

                    DictPartA[currentRule.category] = (DictPartA[currentRule.category].Item1, currentRule.isGreater ? numToSplit - 1 : numToSplit);

                    bool isApproval = currentRule.result == "A";

                    bool isRepproval = currentRule.result == "R";

                    bool shouldApprovePartA = isApproval && !currentRule.isGreater;

                    bool shouldApprovePartB = isApproval && currentRule.isGreater;

                    bool shouldRepprovePartA = isRepproval && !currentRule.isGreater;

                    bool shouldRepprovePartB = isRepproval && currentRule.isGreater;

                    Range rangePartA = new(!currentRule.isGreater ? isApproval ? range.currentWorkflow : workflowDict[!isRepproval ? currentRule.result : range.currentWorkflow.name] : currentWorkflow,
                        DictPartA,
                        currentRule.isGreater ? currentWorkflowRule + 1 : 0);

                    Dictionary<Category, (int, int)> DictPartB = new(range.ranges);

                    DictPartB[currentRule.category] = (currentRule.isGreater ? numToSplit : numToSplit + 1, DictPartB[currentRule.category].Item2);

                    Range rangePartB = new(currentRule.isGreater ? isApproval ? range.currentWorkflow : workflowDict[!isRepproval ? currentRule.result : range.currentWorkflow.name] : currentWorkflow,
                        DictPartB,
                        currentRule.isGreater ? 0 : currentWorkflowRule + 1);



                    if (shouldApprovePartA)
                    {
                        approvedRanges.Add(rangePartA);
                    }
                    else if (!shouldRepprovePartA)
                    {
                        rangeQueue.Add(rangePartA);
                    }

                    if (shouldApprovePartB)
                    {
                        approvedRanges.Add(rangePartB);
                    }
                    else if (!shouldRepprovePartB)
                    {
                        rangeQueue.Add(rangePartB);
                    }


                }
            }
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

    internal class Range(Workflow currentWorkflow, Dictionary<Category, (int, int)> ranges, int currentWorkflowStage)
    {
        public Dictionary<Category, (int, int)> ranges = ranges;

        public Workflow currentWorkflow = currentWorkflow;

        public int currentWorkflowStage = currentWorkflowStage;
    }
}
