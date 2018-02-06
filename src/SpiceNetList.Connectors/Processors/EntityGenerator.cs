﻿using SpiceNetlist.SpiceObjects;
using SpiceNetlist.SpiceObjects.Parameters;
using SpiceNetlist.SpiceSharpConnector.Expressions;
using SpiceSharp;
using SpiceSharp.Circuits;
using System.Collections.Generic;
using System.Linq;

namespace SpiceNetlist.SpiceSharpConnector.Processors
{
    public abstract class EntityGenerator
    {
        public abstract Entity Generate(string name, string type, ParameterCollection parameters, NetList currentNetList);

        public abstract List<string> GetGeneratedTypes();

        protected void SetParameters(Entity entity, ParameterCollection parameters, NetList currentNetList, int toSkip = 0, int count = 0)
        {
            foreach (var parameter in parameters.Values.Skip(toSkip).Take(parameters.Values.Count - toSkip - count))
            {
                if (parameter is AssignmentParameter ap)
                {
                    entity.ParameterSets.SetProperty(ap.Name, currentNetList.ParseDouble(ap.Value));
                }
            }
        }

        protected static void CreateNodes(ParameterCollection parameters, SpiceSharp.Components.Component c)
        {
            //TODO Refactor this .....
            Identifier[] nodes = new Identifier[c.PinCount];
            for (var i = 0; i < c.PinCount; i++)
            {
                nodes[i] = (parameters.Values[i] as SingleParameter).RawValue;
            }
            c.Connect(nodes);
        }
    }
}