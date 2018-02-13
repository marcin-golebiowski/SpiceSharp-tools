﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SpiceNetlist.SpiceObjects.Parameters;

namespace SpiceNetlist.SpiceObjects
{
    public class ParameterCollection : SpiceObject, IEnumerable, IEnumerable<Parameter>
    {
        public ParameterCollection()
        {
            Values = new List<Parameter>();
        }

        public int Count
        {
            get
            {
                return Values.Count;
            }
        }

        protected List<Parameter> Values { get; set; }

        public Parameter this[int index] => Values[index];

        public void Clear()
        {
            Values.Clear();
        }

        public void Add(Parameter parameter)
        {
            Values.Add(parameter);
        }

        public IEnumerator GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        IEnumerator<Parameter> IEnumerable<Parameter>.GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        public void Insert(int index, Parameter parameter)
        {
            Values.Insert(index, parameter);
        }

        public void Merge(ParameterCollection ps2)
        {
            Values.AddRange(ps2.Values);
        }

        public ParameterCollection Skip(int count)
        {
            var result = new ParameterCollection();
            result.Values.AddRange(this.Values.Skip(count));

            return result;
        }

        public ParameterCollection Take(int count)
        {
            var result = new ParameterCollection();
            result.Values.AddRange(this.Values.Take(count));
            return result;
        }

        public string GetString(int parameterIndex)
        {
            var singleParameter = this[parameterIndex] as SingleParameter;
            if (singleParameter == null)
            {
                throw new Exception("Parameter [" + parameterIndex + "] is not string parameter");
            }

            return singleParameter.RawValue;
        }

        public ParameterCollection Clone()
        {
            return new ParameterCollection() { Values = new List<Parameter>(this.Values) };
        }

        public void Remove(int index)
        {
            this.Values.RemoveAt(index);
        }
    }
}
