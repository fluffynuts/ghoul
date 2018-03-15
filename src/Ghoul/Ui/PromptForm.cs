﻿using System.Windows.Forms;

namespace Ghoul.Ui
{
    public partial class PromptForm : Form
    {
        private readonly string _caption;

        public PromptForm(string caption, string message)
        {
            InitializeComponent();
            _caption = caption;
            label1.Text = message;
        }

        public IPromptResult Prompt()
        {
            Text = _caption;
            var result = ShowDialog();
            return new PromptResult(
                result,
                result == DialogResult.OK
                    ? textBox1.Text
                    : ""
            );
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                button1.PerformClick();
            }
        }
    }

    public interface IPromptResult
    {
        DialogResult Result { get; }
        string UserInput { get; }
    }

    public class PromptResult: IPromptResult
    {
        public DialogResult Result { get; }
        public string UserInput { get; }

        public PromptResult(DialogResult result, string userInput)
        {
            Result = result;
            UserInput = userInput;
        }
    }
}