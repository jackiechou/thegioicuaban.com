using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CommonLibrary.ComponentModel
{
    public abstract class AbstractContainer : IContainer
    {
        public abstract string Name { get; }
        public abstract object GetComponent(string name);
        public abstract object GetComponent(string name, System.Type contractType);
        public abstract object GetComponent(System.Type contractType);
        public virtual TContract GetComponent<TContract>()
        {
            return (TContract)GetComponent(typeof(TContract));
        }
        public virtual TContract GetComponent<TContract>(string name)
        {
            return (TContract)GetComponent(name, typeof(TContract));
        }
        public abstract string[] GetComponentList(Type contractType);
        public virtual string[] GetComponentList<TContract>()
        {
            return GetComponentList(typeof(TContract));
        }
        public abstract IDictionary GetComponentSettings(string name);
        public virtual IDictionary GetComponentSettings(System.Type component)
        {
            return GetComponentSettings(component.FullName);
        }
        public virtual IDictionary GetCustomDependencies<TComponent>()
        {
            return GetComponentSettings(typeof(TComponent).FullName);
        }
        public IDictionary GetComponentSettings<TComponent>()
        {
            return GetComponentSettings(typeof(TComponent).FullName);
        }
        public abstract void RegisterComponent(string name, System.Type contractType, System.Type componentType, ComponentLifeStyleType lifestyle);
        public virtual void RegisterComponent(string name, System.Type contractType, System.Type componentType)
        {
            RegisterComponent(name, contractType, componentType, ComponentLifeStyleType.Singleton);
        }
        public virtual void RegisterComponent(string name, System.Type componentType)
        {
            RegisterComponent(name, componentType, componentType, ComponentLifeStyleType.Singleton);
        }
        public virtual void RegisterComponent(System.Type contractType, System.Type componentType)
        {
            RegisterComponent(componentType.FullName, contractType, componentType, ComponentLifeStyleType.Singleton);
        }
        public virtual void RegisterComponent(System.Type contractType, System.Type componentType, ComponentLifeStyleType lifestyle)
        {
            RegisterComponent(componentType.FullName, contractType, componentType, lifestyle);
        }
        public virtual void RegisterComponent(System.Type componentType)
        {
            RegisterComponent(componentType.FullName, componentType, componentType, ComponentLifeStyleType.Singleton);
        }
        public virtual void RegisterComponent<TComponent>() where TComponent : class
        {
            RegisterComponent(typeof(TComponent));
        }
        public virtual void RegisterComponent<TComponent>(string name) where TComponent : class
        {
            RegisterComponent(name, typeof(TComponent), typeof(TComponent), ComponentLifeStyleType.Singleton);
        }
        public virtual void RegisterComponent<TComponent>(string name, ComponentLifeStyleType lifestyle) where TComponent : class
        {
            RegisterComponent(name, typeof(TComponent), typeof(TComponent), lifestyle);
        }
        public virtual void RegisterComponent<TContract, TComponent>() where TComponent : class
        {
            RegisterComponent(typeof(TContract), typeof(TComponent));
        }
        public virtual void RegisterComponent<TContract, TComponent>(string name) where TComponent : class
        {
            RegisterComponent(name, typeof(TContract), typeof(TComponent), ComponentLifeStyleType.Singleton);
        }
        public virtual void RegisterComponent<TContract, TComponent>(string name, ComponentLifeStyleType lifestyle) where TComponent : class
        {
            RegisterComponent(name, typeof(TContract), typeof(TComponent), lifestyle);
        }
        public abstract void RegisterComponentSettings(string name, System.Collections.IDictionary dependencies);
        public virtual void RegisterComponentSettings(System.Type component, System.Collections.IDictionary dependencies)
        {
            RegisterComponentSettings(component.FullName, dependencies);
        }
        public virtual void RegisterComponentSettings<TComponent>(System.Collections.IDictionary dependencies)
        {
            RegisterComponentSettings(typeof(TComponent).FullName, dependencies);
        }
        public abstract void RegisterComponentInstance(string name, System.Type contractType, object instance);
        public void RegisterComponentInstance(string name, object instance)
        {
            RegisterComponentInstance(name, instance.GetType(), instance);
        }
        public void RegisterComponentInstance<TContract>(object instance)
        {
            RegisterComponentInstance(instance.GetType().FullName, typeof(TContract), instance);
        }
        public void RegisterComponentInstance<TContract>(string name, object instance)
        {
            RegisterComponentInstance(name, typeof(TContract), instance);
        }
    }
}
