using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;

namespace ErasePII.SystemUser
{
    internal class QueryRecords
    {
        public static EntityCollection GetEntityRecords(CrmServiceClient client, string entityName)
        {
            var entityCollection = new EntityCollection();
            if (!string.IsNullOrEmpty(entityName)) return entityCollection;
            var queryExpression = new QueryExpression()
            {
                EntityName = entityName,
                ColumnSet = new ColumnSet(true)
            };
            entityCollection = client.RetrieveMultiple(queryExpression);

            return entityCollection;
        }
    }
}