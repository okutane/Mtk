using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace UI
{
    public class ValueChangedEventArgs<T> : EventArgs
    {
        T _newValue;

        public ValueChangedEventArgs(T newValue)
        {
            this._newValue = newValue;
        }

        public T NewValue
        {
            get
            {
                return this._newValue;
            }
        }
    }

    public class DoubleValueTextBox : TextBox
    {
        private double _minValue = 0;

        public event EventHandler<ValueChangedEventArgs<double>> ValueChanged;

        protected virtual void OnValueChanged(ValueChangedEventArgs<double> e)
        {
            if (this.ValueChanged != null)
            {
                ValueChanged(this, e);
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            try
            {
                double value = double.Parse(this.Text);
                if (value < this._minValue)
                {
                    this.ForeColor = Color.Red;
                }
                else
                {
                    this.ForeColor = Color.Black;
                    OnValueChanged(new ValueChangedEventArgs<double>(value));
                }
            }
            catch (FormatException)
            {
                this.ForeColor = Color.Red;
            }

            base.OnTextChanged(e);
        }
    }
}
