﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CommonLibrary.ComponentModel
{
    public interface IContainer
    {
        void RegisterComponent<TComponent>() where TComponent : class;
        void RegisterComponent<TComponent>(string name) where TComponent : class;
        void RegisterComponent<TComponent>(string name, ComponentLifeStyleType lifestyle) where TComponent : class;
        void RegisterComponent<TContract, TComponent>() where TComponent : class;
        void RegisterComponent<TContract, TComponent>(string name) where TComponent : class;
        void RegisterComponent<TContract, TComponent>(string name, ComponentLifeStyleType lifestyle) where TComponent : class;
        void RegisterComponent(Type componentType);
        void RegisterComponent(Type contractType, Type componentType);
        void RegisterComponent(Type contractType, Type componentType, ComponentLifeStyleType lifestyle);
        void RegisterComponent(string name, Type componentType);
        void RegisterComponent(string name, Type contractType, Type componentType);
        void RegisterComponent(string name, Type contractType, Type componentType, ComponentLifeStyleType lifestyle);
        void RegisterComponentInstance(string name, object instance);
        void RegisterComponentInstance(string name, Type contractType, object instance);
        void RegisterComponentInstance<TContract>(object instance);
        void RegisterComponentInstance<TContract>(string name, object instance);
        void RegisterComponentSettings(string name, IDictionary dependencies);
        void RegisterComponentSettings(Type component, IDictionary dependencies);
        void RegisterComponentSettings<TComponent>(IDictionary dependencies);
        object GetComponent(string name);
        TContract GetComponent<TContract>();
        object GetComponent(Type contractType);
        TContract GetComponent<TContract>(string name);
        object GetComponent(string name, Type contractType);
        string[] GetComponentList<TContract>();
        string[] GetComponentList(Type contractType);
        IDictionary GetComponentSettings(string name);
        IDictionary GetComponentSettings(System.Type component);
        IDictionary GetComponentSettings<TComponent>();
        string Name { get; }
    }
}
