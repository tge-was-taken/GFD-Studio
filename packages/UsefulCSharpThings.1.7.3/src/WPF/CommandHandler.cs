using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UsefulThings.WPF
{
    /// <summary>
    /// Creates a command in an easy manner. Not my code.
    /// </summary>
    public class CommandHandler : ICommand
    {
        internal Action<Object> actionWArgs { get; set; }   // KFreon: Action to perform given arguments
        internal Action actionWOArgs { get; set; }   // KFreon: Action to perform given no arguments
        internal bool _canExecute { get; set; }

        /// <summary>
        /// Presents WPF Commands in an easy to use wrapper.
        /// </summary>
        /// <param name="canExecute">If true, can perform command/action.</param>
        public CommandHandler(bool canExecute)
        {
            _canExecute = canExecute;

        }


        /// <summary>
        /// Presents WPF Commands in an easy to use wrapper.
        /// </summary>
        /// <param name="action">Action to perform WITHOUT arguments.</param>
        /// <param name="canExecute">true = command can be performed.</param>
        public CommandHandler(Action action, bool canExecute = true) : this(canExecute)
        {
            actionWOArgs = action;
        }


        /// <summary>
        /// Presents WPF Commands in an easy to use wrapper.
        /// </summary>
        /// <param name="action">Action to perform WITH arguments.</param>
        /// <param name="canExecute">true = command can be performed.</param>
        public CommandHandler(Action<Object> action, bool canExecute = true) : this(canExecute)
        {
            actionWArgs = action;
        }


        /// <summary>
        /// Changes "enabled" status of command.
        /// </summary>
        /// <param name="parameter">CURRENTLY NOT USED</param>
        /// <returns>CanExecute status</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }


        /// <summary>
        /// Executes command.
        /// </summary>
        /// <param name="parameter">Parameter to give to Action. Can be null.</param>
        public virtual void Execute(object parameter)
        {
            // KFreon: Run designated action
            if (actionWOArgs != null)
                ((Action)actionWOArgs)();
            else if (actionWArgs != null)
                ((Action<object>)actionWArgs)(parameter);
        }



        public event EventHandler CanExecuteChanged;
    }
}
