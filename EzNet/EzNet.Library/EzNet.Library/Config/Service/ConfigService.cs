using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using EzNet.Library.Config.Entity;
using EzNet.Library.Common;

namespace EzNet.Library.Config.Service
{
    public class ConfigService : Singleton <ConfigService>
    {
        private ConfigService() { }
        public List<T> GetObject<T>(ConfigSetting configSetting, T entity)
            where T:new()
        {
            List<T> objectList = new List<T>();
            string txtFilePath = configSetting.FilePath + configSetting.FileName;
            XmlDocument doc = new XmlDocument();

            using (XmlTextReader reader = new XmlTextReader(txtFilePath))
            {
                doc.Load(reader);
            }

            XmlNodeList oRootsNodes = doc.SelectSingleNode(configSetting.NodePath).ChildNodes;

            object obj = new object();
            obj = (T)Convert.ChangeType(entity, typeof(T));
            List<object> objs = BuildObject(oRootsNodes, obj);
            objs.ForEach(x =>
            {
                objectList.Add((T)Convert.ChangeType(x, typeof(T)));
            });

            return objectList;
        }

        internal List<object> BuildObject(XmlNodeList nodes, object entity)
        {
            Type oEntityType = entity.GetType();
            Type oRealType;

            if (Array.IndexOf(oEntityType.GetInterfaces(), typeof(System.Collections.IEnumerable)) > -1)
            {
                //List Object
                oRealType = oEntityType.GetGenericArguments()[0];
            }
            else
            {
                //Object
                oRealType = oEntityType;
            }
            PropertyInfo[] oProperties = oRealType.GetProperties();

            
            List<object> entityList = new List<object>();
            var oPropertyObjects = from pi in oProperties
                                   select new
                                   {
                                       name = pi.Name,
                                       type = pi.PropertyType
                                   };

            foreach (XmlNode oNodes in nodes)
            {
                object oNewEntity = Activator.CreateInstance(oRealType);
                foreach (XmlNode oNode in oNodes.ChildNodes)
                {
                    if (oPropertyObjects.Select(x => x.name.ToString()).ToList().Contains(oNode.Name))
                    {
                        PropertyInfo oInfo = oNewEntity.GetType().GetProperty(oNode.Name);
                        if (oNode.FirstChild.HasChildNodes)
                        {
                            object obj = Activator.CreateInstance(oInfo.PropertyType);
                            if (Array.IndexOf(oInfo.PropertyType.GetInterfaces(), typeof(System.Collections.IEnumerable)) > -1)
                            {
                                //List Object
                                IList oIList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(oInfo.PropertyType.GetGenericArguments()[0]));
                                var oObjs = BuildObject(oNode.ChildNodes, obj);
                                oObjs.ForEach(x =>
                                {
                                    oIList.Add(Convert.ChangeType(x, oInfo.PropertyType.GetGenericArguments()[0]));
                                });

                                oInfo.SetValue(oNewEntity, oIList, null);
                            }
                            else
                            {
                                //Object
                                obj = GetObject(oNode.FirstChild, obj);
                                //obj = GetObject(oNode, obj);
                                oInfo.SetValue(oNewEntity, Convert.ChangeType(obj, oInfo.PropertyType), null);
                            }
                        }
                        else
                        {
                            if (!oInfo.PropertyType.IsEnum)
                            {
                                oInfo.SetValue(oNewEntity, Convert.ChangeType(oNode.InnerText, oInfo.PropertyType), null);
                            }
                            else
                            {
                                oInfo.SetValue(oNewEntity, Convert.ToInt16(oNode.InnerText), null);
                            }
                        }
                    }
                }
                entityList.Add(oNewEntity);
            }

            return entityList;
        }

        internal object GetObject(XmlNode oNode, object oEntity)
        {
            PropertyInfo[] oProperties = oEntity.GetType().GetProperties();
            var oPropertyObjects = from pi in oProperties
                                   select new
                                   {
                                       name = pi.Name,
                                       type = pi.PropertyType
                                   };

                object oObject = Activator.CreateInstance(oEntity.GetType());
                foreach (XmlNode oPropertyNode in oNode)
                {
                    if (oPropertyObjects.Select(x => x.name.ToString()).ToList().Contains(oPropertyNode.Name))
                    {
                        PropertyInfo oInfo = oObject.GetType().GetProperty(oPropertyNode.Name);
                        if (!oInfo.PropertyType.IsEnum)
                        {
                            oInfo.SetValue(oObject, Convert.ChangeType(oPropertyNode.InnerText, oInfo.PropertyType), null);
                        }
                        else
                        {
                            oInfo.SetValue(oObject, Convert.ToInt16(oPropertyNode.InnerText), null);
                        }
                    }
                }
                return oObject;

        }

    }
}
