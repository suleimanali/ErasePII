using System;
using System.Collections.Generic;
using ErasePII.SystemUser;
using ErasePII.WebApiApplicationUser;
using Microsoft.Xrm.Tooling.Connector;
using Xrm.Tools.WebAPI;

namespace ErasePII
{
    public static class StartApp
    {
        public static void ApplicationUser(string typeOfErase, Tuple<string, string, CRMWebAPI> api)
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine("The entity logical name(eg: lead, contact): ");
                var entityName = Console.ReadLine()?.ToLower();
                Console.WriteLine();
                Console.WriteLine("The entityID logical name(eg: contactid, leadid): ");
                var entityIdLogicalName = Console.ReadLine()?.ToLower();
                Console.WriteLine();
                if (typeOfErase == "f")
                {
                    Console.WriteLine("Field Logical Name:");
                    var fieldLogicalName = Console.ReadLine()?.ToLower();
                    Console.WriteLine();
                    Console.WriteLine("New field value(eg: '1' ' '): ");
                    var fieldValue = Console.ReadLine()?.ToLower();
                    Console.WriteLine();
                    Console.WriteLine("Field Type:" +
                                      "\nMoney(M)" +
                                      "\nText(T)" +
                                      "\nInteger/Number(N)" +
                                      "\nOptionset(O)" +
                                      "\nMultiple Fields (Select this option when you're deleting multiple fields): (MF)");
                    var fieldType = GetFieldTypeResponse(Console.ReadLine()?.ToLower());
                    Console.WriteLine();
                    Console.WriteLine("Field Type is: " + fieldType);
                    var erasePiiObject = new ErasePiiBuilderWebApi(api.Item3)
                        .FieldName(fieldLogicalName)
                        .EntityName(entityName)
                        .Resource(api.Item2)
                        .EntityIdLogicalName(entityIdLogicalName)
                        .FieldValue(fieldValue)
                        .AccessToken(api.Item1)
                        .FieldType(fieldType)
                        .Build();
                    var eraseFieldData = new EraseFieldDataWebApi(erasePiiObject);
                }
                else if (typeOfErase == "r")
                {
                    var erasePiiObject = new ErasePiiBuilderWebApi(api.Item3)
                        .GetEntityRecords(entityName)
                        .EntityName(entityName)
                        .Resource(api.Item2)
                        .EntityIdLogicalName(entityIdLogicalName)
                        .AccessToken(api.Item1)
                        .Build();
                    var eraseEntityRecord = new EraseRecordDataWebApi(erasePiiObject);
                }
                else if (typeOfErase == "mf")
                {
                    //Entity Logical name...
                    ///Make an array of fields with types following the convention...
                    Console.WriteLine("Create an array of 'Field Logical Name' followed by 'Field Type' " +
                                      "\nEg. ['studentname','text', 'studentId', 'int', 'studentType', 'optionset', " +
                                      "'studentBalance', 'money']");
                    var fieldCollectionDictionary = CreateFieldCollectionDictionary(Console.ReadLine()?.ToLower());
                    var erasePiiObject = new ErasePiiBuilderWebApi(api.Item3)
                        .FieldAndTypeCollection(fieldCollectionDictionary)
                        .Resource(api.Item2)
                        .EntityIdLogicalName(entityIdLogicalName)
                        .AccessToken(api.Item1)
                        .EntityName(entityName)
                        .Build();
                    var eraseFieldData = new EraseFieldDataWebApi(erasePiiObject);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void SystemUser(string typeOfErase, CrmServiceClient _client)
        {
            try
            {

                Console.WriteLine("The entity logical name: ");
                var entityName = Console.ReadLine()?.ToLower();
                Console.WriteLine();

                if (typeOfErase == "f")
                {
                    Console.WriteLine("Field Logical Name:");
                    var fieldLogicalName = '"' + Console.ReadLine()?.ToLower() + '"' + "\n";
                    Console.WriteLine("Field Type:" +
                                      "\nMoney(M)" +
                                      "\nText(T)" +
                                      "\nInteger/Number(N)" +
                                      "\nOptionset(O)" +
                                      "\nMultiple Fields (Select this option when you're deleting multiple fields): (MF)");
                    var fieldType = GetFieldTypeResponse(Console.ReadLine()?.ToLower());

                    var erasePiiObject = new ErasePiiBuilder()
                        .EntityName(entityName)
                        .FieldName(fieldLogicalName)
                        .FieldType(fieldType)
                        .GetEntityRecords(_client, entityName)
                        .Build();

                    var eraseFieldData = new EraseFieldData(erasePiiObject);
                }
                else if (typeOfErase == "r")
                {
                    var erasePiiObject = new ErasePiiBuilder()
                        .EntityName(entityName)
                        .GetEntityRecords(_client, entityName)
                        .Build();
                    var eraseEntityRecord = new EraseRecordData(erasePiiObject);
                }
                else if (typeOfErase == "mf")
                {
                    //Entity Logical name...
                    ///Make an array of fields with types following the convention...
                    Console.WriteLine("Create an array of 'Field Logical Name' followed by 'Field Type' " +
                                      "\nEg. ['studentname','text', 'studentId', 'int', 'studentType', 'optionset', " +
                                      "'studentBalance', 'money']");
                    var fieldCollectionDictionary = CreateFieldCollectionDictionary(Console.ReadLine()?.ToLower());
                    var erasePiiObject = new ErasePiiBuilder()
                        .FieldAndTypeCollection(fieldCollectionDictionary)
                        .EntityName(entityName)
                        .Build();
                    //var eraseFieldData = new EraseFieldData(erasePiiObject);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        private static Dictionary<string, FieldType> CreateFieldCollectionDictionary(string fieldCollectionString)
        {
            try
            {
                var fieldAndFieldType = fieldCollectionString.TrimStart('[').TrimEnd(']');
                var fieldAndFieldTypeSlice = fieldAndFieldType.Split(',');
                var fieldAndFieldTypeDictionary = new Dictionary<string, FieldType>();

                for (var i = 0; i < fieldAndFieldTypeSlice.Length; i++)
                {
                    if (i % 2 == 1) continue;
                    var fieldNameString =
                        ManipulateMultipleFieldString.GetStringFromMultiFieldArray(fieldAndFieldTypeSlice[i]);
                    var fieldTypeString =
                        ManipulateMultipleFieldString.GetStringFromMultiFieldArray(fieldAndFieldTypeSlice[i + 1]);

                    if (fieldAndFieldTypeSlice.Length < (i + 1))
                        break;

                    var getFieldType = GetFieldTypeResponse(fieldTypeString);
                    fieldAndFieldTypeDictionary.Add(fieldNameString, getFieldType);
                }
                return fieldAndFieldTypeDictionary;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        private static FieldType GetFieldTypeResponse(string fieldTypeString)
        {
            try
            {
                switch (fieldTypeString.ToLower())
                {
                    case "o":
                    case "optionset":
                        return FieldType.OptionSet;
                    case "m":
                    case "money":
                        return FieldType.Money;
                    case "t":
                    case "text":
                        return FieldType.Text;
                    case "n":
                    case "int":
                        return FieldType.Int;
                    case "mf":
                        return FieldType.MultipleFieldsWithTypes;
                    default:
                        return FieldType.Default;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
