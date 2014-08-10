﻿namespace VisualMutator.Model.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using TestsTree;
    using UsefulTools.CheckboxedTree;
    using UsefulTools.Core;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Switches;

    public class TestsSelector
    {

        public SelectedTests GetIncludedTests(List<TestNodeNamespace> namespaces)
        {
            ICollection<TestId> selected = namespaces.Cast<CheckedNode>()
                .SelectManyRecursive(node => node.Children, node => node.IsIncluded ?? true, leafsOnly: true)
                .OfType<TestNodeMethod>()
                .Select(m => m.TestId)
                .ToList();

            return new SelectedTests(selected, CreateMinimalTestsInfo(namespaces));
        }

        public string NameExtractor(TestTreeNode node)
        {
            return Switch.Into<string>().FromTypeOf(node)
                .Case<TestNodeNamespace>(n => n.Name)
                .Case<TestNodeClass>(n => n.FullName)
                .Case<TestNodeMethod>(n => n.TestId.ToString())
                .GetResult();
        }

        public List<string> CreateMinimalTestsInfo(IEnumerable<TestNodeNamespace> namespaces)
        {
            var ids = namespaces.SelectMany(n => 
                MinimalTreeId((TestTreeNode) n, NameExtractor, a => 
                (a.Children ?? new NotifyingCollection<CheckedNode>()).Cast<TestTreeNode>()));

            return ids.ToList();
        }

        public List<string> MinimalTreeId<Node>(Node node,
            Func<Node, string> nameExtractor, Func<Node, IEnumerable<Node>> childExtractor) where Node : CheckedNode
        {
            if(node.IsIncluded.HasValue) // fully included or excluded
            {
                return node.IsIncluded.Value ? nameExtractor(node).InList() : new List<string>();
            }
            else
            {
                var strings = childExtractor(node)
                    .SelectMany(n => MinimalTreeId(n, nameExtractor, childExtractor))
                    .Where(n => !string.IsNullOrWhiteSpace(n)).ToList();
                return strings;
            }

        }
    }
}