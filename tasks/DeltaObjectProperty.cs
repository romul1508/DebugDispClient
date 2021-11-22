using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DebugOmgDispClient.Interfaces;

namespace DebugOmgDispClient.Tasks
{
    public class DeltaObjectProperty : IObject
    {
        private int id;
        private string nameProp;
        private Object value;
        private IObject parentObj = null;
        private List<IObject> properties = new List<IObject>();

        public DeltaObjectProperty(string nameProp, Object value)
        {
            this.nameProp = nameProp;
            this.value = value;            
        }

        public DeltaObjectProperty(int id, Object value)
        {
            this.id = id;
            this.value = value;            
        }

        public DeltaObjectProperty(int id, string nameProp, Object value)
        {
            this.id = id;
            this.value = value;
            this.nameProp = nameProp;            
        }

        public DeltaObjectProperty(string nameProp, Object value, List<IObject> properties)
        {
            this.nameProp = nameProp;
            this.value = value;

            this.properties.AddRange(properties);
        }

        public DeltaObjectProperty(int id, Object value, List<IObject> properties)
        {
            this.id = id;
            this.value = value;
            this.properties.AddRange(properties);
        }

        public DeltaObjectProperty(int id, string nameProp, Object value, List<IObject> properties)
        {
            this.id = id;
            this.value = value;
            this.nameProp = nameProp;
            this.properties.AddRange(properties);
        }

        public int ID { get => id; }

        public string UserName { get => nameProp; }

        public Type TypeObject => throw new NotImplementedException();

        public IObject Parent { get => parentObj; set => parentObj = value; }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        public Object Value
        {
            get => value;
            set => this.value = value;
        }

        public List<IObject> getProperties()
        {
            return properties;
        }        
    }
}
