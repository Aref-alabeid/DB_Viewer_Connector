using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NetConnections.PayJoe.Connector.Client.GUI.ViewModels
{
    public class NcCommand : ICommand
    {
        #region Member

        private Action mAction = null;
        private Func<bool> mCanExecuteFunc = null;
        private bool mCanExecute = true;

        #endregion

        public NcCommand(Action pExecute)
        {
            mAction = pExecute;
            mCanExecute = true;
        }

        public NcCommand(Action pExecute, Func<bool> pCanExecute)
        {
            mAction = pExecute;
            mCanExecuteFunc = pCanExecute;
        }

        //public NcCommand(Action<object> execute);
        //public NcCommand(Action<object> execute, Func<object, bool> canExecute);

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (mCanExecuteFunc != null)
                return mCanExecuteFunc();

            return mCanExecute;
        }

        public void ChangeCanExecute()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, null);
        }

        public void Execute(object parameter)
        {
            mAction();
        }
    }
}
