using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace CatalogueComponent
{
   [DataContract]
   class MeasurementDataContract
   {
      [DataMember(Name = "resource_id")]
      public string ResourceID { get; set; }

      [DataMember(Name = "resource_name")]
      public string ResourceName { get; set; }

      [DataMember(Name = "measurement_time")]
      public string MeasurementTime { get; set; }
         
      [DataMember(Name = "cpu_cores")]
      public int CpuCores { get; set; }

      [DataMember(Name = "total_memory")]
      public int TotalMemory { get; set; }

      [DataMember(Name = "free_memory")]
      public int FreeMemory { get; set; }

      [DataMember(Name = "cpu_usage")]
      public double CpuUsage { get; set; }

      [DataMember(Name = "measurement_id")]
      public int MeasurementID { get; set; }
   }
}
