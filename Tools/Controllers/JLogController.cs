using ConvertLib;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tools.Models;

namespace Tools.Controllers
{
    public class JLogController : Controller
    {
        public IActionResult Index() {
            return View();
        }

        [HttpPost]
        public IActionResult Submit(JLogModel model) {
            if (model.LogData.StartsWith('<')) {
                try {
                    XElement root = XElement.Parse(model.LogData);
                    XmlToBean beanConverter = new XmlToBean();
                    root = beanConverter.BuildBeanElement(root);
                    model.Result = root.ToString();
                    return View("Index", model);
                }
                catch (Exception e) {

                }

            }

            IJLog jLog = null;
            if (model.ConvertType == ConvertType.Xml) {
                jLog = new JLog<XmlNode, XElement>();
            } else {
                jLog = new JLog<JsonNode, JContainer>();
            }
            try {
                var format = new JLogFormat();
                format.Format(model.LogData);
                model.LogData = format.Data;
                jLog.Analysis(model.LogData);
                model.Result = jLog.Root.ToString();
            }
            catch (Exception e) {
                model.Result = e.Message;
            }
            
            return View("Index", model);
        }

        
    }
}
