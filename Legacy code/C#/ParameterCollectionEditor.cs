using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Drawing.Design;

namespace PsychComponent
{
    class ParameterCollectionEditor : CollectionEditor
    {
        private List<Type> Types;
        public ParameterCollectionEditor(Type type)
            : base(type)
        {
            Types=new List<Type>();
            Types.Add(typeof(RandomFlatParameter));
            Types.Add(typeof(RandomGaussParameter));
        }

        protected override Type[] CreateNewItemTypes()
        {
            return Types.ToArray();
        }

        protected override bool CanSelectMultipleInstances()
        {
            return false;
        }

        protected override string GetDisplayText(object value)
        {
            return value.GetType().Name;
        }
    }
}
