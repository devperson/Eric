using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnniPanel
{
    public interface INodeProperty
    {
        void SelectNode(AnniSession.File file);
    }
}
