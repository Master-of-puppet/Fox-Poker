using Puppet.Service;
using System;
using System.Collections.Generic;
using Puppet.Core.Model;

namespace Puppet.Service
{
    public interface IDialogData
    {
        bool IsMessageDialog { get; }
        string Title { get; set; }
        string Content { get; set; }
        Action<bool?> ButtonCallback { get; set; }
        System.Func<bool?, string> ButtonName { get; set; }
        Action onDestroy { get; set; }
        void ShowDialog();
        bool EqualTo(IDialogData data);
    }

    public abstract class AbstractDialogData : IDialogData
    {
        public virtual bool IsMessageDialog { get { return false; } }
        public string Title { get; set; }
        public string Content { get; set; }
        public Action<bool?> ButtonCallback { get; set; }
        public Func<bool?, string> ButtonName { get; set; }
        public Action onDestroy { get; set; }

        protected virtual string GetButtonName(bool? button)
        {
            if (button == null)
                return "ĐÓNG";
            else if (button == true)
                return "ĐỒNG Ý";
            else
                return "KHÔNG ĐỒNG Ý";
        }

        public AbstractDialogData()
        {
            ButtonName = GetButtonName;
        }

        public abstract void ShowDialog();

        public bool EqualTo(IDialogData data)
        {
            return GetType() == data.GetType()
                && Title.Equals(data.Title)
                && Content.Equals(data.Content)
                && ButtonCallback == data.ButtonCallback;
        }
    }

    public class DialogConfirm : AbstractDialogData
    {
        public override bool IsMessageDialog { get { return true; } }

        public DialogConfirm(string title, string content, Action<bool?> callback)
            : base()
        {
            this.Title = title;
            this.Content = content;
            this.ButtonCallback = callback;
        }

        public override void ShowDialog()
        {
            DialogConfirmView.Instance.ShowDialog(this);
        }
    }

    public class DialogMessage : AbstractDialogData
    {
        public override bool IsMessageDialog { get { return true; } }

        public DialogMessage(string title, string content, Action<bool?> callback) : base()
        {
            this.Title = title;
            this.Content = content;
            this.ButtonCallback = callback;
        }

        public override void ShowDialog()
        {
            DialogMessageView.Instance.ShowDialog(this);
        }
    }
    public class DialogInput : AbstractDialogData
    {
        public override bool IsMessageDialog { get { return true; } }

        public string startValue;
        public Action<bool?, string> onDoneInput;

        public override void ShowDialog()
        {
            DialogInputView.Instance.ShowDialog(this);
        }
    }
}
