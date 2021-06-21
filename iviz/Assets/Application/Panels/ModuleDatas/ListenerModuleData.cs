﻿using Iviz.Controllers;
using JetBrains.Annotations;

namespace Iviz.App
{
    public abstract class ListenerModuleData : ModuleData
    {
        [NotNull] protected abstract ListenerController Listener { get; }
        public override IController Controller => Listener;

        protected ListenerModuleData([NotNull] string topic, [NotNull] string type) : base(topic, type)
        {
            ModuleListPanel.RegisterDisplayedTopic(Topic);
        }

        public override void Stop()
        {
            base.Stop();
            Listener.StopController();
        }
        
        public override string ToString()
        {
            return $"[{ModuleType} Topic='{Topic}' [{Type}] guid={Configuration.Id}]";
        }
    }
}
