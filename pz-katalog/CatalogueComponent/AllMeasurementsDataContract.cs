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
   class AllMeasurementsDataContract
   {
      [DataMember(Name = "measurement_id")]
      public int MeasurementID { get; set; }
   }
}
