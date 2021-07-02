﻿using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Iviz.App
{
    public class TrashButtonWidget : MonoBehaviour, IWidget
    {
        [SerializeField] Button button;
        [SerializeField] Image image;

        public bool Interactable
        {
            get => button.interactable;
            set => button.interactable = value;
        }

        public Sprite Sprite
        {
            get => image.sprite;
            set => image.sprite = value;
        }

        public event Action Clicked;

        public void OnClicked()
        {
            Clicked?.Invoke();
        }

        public void ClearSubscribers()
        {
            Clicked = null;
        }

        [NotNull]
        public TrashButtonWidget SubscribeClicked(Action f)
        {
            Clicked += f;
            return this;
        }

    }
}