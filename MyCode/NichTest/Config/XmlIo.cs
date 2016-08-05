using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace NichTest
{
    /// <summary>
    /// Xml 操作基类
    /// </summary>
   public class XmlIO
    {
       protected XDocument doc;

       public string filePath;
       protected XElement XmlLoadElment;
       public XmlIO()
       {
       }
     
     public XmlIO(string StrPath)
       { 

       }
     /// <summary> 
     /// 创建Xml文件
     /// </summary>
     virtual public bool BuildXml()
        {
            return true;
        }
     /// <summary> 
     /// 创建节点,示例CreatNode("Node","Item","DataSource")
     /// </summary>
     /// <param name="NodeName">节点名称</param>
     /// <param name="XAttributeName">属性名字</param>
     /// <param name="XAttributeValue">属性的值</param>
     /// <returns>元素节点</returns>
     public XElement CreatNode(string NodeName,string XAttributeName, string XAttributeValue)
        {

          //  XElement EX = XElement.Load(filePath);
           // XElement  db = new XElement("Node", new XAttribute("Item", XAttributeValue));
            XElement db = new XElement(NodeName, new XAttribute(XAttributeName, XAttributeValue));
            XmlLoadElment.Add(db);
            XmlLoadElment.Save(filePath);
            return db;
        }
     /// <summary> 
     /// 获取当前节点,示例("Node","Item","DataSource")
     /// </summary>
     /// <param name="NodeName">节点名称</param>
     /// <param name="XAttributeName">属性名字</param>
     /// <param name="XAttributeValue">属性的值</param>
     /// <returns>元素节点</returns>
     public XElement GetNode(string NodeName, string XAttributeName, string XAttributeValue)
        { 
         
            XElement db=null;
       try
        {
         
            var B = from kk in XmlLoadElment.Descendants(NodeName) where (string)kk.Attribute(XAttributeName) == XAttributeValue select kk;
            
            foreach (var Xmlelement in B)
            {
                return Xmlelement;
            }
        }
        catch
        {
            return db;
        }
        return db;

          
        }

     /// <summary> 
     /// 保存节点,示例("Node","Item","DataSource")
     /// </summary>
     /// <param name="NodeName">节点名称</param>
     /// <param name="XAttributeName">属性名字</param>
     /// <param name="XAttributeValue">属性的值</param>
     /// <returns>元素节点</returns>
     //public XElement SaveNode(string NodeName, string XAttributeName, string XAttributeValue)//Node,Item,XAttributeValue
     //   {

     //       //XElement EX = XElement.Load(filePath);
     //      // XElement db = new XElement("Node", new XAttribute("Item", XAttributeValue));
     //       XElement db = new XElement(NodeName, new XAttribute(XAttributeName, XAttributeValue));

     //       XmlLoadElment.Add(db);
     //       XmlLoadElment.Save(filePath);
     //       return db;
     //   }
    
     /// <summary> 
     /// 创建节点下面的元素节点,示例AddElement(Node,Level)
     /// </summary>
     /// <param name="Node">节点</param>
     /// <param name="New_Element">属性的值</param>
     /// <returns>bool</returns>
     public bool AddElement(XElement Node, XElement New_Element)
        {
            //XElement XmlLoadElment = XElement.Load(filePath);

            try
            {
                Node.Add(New_Element);
                Node.Save(filePath);
               // XmlLoadElment.Add(Node);
                XmlLoadElment.Save(filePath);
                return true;
            }
            catch
            {
                return false;

            }
        }
     /// <summary> 
     /// 修改节点下面的元素节点的值,示例修改权限级别(Node,"Level","1")
     /// </summary>
     /// <param name="Node">节点</param>
     /// <param name="ElementName">元素名称</param>
     /// <param name="ElementValue">元素目标值</param>
     /// <returns>bool</returns>
 
     public bool ModifyElement(XElement Node, string ElementName, string ElementValue)
        {
            try
            {
                Node.Element(ElementName).Value = ElementValue;
                Node.Save(filePath);
                XmlLoadElment.Save(filePath);
                return true;
            }
            catch
            {
                return false;

            }
        }
     /// <summary> 
     /// 读取节点下面的元素节点的值,示例(Node,"Level")
     /// </summary>
     /// <param name="Myxelemnt">节点</param>
     /// <param name="ElementName">元素名称</param>
     /// <returns>string</returns>

     public string ReadElementValue(XElement Myxelemnt, string StrElementName)
        {
            string StrElementValue = null;
             try
             {
                 StrElementValue = Myxelemnt.Element(StrElementName).Value;
             
                return StrElementValue;
            }
            catch
            {
                return StrElementValue;
            }
           
        }
     #region  Dispose()

     // Public implementation of Dispose pattern callable by consumers.
     bool disposed = false;
     public void Dispose()
     {
         Dispose(true);
         GC.SuppressFinalize(this);
     }

     // Protected implementation of Dispose pattern.
     protected virtual void Dispose(bool disposing)
     {
         if (disposed)
             return;

         if (disposing)
         {
             // Free any other managed objects here.
             //
         }

         // Free any unmanaged objects here.
         //
         disposed = true;
     }
     #endregion

    }
    

}
