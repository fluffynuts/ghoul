namespace Ghoul.Ui
{
    internal interface IUserInput
    {
        IPromptResult Prompt(string caption, string message, params string[] options);
    }

    // ReSharper disable once UnusedMember.Global
    internal class UserInput : IUserInput
    {
        public IPromptResult Prompt(string caption, string message, params string[] options)
        {
            var prompt = new PromptForm(caption,
                message,
                options);
            return prompt.Prompt();
        }
    }
}