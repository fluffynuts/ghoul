namespace Ghoul
{
    internal interface IUserInput
    {
        IPromptResult Prompt(string caption, string message);
    }
    // ReSharper disable once UnusedMember.Global
    internal class UserInput: IUserInput
    {
        public IPromptResult Prompt(string caption, string message)
        {
            var prompt = new PromptForm(caption, message);
            return prompt.Prompt();
        }
    }
}