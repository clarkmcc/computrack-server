﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Windows;

namespace CompuTrack_Server
{
    class consoleWriter
    {
        public class TextBoxConsole : TextWriter
        {
            TextBox output = null; //Textbox used to show Console's output.

            /// <summary>
            /// Custom TextBox-Class used to print the Console output.
            /// </summary>
            /// <param name="_output">Textbox used to show Console's output.</param>
            public TextBoxConsole(TextBox _output)
            {
                output = _output;
                //output.ScrollBars = ScrollBar.Both;
                //output.WordWrap = true;
            }

            //<summary>
            //Appends text to the textbox and to the logfile
            //</summary>
            //<param name="value">Input-string which is appended to the textbox.</param>
            public override void Write(char value)
            {
                base.Write(value);
                output.AppendText(value.ToString());//Append char to the textbox
                output.ScrollToEnd();
            }



            public override Encoding Encoding
            {
                get { return System.Text.Encoding.UTF8; }
            }
        }
    }
}
