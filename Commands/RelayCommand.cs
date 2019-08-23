using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        /// The shortcut key gesture
        /// </summary>
        public Key KeyGesture { get; private set; }

        /// <summary>
        /// The modifier to the key gesture
        /// </summary>
        public ModifierKeys GestureModifier { get; private set; }

        /// <summary>
        /// The string representation of the KeyGesture
        /// </summary>
        public string InputGestureText
        {
            get
            {
                if (KeyGesture == Key.None)
                    return string.Empty;
                if (GestureModifier == ModifierKeys.None)
                    return KeyGesture + string.Empty;
                if (GestureModifier.HasFlag(ModifierKeys.Control) && GestureModifier.HasFlag(ModifierKeys.Shift))
                    return "CTRL+SHIFT+" + KeyGesture;
                if (GestureModifier.HasFlag(ModifierKeys.Control))
                    return "CTRL+" + KeyGesture;
                return GestureModifier + "+" + KeyGesture;
            }
        }

        /// <summary>
        /// Sets up a new RelayCommand with supplied Action, check always returns true
        /// </summary>
        /// <param name="execute">The action to execute</param>
        /// <param name="canExecute">The check for the execution</param>
        public RelayCommand(Action execute)
            : this(execute, null, Key.None, ModifierKeys.None) { }

        /// <summary>
        /// Sets up a new RelayCommand with supplied Action, check always returns true
        /// </summary>
        /// <param name="execute">The action to execute</param>
        /// <param name="canExecute">The check for the execution</param>
        public RelayCommand(Action execute, Predicate<object> canExecute)
            : this(execute, canExecute, Key.None, ModifierKeys.None) { }

        /// <summary>
        /// Sets up a new RelayCommand with supplied Action, check always returns true
        /// </summary>
        /// <param name="execute">The action to execute</param>
        /// <param name="key">The Shortcut key</param>
        public RelayCommand(Action execute, Key key) : this(execute, key, ModifierKeys.None) { }

        /// <summary>
        /// Sets up a new RelayCommand with supplied Action, check always returns true
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="key">The shortcut key</param>
        /// <param name="modifier">The shortcut key modifier</param>
        public RelayCommand(Action execute, Key key, ModifierKeys modifier) : this(execute, null, key, modifier) { }

        /// <summary>
        /// Sets up a new RelayCommand with supplied Action and Predicate
        /// </summary>
        /// <param name="execute">The action to execute</param>
        /// <param name="canExecute">The check for the execution</param>
        /// <param name="key">The shortcut key</param>
        public RelayCommand(Action execute, Predicate<object> canExecute, Key key)
            : this(execute, canExecute, key, ModifierKeys.None) { }

        /// <summary>
        /// Sets up a new RelayCommand with supplied Action and Predicate
        /// </summary>
        /// <param name="execute">The action to execute</param>
        /// <param name="canExecute">The check for the execution</param>
        /// <param name="key">The shortcut key</param>
        /// <param name="modifier">The shortcut key modifier</param>
        public RelayCommand(Action execute, Predicate<object> canExecute, Key key, ModifierKeys modifier)
        {
            this.execute = execute ?? throw new NullReferenceException("execute");
            canexecute = canExecute;
            KeyGesture = key;
            GestureModifier = modifier;
        }

        /// <summary>
        /// The check for enabling Execute
        /// </summary>
        /// <param name="parameter">Parameter to pass to the predicate</param>
        /// <returns>If the Command can be executed</returns>
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
