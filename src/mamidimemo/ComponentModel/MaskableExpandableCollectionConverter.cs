﻿// copyright-holders:K.Ito
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections;
using System.Globalization;

namespace zanac.MAmidiMEmo.ComponentModel
{
    /// <summary>
    /// </summary>
    public class MaskableExpandableCollectionConverter : CollectionConverter
    {
        /// <summary>
        /// 
        /// </summary>
        public MaskableExpandableCollectionConverter()
        {
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                ICollection c = value as ICollection;
                if (c != null)
                    return context.PropertyDescriptor.DisplayName + "[" + c.Count + "]";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            PropertyDescriptor[] array = null;
            ICollection list = value as ICollection;
            if (list != null)
            {
                array = new PropertyDescriptor[list.Count];
                Type type = typeof(ICollection);
                int i = 0;
                foreach (object o in list)
                {
                    string name = string.Format(CultureInfo.InvariantCulture,
                        "[{0}]", i.ToString("d" + list.Count.ToString
                        (NumberFormatInfo.InvariantInfo).Length, null));
                    CollectionPropertyDescriptor cpd = new CollectionPropertyDescriptor(context, type, name, o.GetType(), i);
                    array[i] = cpd;
                    i++;
                }
            }
            return new PropertyDescriptorCollection(array);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        private class CollectionPropertyDescriptor : SimplePropertyDescriptor
        {

            private ITypeDescriptorContext context;

            private int index;

            private uint maskValue;

            public override bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            public override object GetEditor(Type editorBaseType)
            {
                if (maskValue != 0)
                    return new SlideEditor(0, (int)maskValue);

                return base.GetEditor(editorBaseType);
            }

            public CollectionPropertyDescriptor(ITypeDescriptorContext context, Type componentType, string name, Type elementType, int index)
                : base(componentType, name, elementType)
            {
                this.context = context;
                this.index = index;

                MaskAttribute att = (MaskAttribute)context.PropertyDescriptor.Attributes[typeof(MaskAttribute)];
                maskValue = att.MaskValue;
            }

            public override object GetValue(object component)
            {
                ICollection c = component as ICollection;
                if (c != null)
                {
                    if (c.Count > index)
                    {
                        int i = 0;
                        foreach (object o in c)
                        {
                            if (i == index)
                                return o;
                            i++;
                        }
                    }
                }
                return null;
            }

            public override void SetValue(object component, object value)
            {
                IList c = component as IList;
                if (c != null)
                {
                    switch (value)
                    {
                        case byte n:
                            {
                                c[index] = (byte)(n & maskValue);
                                break;
                            }
                        case short n:
                            {
                                c[index] = (short)(n & maskValue);
                                break;
                            }
                        case ushort n:
                            {
                                c[index] = (ushort)(n & maskValue);
                                break;
                            }
                        case uint n:
                            {
                                c[index] = (uint)(n & maskValue);
                                break;
                            }
                        case int n:
                            {
                                c[index] = (int)(n & maskValue);
                                break;
                            }
                    }
                }
            }

            public override string Description
            {
                get
                {
                    return context.PropertyDescriptor.Description;
                }
            }


        }
    }
}
