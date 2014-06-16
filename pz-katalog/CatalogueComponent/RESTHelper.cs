using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CatalogueComponent
{
   class RESTHelper
   {
      public RESTHelper( OutputHelper A_tbOut )
      {
         m_OutHelper = A_tbOut;
         m_strPassword = "test";
         m_strUsername = "test@liferay.com";
      }

      public List<ResourcesDataContract> GetResources()
      {
         m_OutHelper.WriteNewLine("Getting available resources...");

         String query = "http://localhost:8080/api/secure/json?serviceClassName=monitor.service.MeasurementServiceUtil&serviceMethodName=getResources";

         var response = new List<ResourcesDataContract>() as object;

         m_OutHelper.WriteNewLine(MakeRequest(query, ref response));
         m_OutHelper.WriteNewLine("----------- Result -----------");

         foreach (ResourcesDataContract data in response as List<ResourcesDataContract>)
            m_OutHelper.WriteNewLine("resource_id: " + data.ResourceID + "\tresource_name: " + data.ResourceName);

         m_OutHelper.WriteNewLine("------------------------------");

         return response as List<ResourcesDataContract>;
      }

      public MeasurementDataContract GetMeasurements( string A_strResourceID )
      {
         m_OutHelper.WriteNewLine("Getting available measurements from resource (ID="+A_strResourceID.ToString()+")...");

         // get all available measurements ids from resource
         string query = "http://localhost:8080/api/secure/json?serviceClassName=monitor.service.MeasurementServiceUtil&serviceMethodName=getMeasurements&resourceId="+A_strResourceID+"&serviceParameters=[resourceId]";

         var allResponse = new List<AllMeasurementsDataContract>() as object;
         m_OutHelper.WriteNewLine(MakeRequest(query, ref allResponse));

         m_OutHelper.WriteNewLine("----------- Result -----------");

         //get measurement details
         //we need to check only one measurement's information - the most recent

         List<AllMeasurementsDataContract> convertedAllResponse = allResponse as List<AllMeasurementsDataContract>;
         
         m_OutHelper.WriteNewLine("measurement_id: " + convertedAllResponse[0].MeasurementID);
         m_OutHelper.WriteNewLine("Getting measurement info...");

         query = "http://localhost:8080/api/secure/json?serviceClassName=monitor.service.MeasurementServiceUtil&serviceMethodName=getMeasurement&measurementId=" + convertedAllResponse[0].MeasurementID + "&serviceParameters=[measurementId]";
         var response = new MeasurementDataContract() as object;
         m_OutHelper.WriteNewLine(MakeRequest(query, ref response));
         m_OutHelper.WriteNewLine("measurement_time: "+(response as MeasurementDataContract).MeasurementTime+"\tcpu_cores: "+(response as MeasurementDataContract).CpuCores);

         m_OutHelper.WriteNewLine("------------------------------");

         return response as MeasurementDataContract;
      }

      /*returns error message or response status*/
      private string MakeRequest( /*[in]*/ string A_strRequestUrl, /*[out]*/ ref object A_ResponseObject)
      {
         try
         {
            HttpWebRequest request = WebRequest.Create(A_strRequestUrl) as HttpWebRequest;
            request.Credentials = new NetworkCredential(m_strUsername, m_strPassword);
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
               if (response.StatusCode != HttpStatusCode.OK)
                  throw new Exception(String.Format( "Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));

               if (A_ResponseObject is List<ResourcesDataContract>)
               {
                  DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(List<ResourcesDataContract>));
                  A_ResponseObject = jsonSerializer.ReadObject(TrimStream(response.GetResponseStream())) as List<ResourcesDataContract>;
               }
               if (A_ResponseObject is List<AllMeasurementsDataContract>)
               {
                  DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(List<AllMeasurementsDataContract>));
                  A_ResponseObject = jsonSerializer.ReadObject(TrimStream(response.GetResponseStream())) as List<AllMeasurementsDataContract>;
               }
               if (A_ResponseObject is MeasurementDataContract)
               {
                  DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(MeasurementDataContract));
                  A_ResponseObject = jsonSerializer.ReadObject(TrimStream(response.GetResponseStream())) as MeasurementDataContract;
               }

               return "Request status: Success!";
            }
         }
         catch (Exception e)
         {
            return e.Message;
         }
      }

      private Stream TrimStream(Stream A_Stream)
      {
         StreamReader reader = new StreamReader(A_Stream);
         string str = reader.ReadToEnd();
         str.Trim();
         str = str.Substring(1, str.Length - 2);
         str = str.Replace("\\", string.Empty);
         return new MemoryStream(Encoding.UTF8.GetBytes(str));
      }

      public void SendResources(string A_strResponseUrl)
      {
      }

      public void SendMeasurements(string A_strResponseUrl, string A_strResourceID)
      {

      }

      private OutputHelper m_OutHelper;
      private string m_strPassword;
      private string m_strUsername;
   }
}
