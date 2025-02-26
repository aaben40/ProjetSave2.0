using System;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ProjetSave.Service
{
    public class TextBoxOutputter : TextWriter
    {
        private TextBox textBox;
        private StringBuilder buffer = new StringBuilder();
        private Dispatcher dispatcher;

        public TextBoxOutputter(TextBox output, Dispatcher dispatcher)
        {
            textBox = output;
            this.dispatcher = dispatcher;
        }

        public override void Write(char value)
        {
            buffer.Append(value);
            if (value == '\n')
            {
                dispatcher.Invoke(() => {
                    textBox.AppendText(buffer.ToString());
                    buffer.Clear();
                });
            }
        }

        public override void Write(string value)
        {
            dispatcher.Invoke(() => textBox.AppendText(value));
        }

        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }
}
