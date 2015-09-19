using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml;
using System.Xml.Schema;
using System.IO;

namespace CommonLibrary.Common
{
    public class XmlValidatorBase
    {
        ArrayList _errs;
        XmlTextReader _reader;
        XmlSchemaSet _schemaSet;
        public ArrayList Errors
        {
            get { return _errs; }
            set { _errs = value; }
        }

        public XmlValidatorBase()
        {
            _errs = new ArrayList();
            _schemaSet = new XmlSchemaSet();
        }
        public XmlSchemaSet SchemaSet
        {
            get { return _schemaSet; }
        }
        protected void ValidationCallBack(object sender, ValidationEventArgs args)
        {
            _errs.Add(args.Message);
        }
        public bool IsValid()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Schemas = _schemaSet;
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
            XmlReader vreader = XmlReader.Create(_reader, settings);
            while (vreader.Read())
            {
            }
            vreader.Close();
            return (_errs.Count == 0);
        }
        public virtual bool Validate(Stream xmlStream)
        {
            xmlStream.Seek(0, SeekOrigin.Begin);
            _reader = new XmlTextReader(xmlStream);
            return IsValid();
        }
        public virtual bool Validate(string filename)
        {
            _reader = new XmlTextReader(filename);
            return IsValid();
        }
    }
}
