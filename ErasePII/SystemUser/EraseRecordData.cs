using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;

namespace ErasePII.SystemUser
{
    public class EraseRecordData
    {
        private readonly CrmServiceClient _crmClientServiceClient;
        private readonly Entity _entity;
        private readonly EntityCollection _entityCollection;

        public EraseRecordData(ErasePiiObject erasePiiObject)
        {
            _crmClientServiceClient =erasePiiObject.ClientContext;
            _entity = erasePiiObject.Entity;
            _entityCollection = erasePiiObject.EntityCollection;

            if (_entityCollection.Entities.Count > 0)
                BulkDeleteEntityRecords();

        }

        private void DeleteEntityRecord(Entity entity)
        => _crmClientServiceClient.Delete(entity.LogicalName, entity.Id);


        private void BulkDeleteEntityRecords()
        {
            try
            {
                foreach (var entity in _entityCollection.Entities)
                    DeleteEntityRecord(entity);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
