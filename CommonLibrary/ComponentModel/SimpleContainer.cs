using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CommonLibrary.ComponentModel
{
    public class SimpleContainer : AbstractContainer
    {
        private ComponentBuilderCollection componentBuilders = new ComponentBuilderCollection();
        private Dictionary<string, IDictionary> componentDependencies = new Dictionary<string, IDictionary>();
        private object componentLock = new object();
        private ComponentTypeCollection componentTypes = new ComponentTypeCollection();
        private Dictionary<System.Type, string> registeredComponents = new Dictionary<System.Type, string>();
        private string _Name;
        public SimpleContainer()
            : this(string.Format("Container_{0}", Guid.NewGuid().ToString()))
        {

        }
        public SimpleContainer(string name)
        {
            _Name = name;
        }
        private void AddBuilder(Type contractType, IComponentBuilder builder)
        {
            lock (componentLock)
            {
                if (!componentTypes[contractType].ComponentBuilders.Contains(builder))
                {
                    componentTypes[contractType].ComponentBuilders.Add(builder);
                }
                if (!componentBuilders.Contains(builder))
                {
                    componentBuilders.Add(builder);
                }
            }
        }
        private object GetComponent(IComponentBuilder builder)
        {
            object component;
            if (builder == null)
            {
                component = null;
            }
            else
            {
                component = builder.BuildComponent();
            }
            return component;
        }
        public override object GetComponent(string name)
        {
            object component = null;
            if (componentBuilders.Contains(name))
            {
                component = GetComponent(componentBuilders[name]);
            }
            return component;
        }
        public override object GetComponent(System.Type contractType)
        {
            object component = null;
            if (componentTypes.Contains(contractType))
            {
                ComponentType type = componentTypes[contractType];
                if (type.ComponentBuilders.Count > 0)
                {
                    component = GetComponent(type.ComponentBuilders[0]);
                }
            }
            return component;
        }
        public override object GetComponent(string name, System.Type contractType)
        {
            object component = null;
            if (componentTypes.Contains(contractType))
            {
                ComponentType type = componentTypes[contractType];
                if (type.ComponentBuilders.Contains(name))
                {
                    component = GetComponent(type.ComponentBuilders[name]);
                }
            }
            return component;
        }
        public override string[] GetComponentList(System.Type contractType)
        {
            List<string> components = new List<string>();
            foreach (KeyValuePair<Type, string> kvp in registeredComponents)
            {
                if (object.ReferenceEquals(kvp.Key.BaseType, contractType))
                {
                    components.Add(kvp.Value);
                }
            }
            return components.ToArray();
        }
        public override System.Collections.IDictionary GetComponentSettings(string name)
        {
            return componentDependencies[name];
        }
        public override string Name
        {
            get { return _Name; }
        }
        public override void RegisterComponent(string name, System.Type contractType, System.Type componentType, ComponentLifeStyleType lifestyle)
        {
            if (!componentTypes.Contains(contractType))
            {
                componentTypes.Add(new ComponentType(contractType));
            }
            IComponentBuilder builder = null;
            switch (lifestyle)
            {
                case ComponentLifeStyleType.Transient:
                    builder = new TransientComponentBuilder(name, componentType);
                    break;
                case ComponentLifeStyleType.Singleton:
                    builder = new SingletonComponentBuilder(name, componentType);
                    break;
            }
            AddBuilder(contractType, builder);
            registeredComponents[componentType] = name;
        }
        public override void RegisterComponentInstance(string name, System.Type contractType, object instance)
        {
            if (!componentTypes.Contains(contractType))
            {
                componentTypes.Add(new ComponentType(contractType));
            }
            AddBuilder(contractType, new InstanceComponentBuilder(name, instance));
        }
        public override void RegisterComponentSettings(string name, System.Collections.IDictionary dependencies)
        {
            componentDependencies[name] = dependencies;
        }
    }
}
