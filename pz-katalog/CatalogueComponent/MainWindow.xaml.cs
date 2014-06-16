using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace CatalogueComponent
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      public MainWindow()
      {
         InitializeComponent();

         m_OutHelper = new OutputHelper(tbOutput);
         m_RESTHelper = new RESTHelper(m_OutHelper);
         m_bHttpListenerEnabled = true;

         m_OutHelper.WriteNewLine("Component started.");

         InitializeCatalogue();

          var a = CreateMeasurementsJson(Filename, "0024545AC620");
          var b = CreateResourcesJson(Filename);

         m_HttpListenerThread = new Thread(new ThreadStart(WaitForRequests));
         m_HttpListenerThread.IsBackground = true;
         m_HttpListenerThread.Start();
      }

      private void WaitForRequests()
      {
         this.Dispatcher.Invoke((Action)(() =>
         {
            m_OutHelper.WriteNewLine("Starting HTTP Listener.");
         }));

         if (!HttpListener.IsSupported)
         {
            this.Dispatcher.Invoke((Action)(() =>
            {
               m_OutHelper.WriteNewLine("Windows XP SP2 or Server 2003 is required to use the HttpListener.");
            }));
            return;
         }

         // URI prefixes are required
         string[] prefixes = { "http://localhost:8081/resources/" };
         if (prefixes == null || prefixes.Length == 0)
            throw new ArgumentException("prefixes");

         // Create a listener.
         HttpListener listener = new HttpListener();
         // Add the prefixes. 

         foreach (string s in prefixes)
         {
            listener.Prefixes.Add(s);
         }

         listener.Start();
         this.Dispatcher.Invoke((Action)(() =>
         {
            m_OutHelper.WriteNewLine("Listening...");
         }));

         while (m_bHttpListenerEnabled)
         {
            Listen(listener);
         }

         // You must close the output stream.
         listener.Stop();

         this.Dispatcher.Invoke((Action)(() =>
         {
            m_OutHelper.WriteNewLine("Server stopped.");
         }));
      }

      private void Listen(HttpListener listener)
      {
         // Note: The GetContext method blocks while waiting for a request. 
         HttpListenerContext context = listener.GetContext();
         HttpListenerRequest request = context.Request;

         if (request.HttpMethod == "GET")
         {
            this.Dispatcher.Invoke((Action)(() =>
            {
               m_OutHelper.WriteNewLine("GET Request detected. Creating response...");
            }));

            // Obtain a response object.
            HttpListenerResponse response = context.Response;

            // Detect response type - get resources or get measurements
            string location = request.RawUrl; // np /resources/5/measurement/3
            string[] arrLocation = location.Split('/');
            arrLocation = arrLocation.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            string responseString = "";



            if (arrLocation.Count() > 0 && !arrLocation[0].Equals("resources") || (arrLocation.Count() != 1 && arrLocation.Count() != 3))
            {
               this.Dispatcher.Invoke((Action)(() =>
               {
                  m_OutHelper.WriteNewLine("Bad request format!");
               }));
               return;
            }

            // /resources
            if (arrLocation.Count() == 1)
            {
               responseString = CreateResourcesJson(Filename);
            }

            // /resources/ID/measurements
            if (arrLocation.Count() == 3 && arrLocation[2].Equals("measurements"))
            {
               //TODO  -  create json string and response
               //string resource_id = arrLocation[2];
                responseString = CreateMeasurementsJson(Filename, arrLocation[2]);
                //responseString = "[{\"measurement_name\":\"pomiar_1\"}{\"measurement_name\":\"pomiar_2\"}{\"measurement_name\":\"pomiar_3\"}{\"measurement_name\":\"pomiar_4\"}{\"measurement_name\":\"pomiar_5\"}]";
            }

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);

            output.Close();
         }
         else if (request.HttpMethod == "PUT")
         {
            this.Dispatcher.Invoke((Action)(() =>
            {
               m_OutHelper.WriteNewLine("PUT Request detected. Creating response...");
            }));

            // Obtain a response object.
            HttpListenerResponse response = context.Response;

            // Detect response type - get resources or get measurements
            string location = request.RawUrl; // np /resources/5/measurement/3
            string[] arrLocation = location.Split('/');

            if (!arrLocation[1].Equals("resources"))
            {
               this.Dispatcher.Invoke((Action)(() =>
               {
                  m_OutHelper.WriteNewLine("Bad request format!");
               }));
               return;
            }
             PutResource(Filename, arrLocation[2]);

            // Construct a response. 
            string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);

            output.Close();
         }
         else if (request.HttpMethod == "DELETE")
         {
            this.Dispatcher.Invoke((Action)(() =>
            {
               m_OutHelper.WriteNewLine("DELETE Request detected. Creating response...");
            }));

            // Obtain a response object.
            HttpListenerResponse response = context.Response;

            // Detect response type - get resources or get measurements
            string location = request.RawUrl; // np /resources/5/measurement/3
            //  /resources/0024545AC620
            string[] arrLocation = location.Split('/');

            if (!arrLocation[1].Equals("resources"))
            {
               this.Dispatcher.Invoke((Action)(() =>
               {
                  m_OutHelper.WriteNewLine("Bad request format!");
               }));
               return;
            }

             DeleteResource(Filename, arrLocation[2]);

            // Construct a response. 
            string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);

            output.Close();
         }
      }


       private void InitializeCatalogue()
      {
         //at start we have to get all available resources from monitor
         List<ResourcesDataContract> resources = m_RESTHelper.GetResources();

          var _doc = new XDocument();
          _doc.Declaration = new XDeclaration("1.0", "utf-8", "yes");
          var nodeResources = new XElement("resources");

          //for each resource get measurement's info and save that information in database
          foreach (ResourcesDataContract data in resources)
          {
              var nodeResource = new XElement("resource");
              //nodeResource.SetAttributeValue("resource_id",data.ResourceID);
              foreach (var property in data.GetType().GetProperties())
              {
                  var atts =
                      property.GetCustomAttributes(typeof (DataMemberAttribute), true) as DataMemberAttribute[];

                  if (atts != null)
                      nodeResource.SetAttributeValue(atts[0].Name, property.GetValue(data));
              }

              MeasurementDataContract measurement = m_RESTHelper.GetMeasurements(data.ResourceID);

              var basicInfo = new List<string> { "ResourceID", "ResourceName", "MeasurementID", "MeasurementTime" }; 
              
              //nodeMeasurement.SetAttributeValue("measurement_id", measurement.MeasurementID);

              foreach (var property in (from property in measurement.GetType().GetProperties() where !basicInfo.Contains(property.Name) select property))
              {
                  var nodeMeasurement = new XElement("measurement");

                  foreach (var basicProperty in (from prop in measurement.GetType().GetProperties() where basicInfo.Contains(prop.Name) select prop))
                  {
                      var attsBasic =
                          basicProperty.GetCustomAttributes(typeof(DataMemberAttribute), true) as DataMemberAttribute[];
                      if (attsBasic != null)
                          nodeMeasurement.SetAttributeValue(attsBasic[0].Name, basicProperty.GetValue(measurement));
                  }

                  var atts =
                      property.GetCustomAttributes(typeof(DataMemberAttribute), true) as DataMemberAttribute[];
                  if (atts != null)
                      nodeMeasurement.SetAttributeValue(atts[0].Name, property.GetValue(measurement));

                  nodeResource.Add(new XElement("measurements", nodeMeasurement));
              }

//              foreach (var property in measurement.GetType().GetProperties())
//              {
//                  var atts =
//                      property.GetCustomAttributes(typeof(DataMemberAttribute), true) as DataMemberAttribute[];
//                  if (atts != null) 
//                      nodeMeasurement.SetAttributeValue(atts[0].Name, property.GetValue(measurement));
//              }

              nodeResources.Add(nodeResource);
          }

          _doc.Add(nodeResources);
          _doc.Save(Filename);

         //now we should start new thread to read requests
      }

      public string CreateResourcesJson(string fileName)
      {
          XDocument doc = XDocument.Load(fileName);
          var str = new StringBuilder();
          
          str.Append("[");
          var elemsList = new List<string>();
          foreach (XElement el in doc.Root.Elements())
          {
              var tmp = new StringBuilder();
              tmp.Append("{");
              tmp.Append(string.Join(",", (from att in el.Attributes() select string.Format("\"{0}\":\"{1}\"", att.Name, att.Value)).ToArray()));
              tmp.Append("}");

              elemsList.Add(tmp.ToString());
          }
          str.Append(string.Join(",", elemsList));
          str.Append("]");

          return (!str.ToString().Equals("[]")) ? str.ToString():string.Empty;
      }

      public string CreateMeasurementsJson(string fileName, string resourceid)
      {
          XDocument doc = XDocument.Load(fileName);
          var str = new StringBuilder();
          
          var res2 = (from measure in doc.Root.Elements("resource").Elements("measurements").Elements("measurement")
                      where
                          measure.Attribute("resource_id").Value.Equals(resourceid, StringComparison.OrdinalIgnoreCase)
                      select measure).ToList();

          str.Append("[");
          var elemsList = new List<string>();
          foreach (var el in res2)
          {
              var basicInfo = new List<string> { "resource_id", "resource_name", "measurement_id", "measurement_time" }; 
              var tmp = new StringBuilder();
              tmp.Append("{");
              tmp.Append(string.Format("\"{0}\":\"{1}\"", "measurement_id", el.Attribute("measurement_id").Value));
              tmp.Append(",");
              tmp.Append(string.Join(",", (from att in el.Attributes() where !basicInfo.Contains(att.Name.ToString()) select string.Format("\"{0}\":\"{1}\"", "measurement_name", att.Name)).ToArray()));
              tmp.Append("}");

              elemsList.Add(tmp.ToString());
          }
          str.Append(string.Join(",", elemsList));
          str.Append("]");

          return (!str.ToString().Equals("[]")) ? str.ToString() : string.Empty;
      }

      private void DeleteResource(string filename, string resourceId)
      {
          XDocument doc = XDocument.Load(filename);
          (from res in doc.Root.Elements("resource")
           where res.Attribute("resource_id").Value.Equals(resourceId, StringComparison.OrdinalIgnoreCase)
           select res).Remove();
          doc.Save(filename);
      }

       private void PutResource(string filename, string resouceid)
       {
           XDocument doc = XDocument.Load(filename);
           var elem = new XElement("resource");
           elem.SetAttributeValue("resource_id", resouceid);
           doc.Root.Add(elem);
           doc.Save(filename);
       }

      private void btnGetResources_Click(object sender, RoutedEventArgs e)
      {
         m_RESTHelper.GetResources();
      }

      private void btnStopHttpServer_Click(object sender, RoutedEventArgs e)
      {
         m_bHttpListenerEnabled = false;
         HttpWebRequest request = WebRequest.Create("http://localhost:8081/resources/") as HttpWebRequest;
         request.GetResponseAsync();
      }

      private void btnTestRequest_Click(object sender, RoutedEventArgs e)
      {
         HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://localhost:8081/resources/15");
         request.Method = "PUT";
         Stream dataStream = request.GetRequestStream();
         request.GetResponseAsync();
      }

      private RESTHelper m_RESTHelper;
      private OutputHelper m_OutHelper;
      private bool m_bHttpListenerEnabled;
      private Thread m_HttpListenerThread;
       private const string Filename = @"D:\lf\pz-monitor\db.xml";
   }
}
