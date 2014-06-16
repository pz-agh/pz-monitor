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
   public class ResourcesDataContract
   {
      [DataMember(Name = "resource_id")]
      public string ResourceID { get; set; }

      [DataMember(Name = "resource_name")]
      public string ResourceName { get; set; }
   }
}
