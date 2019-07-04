using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;

namespace ErasePII.SystemUser
{
    public class EraseFieldData
    {
        private readonly CrmServiceClient _crmClientServiceClient;
        private readonly Dictionary<string, FieldType> _fieldCollectionDictionary;
        private string _fieldName;
        private readonly FieldType _fieldType;
        private readonly Entity _entity;
        private readonly string _fieldBlank;


        public EraseFieldData(ErasePiiObject erasePiiObject)
        {
            _crmClientServiceClient = erasePiiObject.ClientContext;
            _fieldCollectionDictionary = erasePiiObject.FieldCollection;
            _fieldName = erasePiiObject.FieldName;
            _fieldType = erasePiiObject.FieldType;
            _entity = erasePiiObject.Entity;

            CheckFieldType(_fieldType);
        }


        private void CheckFieldType(FieldType fieldType)
        {

            switch (fieldType)
            {
                case FieldType.Int:
                    EraseIntFieldData(_fieldName);
                    break;
                case FieldType.Money:
                    EraseMoneyFieldData(_fieldName);
                    break;
                case FieldType.OptionSet:
                    EraseOptionSetFieldData(_fieldName);
                    break;
                case FieldType.Text:
                    EraseTextFieldData(_fieldName);
                    break;
                case FieldType.MultipleFieldsWithTypes:
                    EraseFieldCollectionData(_entity, _fieldCollectionDictionary);
                    break;
                default:
                    throw new Exception("The filed type was not created...");
            }
        }

        private void EraseFieldCollectionData(Entity entity, Dictionary<string, FieldType> fieldCollectionDictionary)
        {
            try
            {
                if (fieldCollectionDictionary == null) return;
                foreach (var fieldType in fieldCollectionDictionary)
                {
                    _fieldName = fieldType.Key;
                    CheckFieldType(fieldType.Value);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void EraseTextFieldData(string fieldName)
        {
            try
            {
                _crmClientServiceClient.Update(new Entity
                {
                    Id = _entity.Id,
                    LogicalName = _entity.LogicalName,
                    [fieldName] = string.Empty
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void EraseOptionSetFieldData(string fieldName)
        {
            try
            {
                _crmClientServiceClient.Update(new Entity
                {
                    Id = _entity.Id,
                    LogicalName = _entity.LogicalName,
                    [fieldName] = new OptionSetValue()
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void EraseMoneyFieldData(string fieldName)
        {
            try
            {
                _crmClientServiceClient.Update(new Entity
                {
                    Id = _entity.Id,
                    LogicalName = _entity.LogicalName,
                    [fieldName] = new Money(0)
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void EraseIntFieldData(string fieldName)
        {
            try
            {
                _crmClientServiceClient.Update(new Entity
                {
                    Id = _entity.Id,
                    LogicalName = _entity.LogicalName,
                    [fieldName] = 0
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
