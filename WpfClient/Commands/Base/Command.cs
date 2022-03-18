using System;
using System.Windows.Input;


namespace WpfClient.Commands.Base
{
    internal abstract class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public abstract bool CanExecute(object param);
        public abstract void Execute(object param);

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }
    }
}
