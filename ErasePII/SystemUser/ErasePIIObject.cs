using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;

namespace ErasePII.SystemUser
{
    public class ErasePiiObject
    {
        public EntityCollection EntityCollection { get; set; }
        public Entity Entity { get; set; }
        public string EntityName { get; set; }
        public FieldType FieldType { get; set; }
        public string FieldName { get; set; }
        public CrmServiceClient ClientContext { get; set; }
        public Dictionary<string, FieldType> FieldCollection { get; set; }
    }
}
