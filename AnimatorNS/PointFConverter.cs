namespace AnimatorNS
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;

    public class PointFConverter : ExpandableObjectConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                try
                {
                    string[] strArray = ((string) value).Split(new char[] { ',' });
                    float x = 0f;
                    float y = 0f;
                    if (strArray.Length > 1)
                    {
                        x = float.Parse(strArray[0].Trim().Trim(new char[] { '{', 'X', 'x', '=' }));
                        y = float.Parse(strArray[1].Trim().Trim(new char[] { '}', 'Y', 'y', '=' }));
                    }
                    else if (strArray.Length == 1)
                    {
                        x = float.Parse(strArray[0].Trim());
                        y = 0f;
                    }
                    else
                    {
                        x = 0f;
                        y = 0f;
                    }
                    return new PointF(x, y);
                }
                catch
                {
                    throw new ArgumentException("Cannot convert [" + value.ToString() + "] to pointF");
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if ((destinationType == typeof(string)) && (value.GetType() == typeof(PointF)))
            {
                PointF tf = (PointF) value;
                return string.Format("{{X={0}, Y={1}}}", tf.X, tf.Y);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

