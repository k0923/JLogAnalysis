using ConvertLib;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebApplication2.Models;


namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    public class JLogController
    {
        [HttpPost]
        public JLogModel Post([FromBody] JLogModel model)
        {
            if (model.LogData.StartsWith('<'))
            {
                try
                {
                    XElement root = XElement.Parse(model.LogData);
                    XmlToBean beanConverter = new XmlToBean();
                    root = beanConverter.BuildBeanElement(root);
                    model.Result = root.ToString();
                    return model;

                }catch(Exception e)
                {

                }
                
            }

            IJLog jLog = null;
            if (model.IsJson)
            {
                jLog = new JLog<XmlNode, XElement>();
            }
            else
            {
                jLog = new JLog<JsonNode, JContainer>();
            }
            try
            {
                var format = new JLogFormat();
                format.Format(model.LogData);
                model.LogData = format.Data;
                jLog.Analysis(model.LogData);
                model.Result = jLog.Root.ToString();
            }catch(Exception e)
            {
                model.Result = e.Message;
            }
            return model;
           
        }
    }
}
