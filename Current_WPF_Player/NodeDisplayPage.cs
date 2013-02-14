using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AnniPanel
{
    public class NodeDisplayPage : Page
    {
        public NodeDisplayPage(AnniSession session)
        {
            Session = session;
        }
        protected AnniSession Session { get; private set; }
    }
}
