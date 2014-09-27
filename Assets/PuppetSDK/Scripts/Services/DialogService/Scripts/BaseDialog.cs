﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Puppet.Service
{
    public abstract class BaseDialog<T, R> : SingletonPrefab<R>, IDialogView<T> 
        where T : IDialogData 
        where R : SingletonPrefab<R>
    {
        public UILabel labelTitle, labelContent;
        public GameObject buttonTrue, buttonFalse, buttonNull;
        T data;

        public virtual void ShowDialog(T data)
        {
            this.data = data;

            if (labelTitle != null)
                labelTitle.text = data.Title;
            if (labelContent != null)
                labelContent.text = data.Content;

            if (buttonTrue != null && buttonTrue.GetComponentInChildren<UILabel>() != null)
                buttonTrue.GetComponentInChildren<UILabel>().text = data.ButtonName(true);
            if (buttonFalse != null && buttonFalse.GetComponentInChildren<UILabel>() != null)
                buttonFalse.GetComponentInChildren<UILabel>().text = data.ButtonName(false);
            if (buttonNull != null && buttonNull.GetComponentInChildren<UILabel>() != null)
                buttonNull.GetComponentInChildren<UILabel>().text = data.ButtonName(null);
        }

        protected virtual void OnEnable()
        {
            if (buttonTrue != null)
                UIEventListener.Get(buttonTrue).onClick += OnClickButton;
            if (buttonFalse != null)
                UIEventListener.Get(buttonFalse).onClick += OnClickButton;
            if (buttonNull != null)
                UIEventListener.Get(buttonNull).onClick += OnClickButton;
        }

        protected virtual void OnDisable()
        {
            if (buttonTrue != null)
                UIEventListener.Get(buttonTrue).onClick -= OnClickButton;
            if (buttonFalse != null)
                UIEventListener.Get(buttonFalse).onClick -= OnClickButton;
            if (buttonNull != null)
                UIEventListener.Get(buttonNull).onClick -= OnClickButton;
        }

        protected virtual void OnPressButton(bool? pressValue, T data) { }

        void OnClickButton(GameObject obj)
        {
            bool? pressValue = null;
            if (obj == buttonTrue)
                pressValue = true;
            else if (obj == buttonFalse)
                pressValue = false;

            OnPressButton(pressValue, data);

            if (data.ButtonCallback != null)
                data.ButtonCallback(pressValue);

            if(data.onDestroy != null)
                data.onDestroy();

            GameObject.Destroy(gameObject);
        }
    }
}
