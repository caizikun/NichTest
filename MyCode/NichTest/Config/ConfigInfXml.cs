using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace NichTest
{
    public class ConfigXmlIO:XmlIO
    {
        
       

       // private XDocument doc;

        //private string filePath;
        // private XElement XmlLoadElment;
        private XElement NodeDataSource;
        private XElement NodeEquipmentOffset;
       // private XElement XmlLoadElment;
        public ConfigXmlIO()
        {
        }

      public ConfigXmlIO(string StrPath)
        {
            filePath = StrPath;
            if (!File.Exists(filePath))
            {
                BuildXml();
            }
            else
            {
                doc = XDocument.Load(filePath);
                XmlLoadElment = XElement.Load(filePath);
                NodeDataSource = GetNode("Node", "Item", "DataSource");
                NodeEquipmentOffset = GetNode("Node", "Item", "EquipmentOffset");
            }


        }

      public override bool BuildXml()
        {

           


            XDocument xdoc = null;

            xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                            new XElement("Config"));
            xdoc.Save(filePath);
            doc = XDocument.Load(filePath);
            XmlLoadElment = XElement.Load(filePath);
            //-----------------------------------DataSource

            NodeDataSource = CreatNode("Node","Item","DataSource");
            AddElement(NodeDataSource, new XElement("Level", "0"));
            AddElement(NodeDataSource, new XElement("Type", "Access"));
            AddElement(NodeDataSource, new XElement("LocationPath", filePath));
            AddElement(NodeDataSource, new XElement("SqlPath", @"INPCSZ0518\ATS_HOME"));
            AddElement(NodeDataSource, new XElement("DbName", @"ATS_V2"));
            AddElement(NodeDataSource, new XElement("User", @"03cc04e804d8047d05a8061f066906c205a4061b066504ed06ae05db069d")); //140918_0
            AddElement(NodeDataSource, new XElement("PWD", @"05ac06c806b8065d0206061f066906680676")); //140918_0
               
            // AddElement(NodeDataSource, new XElement("Level", "0"));
            //-----------------------------------DataSource               
            NodeEquipmentOffset = CreatNode("Node", "Item", "EquipmentOffset");

            AddElement(NodeEquipmentOffset, new XElement("Scope", "0,0,0,0"));
            AddElement(NodeEquipmentOffset, new XElement("Attennuator", "0,0,0,0"));
            AddElement(NodeEquipmentOffset, new XElement("LightSourceEr", "0,0,0,0"));
            AddElement(NodeEquipmentOffset, new XElement("Vcc", "0"));
            AddElement(NodeEquipmentOffset, new XElement("Icc", "0"));
            return true;
        }

        #region  DataBase
     
      public String DbName
      {
          set { 

               ModifyElement(GetNode("Node", "Item", "DataSource"), "DbName", value);
           }
          get
          {
              return ReadElementValue(GetNode("Node", "Item", "DataSource"), "DbName");
          
          }
          
      }
 
      public string Username
      {
          set
          {

              ModifyElement(GetNode("Node", "Item", "DataSource"), "User", value);
          }
          get
          {
              return ReadElementValue(GetNode("Node", "Item", "DataSource"), "User");

          }
      }
      public string PWD
      {
          set
          {

              ModifyElement(GetNode("Node", "Item", "DataSource"), "PWD", value);
          }
          get
          {
              return ReadElementValue(GetNode("Node", "Item", "DataSource"), "PWD");

          }
      }


        public string DataBaseUserLever
        {
           
            set
            {
                ModifyElement(GetNode("Node", "Item", "DataSource"), "Level", value);
            }
            get
            {
                return ReadElementValue(GetNode("Node", "Item", "DataSource"), "Level");

            }

        }
        private string databaseType;
        public string DatabaseType
        {
            set
            {
                databaseType = value;
                ModifyElement(NodeDataSource, "Type", value);
            }
            get
            {


                return ReadElementValue(GetNode("Node", "Item", "DataSource"), "Type");
            }
        }
        public string DatabasePath
        {

            set
            {
                if (databaseType.ToUpper() == "LOCATIONDATABASE")
                {
                    ModifyElement(NodeDataSource, "LocationPath", value);
                }

            }
            get
            {
                if (databaseType.ToUpper() == "LOCATIONDATABASE")
                {
                    // ModifyElement(NodeDataSource, "LocationPath", value);
                    return ReadElementValue(GetNode("Node", "Item", "DataSource"), "LocationPath");
                }
                else// Sql
                {
                    // ModifyElement(NodeDataSource, "SqlPath", value);
                    return ReadElementValue(GetNode("Node", "Item", "DataSource"), "SqlPath");
                }
            }
        }
        #endregion
        #region EquipmentOffset

        public string ScopeOffset
        {

            set
            {
                ModifyElement(NodeEquipmentOffset, "Scope", value);
            }
            get
            {
                return ReadElementValue(NodeEquipmentOffset, "Scope");
            }//GetNode("Node", "Item", "EquipmentOffset")
        }
        public string AttennuatorOffset
        {
            set
            {
                ModifyElement(NodeEquipmentOffset, "Attennuator", value);

            }
            get
            {
                return ReadElementValue(NodeEquipmentOffset, "Attennuator");
            }

        }
        public string LightSourceEr
        {
            set
            {
                ModifyElement(NodeEquipmentOffset, "LightSourceEr", value);

            }
            get
            {
                return ReadElementValue(NodeEquipmentOffset, "LightSourceEr");
            }

        }
        public string VccOffset
        {

            set
            {
                ModifyElement(NodeEquipmentOffset, "Vcc", value);
            }
            get
            {
                return ReadElementValue(GetNode("Node", "Item", "EquipmentOffset"), "Vcc");
            }
        }
   
        public string IccOffset
        {

            set
            {
                ModifyElement(NodeEquipmentOffset, "Icc", value);
            }
            get
            {
                return ReadElementValue(GetNode("Node", "Item", "EquipmentOffset"), "Icc");
            }
        }

        #endregion

    }
}
