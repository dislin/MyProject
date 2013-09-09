using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;
using EzNet.Library.Config.Entity;

namespace EzNet.Library.Config.Service
{
    public class ConfigService
    {
        public List<T> GetObject<T>(ConfigSetting configSetting, T entity)
            where T:new()
        {
            List<T> objectList = new List<T>();
            PropertyInfo[] oProperties = entity.GetType().GetProperties();

            var oPropertyObjects = from pi in oProperties
                       select new
                       {
                           name = pi.Name,
                           type = pi.PropertyType
                       };

            string txtFilePath = configSetting.FilePath + configSetting.FileName;
            XmlDocument doc = new XmlDocument();

            using (XmlTextReader reader = new XmlTextReader(txtFilePath))
            {
                doc.Load(reader);
            }

            XmlNodeList oRootsNodes = doc.SelectSingleNode(configSetting.NodePath).ChildNodes;

            #region Old
            //foreach (XmlNode oNodes in oRootsNodes)
            //{
            //    T oNewEntity = new T();
            //    foreach (XmlNode oNode in oNodes.ChildNodes)
            //    {
            //        if (oPropertyObjects.Select(x => x.name.ToString()).ToList().Contains(oNode.Name))
            //        {
            //            PropertyInfo oInfo = entity.GetType().GetProperty(oNode.Name);
            //            if (!oInfo.PropertyType.IsEnum)
            //            {
            //                oInfo.SetValue(oNewEntity, Convert.ChangeType(oNode.InnerText, oInfo.PropertyType), null);
            //            }
            //            else
            //            {
            //                oInfo.SetValue(oNewEntity, Convert.ToInt16(oNode.InnerText), null);
            //            }
            //        }
            //    }
            //    objectList.Add(oNewEntity);
            //} 
            #endregion

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
            PropertyInfo[] oProperties = entity.GetType().GetProperties();
            List<object> entityList = new List<object>();
            var oPropertyObjects = from pi in oProperties
                                   select new
                                   {
                                       name = pi.Name,
                                       type = pi.PropertyType
                                   };

            foreach (XmlNode oNodes in nodes)
            {
                object oNewEntity = Activator.CreateInstance(entity.GetType());
                foreach (XmlNode oNode in oNodes.ChildNodes)
                {
                    if (oPropertyObjects.Select(x => x.name.ToString()).ToList().Contains(oNode.Name))
                    {
                        PropertyInfo oInfo = entity.GetType().GetProperty(oNode.Name);
                        if (oNode.FirstChild.HasChildNodes)
                        {
                            //if (Array.IndexOf(type.GetInterfaces(), typeof(IEnumerable)) > -1)
                            //{
                            //    IEnumerable en = o as IEnumerable;
                            //    foreach (object obj in en)
                            //        Console.WriteLine(obj);
                            //}
                            object obj = Activator.CreateInstance(oInfo.PropertyType);
                            List<object> objs = BuildObject(oNode.FirstChild.ChildNodes, obj);
                            oInfo.SetValue(oNewEntity, Convert.ChangeType(objs, oInfo.PropertyType), null);
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

    }
}
