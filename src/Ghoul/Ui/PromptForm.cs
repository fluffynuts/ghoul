using System.Linq;
using System.Windows.Forms;
using PeanutButter.Utils;

namespace Ghoul.Ui
{
    public partial class PromptForm : Form
    {
        private readonly string _caption;

        public PromptForm(
            string caption, 
            string message,
            params string[] options)
        {
            InitializeComponent();
            _caption = caption;
            label1.Text = message;
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(options.Cast<object>().ToArray());
        }

        public IPromptResult Prompt()
        {
            Text = _caption;
            var result = ShowDialog();
            return new PromptResult(
                result,
                result == DialogResult.OK
                    ? comboBox1.Text
                    : ""
            );
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                //button1.PerformClick();
                MessageBox.Show(comboBox1.Text);
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