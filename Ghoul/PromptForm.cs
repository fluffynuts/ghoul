using System.Windows.Forms;

namespace Ghoul
{
    public partial class PromptForm : Form
    {
        public PromptForm(string caption, string message)
        {
            InitializeComponent();
            Text = caption;
            label1.Text = message;
        }

        public PromptResult Prompt()
        {
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
            }
        }
    }

    public class PromptResult
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