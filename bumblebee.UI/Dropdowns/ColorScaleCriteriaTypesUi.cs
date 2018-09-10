﻿using System;
using System.Collections.Generic;
using System.Linq;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using Newtonsoft.Json;
using ProtoCore.AST.AssociativeAST;
using CoreNodeModels;
using bumblebee.Types;

namespace bumblebee.UI
{
    [NodeName("Color Scale Criteria Types")]
    [NodeCategory("archilab_Bumblebee.Types")]
    [NodeDescription("Retrieve all available Color Scale Criteria Types.")]
    [IsDesignScriptCompatible]
    public class ColorScaleCriteriaTypesUi : DSDropDownBase
    {
        private const string OutputName = "Color Scale Criteria Type";
        private const string NoFamilyTypes = "No types were found.";
        public static ColorScaleCriteriaTypes cscTypes = new ColorScaleCriteriaTypes();

        public ColorScaleCriteriaTypesUi() : base(OutputName) { }

        [JsonConstructor]
        public ColorScaleCriteriaTypesUi(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(OutputName, inPorts, outPorts) { }

        protected override SelectionState PopulateItemsCore(string currentSelection)
        {
            Items.Clear();

            var d = new Dictionary<string, int>(cscTypes.Types);

            if (d.Count == 0)
            {
                Items.Add(new DynamoDropDownItem(NoFamilyTypes, null));
                SelectedIndex = 0;
                return SelectionState.Done;
            }

            foreach (var pair in d)
            {
                Items.Add(new DynamoDropDownItem(pair.Key, pair.Value));
            }
            Items = Items.OrderBy(x => x.Name).ToObservableCollection();
            return SelectionState.Restore;
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            if (Items.Count == 0 ||
                Items[0].Name == NoFamilyTypes ||
                SelectedIndex == -1)
            {
                return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
            }

            var args = new List<AssociativeNode>
            {
                AstFactory.BuildStringNode(Items[SelectedIndex].Name)
            };

            var func = new Func<string, int>(ColorScaleCriteriaTypes.ByName);
            var functionCall = AstFactory.BuildFunctionCall(func, args);

            return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), functionCall) };
        }
    }
}