using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DiskpartGUI.Commands
{
    class RelayCommand : ICommand
    {
        /// <summary>
        /// The action to be executed
        /// </summary>
        readonly Action execute;

        /// <summary>
        /// The check for the action
        /// </summary>
        readonly Predicate<object> canexecute;

        /// <summary>
        /// CanExecuteChanged event handler
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Sets up a new RelayCommand with supplied Action and Predicate
        /// </summary>
        /// <param name="execute">The action to execute</param>
        /// <param name="canExecute">The check for the execution</param>
        public RelayCommand(Action execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new NullReferenceException("execute");
            this.execute = execute;
            canexecute = canExecute;
        }

        /// <summary>
        /// Sets up a new RelayCommand with supplied Action, check always returns true
        /// </summary>
        /// <param name="execute"></param>
        public RelayCommand(Action execute) : this(execute, null) { }

        /// <summary>
        /// The check for enabling Execute
        /// </summary>
        /// <param name="parameter">Parameter to pass to the predicate</param>
        /// <returns></returns>
        public virtual bool CanExecute(object parameter)
        {
            return canexecute == null ? true : canexecute(parameter);
        }

        /// <summary>
        /// The method to execute
        /// </summary>
        /// <param name="parameter"></param>
        public virtual void Execute(object parameter)
        {
            execute.Invoke();
        }
    }
}
