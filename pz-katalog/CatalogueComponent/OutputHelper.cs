using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CatalogueComponent
{
   class OutputHelper
   {
      public OutputHelper(TextBox tb)
      {
         m_tbOutput = tb;
      }

      public void WriteNewLine( string text )
      {
         m_tbOutput.Text += text + "\n";
      }
      
      private TextBox m_tbOutput;
   }
}
