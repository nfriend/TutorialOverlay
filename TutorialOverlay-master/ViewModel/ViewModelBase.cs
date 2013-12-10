using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace HelpOverlay.ViewModel
{
    class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void NotifyPropertyChanged<T>(Expression<Func<ViewModelBase, T>> propertyPath)
        {
            MemberExpression me = propertyPath.Body as MemberExpression;
            if (me == null || me.Expression != propertyPath.Parameters[0] || me.Member.MemberType != System.Reflection.MemberTypes.Property)
                throw new InvalidOperationException("something is wrong");
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(me.Member.Name));
        }
    }
}
