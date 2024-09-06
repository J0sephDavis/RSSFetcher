namespace Murong_Xue.InteractiveMode
{
    internal interface IInteractiveCommand
    {
        /// <summary>
        /// processes the command with the given arguments
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        int Handle(string[] args);
        /// <summary>
        /// resturns the command name, e.g., "print"
        /// </summary>
        /// <returns></returns>
        string GetName();
    }
}
